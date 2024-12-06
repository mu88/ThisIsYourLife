using AutoMapper;
using BusinessServices;

namespace Tests.Doubles;

public static class TestMapper
{
    public static IMapper Create()
    {
        var mapperConfiguration = new MapperConfiguration(config => config.AddProfile<AutoMapperProfile>());
        mapperConfiguration.AssertConfigurationIsValid();
        var mapper = mapperConfiguration.CreateMapper();
        return mapper;
    }
}