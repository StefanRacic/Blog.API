using AutoMapper;
using Blog.API.Data;
using Blog.API.Dtos;
using Blog.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UsersController : BaseApiController
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signIngManager;
        private readonly IMapper _mapper;

        public UsersController(IAuthRepository repo, IConfiguration config, UserManager<User> userManager, SignInManager<User> signIngManager, IMapper mapper)
        {
            _repo = repo;
            _config = config;
            _userManager = userManager;
            _signIngManager = signIngManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult loadUser()
        {
            bool isAuthenticated = User.Identity.IsAuthenticated;
            string role = User.IsInRole("Admin") ? "Admin" : "User";


            if (isAuthenticated)
                return Ok(new { User.Identity.Name, role, id = GetUsersId() });
            return BadRequest("Not logged in");
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(UserForRegisterDto userForRegisterDto)
        {
            var result = await _repo.Register(userForRegisterDto);

            if (result == null)
            {
                return BadRequest("User already Exists");
            }
            else
            {
                var user = _mapper.Map<User>(result);
                await _userManager.CreateAsync(user, userForRegisterDto.Password);
                await _userManager.AddToRoleAsync(user, "User");
                return Ok(new
                {
                    token = await GenerateJwtToken(user),
                });
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> login(UserForLoginDto userForLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(userForLoginDto.Email);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            var result = await _signIngManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    token = await GenerateJwtToken(user),
                });
            }

            return BadRequest("Wrong Password");

            //var result = await _repo.Login(userForLoginDto);
            //var user = _mapper.Map<User>(result);

            //var userLogin = await _signIngManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);
            //if (userLogin.Succeeded)
            //{
            //    return Ok(new
            //    {
            //        token = await GenerateJwtToken(user),
            //    });
            //}
            //return BadRequest("Wrong Email or Password");
        }

        private async Task<string> GenerateJwtToken(User user)
        {

            var claims = new List<Claim>
           {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var roles = await _userManager.GetRolesAsync(user);

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