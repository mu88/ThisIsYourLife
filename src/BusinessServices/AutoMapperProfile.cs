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
        }
    }
}