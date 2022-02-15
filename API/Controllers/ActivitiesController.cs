using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
  public class ActivitiesController : BaseApiController  // BaseApiController in BaseApiController.cs
  {
    private readonly DataContext _context;

    public ActivitiesController(DataContext context)  // DataContext from Persistence.cs
    {
      _context = context;
    }

    // return list of all Activity
    [HttpGet]  // /api/activities
    public async Task<ActionResult<List<Activity>>> GetActivities()
    {
        return await _context.Activities.ToListAsync();
    }

    // return activity with same id
    [HttpGet("{id}")] // api/activities/id
    public async Task<ActionResult<Activity>> GetActivity(Guid id)
    {
        return await _context.Activities.FindAsync(id);
    }
  }
}