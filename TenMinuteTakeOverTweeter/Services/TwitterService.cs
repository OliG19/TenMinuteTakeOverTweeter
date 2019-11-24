using System;
using System.Collections.Generic;
using System.Text;
using TenMinuteTakeOverTweeter.Models.Spotify;
using TweetSharp;

namespace TenMinuteTakeOverTweeter.Services
{
    public class TwitterService
    {
        private static readonly string ConsumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey");
        private static readonly string ConsumerSecretKey = Environment.GetEnvironmentVariable("TwitterConsumerKeySecret");
        private static readonly string TwitterToken = Environment.GetEnvironmentVariable("TwitterToken");
        private static readonly string TwitterTokenSecret = Environment.GetEnvironmentVariable("TwitterTokenSecret");

        public void SendTweet(IEnumerable<SpotifyTrack> tracks)
        {
            var service = new TweetSharp.TwitterService(ConsumerKey, ConsumerSecretKey);

            service.AuthenticateWith(TwitterToken, TwitterTokenSecret);

            var listOfTracks = new StringBuilder();

            foreach (var track in tracks)
            {
                track.Artist = track.Artist.Replace(",", "");
                track.Name = track.Name.Replace(",", "");

                if (listOfTracks.Length > 140)
                {
                    service.SendTweet(new SendTweetOptions {Status = $"@BBCR1 ,{listOfTracks}"});
                    listOfTracks.Clear();
                    continue;
                }

                listOfTracks.Append(track.Artist);
                listOfTracks.Append("- ");
                listOfTracks.Append(track.Name);
                listOfTracks.Append(", ");
            }
        }
    }
}
