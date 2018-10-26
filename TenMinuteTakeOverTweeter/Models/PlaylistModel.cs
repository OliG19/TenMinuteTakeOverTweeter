using System.Collections.Generic;

namespace TenMinuteTakeOverTweeter.Models
{
    public class PlaylistModel
    {
        public IEnumerable<PlayListItems> Items { get; set; }
    }

    public class PlayListItems
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
