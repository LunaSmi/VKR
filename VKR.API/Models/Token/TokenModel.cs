namespace VKR.API.Models.Token
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public TokenModel(string accessToken, string refreshtoken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshtoken;
        }
    }
}
