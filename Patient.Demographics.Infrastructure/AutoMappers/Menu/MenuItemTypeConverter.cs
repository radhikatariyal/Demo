//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;
//using Patient.Demographics.Domain.Menu;
//using Patient.Demographics.Domain.Navigation;
//using Patient.Demographics.Data.Entities;

//namespace Patient.Demographics.Infrastructure.AutoMappers
//{
//    public class MenuItemTypeConverter : ITypeConverter<MenuItemEntity, MenuItem>
//    {
//        public MenuItem Convert(MenuItemEntity source, MenuItem destination, ResolutionContext context)
//        {
//            var translations = source.Translations.ToDictionary(k => k.CultureCode, v => v.Name);

//            var children = context.Mapper.Map<ICollection<MenuItemEntity>, ICollection<MenuItem>>(source.Children).ToList();
//            return MenuItem.CreateExisting(source.Id, source.AttributesJson, source.Name, source.Show, source.MenuId,
//                source.MenuType,
//                source.SortOrder, translations, null, null);

//        }
//    }
//}