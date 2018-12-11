//using System;
//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;
//using Patient.Demographics.Domain.Menu;
//using Patient.Demographics.Data.Entities;

//namespace Patient.Demographics.Infrastructure.AutoMappers
//{
//    public class MenuTypeConverter : ITypeConverter<MenuItemEntity, Menu>
//    {
//        public Menu Convert(MenuItemEntity source, Menu destination, ResolutionContext context)
//        {
//            var translations = source.Translations.ToDictionary(k=>k.CultureCode, k=>k.Name);
           
//           var children=context.Mapper.Map<ICollection<MenuItemEntity>,ICollection<MenuItem>>(source.Children).ToList();
//            var audienceIds = new List<Guid>();

//            return null;
//        }
//    }

//}