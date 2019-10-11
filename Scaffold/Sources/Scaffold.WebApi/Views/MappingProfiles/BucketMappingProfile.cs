namespace Scaffold.WebApi.Views.MappingProfiles
{
    using AutoMapper;
    using Scaffold.Application.Features.Bucket;

    public class BucketMappingProfile : Profile
    {
        public BucketMappingProfile()
        {
            this.CreateMap<Bucket, AddBucket.Command>();
            this.CreateMap<Bucket, UpdateBucket.Command>();
            this.CreateMap<Domain.Aggregates.Bucket.Bucket, Bucket>();
        }
    }
}
