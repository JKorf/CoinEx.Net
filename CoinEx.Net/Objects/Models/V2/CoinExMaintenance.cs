using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Maintenance info
    /// </summary>
    public record CoinExMaintenance
    {
        /// <summary>
        /// Start time of the maintenance
        /// </summary>
        [JsonPropertyName("started_at")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// End time of the maintenance
        /// </summary>
        [JsonPropertyName("ended_at")]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// Scope that's impacted
        /// </summary>
        [JsonPropertyName("scope")]
        public IEnumerable<string> Scope { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Protection period start time. The protection period refers to a continuous period following the system maintenance (It's an optional configuration, and may or may not be set). During the protection period, you can cancel orders, place orders (limited to Maker Only Limit Orders), and adjust (add or reduce) margins.
        /// </summary>
        [JsonPropertyName("protect_duration_start")]
        public DateTime? ProtectDurationStart { get; set; }
        /// <summary>
        /// Protection period end time. The protection period refers to a continuous period following the system maintenance (It's an optional configuration, and may or may not be set). During the protection period, you can cancel orders, place orders (limited to Maker Only Limit Orders), and adjust (add or reduce) margins.
        /// </summary>
        [JsonPropertyName("protect_duration_end")]
        public DateTime? ProtectDurationEnd { get; set; }
    }
}
