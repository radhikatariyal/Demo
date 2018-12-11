using System;
using System.ComponentModel;
using Patient.Demographics.Common.TypeConverters;

namespace Patient.Demographics.Common
{
    [Serializable]
    [TypeConverter(typeof(EnumerationTypeConverter<MarkupOn>))]
    public class MarkupOn : Enumeration
    {
        public static readonly MarkupOn COST_PRICE = new MarkupOn("COST_PRICE", "Markup");
        public static readonly MarkupOn SELLING_PRICE = new MarkupOn("SELLING_PRICE", "Margin");

        public MarkupOn()
        {
        }

        protected MarkupOn(string name, string displayName) : base(name, displayName)
        {
        }
    }
}