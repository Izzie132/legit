using Microsoft.Extensions.Options;
using Web.Configuration;

namespace Web.Features.SecretMessage;

public class GetSecretMessage
{
    public record Request(string Password);

    public record Response(string Message);

    public class Endpoint(IOptions<ProjectNameOptions> projectNameOptions) : Endpoint<Request, Response>
    {
        private readonly ProjectNameOptions projectNameOptions = projectNameOptions.Value;

        public override void Configure()
        {
            Get($"secret/{nameof(GetSecretMessage)}");
            AllowAnonymous();
        }

        public override Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken)
        {
            if (request.Password != projectNameOptions.SecretMessagePassword)
            {
                AddError(r => r.Password, "Incorrect password");
            }

            ThrowIfAnyErrors();

            return Task.FromResult(new Response("The secret message is: 'Hello World!'"));
        }
    }
}
