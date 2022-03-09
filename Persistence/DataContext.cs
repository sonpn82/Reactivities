using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

#pragma warning disable CS8618  // remove null warning

namespace Persistence
{
  // Change from DbContext to IdentityDbContext to use dotnet core Identity class
  public class DataContext : IdentityDbContext<AppUser>  // inherit from entityframework dbcontext
  {
    // DataContext from Entity framework to allow to access data in database - get, input, del
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Activity> Activities { get; set; }  // set Activities table with column from properties on Domain/Activity.cs
  }
}