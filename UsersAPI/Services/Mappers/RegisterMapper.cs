﻿using Mapster;
using UsersAPI.Models.DB;
using UsersAPI.Models.DTOs.Incoming;
using UsersAPI.Models.DTOs.Outgoing;

namespace UsersAPI.Services.Mappers
{
    public class RegisterMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Users
            config.NewConfig<User, UserInfoDto>()
                .Map(dto => dto.Division, res => res.Division == null ? "none" : res.Division.DivisionName)
                .Map(dto => dto.Company, res => res.Division == null ? "none" : res.Division.Company.CompanyName)
                .Map(dto => dto.ProfileImage, res => "image")
                .Map(dto => dto.TimeOff, res => res.UsersTimeOffs.First())
                .Map(dto => dto.UpperUser, res => res.UpperUser == null ? null : new UpperUserInfoDto(res.UpperUser))
                .RequireDestinationMemberSource(true);
            config.NewConfig<User, UserShortInfoDto>()
                .Map(dto => dto.Division, res => res.Division == null ? "none" : res.Division.DivisionName)
                .Map(dto => dto.Company, res => res.Division == null ? "none" : res.Division.Company.CompanyName)
                .Map(dto => dto.ProfileImage, res => "image")
                .RequireDestinationMemberSource(true);
            config.NewConfig<UserAddNewDto, User>()
                .Map(x => x.CreatedAt, x => DateTime.Now)
                .RequireDestinationMemberSource(false);

            // Time off
            config.NewConfig<UsersTimeOff, TimeOffInfoDto>()
                .RequireDestinationMemberSource(true);
            config.NewConfig<UsersTimeOff, TimeOffWithUserDto>()
                .Map(x => x.User, x => x.User.Adapt<UserShortInfoDto>())
                .RequireDestinationMemberSource(true);
            config.NewConfig<TimeOffAddUpdateDto, UsersTimeOff>()
                .Map(x => x.StartTimeOff, x => DateTime.Parse(x.StartTimeOff))
                .Map(x => x.EndTimeOff, x => DateTime.Parse(x.EndTimeOff))
                .RequireDestinationMemberSource(false);

            // Divisions
            config.NewConfig<Division, DivisionsTreeDto>()
                .Map(x => x.DivisionId, x => x.DivisionId)
                .Map(x => x.DivisionName, x => x.DivisionName)
                .Map(x => x.CompanyName, x => x.Company.CompanyName)
                .Map(x => x.InverseUpperDivision, x => x.InverseUpperDivision == null ? null : x.InverseUpperDivision.Count == 0 ? null : x.InverseUpperDivision.Adapt<List<DivisionsTreeItemDto>>())
                .RequireDestinationMemberSource(true);
            config.NewConfig<DivisionsTreeDto, DivisionsTreeItemDto>()
                .Map(x => x.InverseUpperDivision, x => x.InverseUpperDivision == null ? null : 
                    x.InverseUpperDivision.Count == 0 ? null : x.InverseUpperDivision)
                .RequireDestinationMemberSource(true);
        }
    }
}
