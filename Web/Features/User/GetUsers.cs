using Microsoft.EntityFrameworkCore;
using Web.Database;

namespace Web.Features.User;

public class GetUsers
{
    public record Response(ICollection<Response.UserResponse> Users)
    {
        public record UserResponse(int Id, string Name, string Email) { }
    }

    public class Endpoint(DataContext dataContext) : EndpointWithoutRequest<Response>
    {
        public override void Configure()
        {
            Get($"user/{nameof(GetUsers)}");
            AllowAnonymous();
        }

        public override async Task<Response> ExecuteAsync(CancellationToken ct)
        {
            var users = await dataContext.Users.ToListAsync(ct);
            return new Response(
                users.Select(u => new Response.UserResponse(u.Id, u.Name, u.Email)).ToList()
            );
        }
    }
}
