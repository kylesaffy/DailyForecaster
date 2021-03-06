﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DailyForecaster.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

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
		[Route("ScheduledTransactionsRun")]
		[HttpGet]
		public ActionResult ScheduledTransactionsRun()
		{
			DateTime start = DateTime.Now;
			ScheduledTransactions transactions = new ScheduledTransactions();
			transactions.Check();
			DateTime end = DateTime.Now;
			AutomatedLog log = new AutomatedLog();
			log.SaveLog(start, end, "ScheduledTransactionsRun", true);
			return Ok(true);
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
			ClickTracker tracker = new ClickTracker();
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
			EmailPreferences preferences = new EmailPreferences();
			//FirebaseUser item = users.Where(x => x.Email == "kylesaffy@gmail.com").FirstOrDefault();
			//model = email.DailyEmailSend(item.FirebaseUserId);
			foreach (FirebaseUser item in users)
			{
				preferences.CheckPrefrences(item);
				if (model.result && preferences.CheckDaily(item))
				{
					model = email.DailyEmailSend(item.FirebaseUserId);
				}
			}
			return Ok(model);
		}
		[Route("Register")]
		[HttpPost]
		public async Task<ActionResult> Register()
		{
			string json = "{\"CollectionsId\": \"30c1ce56-964a-4ceb-85df-cceb3a09b417\",\"UserId\": \"uTSmqNJBCHPHwE7XSNVjpzR1mSf2\"}";
			RegisterModel model = JsonConvert.DeserializeObject<RegisterModel>(json);
			YodleeModel yodleeModel = new YodleeModel();
			return Ok(await yodleeModel.Register(model.UserId, model.CollectionsId));
		}
		[Route("RunReaderCall")]
		[HttpGet]
		public async Task<ActionResult> RunReaderCall(string url)
		{
			string accountId = "c07f0c85-a275-4626-a7a0-48b79e5354b6";
			string userId = "PkPnJIuCxbS14NrcWJ6VQpp2cdn2";
			string cftypeid = "4f19ee58-9a78-43b5-8cb6-331b75dc8b39";
			ExpenseModel reader = new ExpenseModel();
			ReturnModel model = await reader.BuildPartial(url, accountId, userId, cftypeid);
			return Ok(model);
		}
		[Route("GetYodlee")]
		[HttpGet]
		public async Task<ActionResult> GetYodlee(string id)
		{
			YodleeAccountModel model = new YodleeAccountModel();
			return Ok(await model.GetYodleeAccounts(id));
		}
		[Route("RemoveAutoUpdates")]
		[HttpGet]
		public async Task<ActionResult> RemoveAutoUpdates(string id)
		{
			YodleeModel model = new YodleeModel();
			return Ok(await model.RemoveAutoUpdates(id));
		}
		[Route("GetYodleeFNB")]
		[HttpGet]
		public async Task<ActionResult> GetYodleeFNB(string id)
		{
			YodleeAccountModel model = new YodleeAccountModel();
			List<YodleeAccountLevel> accounts = await model.GetYodleeAccounts(id);
			return Ok(accounts.Where(x => x.providerId == "15376").ToList().Count());
		}
		[Route("DeleteYodlee")]
		[HttpGet]
		public async Task<ActionResult> DeleteYodlee(string id, bool ans, string collectionsId)
		{
			IncludeYodlee includeYodlee = new IncludeYodlee(collectionsId);
			return Ok(await includeYodlee.Update(ans, id));
		}
		[Route("RunYodlee")]
		[HttpGet]
		public async Task<ActionResult> RunYodlee(string id)
		{
			Account model = new Account();
			List<Account> accounts = model.GetAccounts(id);
			return Ok(await model.UpdateAccounts(id, accounts));
		}
		[Route("Reader")]
		[HttpGet]
		public async Task<ActionResult> Reader(string url)
		{
			RunReader reader = new RunReader();
			reader = await reader.GetRunReader(url);
			return Ok(reader);
		}
		[Route("ManualLogout")]
		[HttpGet]
		public async Task<ActionResult> ManualLogout(string id)
		{
			LogoffModel model = new LogoffModel();
			DateTime result = await model.RevokeToken(id);
			return Ok(result);
		}
		[Route("Test")]
		[HttpGet]
		public async Task<ActionResult> Test(string id)
		{
			YodleeTokenGenerator generator = new YodleeTokenGenerator();
			string key = await generator.CreateToken(id);
			return Ok(key);
		}
		[Route("Trainer")]
		[HttpGet]
		public async Task<ActionResult> Trainer(string url)
		{
			FormRecogniser recogniser = new FormRecogniser();
			return Ok(await recogniser.Trainer(url));
		}
		[Route("RecreateSim")]
		[HttpGet]
		public ActionResult RecreateSim(string id, string userId)
		{
			Simulation sim = new Simulation();
			sim = sim.Get(id);
			sim.Recreate(userId);
			return Ok(new BudgetVM("e146a403-0781-4826-af14-cadcaf245e09"));
		}
		[Route("ReBuildSim")]
		[HttpGet]
		public ActionResult ReBuildSim(string id, string userId, string collectionsId)
		{
			SimulationAssumptions assumptions = new SimulationAssumptions(id);
			Simulation sim = new Simulation(assumptions, collectionsId);
			sim = sim.BuildSimulation(assumptions, userId, collectionsId);
			return Ok(sim);
		}
		[Route("CreateSim")]
		[HttpGet]
		public ActionResult CreateSim(string id, string userId)
		{
			Simulation sim = new Simulation();
			sim = sim.Get(id);
			sim.SimulationAssumptions = new SimulationAssumptions(sim.SimulationAssumptionsId);
			sim = sim.BuildSimulation(sim.SimulationAssumptions, userId, sim.CollectionsId);
			return Ok(sim);
		}
		[Route("General")]
		[HttpGet]
		public ActionResult General(string email)
		{
			return Ok(new UnseenModel(email));
		}
	}
	public class RegisterModel
	{
		public string CollectionsId { get; set; }
		public string UserId { get; set; }
	}
}
