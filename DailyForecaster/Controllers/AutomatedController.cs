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
		[Route("UpdateYodleeAccounts")]
		[HttpGet]
		public async Task<ActionResult> UpdateYodleeAccounts()
		{
			DateTime start = DateTime.Now;
			YodleeAccountModel model = new YodleeAccountModel();
			bool result = await model.UpdateYodlee();
			DateTime end = DateTime.Now;
			AutomatedLog log = new AutomatedLog();
			log.SaveLog(start, end, "UpdateYodleeAccounts", result);
			return Ok(result);
		}
		[Route("UpdateAccounts")]
		[HttpGet]
		public async Task<ActionResult> UpdateAccounts()
		{
			DateTime start = DateTime.Now;
			Collections collections = new Collections();
			bool result = await collections.YodleeAccountConnect();
			DateTime end = DateTime.Now;
			AutomatedLog log = new AutomatedLog();
			log.SaveLog(start, end, "UpdateAccounts", result);
			return Ok(result);
		}
		[Route("BudgetDuplicate")]
		[HttpGet]
		public ActionResult BudgetDuplicate()
		{
			DateTime start = DateTime.Now;
			Collections collections = new Collections();
			bool result = collections.CollectionCycle();
			DateTime end = DateTime.Now;
			AutomatedLog log = new AutomatedLog();
			log.SaveLog(start, end, "BudgetDuplicate", result);
			return Ok(result);
		}
		[Route("UpdateTransactions")]
		[HttpGet]
		public async Task<ActionResult> UpdateTransactions()
		{
			DateTime start = DateTime.Now;
			AutomatedCashFlow automatedCashFlow = new AutomatedCashFlow();
			bool result = await automatedCashFlow.UpdateTransactions();
			automatedCashFlow.TransactionClassifier();
			DateTime end = DateTime.Now;
			AutomatedLog log = new AutomatedLog();
			log.SaveLog(start, end, "UpdateTransactions", result);
			return Ok(result);
		}
		[Route("UserActivitity")]
		[HttpGet]
		public async Task<ActionResult> UserActivitity()
		{
			DateTime start = DateTime.Now;
			ClickTracker tracker= new ClickTracker();
			bool result = false;
			try
			{
				result = await tracker.ActivityTracker();
			}
			catch
			{
				result = false;
			}
			DateTime end = DateTime.Now;
			AutomatedLog log = new AutomatedLog();
			log.SaveLog(start, end, "UserActivitity", result);
			return Ok(result);
		}
		[Route("RunAmort")]
		[HttpGet]
		public ActionResult RunAmort()
		{
			Account account = new Account("e8db523a-8391-4264-86a3-5dfa47870a34");
			AccountAmortisation amortisation = new AccountAmortisation();
			return Ok(amortisation.CalculateMonthly(account));
		}
		[Route("DailyGrind")]
		public ActionResult DailyGrind()
		{
			FirebaseUser firebaseUser = new FirebaseUser();
			List<FirebaseUser> users = firebaseUser.GetUserList();
			ReturnModel model = new ReturnModel() { result = true };
			EmailFunction email = new EmailFunction();
			foreach(FirebaseUser item in users)
			{
				if(model.result)
				{
					model = email.DailyEmailSend(item.FirebaseUserId);
				}
			}
			return Ok(model);
		}
	}
}
