using System;

namespace Patient.Demographics.Common.Validation
{
    public static class ArgumentValidator
    {
        public static void EnsureIsNotNull(object argument, string paramName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void EnsureIsNotNullOrWhitespace(string argument, string paramName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException(paramName, "Argument cannot be null or whitespace");
            }
        }

        public static void EnsureIsValid(Func<bool> validator, string paramName, string message)
        {
            if (!validator())
            {
                throw new ArgumentException(message, paramName);
            }
        }

        public static void EnsureIsNotEmptyGuid(Guid argument, string paramName)
        {
            if (argument == Guid.Empty)
            {
                throw new ArgumentException("Argument cannot be an empty Guid", paramName);
            }
        }
    }
}
