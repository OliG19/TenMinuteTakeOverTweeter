using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading.Tasks;
using TenMinuteTakeOverTweeter.Services;

namespace TenMinuteTakeOverTweeter
{
    public static class SpotifyFunction
    {
        private static readonly SpotifyService SpotifyService = new SpotifyService();
        private static readonly TwitterService TwitterService = new TwitterService();

        [FunctionName("SpotifyFunction")]
        public static async Task Run([TimerTrigger("55 59 8 ? * * *")] TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var tracks = await SpotifyService.GetSpotifyUserDetailsAsync();

            TwitterService.SendTweet(tracks);
        }
    }
}
