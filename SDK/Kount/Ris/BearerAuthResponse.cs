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
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Token Type
        /// </summary>
        public string TokenType { get; set; }
        
        /// <summary>
        /// Expires in (seconds)
        /// </summary>
        public string ExpiresIn { get; set; }
        
        /// <summary>
        /// Scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Construct BearerAuthResponse
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="tokenType"></param>
        /// <param name="expiresIn"></param>
        /// <param name="scope"></param>
        public BearerAuthResponse(string accessToken, string tokenType, string expiresIn, string scope)
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