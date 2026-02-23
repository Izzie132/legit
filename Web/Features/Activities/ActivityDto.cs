namespace Web.Features.Activities;

public record ActivityDto(
    string Name,
    string UserId,
    string Title,
    decimal DistanceInMeters,
    DateTime DateOfActivity,
    Uri? ImageUri,
    ICollection<int>? CommentIndices
);
