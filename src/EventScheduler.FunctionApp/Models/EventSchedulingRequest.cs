using System;

using Newtonsoft.Json;

namespace EventScheduler.FunctionApp.Models
{
    /// <summary>
    /// This represents the request entity for scheduling.
    /// </summary>
    public class EventSchedulingRequest : EventRequest
    {
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
