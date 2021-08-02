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
        }
    }
}