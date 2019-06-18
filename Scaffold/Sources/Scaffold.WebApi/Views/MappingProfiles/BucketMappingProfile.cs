namespace Scaffold.WebApi.Views.MappingProfiles
{
    using AutoMapper;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.WebApi.Views;

    public class BucketMappingProfile : Profile
    {
        public BucketMappingProfile()
        {
            this.CreateMap<Bucket, AddBucket.Command>();
            this.CreateMap<Bucket, UpdateBucket.Command>();

            this.CreateMap<Domain.Entities.Bucket, Bucket>();
        }
    }
}
