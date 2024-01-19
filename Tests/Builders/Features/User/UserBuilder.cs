using NodaTime;

namespace Builders.Features.User;

public class UserBuilder : IBuilder<Web.Features.User.User>
{
    public int Id { get; set; } = default!;
    public string Name { get; set; }
    public string Email { get; set; }
    public Instant CreatedAt { get; set; }

    public UserBuilder(Faker? faker = null)
    {
        faker ??= new Faker();
        Name = faker.Person.FullName;
        Email = faker.Person.Email;
        CreatedAt = faker.Noda().Instant.Recent();
    }

    public Web.Features.User.User Build() =>
        Web.Features.User.User.CreateTestUser(id: Id, name: Name, email: Email, createdAt: CreatedAt);
}
