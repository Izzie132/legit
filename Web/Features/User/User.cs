namespace Web.Features.User;

public class User(string name, string email)
{
    public int Id { get; private set; }
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;

    public static User CreateTestUser(int id, string name, string email) => new(name, email) { Id = id };
}
