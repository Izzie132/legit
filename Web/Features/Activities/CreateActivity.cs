using FluentValidation;
using NodaTime;
using Web.Database;

namespace Web.Features.Activities;

public static class CreateActivity
{
    public record Request(DateTime DateOfActivity, string Title, decimal DistanceInMeters);

    public record Response(int Id);

    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.DistanceInMeters).GreaterThan(0);
        }
    }

    public class Endpoint(DataContext dataContext, ZonedClock clock) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post($"activity/{nameof(CreateActivity)}");
            AllowAnonymous();
        }

        public override async Task<Response> ExecuteAsync(Request request, CancellationToken ct)
        {
            // ToDo isd - update ID here
            var activity = new Activity(
                "testUser",
                request.DateOfActivity,
                request.Title,
                request.DistanceInMeters,
                clock.GetCurrentInstant()
            );
            dataContext.Activities.Add(activity);

            await dataContext.SaveChangesAsync(ct);

            return new Response(activity.Id);
        }
    }
}
