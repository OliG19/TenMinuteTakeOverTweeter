using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TenMinuteTakeOverTweeter.Models;
using TenMinuteTakeOverTweeter.Models.Spotify;

namespace TenMinuteTakeOverTweeter.Services
{

    public class SpotifyService
    {

        private static readonly string UserId = Environment.GetEnvironmentVariable("SpotifyUserId");
        private static readonly string ClientId = Environment.GetEnvironmentVariable("SpotifyClientId");
        private static readonly string ClientSecret = Environment.GetEnvironmentVariable("SpotifyClientSecret");
        private static readonly string PlaylistName = Environment.GetEnvironmentVariable("PlaylistName");
        private const string Url = "https://api.spotify.com/v1";
        private const string TokenUrl = "https://accounts.spotify.com/api/token";
        private static readonly HttpClient Client = new HttpClient();
        private static AccessToken _accessToken;

        public async Task<IEnumerable<SpotifyTrack>> GetSpotifyUserDetailsAsync()
        {
            _accessToken = await GetTokenAsync();

            var playlists = await GetUsersPlaylistsAsync();
            
            return await GetPlaylistTracksAsync(playlists);
        }

        private static async Task<IEnumerable<SpotifyTrack>> GetPlaylistTracksAsync(PlaylistInformation playlists)
        {
            var playlistId = playlists.Details.FirstOrDefault(playlist => playlist.Name == PlaylistName).Id;

            var requestPlaylistTracks =
                new HttpRequestMessage(HttpMethod.Get, new Uri($"{Url}/playlists/{playlistId}/tracks"));

            requestPlaylistTracks.Headers.Authorization =
                new AuthenticationHeaderValue(_accessToken.token_type, _accessToken.access_token);

            var playlistTrackResponse = await Client.SendAsync(requestPlaylistTracks);
            var result = playlistTrackResponse.Content.ReadAsStringAsync().Result;

            var tracks = JsonConvert.DeserializeObject<TrackInformation>(result);

            return tracks.Details.Select(trackDetails => new SpotifyTrack
                {
                    Name = trackDetails.Songs.Name,
                    Artist = trackDetails.Songs.Artists.First().Name
                })
                .ToList();
        }

        private static async Task<PlaylistInformation> GetUsersPlaylistsAsync()
        {
            var requestPlaylistMessage = new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Url}/users/{UserId}/playlists"));

            requestPlaylistMessage.Headers.Authorization =
                new AuthenticationHeaderValue(_accessToken.token_type, _accessToken.access_token);

            var playlistResponse = await Client.SendAsync(requestPlaylistMessage);
            var result = playlistResponse.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<PlaylistInformation>(result);
        }

        private async Task<AccessToken> GetTokenAsync()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(TokenUrl));
            var creds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));
            var requestData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            requestMessage.Headers.Add("Authorization", $"Basic {creds}");
            requestMessage.Content = new FormUrlEncodedContent(requestData);

            var response = await Client.SendAsync(requestMessage);
            var result = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<AccessToken>(result);
        }
    }
}
