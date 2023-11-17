namespace Builders.Features.User;

public class UserBuilder : IBuilder<Web.Features.User.User>
{
    public int Id { get; set; } = default!;
    public string Name { get; set; }
    public string Email { get; set; }

    public UserBuilder(Faker? faker = null)
    {
        faker ??= new Faker();
        Name = faker.Person.FullName;
        Email = faker.Person.Email;
    }

    public Web.Features.User.User Build() =>
        Web.Features.User.User.CreateTestUser(id: Id, name: Name, email: Email);
}
