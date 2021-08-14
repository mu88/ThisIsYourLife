using AutoMapper;
using DTO.LifePoint;
using DTO.Location;
using DTO.Person;
using Entities;

namespace BusinessServices
{
    public class AutoMapperProfile : Profile
    {
        /// <inheritdoc />
        public AutoMapperProfile()
        {
            CreateMapsForLifePoint();
            CreateMapsForPerson();
        }

        private void CreateMapsForPerson()
        {
            CreateMap<Person, ExistingPerson>();
            CreateMap<PersonToCreate, Person>().ForMember(x => x.Id, options => options.Ignore());
        }

        private void CreateMapsForLifePoint()
        {
            CreateMap<LifePoint, ExistingLocation>();
            CreateMap<LifePoint, ExistingLifePoint>().ForMember(x => x.CreatedBy, y => y.MapFrom(z => z.CreatedBy.Name));
            CreateMap<LifePointToCreate, LifePoint>()
                .ForCtorParam("createdBy", options => options.MapFrom((_, context) => context.Items[nameof(LifePoint.CreatedBy)]))
                .ForMember(x => x.CreatedBy, options => options.MapFrom((_, _, _, context) => context.Items[nameof(LifePoint.CreatedBy)]))
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}