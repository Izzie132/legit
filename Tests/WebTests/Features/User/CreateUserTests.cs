using System.Net;
using Builders.Features.User;
using Microsoft.EntityFrameworkCore;
using NodaTime.Text;
using Web.Features.User;
using Web.Infrastructure.Exceptions;
using WebTests.Mocks;

namespace WebTests.Features.User;

public class CreateUserTests : BaseWebTest
{
    public CreateUserTests(WebTestFixture f, ITestOutputHelper o)
        : base(f, o) { }

    [Fact]
    public async Task ValidRequest_CreatesUser()
    {
        const string createdAtIsoString = "2022-01-01T09:30:18Z";
        MockClockService.Clock.Reset(InstantPattern.General.Parse(createdAtIsoString).Value);

        var (httpResponseMessage, response) = await Fixture.Client.POSTAsync<
            CreateUser.Endpoint,
            CreateUser.Request,
            CreateUser.Response
        >(new CreateUser.Request("Ben", "ben@ghyston.com"));

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);

        var databaseUser = await DataContext.Users.SingleOrDefaultAsync();
        Assert.NotNull(databaseUser);
        Assert.Equal("Ben", databaseUser.Name);
        Assert.Equal("ben@ghyston.com", databaseUser.Email);
        Assert.Equal(InstantPattern.General.Parse(createdAtIsoString).Value, databaseUser.CreatedAt);
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
