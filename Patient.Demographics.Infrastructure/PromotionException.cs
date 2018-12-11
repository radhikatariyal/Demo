using System;

namespace Patient.Demographics.Infrastructure
{
    [Serializable]
    public class PromotionException : ApplicationException
    {
        public PromotionException()
        {
        }

        public PromotionException(string message) : base(message)
        {
        }

        public PromotionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}