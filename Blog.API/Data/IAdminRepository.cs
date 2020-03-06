using Blog.API.Dtos;
using Blog.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public interface IAdminRepository
    {
        Task<IList<UserDto>> GetAllUsers();
        Task<UserDto> AddUser(UserDto userDto);
        Task<UserDto> RemoveUser(int id);
        Task<UserDto> UpdateUser(UserDto userDto);
    }
}
