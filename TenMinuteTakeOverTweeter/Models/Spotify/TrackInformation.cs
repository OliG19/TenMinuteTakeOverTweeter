using Newtonsoft.Json;
using System.Collections.Generic;

namespace TenMinuteTakeOverTweeter.Models
{
    public class TrackInformation
    {
        [JsonProperty("items")]
        public IEnumerable<TrackDetails> Details { get; set; }


        public class TrackDetails
        {
            [JsonProperty("track")]
            public SongDetails Songs { get; set; }

            public class SongDetails
            {
                public IEnumerable<ArtistDetails> Artists { get; set; }
                public string Name { get; set; }

                public class ArtistDetails
                {
                    public string Name { get; set; }
                }
            }
        }
    }
}
