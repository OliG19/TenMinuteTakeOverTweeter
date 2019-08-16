using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TenMinuteTakeOverTweeter.Models;
using TenMinuteTakeOverTweeter.Models.Spotify;

namespace TenMinuteTakeOverTweeter.Clients
{

    public class SpotifyClient
    {
        private static HttpClient client = new HttpClient();

        static readonly string userId = Environment.GetEnvironmentVariable("UserId");
        static readonly string clientId = Environment.GetEnvironmentVariable("ClientId");
        static readonly string clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        static readonly string playlistName = Environment.GetEnvironmentVariable("PlaylistName");
        private static readonly string URL = "https://api.spotify.com/v1";
        private static AccessToken accessToken = new AccessToken();

        public async Task<IEnumerable<SpotifyTrack>> GetSpotifyUserDetailsAsync()
        {
            //Get Access Token
            await GetToken();

            //Get a list of a specified users playlist   
            var playlists = await GetUsersPlaylists();

            //Get all tracks from a playlist
            return await GetPlaylistTracks(playlists);
        }

        private static async Task<IEnumerable<SpotifyTrack>> GetPlaylistTracks(PlaylistInformation playlists)
        {
            var playlistId = playlists.Details.Where(playlist => playlist.Name == playlistName)
                .Select(item => item.Id).Single();

            var requestPlaylistTracks = new HttpRequestMessage(HttpMethod.Get, new Uri($"{URL}/playlists/{playlistId}/tracks"));

            requestPlaylistTracks.Headers.Authorization = new AuthenticationHeaderValue(accessToken.token_type, accessToken.access_token);

            var playlistTrackResponse = await client.SendAsync(requestPlaylistTracks);
            var result = playlistTrackResponse.Content.ReadAsStringAsync().Result;

            var tracks = JsonConvert.DeserializeObject<TrackInformation>(result);

            return tracks.Details.Select(trackDetails => new SpotifyTrack
                {
                    Name = trackDetails.Songs.Name,
                    Artist = trackDetails.Songs.Artists.First().Name
                })
                .ToList();
        }


        private static async Task<PlaylistInformation> GetUsersPlaylists()
        {
            var requestPlaylistMessage = new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{URL}/users/{userId}/playlists"));

            requestPlaylistMessage.Headers.Authorization = new AuthenticationHeaderValue(accessToken.token_type, accessToken.access_token);

            var playlistResponse = await client.SendAsync(requestPlaylistMessage);
            var result = playlistResponse.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<PlaylistInformation>(result);     
        }

        private static async Task GetToken()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://accounts.spotify.com/api/token"));
            var creds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            var requestData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            requestMessage.Headers.Add("Authorization", $"Basic {creds}");
            requestMessage.Content = new FormUrlEncodedContent(requestData);

            var response = await client.SendAsync(requestMessage);
            var result = response.Content.ReadAsStringAsync().Result;

            accessToken = JsonConvert.DeserializeObject<AccessToken>(result);
        }
    }
}
