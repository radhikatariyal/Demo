using Patient.Demographics.Common;
using System;

namespace Patient.Demographics.Domain
{
    [Serializable]
    public class Address
    {
        public Guid Id { get; private set; }
        public string Line1 { get; private set; }
        public string Line2 { get; private set; }
        public string Line3 { get; private set; }
        public string Town { get; private set; }
        public string County { get; private set; }
        public string PostCode { get; private set; }
        public string CountryIsoCode { get; private set; }
   
        public static Address CreateNew(string line1, string line2, string line3, string town, string county, string postCode, string countryIsoCode)
        {
            return new Address(Identifier.NewId(), line1, line2, line3, town, county, postCode, countryIsoCode);
        }

        public static Address CreateExisting(Guid id, string line1, string line2, string line3, string town, string county, string postCode, string countryIsoCode)
        {
            return new Address(id, line1, line2, line3, town, county, postCode, countryIsoCode);
        }

        public bool TryUpdate(string line1, string line2, string line3, string town, string county, string postCode, string countryIsoCode)
        {
            if (NeedsUpdating(line1, line2, line3, town, county, postCode, countryIsoCode))
            {
                Line1 = line1;
                Line2 = line2;
                Line3 = line3;
                Town = town;
                County = county;
                PostCode = postCode;
                CountryIsoCode = countryIsoCode;
                return true;
            }

            return false;
        }

        private Address(Guid id, string line1, string line2, string line3, string town, string county, string postCode, string countryIsoCode)
        {
            Id = id;
            Line1 = line1;
            Line2 = line2;
            Line3 = line3;
            Town = town;
            County = county;
            PostCode = postCode;
            CountryIsoCode = countryIsoCode;
        }

        private bool NeedsUpdating(string line1, string line2, string line3, string town, string county, string postCode, string countryIsoCode)
        {
            return line1 != Line1 ||
                line2 != Line2 ||
                line3 != Line3 ||
                town != Town ||
                county != County ||
                postCode != PostCode ||
                countryIsoCode != CountryIsoCode;
        }
    }
}
