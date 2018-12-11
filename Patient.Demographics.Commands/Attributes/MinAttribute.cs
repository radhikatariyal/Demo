using System.ComponentModel.DataAnnotations;

namespace Patient.Demographics.Commands.Attributes
{
    public class MinAttribute : RangeAttribute
    {
        public MinAttribute(int minimum) : base(minimum, int.MaxValue)
        {

        }
    }
}
