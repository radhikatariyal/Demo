using System;

namespace Patient.Demographics.CrossCutting.Security
{
    [Serializable]
    public class RefreshTokenExpiredException : ApplicationException
    {
        public RefreshTokenExpiredException(string message) : base(message)
        {
        }
    }
}