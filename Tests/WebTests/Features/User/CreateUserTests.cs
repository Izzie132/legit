using System.Net;
using Builders.Features.User;
using Microsoft.EntityFrameworkCore;
using Web.Exceptions;
using Web.Features.User;

namespace IntegrationTests.Features.User;

public class CreateUserTests : BaseWebTest
{
    public CreateUserTests(WebTestFixture f, ITestOutputHelper o)
        : base(f, o) { }

    [Fact]
    public async Task ValidRequest_CreatesUser()
    {
        var (rsp, res) = await Fixture
            .Client
            .POSTAsync<CreateUser.Endpoint, CreateUser.Request, CreateUser.Response>(
                new CreateUser.Request("Ben", "ben@ghyston.com")
            );

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);

        var databaseUser = await DataContext.Users.SingleOrDefaultAsync();
        Assert.NotNull(databaseUser);
        Assert.Equal("Ben", databaseUser.Name);
        Assert.Equal("ben@ghyston.com", databaseUser.Email);
    }

    [Fact]
    public async Task UserAlreadyExists_ThrowsUserVisibleException()
    {
        var user = new UserBuilder().Build();

        AddEntity(user);

        var (rsp, res) = await Client.POSTAsync<
            CreateUser.Endpoint,
            CreateUser.Request,
            ApiErrorResponse
        >(new CreateUser.Request(user.Name, user.Email));

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Contains("Email", res.UserVisibleMessage);
    }

    [Fact]
    public async Task InvalidEmailAddress_ThrowsUserVisibleException()
    {
        var (rsp, res) = await Client.POSTAsync<
            CreateUser.Endpoint,
            CreateUser.Request,
            ApiErrorResponse
        >(new CreateUser.Request("Ben", "invalidEmailAddress"));

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Contains("Email", res.UserVisibleMessage);
    }
}
