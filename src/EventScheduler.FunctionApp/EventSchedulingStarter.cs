using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using EventScheduler.FunctionApp.Models;

namespace EventScheduler.FunctionApp
{
    /// <summary>
    /// This represents the starter entity for event scheduling on GitHub.
    /// </summary>
    public class EventSchedulingtarter
    {
        /// <summary>
        /// Invokes the durable event for event scheduling on GitHub.
        /// </summary>
        /// <param name="req"><see cref="HttpRequestMessage"/> instance.</param>
        /// <param name="starter"><see cref="IDurableOrchestrationClient"/> instance.</param>
        /// <returns>Returns the <see cref="HttpResponseMessage"/> instance.</returns>
        [FunctionName("EventSchedulingHttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orchestrations/{orchestrationName}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            string orchestrationName,
            ILogger log)
        {
            var input = await req.Content.ReadAsAsync<EventSchedulingRequest>();

            var instanceId = await starter.StartNewAsync<EventSchedulingRequest>(orchestrationName, instanceId: null, input: input);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
