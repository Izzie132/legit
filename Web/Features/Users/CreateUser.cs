using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Web.Database;

namespace Web.Features.Users;

public static class CreateUser
{
    public record Request(string Name, string Email);

    public record Response(int Id, string Name, string Email);

    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).Length(1, 50);
            RuleFor(x => x.Email).EmailAddress();
        }
    }

    public class Endpoint(DataContext dataContext, ILogger<Endpoint> logger, ZonedClock clock) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post($"user/{nameof(CreateUser)}");
            AllowAnonymous();
        }

        public override async Task<Response> ExecuteAsync(Request request, CancellationToken cancellationToken)
        {
            if (await dataContext.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            {
                AddError(r => r.Email, "A user with this email already exists");
            }

            ThrowIfAnyErrors();

            var user = new User(request.Name, request.Email, clock.GetCurrentInstant());
            dataContext.Users.Add(user);
            await dataContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Created user {UserId} with email {UserEmail}", user.Id, user.Email);

            return new Response(user.Id, user.Name, user.Email);
        }
    }
}
