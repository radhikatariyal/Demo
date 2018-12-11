using BIWorldwide.GPSM.Common;

using BIWorldwide.GPSM.Events.Brand;
using System;
using System.Collections.Generic;

namespace BIWorldwide.GPSM.Domain
{
    [Serializable]
    public sealed class Brand : AggregateRoot
    {
        private Brand()
        {

        }

        private Brand(string setId, string brand, string description)
        {            
            SetId = setId;
            BrandName = brand;
            Description = description;;
            State = AggregateState.NotModified;
        }

        public string SetId { get; private set; }

        public string BrandName { get; private set; }
        public string Description { get; private set; }

    

        public static Brand CreateNew(string setId, string brand, string description)
        {
            var brands = new Brand
            {
                SetId = setId,
                BrandName = brand,
                Description = description
            
            };

            brands.LoadEvent(new BrandCreatedEvent(brands.SetId, brand, description));
            brands.State = AggregateState.New;
            return brands;
        }

        //public static Set Update(Guid id, string setId, string name, string description, List<string> countriesIsoCode)
        //{
        //    return new Set(id, setId, name, description, countriesIsoCode);
        //}

        //public void Update(string setId, string name, string description, List<string> countriesIsoCode, List<string> countriesIsoCodeToAdd, List<string> countriesIsoCodeToRemove)
        //{
        //    SetId = setId;
        //    Name = name;
        //    Description = description;
        //    CountriesIsoCode = countriesIsoCode;
        //    LoadEvent(new SetUpdatedEvent(Id, setId, name, description, countriesIsoCodeToAdd, countriesIsoCodeToRemove));
        //    State = AggregateState.Modified;
        //}
        
    }
}
