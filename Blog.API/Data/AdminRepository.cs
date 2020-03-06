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
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AdminRepository(DataContext context, RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<IList<UserDto>> GetAllUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            var usersList = _mapper.Map<List<UserDto>>(users);
            return usersList;
        }

        public async Task<UserDto> AddUser(UserDto userDto)
        {
            var userToAdd = _mapper.Map<User>(userDto);
            await _context.Users.AddAsync(userToAdd);
            await _context.SaveChangesAsync();

            return userDto;
        }
        

        public Task<UserDto> UpdateUser(UserDto userDto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> RemoveUser(int id)
        {
            var user =  await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            var blogsToDelete = await _context.Blogs.Where(blog => blog.UserId == id).ToListAsync();
            _context.Blogs.RemoveRange(blogsToDelete);
            await _context.SaveChangesAsync();
            var userToReturn =_mapper.Map<UserDto>(user);
            return userToReturn;
        }
    }
}
