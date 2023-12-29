using System.Net;
using Builders.Features.User;
using Web.Features.User;

namespace IntegrationTests.Features.User;

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

        var (rsp, res) = await Client.GETAsync<GetUsers.Endpoint, GetUsers.Response>();

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.Equal(2, res.Users.Count);
        Assert.Contains(res.Users, u => u.Id == user.Id);
        Assert.Contains(res.Users, u => u.Id == user2.Id);
    }
}
