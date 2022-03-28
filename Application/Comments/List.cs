using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    // Get a list of commentDto from a query with activityId
    public class List
    {
        public class Query : IRequest<Result<List<CommentDto>>>
        {
            public Guid ActivityId { get; set; }
        }

    public class Handler : IRequestHandler<Query, Result<List<CommentDto>>>
    {
      private readonly DataContext _context;  // to get activity
      private readonly IMapper _mapper;  // map from comment to commentDto

      public Handler(DataContext context, IMapper mapper)
      {
        _context = context;
        _mapper = mapper;
      }

      public async Task<Result<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
      {
        // get list of comment from activityId, 
        // also order by created date and map to commentDto
        var comments = await _context.Comments
            .Where(x => x.Activity!.Id == request.ActivityId)
            .OrderByDescending(x => x.CreatedAt)  // newest is at 1st location
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)  // mapper
            .ToListAsync();

        return Result<List<CommentDto>>.Success(comments);
      }
    }
  }
}