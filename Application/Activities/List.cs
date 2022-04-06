using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

// process the request from controller to access data from database (get, put, del)
// using MediatR package and using DataContext from Entity framework in Persistence
namespace Application.Activities
{
  public class List
  {
    // return a List of Activity object - Activity from Domain/Activity.cs
    // IRequest is from MediatR
    // Return a Result instead of a List - Result will handle the error for this action
    public class Query: IRequest<Result<PagedList<ActivityDto>>> // change from Activity to ActivityDto to avoid infinite loop error
    {
      public ActivityParams? Params { get; set; }  // use PagedList to get pagination result and ActivityParams for filtering
    }   

    // IRequestHandler from MediatR
    public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>     // change from Activity to ActivityDto to avoid infinite loop error
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;  // to get currentuser for passing down in mapper for follower/folowing feature
        _context = context;  // to get activities data
        _mapper = mapper;  // for data mapping
      }

        // Task is an asynchronos opeartion that return a value
        // to handle a request of a list of Activity
      public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)   // change from Activity to ActivityDto to avoid infinite loop error
        {  // ToListAsync is from Entity framework
           // cancellation token to handle when user cancel the data loading 
           // changed from return activities to return a query for paging
          var query = _context.Activities
              .Where(d => d.Date >= request.Params!.StartDate)  // date filtering
              .OrderBy(d => d.Date)  // sort activities by date
              .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, 
                new {currentUsername = _userAccessor.GetUsername()})  // projected mapping from activity to activityDto
              .AsQueryable();

          // filter the query base on isGoing 
          if (request.Params!.IsGoing && !request.Params.IsHost)
          {
            // query for events which current username is in Attendees list
            query = query.Where(x => x.Attendees!.Any(a => a.Username == _userAccessor.GetUsername()));
          }

          // filter the query base on isHost
          if (!request.Params!.IsGoing && request.Params.IsHost)
          {
            // query for events which current username is the HostUsername
            query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
          }
   
          return Result<PagedList<ActivityDto>>.Success(
            await PagedList<ActivityDto>.CreateAsync(query, request.Params!.PageNumber, 
              request.Params.PageSize)
          );     // change from Activity to ActivityDto to avoid infinite loop error
        }
    }
  }
}