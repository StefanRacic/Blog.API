using AutoMapper;
using Blog.API.Dtos;
using Blog.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public class automapper
    {
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<BlogItemDto, BlogItem>();
                CreateMap<BlogItem, BlogItemDto>();
                CreateMap<UserDto, User>();
                CreateMap<User, UserDto>();
                CreateMap<User, UserForRegisterDto>();
                CreateMap<UserForRegisterDto, User>();
                CreateMap<User, UserForLoginDto>();
                CreateMap<UserForLoginDto, User>();
            }
        }
    }
}
