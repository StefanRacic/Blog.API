using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Models
{
    public class User : IdentityUser<int>
    {
        public List<BlogItem> Blogs { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
