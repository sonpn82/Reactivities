using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
  }
}