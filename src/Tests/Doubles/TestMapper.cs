using AutoMapper;
using BusinessServices;

namespace Tests.Doubles
{
    public static class TestMapper
    {
        public static IMapper Create() => new MapperConfiguration(config => config.AddProfile(typeof(AutoMapperProfile))).CreateMapper();
    }
}