using System;
using System.ComponentModel;
using System.Globalization;

namespace Patient.Demographics.Common.TypeConverters
{
    public class EnumerationTypeConverter<TEnum> : TypeConverter where TEnum : Enumeration, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var valueAsString = value as string;
            if (valueAsString != null)
            {
                return Enumeration.FromName<TEnum>(valueAsString);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
