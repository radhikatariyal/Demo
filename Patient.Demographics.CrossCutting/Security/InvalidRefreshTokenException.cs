using System;

namespace Patient.Demographics.CrossCutting.Security
{
    [Serializable]
    public class InvalidRefreshTokenException : ApplicationException
    {
        public InvalidRefreshTokenException(string message) : base(message)
        {
        }
    }
}