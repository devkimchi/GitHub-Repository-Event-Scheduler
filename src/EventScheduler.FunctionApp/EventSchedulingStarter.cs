using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using EventScheduler.FunctionApp.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace EventScheduler.FunctionApp
{
    /// <summary>
    /// This represents the starter entity for event scheduling on GitHub.
    /// </summary>
    public class EventSchedulingStater
    {
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSchedulingStater"/> class.
        /// </summary>
        /// <param name="settings"><see cref="JsonSerializerSettings"/> instance.</param>
        public EventSchedulingStater(JsonSerializerSettings settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Invokes the durable event for event scheduling on GitHub.
        /// </summary>
        /// <param name="req"><see cref="HttpRequestMessage"/> instance.</param>
        /// <param name="starter"><see cref="IDurableOrchestrationClient"/> instance.</param>
        /// <param name="orchestratorName">Orchestrator function name</param>
        /// <returns>Returns the <see cref="HttpResponseMessage"/> instance.</returns>
        [FunctionName("SetSchedule")]
        public async Task<HttpResponseMessage> SetSchedule(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orchestrators/{orchestratorName}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            string orchestratorName,
            ILogger log)
        {
            var input = await req.Content.ReadAsAsync<EventSchedulingRequest>();
            var instanceId = await starter.StartNewAsync<EventSchedulingRequest>(orchestratorName, instanceId: null, input: input);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        /// <summary>
        /// Invokes the durable event for event scheduling status check.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="starter"><see cref="IDurableOrchestrationClient"/> instance.</param>
        /// <param name="orchestratorName">Orchestrator function name</param>
        /// <returns>Returns the <see cref="IActionResult"/> instance.</returns>
        [FunctionName("GetSchedules")]
        public async Task<IActionResult> GetSchedules(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orchestrators/{orchestratorName}/schedules")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            string orchestratorName,
            ILogger log)
        {
            log.LogInformation($"Getting the list of schedules...");

            var status = (await starter.GetStatusAsync())
                         .OrderByDescending(p => p.Input.Value<DateTime>("schedule"));
            var result = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(status, this._settings),
                StatusCode = (int) HttpStatusCode.OK,
                ContentType = "application/json"
            };

            return result;
        }

        /// <summary>
        /// Invokes the durable event for event scheduling status check.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="starter"><see cref="IDurableOrchestrationClient"/> instance.</param>
        /// <param name="orchestratorName">Orchestrator function name</param>
        /// <param name="instanceId">Orchestrator instance ID.</param>
        /// <returns>Returns the <see cref="IActionResult"/> instance.</returns>
        [FunctionName("GetSchedule")]
        public async Task<IActionResult> GetSchedule(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orchestrators/{orchestratorName}/schedules/{instanceId}")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            string orchestratorName,
            string instanceId,
            ILogger log)
        {
            var status = await starter.GetStatusAsync(instanceId);
            var result = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(status, this._settings),
                StatusCode = (int) HttpStatusCode.OK,
                ContentType = "application/json"
            };

            log.LogInformation($"Retrieved the schedule for '{instanceId}'.");

            return result;
        }
    }
}
