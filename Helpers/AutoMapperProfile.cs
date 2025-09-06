//Helpers/MappingProfile.cs
using AutoMapper;
using FacesHunter.Dtos;
using FacesHunter.DTOs;
using FacesHunter.Models;

namespace FacesHunter.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap < Person, PersonCreateDto>();
            CreateMap<PersonCreateDto, Person>();
            CreateMap<Person, PersonDto>().ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()));
        }
    }
}




  