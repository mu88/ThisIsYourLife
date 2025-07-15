using AutoMapper;
using BusinessServices;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests.Doubles;

public static class TestMapper
{
    public static IMapper Create()
    {
        var mapperConfiguration = new MapperConfiguration(config => config.AddProfile<AutoMapperProfile>(), new NullLoggerFactory());
        mapperConfiguration.AssertConfigurationIsValid();
        var mapper = mapperConfiguration.CreateMapper();
        return mapper;
    }
}