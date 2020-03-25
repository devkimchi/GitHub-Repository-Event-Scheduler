using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
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
    /// This represents the trigger entity for event scheduling on GitHub.
    /// </summary>
    public class RepositoryDispatchEventTrigger
    {
        private readonly JsonMediaTypeFormatter _formatter;
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDispatchEventTrigger"/> class.
        /// </summary>
        /// <param name="formatter"><see cref="JsonMediaTypeFormatter"/> instance.</param>
        /// <param name="client"><see cref="HttpClient"/> instance.</param>
        public RepositoryDispatchEventTrigger(JsonMediaTypeFormatter formatter, HttpClient client)
        {
            this._formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            this._client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Invokes the activity trigger for event scheduling on GitHub.
        /// </summary>
        /// <param name="input"><see cref="EventSchedulingRequest"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns the output payload.</returns>
        [FunctionName("CallMergePrRepositoryDispatchEvent")]
        public async Task<object> MergePr(
            [ActivityTrigger] EventSchedulingRequest input,
            ILogger log)
        {
            var payload = await this.CallRepositoryDispatchEvent("merge-pr", input);

            return payload;
        }

        /// <summary>
        /// Invokes the HTTP trigger for event publishing on GitHub.
        /// </summary>
        /// <param name="input"><see cref="HttpRequest"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns the output payload.</returns>
        [FunctionName("CallPublishRepositoryDispatchEvent")]
        public async Task<IActionResult> Publish(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "events/publish")] HttpRequest req,
            ILogger log)
        {
            var input = JsonConvert.DeserializeObject<EventRequest>(await new StreamReader(req.Body).ReadToEndAsync());

            var payload = await this.CallRepositoryDispatchEvent("publish", input);

            var result = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(payload, this._formatter.SerializerSettings),
                StatusCode = (int) HttpStatusCode.OK,
                ContentType = "application/json"
            };

            return result;
        }

        private async Task<RepositoryDispatchEventRequest<T>> CallRepositoryDispatchEvent<T>(string eventType, T input) where T : EventRequest
        {
            var authKey = Environment.GetEnvironmentVariable("GitHub__AuthKey");
            var requestUri = $"{Environment.GetEnvironmentVariable("GitHub__BaseUri").TrimEnd('/')}/repos/{input.Owner}/{input.Repository}/{Environment.GetEnvironmentVariable("GitHub__Endpoints__Dispatches").TrimStart('/')}";
            var accept = Environment.GetEnvironmentVariable("GitHub__Headers__Accept");
            var userAgent = Environment.GetEnvironmentVariable("GitHub__Headers__UserAgent");

            this._client.DefaultRequestHeaders.Clear();
            this._client.DefaultRequestHeaders.Add("Authorization", authKey);
            this._client.DefaultRequestHeaders.Add("Accept", accept);
            this._client.DefaultRequestHeaders.Add("User-Agent", userAgent);

            var payload = new RepositoryDispatchEventRequest<T>(eventType, input);

            using (var content = new ObjectContent<RepositoryDispatchEventRequest<T>>(payload, this._formatter, "application/json"))
            using (var response = await this._client.PostAsync(requestUri, content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
            }

            return payload;
        }
    }
}
