using AutoMapper;
using BusinessServices;

namespace Tests
{
    public static class TestMapper
    {
        public static IMapper Create()
        {
            return new MapperConfiguration(config => config.AddProfile(typeof(AutoMapperProfile))).CreateMapper();
        }
    }
}