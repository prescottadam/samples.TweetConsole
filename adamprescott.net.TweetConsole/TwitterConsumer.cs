namespace adamprescott.net.TweetConsole
{
    using DotNetOpenAuth.Messaging;
    using DotNetOpenAuth.OAuth;
    using DotNetOpenAuth.OAuth.ChannelElements;
    using System.Collections.Generic;
    using System.Net;

    public class TwitterConsumer
    {
        private string _requestToken = string.Empty;

        public DesktopConsumer Consumer { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        public TwitterConsumer(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;

            var providerDescription = new ServiceProviderDescription
            {
                RequestTokenEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/request_token", HttpDeliveryMethods.PostRequest),
                UserAuthorizationEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/authorize", HttpDeliveryMethods.GetRequest),
                AccessTokenEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/access_token", HttpDeliveryMethods.GetRequest),
                TamperProtectionElements = new ITamperProtectionChannelBindingElement[] 
                {
                    new HmacSha1SigningBindingElement()
                }
            };

            Consumer = new DesktopConsumer(
                providerDescription,
                new TokenManager(ConsumerKey, ConsumerSecret));
            return;
        }

        public string BeginAuth()
        {
            var requestArgs = new Dictionary<string, string>();
            return Consumer.RequestUserAuthorization(requestArgs, null, out _requestToken).AbsoluteUri;
        }

        public string CompleteAuth(string verifier)
        {
            var response = Consumer.ProcessUserAuthorization(_requestToken, verifier);
            return response.AccessToken;
        }

        public HttpWebRequest PrepareAuthorizedRequest(MessageReceivingEndpoint endpoint, string accessToken, IEnumerable<MultipartPostPart> parts)
        {
            return Consumer.PrepareAuthorizedRequest(endpoint, accessToken, parts);
        }

        public IConsumerTokenManager TokenManager
        {
            get
            {
                return Consumer.TokenManager;
            }
        }
    }
}
