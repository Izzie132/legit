using NodaTime;

namespace Web.Features.Activities;

public class Activity(string userId, DateTime dateOfActivity, string title, decimal distanceInMeters, Instant createdAt)
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = userId;
    public DateTime DateOfActivity { get; private set; } = dateOfActivity;
    public string Title { get; private set; } = title;
    public string? Description { get; private set; } = title;
    public decimal DistanceInMeters { get; private set; } = distanceInMeters;
    public Instant CreatedAt { get; private set; } = createdAt;
}
