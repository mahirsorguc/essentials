using AutoMapper;
using HMS.MainApp.Samples;

namespace HMS.MainApp;

public class MainAppAutoMapperProfile : Profile
{
    public MainAppAutoMapperProfile()
    {
        CreateMap<Sample, SampleDto>();
    }
}