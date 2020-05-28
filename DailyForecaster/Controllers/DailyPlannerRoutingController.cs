using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DailyForecaster.Models;
using System.Web.Http.Description;
using System.Web.Http;

namespace DailyForecaster.Controllers
{
    [Route("api/DailyPlannerRouting")]
    public class DailyPlannerRoutingController : ApiController
    {
        [Route("Create")]
        //[ResponseType(typeof(ManualCashFlow))]
        public IHttpActionResult Create()
        {
            Collections collections = new Collections();
            string blobString = collections.Account.Institution.BlobString;
            double total = numbers
            return Ok(new { Name = "Success" });
        }
    }
}