using System;

namespace Patient.Demographics.Infrastructure
{
    [Serializable]
    public class EventException : ApplicationException
    {
        public EventException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}