using MongoHeadSample.Models;
using MongoHeadSample.ViewModels;
using AutoMapper;

namespace MongoHeadSample._3rdParties.AutoMapper;

public class AutoMapperProfile: Profile
{
    public AutoMapperProfile()
    {
        CreateMap<PersonViewModel, Person>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => $"{src._id}"))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => $"{src.FirstName}"))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => $"{src.LastName}"))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => $"{src.Age}"));

        CreateMap<Person, PersonViewModel>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => $"{src._id}"))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => $"{src.FirstName}"))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => $"{src.LastName}"))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => $"{src.Age}"));
    }
}
