using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // route will be /api/ourcontrollername, ex /api/activities
    public class BaseApiController : ControllerBase
    {
        
    }
}