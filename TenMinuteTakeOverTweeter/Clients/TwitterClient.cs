using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TenMinuteTakeOverTweeter.Clients
{
    public class TwitterClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly string ConsumerKey = "8IVqZ9ID5Sbw3e4k78DtkjUsX";
        private static readonly string ConsumerSecretKey = "412xE36zl7WCoXvksK4i8eZDix66l0Uk9oSFtgdNhDgOnk7NjX";
        private static AccessToken accessToken = new AccessToken();

        public static async Task GetToken()
        {
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://api.twitter.com/oauth2/token"));
            var creds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ConsumerKey}:{ConsumerSecretKey}"));

            var requestData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            requestMessage.Headers.Add("Authorization", $"Basic {creds}");
            requestMessage.Content = new FormUrlEncodedContent(requestData);

            var response = await HttpClient.SendAsync(requestMessage);
            var result = response.Content.ReadAsStringAsync().Result;

            accessToken = JsonConvert.DeserializeObject<AccessToken>(result);
        }
    }
}
