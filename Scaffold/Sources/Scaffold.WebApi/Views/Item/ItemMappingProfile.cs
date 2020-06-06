namespace Scaffold.WebApi.Views.Item
{
    using AutoMapper;

    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {
            this.CreateMap<Domain.Aggregates.Bucket.Item, Item>();
        }
    }
}
