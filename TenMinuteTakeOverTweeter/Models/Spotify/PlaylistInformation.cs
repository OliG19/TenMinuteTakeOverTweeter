using System.Collections.Generic;
using Newtonsoft.Json;

namespace TenMinuteTakeOverTweeter.Models
{
    public class PlaylistInformation
    {
        [JsonProperty("items")]
        public IEnumerable<PlayListDetails> Details { get; set; }
    }

    public class PlayListDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
