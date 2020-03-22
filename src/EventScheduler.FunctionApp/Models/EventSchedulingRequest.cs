using System;

using Newtonsoft.Json;

namespace EventScheduler.FunctionApp.Models
{
    /// <summary>
    /// This represents the request entity for scheduling.
    /// </summary>
    public class EventSchedulingRequest : ClientPayload
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

        /// <summary>
        /// Gets or sets the PR issue ID to merge.
        /// </summary>
        [JsonProperty("issueId")]
        public virtual int IssueId { get; set; }

        /// <summary>
        /// Gets or sets the schedule to merge.
        /// </summary>
        [JsonProperty("schedule")]
        public virtual DateTimeOffset Schedule { get; set; }
    }
}
