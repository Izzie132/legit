using System.Net;
using Builders.Features.User;
using Web.Features.User;

namespace WebTests.Features.User;

public class GetUsersTests : BaseWebTest
{
    public GetUsersTests(WebTestFixture f, ITestOutputHelper o)
        : base(f, o) { }

    [Fact]
    public async Task ValidRequest_ReturnsUsers()
    {
        var user = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();

        AddEntities(user, user2);

        var (httpResponseMessage, response) = await Client.GETAsync<GetUsers.Endpoint, GetUsers.Response>();

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal(2, response.Users.Count);
        Assert.Contains(response.Users, u => u.Id == user.Id);
        Assert.Contains(response.Users, u => u.Id == user2.Id);
    }
}
