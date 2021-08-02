using AutoMapper;
using DTO;
using Entities;

namespace BusinessServices
{
    public class AutoMapperProfile : Profile
    {
        /// <inheritdoc />
        public AutoMapperProfile()
        {
            CreateMap<LifePoint, ExistingLocation>();
            CreateMap<LifePoint, ExistingLifePoint>().ForMember(x => x.CreatedBy, y => y.MapFrom(z => z.CreatedBy.Name));
            CreateMap<LifePointToCreate, LifePoint>()
                .ForCtorParam("createdBy", options => options.MapFrom((src, context) => context.Items[nameof(LifePoint.CreatedBy)]))
                .ForMember(x => x.CreatedBy, options => options.MapFrom((src, dest, destMember, context) => context.Items[nameof(LifePoint.CreatedBy)]))
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}