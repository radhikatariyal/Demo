using System;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class IdentityUserResultException : ApplicationException
    {
        private readonly IdentityResult _identityResult;

        public IdentityUserResultException(IdentityResult identityResult, string message = "") : base(message)
        {
            _identityResult = identityResult;
        }

        public override string Message
        {
            get
            {
                string message = base.Message;

                if (_identityResult?.Errors != null && _identityResult.Errors.Any())
                {
                    message += " Specific error details: " + string.Join("; ", _identityResult.Errors.ToArray());
                }

                return message;
            }
        }
    }
}
