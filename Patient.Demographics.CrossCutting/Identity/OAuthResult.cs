using System;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class OAuthResult
    {
        public string access_token { get; set; }
        public string token_type => "bearer";
        public string expires_in { get; set; }
        public string username { get; set; }
        public Guid userid { get; set; }
        public bool isAdmin { get; set; }
        public bool isPasswordExpired { get; set; }
    }
}