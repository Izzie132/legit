using NodaTime;

namespace Web.Features.Users;

public class User(string name, string email, Instant createdAt)
{
    public int Id { get; private set; }
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public Instant CreatedAt { get; private set; } = createdAt;

    public static User CreateTestUser(int id, string name, string email, Instant createdAt) =>
        new(name, email, createdAt) { Id = id };
}
