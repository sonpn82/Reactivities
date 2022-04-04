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

namespace Application.Activities
{
    public class Details
    {
        // Query return some data, here is Activity
        public class Query: IRequest<Result<ActivityDto>>  // replace Activity with ActivityDto to avoid infinite loop when loading activity
        {
           public Guid Id {get; set;}   // Id parameter in the request link
        }

        // Handler to handle the Query request
        public class Handler: IRequestHandler<Query, Result<ActivityDto>>
        {
            private readonly DataContext _context;  // to get Activities data
            private readonly IMapper _mapper;  // to map data
            private readonly IUserAccessor _userAccessor;  // to get current user and passdown to mapper for following feature

        public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

        public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, 
                        new {currentUsername = _userAccessor.GetUsername()})   // project from activity to activityDto  (AutoMapper)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                return Result<ActivityDto>.Success(activity!);
            }
        }
    }

  
}