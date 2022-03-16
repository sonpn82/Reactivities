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
            private readonly DataContext _context;
      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
      }

            public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)   // project from activity to activityDto  (AutoMapper)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                return Result<ActivityDto>.Success(activity!);
            }
        }
    }

  
}