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

    // set Activities table with column from properties on Domain/Activity.cs
    public DbSet<Activity> Activities { get; set; }  
   
    // ActivityAttendee is the joint table between Activity tbl & User tbl
    // created for many to many relationship
    public DbSet<ActivityAttendee> ActivityAttendees { get; set;}

    // Photos table name
    public DbSet<Photo> Photos { get; set; }

    // Comment table name
    public DbSet<Comment> Comments {get; set;}

    // Following table name
    public DbSet<UserFollowing> UserFollowings {get; set;}

    // to config many to many relationship between Activity table and Attendee table
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<ActivityAttendee>
        (x => x.HasKey(aa => new {aa.AppUserId, aa.ActivityId}));

      // one App user with many Activities
      builder.Entity<ActivityAttendee>()
        .HasOne(u => u.AppUser)
        .WithMany(a => a.Activities)
        .HasForeignKey(aa => aa.AppUserId);
      
      // One Activity with many AppUsers
      builder.Entity<ActivityAttendee>()
        .HasOne(u => u.Activity)
        .WithMany(a => a.Attendees)
        .HasForeignKey(aa => aa.ActivityId);

      // One Activity with many comment
      builder.Entity<Comment>()
        .HasOne(a => a.Activity)
        .WithMany(c => c.Comments)
        .OnDelete(DeleteBehavior.Cascade);  // delete the Activity also delete the related comment
      
      // following and follower features
      builder.Entity<UserFollowing>(b => 
      {
        b.HasKey(k => new {k.ObserverId, k.TargetId});  // the 2 columns in UserFollowing table - also 2 primary keys
        
        // one Observer (AppUser) has many following
        b.HasOne(o => o.Observer)
          .WithMany(f => f.Followings)
          .HasForeignKey(o => o.ObserverId)  // 1 foreign key with appuser table id column
          .OnDelete(DeleteBehavior.Cascade);  // delete user also delete following

        // one Target (AppUser) has many followers
        b.HasOne(o => o.Target)
          .WithMany(f => f.Followers)
          .HasForeignKey(o => o.TargetId)  // another foreign key with appuser able
          .OnDelete(DeleteBehavior.Cascade);  // delete user also delete follower
      });
    }
  }
}