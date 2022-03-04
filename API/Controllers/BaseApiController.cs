using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Application.Core;

namespace API.Controllers
{
    [ApiController]  // controller will auto send back 400 response validation error
    [Route("api/[controller]")]  // route will be /api/ourcontrollername, ex /api/activities
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        //protected prop can be accessed by all derived class
        // Allow all derived controller to have a Mediator - if mediator is null then use Httpcontext...
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        // All derived controller will be able to handle the Result object & return an HttpResponse
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();  // handle the null case - not provided by Result.cs
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
                
            return BadRequest(result.Error);
        }
    }
}