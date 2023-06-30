using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UsersAPI.Models;
using UsersAPI.Models.DTOs.Incoming;
using UsersAPI.Models.DTOs.Outgoing;

namespace UsersAPI.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<NewUserDto, User>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.ThirdName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.DivisionId, opt => opt.MapFrom(src => src.DivisionId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.VacationCount, opt => opt.MapFrom(src => src.VacationCount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now.Date));

            CreateMap<User, UserInfoDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.ThirdName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                .ForMember(dest => dest.Division, opt => opt.MapFrom(src => src.Division))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.VacationCount, opt => opt.MapFrom(src => src.VacationCount))
                .ForMember(dest => dest.TimeOff, opt => opt.MapFrom(src => 
                    src.UsersTimeOffs.Count() == 0 ? null : src.UsersTimeOffs.First()))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => "http://localhost:9091/api/image/" + 
                (src.ProfileImages.Count() == 0 ? "0" : src.ProfileImages.First().ImageId)));
        }
    }
}
