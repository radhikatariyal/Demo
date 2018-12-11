using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.CrossCutting.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Patient.Demographics.Data.Identity
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly double _refreshTokenLifeTime;
        private readonly Func<IRefreshTokenService> _refreshTokenServiceProvider;
        public RefreshTokenProvider(double refreshTokenLifeTime, Func<IRefreshTokenService> refreshTokenServiceProvider)
        {
            _refreshTokenLifeTime = refreshTokenLifeTime;
            _refreshTokenServiceProvider = refreshTokenServiceProvider;
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {

            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];
            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }
            var idClaim = context.Ticket.Identity.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(idClaim.Value);

            var issuedUtc = DateTime.UtcNow;
            var expiresUtc = DateTime.UtcNow.AddMinutes(_refreshTokenLifeTime);

            context.Ticket.Properties.IssuedUtc = issuedUtc;
            context.Ticket.Properties.ExpiresUtc = expiresUtc;
            string protectedTicket = context.SerializeTicket();

            var refreshTokenService = _refreshTokenServiceProvider();
            try
            {
                await refreshTokenService.RevokeTokens(userId, clientid);

                var refreshTokenId =
                    await refreshTokenService.CreateToken(context.Ticket.Identity.Name, clientid, issuedUtc,
                        expiresUtc, protectedTicket, userId);
                context.SetToken(refreshTokenId.ToString());
            }
            finally
            {
                refreshTokenService.Dispose();
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var refreshTokenService = _refreshTokenServiceProvider();

            try
            {
                //var token = await refreshTokenService.RetrieveToken(new Guid(context.Token));
                //context.DeserializeTicket(token.ProtectedTicket);
            }
            catch (RefreshTokenExpiredException)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "expired token";
            }
            catch (InvalidRefreshTokenException)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "invalid token";
            }
            finally
            {
                refreshTokenService.Dispose();
            }


        }
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}