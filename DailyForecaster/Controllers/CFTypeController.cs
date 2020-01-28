using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DailyForecaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CFTypeController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetCFType(string id)
        {
            return null;
        }
    }
}