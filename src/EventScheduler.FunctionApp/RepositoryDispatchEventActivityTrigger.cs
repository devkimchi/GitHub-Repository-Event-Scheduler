using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

using EventScheduler.FunctionApp.Models;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace EventScheduler.FunctionApp
{
    /// <summary>
    /// This represents the activity trigger entity for event scheduling on GitHub.
    /// </summary>
    public class RepositoryDispatchEventActivityTrigger
    {
        private readonly MediaTypeFormatter _formatter;
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDispatchEventActivityTrigger"/> class.
        /// </summary>
        /// <param name="formatter"><see cref="JsonMediaTypeFormatter"/> instance.</param>
        /// <param name="client"><see cref="HttpClient"/> instance.</param>
        public RepositoryDispatchEventActivityTrigger(JsonMediaTypeFormatter formatter, HttpClient client)
        {
            this._formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            this._client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Invokes the activity trigger for event scheduling on GitHub.
        /// </summary>
        /// <param name="input"><see cref="ScheduleRequest"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns></returns>
        [FunctionName("CallRepositoryDispatchEvent")]
        public async Task<object> RunActivity(
            [ActivityTrigger] EventSchedulingRequest input,
            ILogger log)
        {
            var authKey = Environment.GetEnvironmentVariable("GitHub__AuthKey");
            var requestUri = $"{Environment.GetEnvironmentVariable("GitHub__BaseUri").TrimEnd('/')}/repos/{input.Owner}/{input.Repository}/{Environment.GetEnvironmentVariable("GitHub__Endpoints__Dispatches").TrimStart('/')}";
            var accept = Environment.GetEnvironmentVariable("GitHub__Headers__Accept");
            var userAgent = Environment.GetEnvironmentVariable("GitHub__Headers__UserAgent");

            this._client.DefaultRequestHeaders.Clear();
            this._client.DefaultRequestHeaders.Add("Authorization", authKey);
            this._client.DefaultRequestHeaders.Add("Accept", accept);
            this._client.DefaultRequestHeaders.Add("User-Agent", userAgent);

            var payload = new RepositoryDispatchEventRequest<EventSchedulingRequest>("merge-pr", input);

            using (var content = new ObjectContent<RepositoryDispatchEventRequest<EventSchedulingRequest>>(payload, this._formatter, "application/json"))
            using (var response = await this._client.PostAsync(requestUri, content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
            }

            return payload;
        }
    }
}
