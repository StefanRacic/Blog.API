using AutoMapper;
using Blog.API.Dtos;
using Blog.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public class BlogRepository : IBlogRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;


        public BlogRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<BlogItemDto>> GetAllBlogs()
        {
            var blogItems = await _context.Blogs.ToListAsync();
            var blogList = _mapper.Map<List<BlogItemDto>>(blogItems);

            return blogList;
        }

        public async Task<List<BlogItemDto>> GetMyBlogs(int userId)
        {
            var blogItems = await _context.Blogs.Where(item => item.UserId == userId).ToListAsync();
            var blogList = _mapper.Map<List<BlogItemDto>>(blogItems);

            return blogList;
        }
        public async Task<BlogItemDto> AddBlog(BlogItemDto blogItemDto)
        {
            var blogToAdd = _mapper.Map<BlogItem>(blogItemDto);
            await _context.Blogs.AddAsync(blogToAdd);
            await _context.SaveChangesAsync();

            return blogItemDto;
        }


        public async Task<BlogItemDto> Delete(int id, int userId)
        {
            var blogItemToDelete = await _context.Blogs.FindAsync(id);
            if (blogItemToDelete == null || blogItemToDelete.UserId != userId)
            {
                return null;
            }

            _context.Blogs.Remove(blogItemToDelete);
            await _context.SaveChangesAsync();
            var blogItemToDeleteDto = _mapper.Map<BlogItemDto>(blogItemToDelete);

            return blogItemToDeleteDto;
        }



        public async Task<BlogItemDto> Update(int id, BlogItemDto updatedblogItemDto)
        {
            var updatedBlog = _mapper.Map<BlogItem>(updatedblogItemDto);
            
            _context.Blogs.Update(updatedBlog);
            await _context.SaveChangesAsync();

            return updatedblogItemDto;
        }


    }
}
