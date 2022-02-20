using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
  public class ActivitiesController : BaseApiController  // BaseApiController in BaseApiController.cs
  {
    // Using Mediator to access to database data - get, put, del ...
    // return list of all Activity
    [HttpGet]  // /api/activities
    public async Task<ActionResult<List<Activity>>> GetActivities()
    { 
      // Mediator is passdown from its parent class of BaseApiController
        return await Mediator.Send(new List.Query());
    }

    // return activity with same id
    [HttpGet("{id}")] // api/activities/id
    public async Task<ActionResult<Activity>> GetActivity(Guid id)
    { 
      // using mediator to get data from an id of activity 
      return await Mediator.Send(new Details.Query{Id = id});
    }

    // create a new activity
    [HttpPost]
    // program can automatically look for Activity object in request body
    public async Task<IActionResult> CreateActivity(Activity activity)
    {
      return Ok(await Mediator.Send(new Create.Command {Activity = activity}));
    }

    // Update the activity
    [HttpPut("{id}")]
    public async Task<IActionResult> EditActivity(Guid id, Activity activity)
    {
      activity.Id = id;
      return Ok(await Mediator.Send(new Edit.Command{Activity = activity}));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
      return Ok(await Mediator.Send(new Delete.Command{Id = id}));
    }
  }
}