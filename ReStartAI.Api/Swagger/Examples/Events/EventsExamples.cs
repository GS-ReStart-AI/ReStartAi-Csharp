using ReStartAI.Api.Controllers.IoT;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Events
{
    public class PostEventRequestExample : IExamplesProvider<EventsController.PostEventRequest>
    {
        public EventsController.PostEventRequest GetExamples()
        {
            return new EventsController.PostEventRequest(
                Tipo: "job_view",
                Metadata: new Dictionary<string, object>
                {
                    { "vagaId", "000000000000000000000002" },
                    { "origem", "mobile" },
                    { "screen", "JobDetails" }
                }
            );
        }
    }

    public class PostEventResponseExample : IExamplesProvider<EventsController.PostEventResponse>
    {
        public EventsController.PostEventResponse GetExamples()
        {
            return new EventsController.PostEventResponse(
                Id: "000000000000000000000020",
                JobsViewedToday: 5,
                ApplyClicksToday: 2,
                LastEventAt: DateTime.UtcNow.AddMinutes(-1)
            );
        }
    }

    public class EventItemListResponseExample : IExamplesProvider<IEnumerable<EventsController.EventItem>>
    {
        public IEnumerable<EventsController.EventItem> GetExamples()
        {
            return new[]
            {
                new EventsController.EventItem(
                    Id: "000000000000000000000020",
                    Tipo: "job_view",
                    TimestampUtc: DateTime.UtcNow.AddMinutes(-5)
                ),
                new EventsController.EventItem(
                    Id: "000000000000000000000021",
                    Tipo: "apply_click",
                    TimestampUtc: DateTime.UtcNow.AddMinutes(-2)
                )
            };
        }
    }
}