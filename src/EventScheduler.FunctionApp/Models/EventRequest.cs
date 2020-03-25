using Newtonsoft.Json;

namespace EventScheduler.FunctionApp.Models
{
    /// <summary>
    /// This represents the request entity for event.
    /// </summary>
    public class EventRequest : ClientPayload
    {
        /// <summary>
        /// Gets or sets the owner or organisation of the repository.
        /// </summary>
        [JsonProperty("owner")]
        public virtual string Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        [JsonProperty("repository")]
        public virtual string Repository { get; set; }
    }
}
