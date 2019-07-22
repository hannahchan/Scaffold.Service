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
                .AddTransform<string>(value => string.IsNullOrEmpty(value) ? null : value)
                .ForMember(dest => dest.BucketId, opt => opt.Ignore());

            this.CreateMap<Item, UpdateItem.Command>()
                .AddTransform<string>(value => string.IsNullOrEmpty(value) ? null : value)
                .ForMember(dest => dest.BucketId, opt => opt.Ignore())
                .ForMember(dest => dest.ItemId, opt => opt.Ignore());

            this.CreateMap<Domain.Aggregates.Bucket.Item, Item>();
        }
    }
}
