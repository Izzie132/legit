using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Polly;
using Polly.Retry;

namespace Audit;

public static class SonatypeService
{
    public static async Task<ICollection<ComponentReportResponse>> GetResponses(
        ICollection<PackageDetails> packages,
        string sonatypeOssIndexUsername,
        string sonatypeOssIndexApiToken
    )
    {
        using var httpClient = GetHttpClient(sonatypeOssIndexUsername, sonatypeOssIndexApiToken);
        var request = BuildRequest(packages);
        return await GetResponses(httpClient, request);
    }

    private const string SonatypeOssIndexApiUrl = "https://ossindex.sonatype.org/api/v3/authorized/component-report";

    private static readonly JsonSerializerOptions SonatypeOssIndexApiJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private sealed record ComponentReportRequest(ICollection<string> Coordinates);

#pragma warning disable CA1812
    private sealed record WrappedResponse(ICollection<ComponentReportResponse> Results);
#pragma warning restore CA1812

    private static HttpClient GetHttpClient(string sonatypeOssIndexUsername, string sonatypeOssIndexApiToken)
    {
        var httpClient = new HttpClient { BaseAddress = new(SonatypeOssIndexApiUrl) };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sonatypeOssIndexUsername}:{sonatypeOssIndexApiToken}"))
        );

        return httpClient;
    }

    private static async Task<ICollection<ComponentReportResponse>> GetResponses(
        HttpClient httpClient,
        ComponentReportRequest request
    )
    {
        var resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(
                new RetryStrategyOptions
                {
                    MaxRetryAttempts = 5,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = async outcome =>
                        await ConsoleHelpers.WriteLineYellow(
                            $"Call to Sonatype API failed. Retrying ... (Error was: '{outcome.Outcome.Exception?.Message})'"
                        ),
                }
            )
            .Build();

        return await resiliencePipeline.ExecuteAsync(async _ => await UnstableGetResponses());

        async Task<ICollection<ComponentReportResponse>> UnstableGetResponses()
        {
            var httpResponse = await httpClient.PostAsJsonAsync("", request);

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var wrappedResponseString = $"{{ \"results\": {responseString} }}";

            var wrappedResponse = JsonSerializer.Deserialize<WrappedResponse>(
                wrappedResponseString,
                SonatypeOssIndexApiJsonSerializerOptions
            );

            if (wrappedResponse == null)
            {
                throw new ArgumentException($"Failed to deserialize response from Sonatype. Raw response: {responseString}");
            }

            return wrappedResponse.Results;
        }
    }

    private static ComponentReportRequest BuildRequest(ICollection<PackageDetails> packages) =>
        new(packages.Select(p => p.Coordinates).ToList());
}
