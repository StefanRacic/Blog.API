using Blog.API.Dtos;
using Blog.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public interface IBlogRepository
    {
        Task<List<BlogItemDto>> GetAllBlogs();
        Task<List<BlogItemDto>> GetMyBlogs(int userId);
        Task<BlogItemDto> GetBlog(int id);
        Task<BlogItemDto> AddBlog(BlogItemDto blogItemDto);
        Task<BlogItemDto> Delete(int id, int userId);
        Task<BlogItemDto> Update(int id, BlogItemDto blogItem);
    }
}
