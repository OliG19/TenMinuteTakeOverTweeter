using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Spotify.API.NetCore;
using TenMinuteTakeOverTweeter.Clients;
using TenMinuteTakeOverTweeter.Models;
using TenMinuteTakeOverTweeter.Models.Spotify;

namespace TenMinuteTakeOverTweeter
{
    public static class SpotifyFunction
    {
        private static readonly SpotifyClient SpotifyClient = new SpotifyClient();
        private static readonly TwitterClient TwitterClient = new TwitterClient();


        [FunctionName("SpotifyFunction")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var tracksToTweet = await SpotifyClient.GetSpotifyUserDetailsAsync();

            //TODO: Tweet tracks from spotify
            await TwitterClient.GetToken();
        }
    }
}
