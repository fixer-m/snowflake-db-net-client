using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class LoginRequestData
    {
        [JsonPropertyName("CLIENT_APP_ID")]
        public string ClientAppId { get; set; }

        [JsonPropertyName("CLIENT_APP_VERSION")]
        public string ClientAppVersion { get; set; }

        [JsonPropertyName("ACCOUNT_NAME")]
        public string AccountName { get; set; }

        [JsonPropertyName("LOGIN_NAME")]
        public string LoginName { get; set; }

        [JsonPropertyName("PASSWORD")]
        public string Password { get; set; }

        [JsonPropertyName("AUTHENTICATOR")]
        public string Authenticator { get; set; }

        [JsonPropertyName("CLIENT_ENVIRONMENT")]
        public LoginRequestClientEnv ClientEnvironment { get; set; }

        [JsonPropertyName("RAW_SAML_RESPONSE")]
        public string RawSamlResponse { get; set; }

        [JsonPropertyName("TOKEN")]
        public string Token { get; set; }

        [JsonPropertyName("PROOF_KEY")]
        public string ProofKey { get; set; }

        //  [JsonPropertyName("SESSION_PARAMETERS")]
        //  public Dictionary<SFSessionParameter, Object> Session_Parameters { get; set; }

        public override string ToString()
        {
            return $"Login: {LoginName}; Account_Name: {AccountName};";
        }
    }
}
