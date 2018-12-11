using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Patient.Demographics.Common
{
    [Serializable]
    public class OauthErrorMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("modelState")]
        public Dictionary<string,List<string>> ModelState { get; set; }
    }
}