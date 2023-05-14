namespace VkWallPostgre.Infostracture
{
    public class VkOpenIdHelper
    {
        public string ClientId { get; }
        public string ClientSecret { get; }

        public VkOpenIdHelper(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public VkOpenIdHelper(OpenIdParams openIdParams)
            : this(openIdParams.ClientId, openIdParams.ClientSecret) { }

        public string BuildCodeFlowString(string redirectUri)
        {
            var queries = new KeyValuePair<string, string?>[]
            {
                new KeyValuePair<string, string?>("client_id", ClientId),
                new KeyValuePair<string, string?>("display", "page"),
                new KeyValuePair<string, string?>("redirect_uri", redirectUri),
                new KeyValuePair<string, string?>("scope", "wall"),
                new KeyValuePair<string, string?>("response_type", "code"),
                new KeyValuePair<string, string?>("state", new Random().Next().ToString())

            };
            var queriesString = QueryString.Create(queries);

            return $"https://oauth.vk.com/authorize{queriesString}";
        }

        public string BuildTokenByCodeString(string redirectUri, string code)
        {
            var queries = new KeyValuePair<string, string?>[]
            {
                new KeyValuePair<string, string?>("client_id", ClientId),
                new KeyValuePair<string, string?>("client_secret", ClientSecret),
                new KeyValuePair<string, string?>("redirect_uri", redirectUri),
                new KeyValuePair<string, string?>("code", code),
            };
            var queriesString = QueryString.Create(queries);

            return $"https://oauth.vk.com/access_token{queriesString}";
        }
    }
}
