using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    // to update an Attendance in the Activity.Attendees list - 
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

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
        var activity = await _context.Activities!
            .Include(a => a.Attendees)!
            .ThenInclude(u => u.AppUser)
            .SingleOrDefaultAsync(x => x.Id == request.Id);

        if (activity == null) return null!;

        var user = await _context.Users.FirstOrDefaultAsync(
            x => x.UserName == _userAccessor.GetUsername());

        if (user == null) return null!;

        // do not need to use Async here, already got all data in memory
        var hostUsername = activity.Attendees!.FirstOrDefault(x => x.IsHost)?.AppUser?.UserName;

        // find the attendance in the list of activity Attendees
        var attendance = activity.Attendees!.FirstOrDefault(x => x.AppUser!.UserName == user.UserName);

        // if ther is attendance in the list and user is the host then activity will be cancel
        if (attendance != null && hostUsername == user.UserName)
            activity.IsCancelled = !activity.IsCancelled;

        // if there is attendence in the list and user is not the host then user will be removed from the event
        if (attendance != null && hostUsername != user.UserName)
            activity.Attendees!.Remove(attendance);

        // if there is no attendance in the list then a new attendance will be created and input to the list in Activity.Attendees
        if (attendance == null)
        {
            attendance = new ActivityAttendee
            {
                AppUser = user,
                Activity = activity,
                IsHost = false
            };

            // add the attendance to the list
            activity.Attendees!.Add(attendance);
        }
        
        // save changed to database
        var result = await _context.SaveChangesAsync() > 0;

        return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating attendance");
      }
    }
  }
}