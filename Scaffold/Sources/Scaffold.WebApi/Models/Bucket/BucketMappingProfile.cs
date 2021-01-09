namespace Scaffold.WebApi.Models.Bucket
{
    using AutoMapper;

    public class BucketMappingProfile : Profile
    {
        public BucketMappingProfile()
        {
            this.CreateMap<Domain.Aggregates.Bucket.Bucket, Bucket>();
        }
    }
}
