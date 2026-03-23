using FluentValidation;
using Web.Database;

namespace Web.Features.Activities;

public static class CreateActivity
{
    public record Request(ActivityDto Activity);

    public record Response(int Id);

    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Activity.Title).NotEmpty();
            RuleFor(x => x.Activity.DistanceInMeters).GreaterThan(0);
        }
    }

    public class Endpoint(DataContext dataContext) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post($"activity/{nameof(CreateActivity)}");
            AllowAnonymous();
        }

        public override async Task<Response> ExecuteAsync(Request request, CancellationToken ct)
        {
            var activity = new Activity(
                request.Activity.UserId,
                request.Activity.DateOfActivity,
                request.Activity.Title,
                request.Activity.DistanceInMeters
            );

            dataContext.Activities.Add(activity);

            await dataContext.SaveChangesAsync(ct);

            return new Response(activity.Id);
        }
    }
}
