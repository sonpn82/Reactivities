using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
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
    public class Query: IRequest<Result<List<Activity>>>{}

    // IRequestHandler from MediatR
    public class Handler : IRequestHandler<Query, Result<List<Activity>>>
    {
        private readonly DataContext _context;
        public Handler(DataContext context)
        {
            _context = context;
        }

        // Task is an asynchronos opeartion that return a value
        // to handle a request of a list of Activity
        public async Task<Result<List<Activity>>> Handle(Query request, CancellationToken cancellationToken)
        {  // ToListAsync is from Entity framework
           // cancellation token to handle when user cancel the data loading 
            return Result<List<Activity>>.Success(await _context.Activities.ToListAsync(cancellationToken));
        }
    }
  }
}