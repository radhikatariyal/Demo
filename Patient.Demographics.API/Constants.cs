namespace Patient.Demographics.Web
{
    public static class Constants
    {
        public static class Cookies
        {
            public const string ACCESSTOKEN = "access_token";
            public const string X_XSRF = "X-XSRF-Cookie";
        }

        public static class Tokens
        {
            public const string X_XSRF = "X-XSRF-Token";
        }

        public static class Claims
        {
            public const string IMPERSONATOR_ID = "impersonator-id";
            public const string DESTINATION_URL = "destination-url";
        }
    }
}