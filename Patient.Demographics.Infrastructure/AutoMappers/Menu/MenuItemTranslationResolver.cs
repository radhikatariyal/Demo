//using System;
//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;
//using Patient.Demographics.Domain.Menu;
//using Patient.Demographics.Data.Entities;

//namespace Patient.Demographics.Infrastructure.AutoMappers
//{
//    public class MenuItemTranslationResolver :
//        IMemberValueResolver<MenuItem, MenuItemEntity, IReadOnlyDictionary<string, string>, ICollection<MenuItemTranslationEntity>>
//    {

//        public ICollection<MenuItemTranslationEntity> Resolve(MenuItem source, MenuItemEntity destination, IReadOnlyDictionary<string, string> sourceMember, ICollection<MenuItemTranslationEntity> destMember, ResolutionContext context)
//        {
//            List<MenuItemTranslationEntity> entities =
//                sourceMember.Select(x => new MenuItemTranslationEntity
//                {
//                    Name = x.Value,
//                    CultureCode = x.Key,
//                    MenuItemId = destination.Id
//                })
//                    .ToList();
//            return entities;

//        }
//    }
//    public class MenuAudienceResolver :
//        IMemberValueResolver<Menu, MenuItemEntity, IList<Guid>, ICollection<MenuItemAudienceEntity>>
//    {

//        public ICollection<MenuItemAudienceEntity> Resolve(Menu source, MenuItemEntity destination, IList<Guid> sourceMember, ICollection<MenuItemAudienceEntity> destMember, ResolutionContext context)
//        {
//            List<MenuItemAudienceEntity> entities =
//                sourceMember.Select(x => new MenuItemAudienceEntity
//                {
//                    MenuItemId = source.Id
//                })
//                    .ToList();
//            return entities;

//        }
//    }
//    public class MenuItemAudienceResolver :
//        IMemberValueResolver<MenuItem, MenuItemEntity, IReadOnlyList<Guid>, ICollection<MenuItemAudienceEntity>>
//    {

//        public ICollection<MenuItemAudienceEntity> Resolve(MenuItem source, MenuItemEntity destination, IReadOnlyList<Guid> sourceMember, ICollection<MenuItemAudienceEntity> destMember, ResolutionContext context)
//        {
//            if (sourceMember == null || sourceMember.Count == 0)
//            {
//                return new List<MenuItemAudienceEntity>();
//            }
//            List<MenuItemAudienceEntity> entities =
//                sourceMember.Select(x => new MenuItemAudienceEntity
//                {
//                    MenuItemId = source.Id
//                })
//                    .ToList();
//            return entities;

//        }
//    }
//}