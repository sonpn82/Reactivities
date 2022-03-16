using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      // Map an Activity to another Activity
      // Activity1.Title = Activity2.Title
      // Activity1.Date = Activity2.Date ...
      CreateMap<Activity, Activity>();  
      CreateMap<Activity, ActivityDto>()  // additional map to ActivityDto class to avoid infinite loop error, but some prop of ActivityDto can not be mapped
      // add here for props that do not match
      // 1st is Hostusername, which can be get from Attendees.Appuser.username and that attendees has IsHost = true
        .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees!
          .FirstOrDefault(x => x.IsHost)!.AppUser!.UserName));

      // create a new map from ActivityAttendee to Profiles.Profile also
      CreateMap<ActivityAttendee, Profiles.Profile>()
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser!.DisplayName))
        .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser!.UserName))
        .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser!.Bio));
    }
  }
}