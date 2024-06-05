using System.Net;
using Builders.Features.User;
using Microsoft.EntityFrameworkCore;
using NodaTime.Text;
using Web.Features.Users;
using Web.Infrastructure.Exceptions;

namespace WebTests.Features.User;

public class CreateUserTests(WebTestFixture f) : BaseWebTest(f)
{
    [Fact]
    public async Task ValidRequest_CreatesUser()
    {
        var createdAtInstant = InstantPattern.General.Parse("2022-01-01T09:30:18Z").Value;
        FakeClock.Reset(createdAtInstant);
        var (httpResponseMessage, _) = await Client.POSTAsync<CreateUser.Endpoint, CreateUser.Request, CreateUser.Response>(
            new CreateUser.Request("Ben", "ben@ghyston.com")
        );

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);

        var databaseUser = await DataContext.Users.SingleOrDefaultAsync();
        Assert.NotNull(databaseUser);
        Assert.Equal("Ben", databaseUser.Name);
        Assert.Equal("ben@ghyston.com", databaseUser.Email);
        Assert.Equal(createdAtInstant, databaseUser.CreatedAt);
    }

    [Fact]
    public async Task UserAlreadyExists_ThrowsUserVisibleException()
    {
        var user = new UserBuilder().Build();

        AddEntity(user);

        var (httpResponseMessage, response) = await Client.POSTAsync<
            CreateUser.Endpoint,
            CreateUser.Request,
            ApiErrorResponse
        >(new CreateUser.Request(user.Name, user.Email));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        Assert.Contains("Email", response.UserVisibleMessage);
    }

    [Fact]
    public async Task InvalidEmailAddress_ThrowsUserVisibleException()
    {
        var (httpResponseMessage, response) = await Client.POSTAsync<
            CreateUser.Endpoint,
            CreateUser.Request,
            ApiErrorResponse
        >(new CreateUser.Request("Ben", "invalidEmailAddress"));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        Assert.Contains("Email", response.UserVisibleMessage);
    }
}
