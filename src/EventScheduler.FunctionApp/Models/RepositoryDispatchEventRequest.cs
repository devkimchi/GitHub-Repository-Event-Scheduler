using System;

using Newtonsoft.Json;

namespace EventScheduler.FunctionApp.Models
{
    /// <summary>
    /// This represents the entity for the repository dispatch event on GitHub.
    /// </summary>
    /// <typeparam name="T">Type of client payload.</typeparam>
    public class RepositoryDispatchEventRequest<T> where T : ClientPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDispatchEventRequest"/> class.
        /// </summary>
        public RepositoryDispatchEventRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDispatchEventRequest"/> class.
        /// </summary>
        /// <param name="eventType">Event type value.</param>
        /// <param name="clientPayload">Client payload.</param>
        public RepositoryDispatchEventRequest(string eventType, T clientPayload)
        {
            this.EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            this.ClientPayload = clientPayload ?? throw new ArgumentNullException(nameof(clientPayload));
        }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        [JsonProperty("event_type")]
        public virtual string EventType { get; set; }

        /// <summary>
        /// Gets or sets the client payload.
        /// </summary>
        [JsonProperty("client_payload")]
        public virtual T ClientPayload { get; set; }
    }
}
