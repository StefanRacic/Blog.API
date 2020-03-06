using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Blog.API.Data;
using Blog.API.Dtos;
using Blog.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AdminController : BaseApiController
    {
        private readonly IAdminRepository _repo;
        private readonly UserManager<User> _userMenager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AdminController(IAdminRepository repo, UserManager<User> userMenager, IConfiguration config, IMapper mapper)
        {
            _repo = repo;
            _userMenager = userMenager;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> getUsers()
        {
            var users = await _repo.GetAllUsers();

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> addUser(UserDto userDto)
        {
            var userToAdd = new User
            {
                Email = userDto.Email,
                UserName = userDto.UserName
            };

            var result = await _userMenager.CreateAsync(userToAdd, userDto.Password);

            if (result.Succeeded)
            {
                await _userMenager.AddToRoleAsync(userToAdd, "User");

                return Ok(new
                {
                    token = await GenerateJwtToken(userToAdd)
                });
            }
            return BadRequest(result.Errors);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            var success = await _repo.RemoveUser(id);
            if (success != null)
                return Ok("User Deleted");
            return BadRequest("No Such User");
        }

        private async Task<string> GenerateJwtToken(User user)
        {

            var claims = new List<Claim>
           {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var roles = await _userMenager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}