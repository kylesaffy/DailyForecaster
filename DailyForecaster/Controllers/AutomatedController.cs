using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyForecaster.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DailyForecaster.Controllers
{
	[Route("[controller]")]
	[EnableCors("AllowOrigin")]
	[ApiController]
	public class AutomatedController : ControllerBase
	{
		[Route("UpdateAccounts")]
		[HttpGet]
		public async Task<ActionResult> UpdateAccounts()
		{
			Collections collections = new Collections();
			return Ok(await collections.YodleeAccountConnect());
		}
		[Route("BudgetDuplicate")]
		[HttpGet]
		public ActionResult BudgetDuplicate()
		{
			Collections collections = new Collections();
			return Ok(collections.CollectionCycle());
		}
		[Route("UpdateTransactions")]
		[HttpGet]
		public async Task<ActionResult> UpdateTransactions()
		{
			AutomatedCashFlow automatedCashFlow = new AutomatedCashFlow();
			return Ok(await automatedCashFlow.UpdateTrandactions());
		}
	}
}
