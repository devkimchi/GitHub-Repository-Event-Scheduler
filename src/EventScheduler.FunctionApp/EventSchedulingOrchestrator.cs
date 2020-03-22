using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using EventScheduler.FunctionApp.Models;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

using Microsoft.Extensions.Logging;

namespace EventScheduler.FunctionApp
{
    /// <summary>
    /// This represents the orchestrator entity for event scheduling on GitHub.
    /// </summary>
    public class EventSchedulingOrchestrator
    {
        private static TimeSpan threshold = new TimeSpan(7, 0, 0, 0);

        /// <summary>
        /// Invokes the orchestrator for event scheduling on GitHub.
        /// </summary>
        /// <param name="context"><see cref="IDurableOrchestrationContext"/> instance.</param>
        /// <returns>Returns the list of string.</returns>
        [FunctionName("schedule-event")]
        public async Task<object> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var input = context.GetInput<EventSchedulingRequest>();

            // Set the maximum duration. Max duration can't exceed 7 days.
            var maxDuration = TimeSpan.Parse(Environment.GetEnvironmentVariable("Duration__Max"), CultureInfo.InvariantCulture);
            if (maxDuration > threshold)
            {
                return "Now allowed";
            }

            // Get the scheduled time
            var scheduled = input.Schedule.UtcDateTime;

            // Get the function initiated time.
            var initiated = context.CurrentUtcDateTime;

            // Get the difference between now and schedule
            var datediff = (TimeSpan)(scheduled - initiated);

            // Complete if datediff is longer than the max duration
            if (datediff >= maxDuration)
            {
                return "Too far away";
            }

            await context.CreateTimer(scheduled, CancellationToken.None);

            var output = await context.CallActivityAsync<object>("CallRepositoryDispatchEvent", input);

            return output;
        }
    }
}
