using System;

namespace Patient.Demographics.Infrastructure
{
    [Serializable]
    public class RepositoryException : ApplicationException
    {
        public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
