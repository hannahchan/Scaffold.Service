namespace Scaffold.WebApi.Models.Item
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
