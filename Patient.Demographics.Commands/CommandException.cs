using System;
using System.Net;

namespace Patient.Demographics.Commands
{
    [Serializable]
    public class CommandException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public CommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public CommandException(string message, Exception innerException, HttpStatusCode statusCode)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
