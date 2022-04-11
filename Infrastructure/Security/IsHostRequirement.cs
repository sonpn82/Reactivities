using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    // check if an user is the host of an event or not
    // need database, activity id in the link and the token to get user id also
    public class IsHostRequirement : IAuthorizationRequirement
    {        
    }

  public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
  {
    private readonly DataContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IsHostRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;  // database
      _httpContextAccessor = httpContextAccessor;  // contain link which contain activity id in the link
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
    {      
        // get userId from token-Claims-NameIdentifier in User database
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        // check if this id is valid - it is in database or not
        if (userId == null) return Task.CompletedTask;     

        // get activity Id from the parameter in the link
        var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues.
                        SingleOrDefault(x => x.Key == "id").Value?.ToString()!);

        // check the ActivityAttendee table to find the attendee with userId and activityId
        var attendee = _dbContext.ActivityAttendees
                        .AsNoTracking()  // must use AsNoTracking here, if not then when edit an activity the attendee object here will persist and make HostUSerName become null, attendees become empty array
                        .SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activityId)
                        .Result;          

        // if no attendee is found then exit
        if (attendee == null) return Task.CompletedTask; 
  
        // if attendee is Host then requirement is satisfied
        if (attendee.IsHost) {
          context.Succeed(requirement);
        }        

        return Task.CompletedTask;
    }
  }
}