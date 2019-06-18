namespace Scaffold.WebApi.Views.MappingProfiles
{
    using AutoMapper;
    using Scaffold.Application.Features.Item;
    using Scaffold.WebApi.Views;

    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {
            this.CreateMap<Item, AddItem.Command>()
                .ForMember(dest => dest.BucketId, opt => opt.Ignore());

            this.CreateMap<Item, UpdateItem.Command>()
                .ForMember(dest => dest.BucketId, opt => opt.Ignore())
                .ForMember(dest => dest.ItemId, opt => opt.Ignore());

            this.CreateMap<Domain.Entities.Item, Item>();
        }
    }
}
