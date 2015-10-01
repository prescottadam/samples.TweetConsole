namespace adamprescott.net.TweetConsole
{
    using DotNetOpenAuth.Messaging;
    using System;
    using System.Diagnostics;

    class Program
    {
        const string _consumerKey = "";     // your application's consumer key
        const string _consumerSecret = "";  // your application's consumer key
        private readonly TwitterConsumer _twitter;

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        public Program()
        {
            _twitter = new TwitterConsumer(_consumerKey, _consumerSecret);
        }

        void Run()
        {
            var url = _twitter.BeginAuth();
            Process.Start(url);
            Console.Write("Enter PIN: ");
            var pin = Console.ReadLine();
            var accessToken = _twitter.CompleteAuth(pin);

            while (true)
            {
                Console.Write("Tweet ('x' to exit) /> ");
                var tweet = Console.ReadLine();
                if (string.Equals("x", tweet, StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
                Tweet(accessToken, tweet);
            }
        }

        void Tweet(string accessToken, string message)
        {
            var endpoint = new MessageReceivingEndpoint(
                "https://api.twitter.com/1.1/statuses/update.json",
                HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);

            var parts = new[]
            {
                MultipartPostPart.CreateFormPart("status", message)
            };

            var request = _twitter.PrepareAuthorizedRequest(endpoint, accessToken, parts);

            var response = request.GetResponse();
        }
    }
}
