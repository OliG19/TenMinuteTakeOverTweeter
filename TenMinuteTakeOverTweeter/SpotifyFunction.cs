using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using TenMinuteTakeOverTweeter.Models;

namespace TenMinuteTakeOverTweeter
{
    public static class SpotifyFunction

    {
        private static HttpClient client = new HttpClient();

        static readonly string userId = Environment.GetEnvironmentVariable("UserId");
        static readonly string clientDetails = Environment.GetEnvironmentVariable("ClientDetails");
        static readonly string playlistName = Environment.GetEnvironmentVariable("PlaylistName");

        [FunctionName("SpotifyFunction")]

        public static async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            //Get Access Token
            var accessToken = await GetToken();

            //Get a list of a specified users playlist   
            var playlistId = await GetUsersPlaylists(accessToken);

            //Get all tracks from a playlist
            await GetPlaylistTracks(playlistId);
        }

        private static async Task<AccessToken> GetToken()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://accounts.spotify.com/api/token"));

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(clientDetails)));

            var requestData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            requestMessage.Content = new FormUrlEncodedContent(requestData);

            var response = await client.SendAsync(requestMessage);
            var result = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<AccessToken>(result);
        }

        private static async Task<string> GetUsersPlaylists(AccessToken accessToken)
        {
            var requestPlaylistMessage = new HttpRequestMessage(HttpMethod.Get,
                new Uri($"https://api.spotify.com/v1/users/{userId}/playlists"));

            requestPlaylistMessage.Headers.Authorization = new AuthenticationHeaderValue(accessToken.token_type, accessToken.access_token);

            var playlistResponse = await client.SendAsync(requestPlaylistMessage);
            var result = playlistResponse.Content.ReadAsStringAsync().Result;

            var playlist = JsonConvert.DeserializeObject<PlaylistModel>(result);

            //returns the id of the playlist which is passed in
            return playlist.Items.Where(i => i.name == playlistName).Select(n => n.id).FirstOrDefault();
        }

        private static Task GetPlaylistTracks(string playlistId)
        {
            throw new NotImplementedException();
        }
    }
}
