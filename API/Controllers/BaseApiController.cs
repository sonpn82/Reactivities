using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // route will be /api/ourcontrollername, ex /api/activities
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        //protected prop can be accessed by all derived class
        // Allow all derived controller to have a Mediator - if mediator is null then use Httpcontext...
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}