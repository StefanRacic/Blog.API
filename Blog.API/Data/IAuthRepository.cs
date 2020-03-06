using Blog.API.Dtos;
using Blog.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public interface IAuthRepository
    {
        Task<UserForRegisterDto> Register(UserForRegisterDto userForRegisterDto);
        Task<UserForLoginDto> Login(UserForLoginDto userForLogin);
        Task<bool> UserExists(string email);
    }
}
