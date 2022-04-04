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

namespace Application.Followers
{
    // class to toggle the follow / unfollow feature
    public class FollowToggle
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string? TargetUsername { get; set; }  // prop of this request - the current user click on the target user to follow/unfollow
        }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;  // to get user info
      private readonly IUserAccessor _userAccessor;  // to get current user info

      public Handler(DataContext context, IUserAccessor userAccessor)      
      {
        _context = context;
        _userAccessor = userAccessor;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        // get the current username (is observer in the following table)
        var observer = await _context.Users.FirstOrDefaultAsync(x => 
            x.UserName == _userAccessor.GetUsername());

        // get the request username (the person selected when current user click follow/unfollow)
        var target = await _context.Users.FirstOrDefaultAsync(x => 
            x.UserName == request.TargetUsername);

        if (target == null || observer == null) return null!;

        // find this data in the UserFollowings table (by a couple key: observer.id and target.id)
        var following = await _context.UserFollowings.FindAsync(observer.Id, target.Id);

        // if not found then current user will follow this user
        if (following == null)
        {
            following = new UserFollowing
            {
                Observer = observer,
                Target = target
            };

            // add new userfollowing to database
            _context.UserFollowings.Add(following);
        } else
        // unfollow this user by removing its data from UserFollowings database
        {
            _context.UserFollowings.Remove(following);
        }

        var success = await _context.SaveChangesAsync() > 0;

        if (success) return Result<Unit>.Success(Unit.Value);

        return Result<Unit>.Failure("Failed to update following");
      }
    }
  }
}