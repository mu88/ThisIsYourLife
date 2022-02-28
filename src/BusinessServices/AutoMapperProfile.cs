using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using DTO.LifePoint;
using DTO.Location;
using DTO.Person;
using Entities;

namespace BusinessServices;

[ExcludeFromCodeCoverage]
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
        CreateMap<LifePoint, ExistingLifePoint>();
        CreateMap<LifePointToCreate, LifePoint>()
            .ForCtorParam("createdBy", options => options.MapFrom((_, context) => context.Items[nameof(LifePoint.CreatedBy)]))
            .ForMember(x => x.CreatedBy, options => options.MapFrom((_, _, _, context) => context.Items[nameof(LifePoint.CreatedBy)]))
            .ForCtorParam("imageId", options => options.MapFrom((_, context) => context.Items[nameof(LifePoint.ImageId)]))
            .ForMember(x => x.ImageId, options => options.MapFrom((_, _, _, context) => context.Items[nameof(LifePoint.ImageId)]))
            .ForMember(x => x.Id, options => options.Ignore());
    }
}