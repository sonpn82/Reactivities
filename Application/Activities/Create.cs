using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;  // to validate the data - added by nuget
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
  public class Create
  {
    // Command do not return any data
    public class Command: IRequest<Result<Unit>> // IRequest is mediator request - return a Result type from our Application.Core
      {
        public Activity Activity { get; set; } = new Activity();  // request with an Activity object in the body
      }  

    // class for validation of when create an activity
    // also have to add FluentValidation service to the Startup.cs
    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(command => command.Activity).SetValidator(new ActivityValidator()); // Validate the activity using our ActivityValidator
      }
    }

    // class to handle the request
    public class Handler : IRequestHandler<Command, Result<Unit>>
      {
        private readonly DataContext _context;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IUserAccessor userAccessor)
        {
          _context = context;
          _userAccessor = userAccessor;
      }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
          // get the current user who create the activity
          var user = await _context.Users.FirstOrDefaultAsync(
              x => x.UserName == _userAccessor.GetUsername());

          // create a new ActivityAttendee and set IsHost to true
          // the person who create the Activity is its host
          var attendee = new ActivityAttendee
          {
            AppUser = user,
            Activity = request.Activity,
            IsHost = true
          };

          // add the attendee to the Attendees list
          request.Activity.Attendees!.Add(attendee);

          // add the created activity to the Activities list
          // Not use AddAsync here, only add Activity to memory, not to database
          _context.Activities.Add(request.Activity);
          var result = await _context.SaveChangesAsync() > 0;

          // handle the error when create an activity by using Result
          if (!result) return Result<Unit>.Failure("Failure to create activity");

          return Result<Unit>.Success(Unit.Value); // return a success message
        }
      }
  }
}