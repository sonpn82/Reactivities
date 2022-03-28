using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace Application.Comments
{
    // Class to create a new comment with the body of comment in the body request 
    // and also the id of activity
    // Remain info is from current user and the activity
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public string?  Body { get; set; }
            public Guid ActivityId { get; set; }
        }

    // Validation when creating comment, Body not empty
    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
          RuleFor(x => x.Body).NotEmpty();
      }
    }

    public class Handler : IRequestHandler<Command, Result<CommentDto>>
    {
      private readonly DataContext _context;  // for activity data
      private readonly IMapper _mapper;  // for data mapping from comment to commentDto
      private readonly IUserAccessor _userAccessor;  // get current user

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        _context = context;
        _mapper = mapper;
        _userAccessor = userAccessor;
      }

      public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
      {
        // Get the activity from activityId in the request
        var activity = await _context.Activities.FindAsync(request.ActivityId);

        if (activity == null) return null!;

        // Get the user with photos list from current user
        var user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        
        // create a new comment with activity, user and body in request body
        var comment = new Comment
        {
            Author = user,
            Activity = activity,
            Body = request.Body
        };

        // add this comment to comment list in activity
        activity.Comments.Add(comment);

        // save change to database
        var success = await _context.SaveChangesAsync() > 0;

        // return to client a CommentDto object by using mapper
        if (success) return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));

        return Result<CommentDto>.Failure("Failed to add comment");
      }
    }
  }
}