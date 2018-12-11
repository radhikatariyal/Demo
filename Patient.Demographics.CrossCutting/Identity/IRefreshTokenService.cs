using System;
using System.Threading.Tasks;
using Patient.Demographics.Common;
using Patient.Demographics.CrossCutting.Security;

namespace Patient.Demographics.CrossCutting.Identity
{
    public interface IRefreshTokenService:IDisposable
    {
        /// <summary>
        /// Creates a refresh token and stores it in the database
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="clientId"></param>
        /// <param name="issuedUtc"></param>
        /// <param name="expiresUtc"></param>
        /// <param name="protectedTicket"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Guid> CreateToken(string subject, string clientId, DateTime issuedUtc, DateTime expiresUtc, string protectedTicket, Guid userId);
        /// <summary>
        /// Retrieves the token if it is valid and then removes from storage.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns>RefreshToken</returns>
        /// <exception cref="InvalidRefreshTokenException">If token does not exist</exception>
        /// <exception cref="RefreshTokenExpiredException">If the refresh token already expired</exception>
       // Task<RefreshToken> RetrieveToken(Guid tokenId);
        /// <summary>
        /// Revokes the user tokens for a given client application
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task RevokeTokens(Guid userId, string clientId);
    }
}
