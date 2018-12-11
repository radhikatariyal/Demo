using System;

namespace Patient.Demographics.Infrastructure
{
    [Serializable]
    public class ConcurrencyException : ApplicationException
    {
         public ConcurrencyException(string message) : base(message){ }
    }
}