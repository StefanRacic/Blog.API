using AutoMapper;
using Blog.API.Dtos;
using Blog.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;

        public AuthRepository(DataContext context, RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
        }
        public async Task<UserForLoginDto> Login(UserForLoginDto userForLoginDto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserForRegisterDto> Register(UserForRegisterDto userForRegisterDto)
        {
            var userInDbEmail = await _context.Users.AnyAsync(u => u.Email == userForRegisterDto.Email);
            var userInDbUsername = await _context.Users.AnyAsync(u => u.UserName == userForRegisterDto.UserName);
            if (userInDbEmail || userInDbUsername)
            {
                return null;
            }
            else
            {

                return userForRegisterDto;
            }
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync(user => user.Email == email))
                return true;

            return false;
        }
    }
}
