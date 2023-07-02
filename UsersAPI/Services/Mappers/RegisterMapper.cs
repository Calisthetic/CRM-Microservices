﻿using Mapster;
using UsersAPI.Models;
using UsersAPI.Models.DTOs.Incoming;
using UsersAPI.Models.DTOs.Outgoing;

namespace UsersAPI.Services.Mappers
{
    public class RegisterMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserInfoDto>()
                .Map(dto => dto.Division, res => res.Division == null ? "none" : res.Division.DivisionName)
                .Map(dto => dto.Company, res => res.Division == null ? "none" : res.Division.Company.CompanyName)
                .Map(dto => dto.ProfileImage, res => "image")
                .Map(dto => dto.TimeOff, res => res.UsersTimeOffs.First())
                .Map(dto => dto.UpperUser, res => res.UpperUser == null ? null : new UpperUserInfoDto(res.UpperUser))
                .RequireDestinationMemberSource(true);

            config.NewConfig<UserAddNewDto, User>()
                .Map(d => d.CreatedAt, x => DateTime.Now)
                .RequireDestinationMemberSource(false);
        }
    }
}
