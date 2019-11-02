namespace Scaffold.WebApi.Views.MappingProfiles
{
    using AutoMapper;
    using Scaffold.Application.Features.Bucket;

    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {
            this.CreateMap<Item, AddItem.Command>()
                .ForMember(dest => dest.BucketId, opt => opt.Ignore());

            this.CreateMap<Item, UpdateItem.Command>()
                .ForMember(dest => dest.BucketId, opt => opt.Ignore())
                .ForMember(dest => dest.ItemId, opt => opt.Ignore());

            this.CreateMap<Domain.Aggregates.Bucket.Item, Item>();
        }
    }
}
