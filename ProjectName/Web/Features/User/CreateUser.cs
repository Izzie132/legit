using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Web.Database;

namespace Web.Features.User;

public class CreateUser
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

    public class Endpoint(DataContext dataContext) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post($"user/{nameof(CreateUser)}");
            AllowAnonymous();
        }

        public override async Task<Response> ExecuteAsync(
            Request request,
            CancellationToken cancellationToken
        )
        {
            if (await dataContext.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            {
                AddError(r => r.Email, "A user with this email already exists");
            }

            ThrowIfAnyErrors();

            var user = new User(request.Name, request.Email);
            dataContext.Users.Add(user);
            await dataContext.SaveChangesAsync(cancellationToken);

            return new Response(user.Id, user.Name, user.Email);
        }
    }
}
