using System;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.ServiceModel.Security.Tokens;
using Patient.Demographics.Common.Validation;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Thinktecture.IdentityModel.Tokens;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class JwtFormatter : ISecureDataFormat<AuthenticationTicket>
    {

        private readonly string _issuer;
        private readonly string _audienceId;
        private readonly string _audienceSecret;

        public JwtFormatter(string issuer, string audienceId, string audienceSecret)
        {
            _issuer = issuer;
            _audienceId = audienceId;
            _audienceSecret = audienceSecret;
        }

        public string Protect(AuthenticationTicket data)
        {
            ArgumentValidator.EnsureIsNotNull(data, nameof(data));

            var keyByteArray = TextEncodings.Base64Url.Decode(_audienceSecret);
            var signingKey = new HmacSigningCredentials(keyByteArray);
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;
            var token = new JwtSecurityToken(_issuer, _audienceId, data.Identity.Claims, issued.Value.UtcDateTime,
                expires.Value.UtcDateTime, signingKey);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            return jwt;
        }


        [DebuggerNonUserCode]
        public AuthenticationTicket Unprotect(string protectedText)
        {
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningToken = new BinarySecretSecurityToken(TextEncodings.Base64Url.Decode(_audienceSecret)),
                ValidAudience = _audienceId,
                ValidIssuer = _issuer,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromSeconds(5)
            };
            try {
                var handler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken = null;
                var claimsPrincipal = handler.ValidateToken(protectedText, validationParameters, out validatedToken);
                return new AuthenticationTicket(claimsPrincipal.Identity as ClaimsIdentity,
                    new AuthenticationProperties());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}