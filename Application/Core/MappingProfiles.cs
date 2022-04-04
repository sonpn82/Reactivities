using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;

namespace Application.Core
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      string? currentUsername = null;  // to get current user inside this class method

      // Map an Activity to another Activity
      // Activity1.Title = Activity2.Title
      // Activity1.Date = Activity2.Date ...
      CreateMap<Activity, Activity>();  
      CreateMap<Activity, ActivityDto>()  // additional map to ActivityDto class to avoid infinite loop error, but some prop of ActivityDto can not be mapped
      // add here for props that do not match
      // 1st is Hostusername, which can be get from Attendees.Appuser.username and that attendees has IsHost = true
        .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees!
          .FirstOrDefault(x => x.IsHost)!.AppUser!.UserName));

      // create a new map from ActivityAttendee to AttendeeDto 
      CreateMap<ActivityAttendee, AttendeeDto>()
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser!.DisplayName))
        .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser!.UserName))
        .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser!.Bio))
        .ForMember(d => d.Image, 
          o=> o.MapFrom(s => s.AppUser!.Photos!.FirstOrDefault(x => x.IsMain)!.Url))  // map the image
        .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.AppUser!.Followers!.Count))  // use Count to get number of followers from follower list
        .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.AppUser!.Followings!.Count))
        .ForMember(d => d.Following, 
          o => o.MapFrom(s => s.AppUser!.Followers!.Any(x => x.Observer!.UserName == currentUsername)));

      // map from AppUser to Profile
      // already have username, displayname and bio. Need Image + follow
      // Image will be the photo which is set as Main in user photo collection
      CreateMap<AppUser, Profiles.Profile>()
        .ForMember(d => d.Image, o=> o.MapFrom(s => s.Photos!.FirstOrDefault(x => x.IsMain)!.Url))
        .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers!.Count))  // use Count to get number of followers from follower list
        .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.Followings!.Count))
        .ForMember(d => d.Following, 
          o => o.MapFrom(s => s.Followers!.Any(x => x.Observer!.UserName == currentUsername)));
    
      // map from Comment to CommentDto
      // extra info is mapped from Comment.Author
      CreateMap<Comment, CommentDto>()
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author!.DisplayName))
        .ForMember(d => d.Username, o => o.MapFrom(s => s.Author!.UserName))
        .ForMember(d => d.Image, o=> o.MapFrom(s => s.Author!.Photos!.FirstOrDefault(x => x.IsMain)!.Url));  // map the image
    }
  }
}