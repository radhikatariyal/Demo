//using System;
//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;
//using Patient.Demographics.Common.Extensions;
//using Patient.Demographics.Domain.Menu;
//using Patient.Demographics.Repository.Dtos.Menu;
//using Patient.Demographics.Data.Entities;

//namespace Patient.Demographics.Infrastructure.AutoMappers
//{
//    public class MenuMapper : Profile
//    {
//        public MenuMapper()
//        {
//            SetupMappings();
//        }

//        public void SetupMappings()
//        {
//            MapMenuToMenuEntity();

//            MapMenuITemToEntity();

//            CreateMap<MenuItemTranslationEntity, KeyValuePair<string, string>>()
//                .ConvertUsing<MenuTranslationTypeConverter>();
//            CreateMap<MenuItemEntity, Menu>().ConvertUsing<MenuTypeConverter>();
//            CreateMap<MenuItemEntity, MenuItem>().ConvertUsing<MenuItemTypeConverter>();

//            CreateMap<MenuItemDto, PlatformMenuDto>()
//                .ForMember(m => m.Children, o => o.Ignore())
//                .ForMember(m => m.DisplayIndex, o => o.MapFrom(m => m.SortOrder))
//                .ForMember(m => m.Id, o => o.MapFrom(m => m.Id))
//                .ForMember(m => m.ParentId, o => o.MapFrom(m => m.ParentId))
//                .ForMember(m => m.MenuType, o => o.MapFrom(m => m.MenuType))
//                .ForMember(m => m.Show, o => o.MapFrom(m => m.Show))
//                .ForMember(m => m.Url, o => o.Ignore())
//                .ForMember(m => m.Translations, o => o.MapFrom(m => m.Translations));

//            CreateMap<MenuItemEntity, MenuItemDto>()
//                .ForMember(m => m.Children, o => o.MapFrom(entity => entity.Children))
//                .ForMember(m => m.AttributesJson, o => o.MapFrom(entity => entity.AttributesJson))
//                .ForMember(m => m.Show, o => o.MapFrom(entity => entity.Show))
//                .ForMember(m => m.Id, o => o.MapFrom(entity => entity.Id))
//                .ForMember(m => m.MenuType, o => o.MapFrom(entity => entity.MenuType.Name.LowerFirstLetter()))
//                .ForMember(m => m.MenuTypeDisplayName, o => o.MapFrom(entity => entity.MenuType.DisplayName))
//                .ForMember(m => m.ParentId, o => o.MapFrom(entity => entity.ParentId))
//                .ForMember(m => m.MenuId, o => o.MapFrom(entity => entity.MenuId))
//                .ForMember(m => m.Name, o => o.MapFrom(entity => entity.Name))
//                .ForMember(m => m.SortOrder, o => o.MapFrom(entity => entity.SortOrder))
//                .ForMember(m => m.Translations, o => o.MapFrom(entity => entity.Translations));
//        }

//        private void MapMenuToMenuEntity()
//        {
//            CreateMap<Menu, MenuItemEntity>()
//                .ForMember(dto => dto.Id, opts => opts.MapFrom(menu => menu.Id))
//                .ForMember(dto => dto.AttributesJson, opts => opts.MapFrom(menu => menu.AttributesJson))
//                .ForMember(dto => dto.MenuType, opts => opts.MapFrom(menu => menu.MenuType))
//                .ForMember(dto => dto.Children, opts => opts.MapFrom(menu => menu.Children))
//                .ForMember(dto => dto.MenuId, opts => opts.MapFrom(menu => menu.Id))
//                .ForMember(dto => dto.Name, opts => opts.MapFrom(menu => menu.Name))
//                .ForMember(dto => dto.ParentId, opts => opts.Ignore())
//                .ForMember(dto => dto.Parent, opts => opts.Ignore())
//                .ForMember(dto => dto.Show, opts => opts.MapFrom(menu => menu.Show))
//                .ForMember(dto => dto.MenuAudiences, opts => opts.ResolveUsing<MenuAudienceResolver, IList<Guid>>(v2 => v2.AudienceIds))
//                .ForMember(dto => dto.SortOrder, opts => opts.MapFrom(menu => menu.SortOrder))
//                .ForMember(dto => dto.Translations,
//                    opts => opts.ResolveUsing<MenuTranslationResolver, IReadOnlyDictionary<string, string>>(
//                        entity => entity.Translations));
//        }

//        private void MapMenuITemToEntity()
//        {
//            CreateMap<MenuItem, MenuItemEntity>()
//                .ForMember(dto => dto.Id, opts => opts.MapFrom(menuItem => menuItem.Id))
//                .ForMember(dto => dto.AttributesJson, opts => opts.MapFrom(menuItem => menuItem.AttributesJson))
//                .ForMember(dto => dto.MenuType, opts => opts.MapFrom(menuItem => menuItem.MenuType))
//                .ForMember(dto => dto.Children, opts => opts.MapFrom(menuItem => menuItem.Children))
//                .ForMember(dto => dto.MenuId, opts => opts.MapFrom(menuItem => menuItem.MenuId))
//                .ForMember(dto => dto.Name, opts => opts.MapFrom(menuItem => menuItem.Name))
//                .ForMember(dto => dto.ParentId, opts => opts.MapFrom(menuItem => menuItem.ParentId))
//                .ForMember(dto => dto.Parent, opts => opts.Ignore())
//                .ForMember(dto => dto.Show, opts => opts.MapFrom(menuItem => menuItem.Show))
//                .ForMember(dto => dto.SortOrder, opts => opts.MapFrom(menuItem => menuItem.SortOrder))
//                .ForMember(dto => dto.MenuAudiences, opts => opts.ResolveUsing<MenuItemAudienceResolver, IReadOnlyList<Guid>>(v2 => v2.AudienceIds))
//                .ForMember(dto => dto.Translations,
//                    opts => opts.ResolveUsing<MenuItemTranslationResolver,
//                        IReadOnlyDictionary<string, string>>(entity => entity.Translations));
//        }
//    }
//}