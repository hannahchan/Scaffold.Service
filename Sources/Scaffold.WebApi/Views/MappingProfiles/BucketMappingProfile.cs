namespace Scaffold.WebApi.Views.MappingProfiles
{
    using AutoMapper;
    using Scaffold.Application.Features.Bucket;
    using Scaffold.WebApi.Views;

    public class BucketMappingProfile : Profile
    {
        public BucketMappingProfile()
        {
            this.CreateMap<Bucket, AddBucket.Command>()
                .ForMember(dest => dest.RequestId, opt => opt.Ignore());

            this.CreateMap<Bucket, UpdateBucket.Command>()
                .ForMember(dest => dest.RequestId, opt => opt.Ignore());
        }
    }
}
