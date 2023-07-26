using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using ContactBook.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Data
{
    public class ContactBookContext : IdentityDbContext
    {
        public DbSet<AppUser> appUsers { get; set; }
        public ContactBookContext(DbContextOptions<ContactBookContext> options) : base(options){ }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
