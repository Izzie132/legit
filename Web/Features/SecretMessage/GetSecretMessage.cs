using Microsoft.Extensions.Options;
using Web.Configuration;

namespace Web.Features.SecretMessage;

public static class GetSecretMessage
{
    public record Request(string Password);

    public record Response(string Message);

    public class Endpoint(IOptions<AppOptions> projectNameOptions) : Endpoint<Request, Response>
    {
        private readonly AppOptions appOptions = projectNameOptions.Value;

        public override void Configure()
        {
            Get($"secret/{nameof(GetSecretMessage)}");
            AllowAnonymous();
        }

        public override Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken)
        {
            if (request.Password != appOptions.SecretMessagePassword)
            {
                AddError(r => r.Password, "Incorrect password");
            }

            ThrowIfAnyErrors();

            return Task.FromResult(new Response("The secret message is: 'Hello World!'"));
        }
    }
}
