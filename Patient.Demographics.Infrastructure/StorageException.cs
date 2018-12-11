using System;

namespace Patient.Demographics.Infrastructure
{
    [Serializable]
    public class StorageException : ApplicationException
    {
        public StorageException() { }
        public StorageException(string message) : base(message) { }
        public StorageException(string message, Exception inner) : base(message, inner) { }
    }
}
