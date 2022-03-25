using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
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
    public class Query: IRequest<Result<List<ActivityDto>>>{}   // change from Activity to ActivityDto to avoid infinite loop error

    // IRequestHandler from MediatR
    public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>     // change from Activity to ActivityDto to avoid infinite loop error
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper)
      {
        _context = context;
        _mapper = mapper;
      }

        // Task is an asynchronos opeartion that return a value
        // to handle a request of a list of Activity
      public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)   // change from Activity to ActivityDto to avoid infinite loop error
        {  // ToListAsync is from Entity framework
           // cancellation token to handle when user cancel the data loading 
           var activities = await _context.Activities
                .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)  // projected mapping from activity to activityDto
                .ToListAsync(cancellationToken);
   
            return Result<List<ActivityDto>>.Success(activities);     // change from Activity to ActivityDto to avoid infinite loop error
        }
    }
  }
}