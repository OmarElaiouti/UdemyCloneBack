using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAl.Models;
using Udemy.DAL.DTOs;
using Udemy.DAL.DTOs.CoursePartsDtos;

namespace Udemy.CU.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Note, NoteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.NoteId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));

            CreateMap<User, InstructorDto>()
                .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
  }
    }
}
