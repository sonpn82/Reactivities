using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
  //[AllowAnonymous] enable anyone to get data without login token
  public class ActivitiesController : BaseApiController  // BaseApiController in BaseApiController.cs
  {
    // Using Mediator to access to database data - get, put, del ...
    // return list of all Activity
    [HttpGet]  // /api/activities?pageNumber=x&pageSize=y
    public async Task<IActionResult> GetActivities([FromQuery]ActivityParams param)  // add paging here - ActivityParams for filtering
    { 
      // Mediator is passdown from its parent class of BaseApiController
        return HandlePagedResult(await Mediator.Send(new List.Query{Params = param}));  // use HandlePageResult instead of HandleResult to get paging
    }

    // return activity with same id 
    [HttpGet("{id}")] // api/activities/id
    public async Task<IActionResult> GetActivity(Guid id)  // IActionResult can return an HttpResponse
    { 
      // using mediator to get data from an id of activity  
      // HandleResult will handle the error handling when query for activity id 
      return HandleResult(await Mediator.Send(new Details.Query{Id = id}));
    }

    // create a new activity
    [HttpPost]
    // program can automatically look for Activity object in request body
    public async Task<IActionResult> CreateActivity(Activity activity)
    {
      // HandleResult will handle the error handling when create an activity
      return HandleResult(await Mediator.Send(new Create.Command {Activity = activity}));
    }

    // Edit the activity
    // /api/activities/id
    [Authorize(Policy = "IsActivityHost")]  // service to check if user is host of the activity to be edited or not - IdentityServiceExtention.cs + IsHostRequirement.cs
    [HttpPut("{id}")]
    public async Task<IActionResult> EditActivity(Guid id, Activity activity)
    {
      activity.Id = id;
      // Handle error using HandleResult
      return HandleResult(await Mediator.Send(new Edit.Command{Activity = activity}));
    }

    // Delete an activity
    // /api/activities/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
      // handle error using HandleResult
      return HandleResult(await Mediator.Send(new Delete.Command{Id = id}));
    }

    // Attend or cancel an activity by user
    // /api/activities/id/attend
    [HttpPost("{id}/attend")]
    public async Task<IActionResult> Attend(Guid id)
    {
      return HandleResult(await Mediator.Send(new UpdateAttendance.Command{Id = id}));
    }
  }
}