using ReStartAI.Application.Dto;
using ReStartAI.Api.Controllers;
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
                Metadata: new Dictionary<string, object?>
                {
                    { "vagaId", "64f1234567890abcdefv001" },
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
                Id: "64f1234567890abcdefe001",
                JobsViewedToday: 3,
                ApplyClicksToday: 1,
                LastEventAt: DateTime.UtcNow
            );
        }
    }

    public class EventItemListResponseExample : IExamplesProvider<List<EventsController.EventItem>>
    {
        public List<EventsController.EventItem> GetExamples()
        {
            return new List<EventsController.EventItem>
            {
                new EventsController.EventItem(
                    Id: "64f1234567890abcdefe001",
                    Tipo: "job_view",
                    TimestampUtc: DateTime.UtcNow.AddMinutes(-5)
                ),
                new EventsController.EventItem(
                    Id: "64f1234567890abcdefe002",
                    Tipo: "apply_click",
                    TimestampUtc: DateTime.UtcNow.AddMinutes(-2)
                )
            };
        }
    }
}