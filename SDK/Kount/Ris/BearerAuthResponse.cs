using System.Text.Json.Serialization;

namespace Kount.Ris
{
    /// <summary>
    /// Object to store Bearer Auth Response
    /// </summary>
    public class BearerAuthResponse
    {
        /// <summary>
        /// Access Token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Token Type
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        
        /// <summary>
        /// Expires in (seconds)
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        /// <summary>
        /// Scope
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Construct BearerAuthResponse
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="tokenType"></param>
        /// <param name="expiresIn"></param>
        /// <param name="scope"></param>
        public BearerAuthResponse(string accessToken, string tokenType, int expiresIn, string scope)
        {
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
            Scope = scope;
        }
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public BearerAuthResponse()
        {
        }
        
    }
}