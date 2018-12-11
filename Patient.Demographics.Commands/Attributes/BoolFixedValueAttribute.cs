using System.ComponentModel.DataAnnotations;

namespace Patient.Demographics.Commands.Attributes
{
    public class BoolFixedValueAttribute : ValidationAttribute
    {
        private readonly  bool _value;

        public BoolFixedValueAttribute(bool value, string errorMessage) : base(errorMessage)
        {
            _value = value;
        }

        public override bool IsValid(object value)
        {
            return (bool)value == _value;
        }
    }
}