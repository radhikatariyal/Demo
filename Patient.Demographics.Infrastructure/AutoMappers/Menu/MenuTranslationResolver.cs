//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;
//using Patient.Demographics.Domain.Menu;
//using Patient.Demographics.Data.Entities;

//namespace Patient.Demographics.Infrastructure.AutoMappers
//{
//    public class MenuTranslationResolver :
//        IMemberValueResolver<Menu, MenuItemEntity, IReadOnlyDictionary<string, string>, ICollection<MenuItemTranslationEntity>>
//    {

//        public ICollection<MenuItemTranslationEntity> Resolve(Menu source, MenuItemEntity destination, IReadOnlyDictionary<string, string> sourceMember, ICollection<MenuItemTranslationEntity> destMember, ResolutionContext context)
//        {
//            List<MenuItemTranslationEntity> entities =
//                sourceMember.Select(x => new MenuItemTranslationEntity
//                    {
//                        Name = x.Value,
//                        CultureCode = x.Key,
//                        MenuItemId = destination.Id
//                    })
//                    .ToList();
//            return entities;

//        }
//    }
//}