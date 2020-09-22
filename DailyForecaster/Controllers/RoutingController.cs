using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DailyForecaster.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FirebaseAuth = FirebaseAdmin.Auth.FirebaseAuth;

namespace DailyForecaster.Controllers
{
	[Route("[controller]")]
	[EnableCors("AllowOrigin")]
	[ApiController]
	public class RoutingController : ControllerBase
	{				
		[Route("OpenPost")]
		[HttpPost]
		public ActionResult OpenPost([FromBody] JsonElement json)
		{
			return Ok();
		}
		[Route("Get")]
		[HttpGet]
		public async Task<ActionResult> Get()
		{
			string authHeader = this.HttpContext.Request.Headers["Authorization"];
			FirebaseToken auth = await validate(authHeader);
			if (auth != null)
			{
				FirebaseUser firebaseUser = new FirebaseUser();
				try
				{
					new ClickTracker(this.HttpContext.Request.Headers["Route"]
						, true,
						false,
						"collectionsId: " + this.HttpContext.Request.Headers["collectionsId"] + ", accountId: " + this.HttpContext.Request.Headers["accountId"] + ", startDate: " + this.HttpContext.Request.Headers["startDate"] + ", endDate: " + this.HttpContext.Request.Headers["startDate"] + ", email: " + auth.Claims["email"].ToString(),
						firebaseUser.GetUserId(auth.Claims["email"].ToString()),
						true);
				}
				catch (Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
				}
				string route = this.HttpContext.Request.Headers["Route"];
				string collectionsId = "";
				if (this.HttpContext.Request.Headers["collectionsId"] != "")
				{
					collectionsId = this.HttpContext.Request.Headers["collectionsId"];
				}
				string accountId = "";
				if (this.HttpContext.Request.Headers["accountId"] != "")
				{
					accountId = this.HttpContext.Request.Headers["accountId"];
				}
				DateTime startDate = DateTime.Now;
				if (this.HttpContext.Request.Headers["startDate"].Count() != 0)
				{
					startDate = DateConvert(this.HttpContext.Request.Headers["startDate"]);
				}
				DateTime endDate = DateTime.Now;
				if (this.HttpContext.Request.Headers["endDate"].Count() != 0)
				{
					endDate = DateConvert(this.HttpContext.Request.Headers["endDate"]);
				}		 			
				if (auth.ExpirationTimeSeconds > DateTimeOffset.Now.ToUnixTimeSeconds())
				{
					FirebaseUser user = new FirebaseUser();
					if (!user.Exsits(auth.Claims["email"].ToString()))
					{
						new FirebaseUser(auth.Claims["email"].ToString(), auth.Uid);
					}
					switch (route)
					{
						case "UnseenCount":
							return UnseenCount(auth.Claims["email"].ToString());
						case "Index":
							return Index(auth.Claims["email"].ToString());
						case "GetAccounts":
							return GetAccounts(collectionsId, auth.Claims["email"].ToString());
						case "GetReportedTransaction":
							return GetReportedTransactions(accountId, startDate, endDate);
						case "GetAccount":
							return GetAccount(accountId);
						case "GetCollectionsMenu":
							return GetCollectionsMenu(auth.Claims["email"].ToString());
						case "EditBudget":
							return EditBudget(auth.Claims["email"].ToString(), collectionsId);
						case "SafeToSpend":
							return SafeToSpend(auth.Claims["email"].ToString(), collectionsId);
					}
				}
			}
			return Ok();
		}
		[Route("Post")]
		[HttpPost]
		public async Task<ActionResult> Post([FromBody] JsonElement json)
		{
			string authHeader = this.HttpContext.Request.Headers["Authorization"];
			string route = this.HttpContext.Request.Headers["Route"];
			string collectionsId = "";
			if(this.HttpContext.Request.Headers["collectionsId"] != "")
			{
				collectionsId = this.HttpContext.Request.Headers["collectionsId"];
			}
			FirebaseToken auth = await validate(authHeader);
			if (auth != null)
			{
				FirebaseUser firebaseUser = new FirebaseUser();
				new ClickTracker(this.HttpContext.Request.Headers["Route"],
				false,
				true,
				"collectionsId: " + this.HttpContext.Request.Headers["collectionsId"] + ", accountId: " + this.HttpContext.Request.Headers["accountId"] + ", startDate: " + this.HttpContext.Request.Headers["startDate"] + ", endDate: " + this.HttpContext.Request.Headers["startDate"] + ", email: " + auth.Claims["email"].ToString(),
				firebaseUser.GetUserId(auth.Claims["email"].ToString()) + ", body: " + json,
				true);
			
				if (auth.ExpirationTimeSeconds > DateTimeOffset.Now.ToUnixTimeSeconds())
				{
					switch (route)
					{
						case "NewUser":
							new FirebaseUser(auth.Claims["email"].ToString(), auth.Uid);
							return Ok();
						case "BuildSimulation":
							return BuildSimulation(json.GetRawText(), collectionsId);
						case "AccountChange":
							return AccountChange(json);
						case "EditBudget":
							return EditBudget(json);
						case "BudgetChange":
							return BudgetChange(json, auth.Uid);
						case "BudgetTransactionDelete":
							return BudgetTransactionDelete(json);
						case "UpdateSplits":
							return UpdateSplits(json);
						case "LoginEmail":
							return LoginEmail(auth.Claims["email"].ToString());
					}
				}
			}
			return Ok();
		}
		[Route("Create")]
		[HttpPost]
		public async Task<ActionResult> Create([FromBody] JsonElement json)
		{
			UserModel model = new UserModel();
			return Ok(await model.CreateUser(JsonConvert.DeserializeObject<UserModel>(json.GetRawText())));
		}
		[Route("PostTest")]
		[HttpPost]
		public async Task<ActionResult> PostTest([FromBody] JsonElement json)
		{
			string authHeader = this.HttpContext.Request.Headers["Authorization"];
			string route = this.HttpContext.Request.Headers["Route"];
			string collectionsId = "";
			if (this.HttpContext.Request.Headers["collectionsId"] != "")
			{
				collectionsId = this.HttpContext.Request.Headers["collectionsId"];
			}
			if(authHeader == "A90986C6-B81D-45C7-97E2-B6EEE486A2D0E9BE35C6-36DB-441D-93CE-EF950BA6A282DD2C9CE3-4895-46CA-9307-386BF3391CC1" && (route == "BuildSimulation" || route == "UpdateSimulation"))
			{
				switch (route)
				{
					case "BuildSimulation":
						return BuildSimulation(json.GetRawText(), collectionsId);
					case "UpdateSimulation":
						return UpdateSimulation(json.GetRawText());
				}
			}
			return Ok();
		}
		/// <summary>
		/// Login in Email notification
		/// </summary>
		/// <param name="emailAddress">Email Address of the user</param>
		/// <returns>Return Model of the outcome of the sending of the email</returns>
		private ActionResult LoginEmail(string emailAddress)
		{
			EmailFunction email = new EmailFunction()
			{
				To = emailAddress,
				Subject = "Login",
			};
			return (Ok(email.SendEmail("")));
		}
		/// <summary>
		/// Updates list of transaction splits passed to it with reference to the database version and the version passed
		/// </summary>
		/// <param name="json">JSON version of the list of split transactions</param>
		/// <returns>Updated lsit of split transactions</returns>
		private ActionResult UpdateSplits(JsonElement json)
		{
			List<SplitTransactions> splits = JsonConvert.DeserializeObject<List<SplitTransactions>>(json.GetRawText());
			List<SplitTransactions> newList = new List<SplitTransactions>();
			foreach(SplitTransactions item in splits)
			{
				newList.Add(item.UpdateSplit());
			}
			return Ok(newList);
		}
		/// <summary>
		/// Deletes the object that is passed to it
		/// </summary>
		/// <param name="json">The JSON version of the object</param>
		/// <returns>Ok</returns>
		private ActionResult BudgetTransactionDelete(JsonElement json)
		{
			BudgetTransaction transaction = JsonConvert.DeserializeObject<BudgetTransaction>(json.GetRawText());
			transaction.Delete();
			return Ok();
		}
		/// <summary>
		/// Adds or ammends a budget transaction object
		/// </summary>
		/// <param name="json">the JSON version of the object</param>
		/// <param name="userId">The firebase userId</param>
		/// <returns></returns>
		private ActionResult BudgetChange(JsonElement json, string userId)
		{
			BudgetTransaction transaction = JsonConvert.DeserializeObject<BudgetTransaction>(json.GetRawText());
			transaction.Save(userId);
			return Ok(transaction);
		}
		/// <summary>
		/// Returns a VM that contains BudgetTransactionComparison, a list of the reportedTransactions and the budget item in question
		/// </summary>
		/// <param name="email">Email Address of the user</param>
		/// <param name="collectionsId">Id of the collection that the user is interacting with</param>
		/// <returns>A safe to spend object that contains all of the elements that are needed</returns>
		private ActionResult SafeToSpend(string email, string collectionsId)
		{
			SafeToSpendVM vm = new SafeToSpendVM(collectionsId, email);
			return Ok(vm);
		}
		private ActionResult EditBudget(JsonElement json)
		{
			Collections collection = JsonConvert.DeserializeObject<Collections>(json.GetRawText());

			return Ok();
		}
		/// <summary>
		/// Either creates or retrieves a budget object packaged a the only budget in a collection object
		/// </summary>
		/// <param name="email">Email address of the user</param>
		/// <param name="collectionsId">Id of the collection that the user is interacting with</param>
		/// <returns>A collection object with a single budget object within it</returns>
		private ActionResult EditBudget(string email, string collectionsId)
		{
			BudgetVM vm = new BudgetVM(collectionsId, email);
			return Ok(vm);
		}
		/// <summary>
		/// Populates the menu data for Vue FE
		/// </summary>
		/// <param name="email">The users email address is needed as an identifier</param>
		/// <returns>The required data to populate the FE left Menu</returns>
		private ActionResult GetCollectionsMenu(string email)
		{
			List<MenuData> menu = new List<MenuData>();
			List<MenuData> subMenu = new List<MenuData>();
			menu.Add(new MenuData()
			{
				Category = true,
				Title = "Dashboards"
			});
			subMenu.Add(new MenuData()
			{
				Title = "Homepage",
				Key = "Homepage",
				Url = "/dashboard/homepage"
			});
			subMenu.Add(new MenuData()
			{
				Title = "Collections",
				Key = "Collections",
				Url = "/dashboard/collections"
			});
			subMenu.Add(new MenuData()
			{
				Title = "Budget",
				Key = "Budget",
				Url = "/dashboard/budget"
			});
			subMenu.Add(new MenuData()
			{
				Title = "Safe To Spend",
				Key = "SafeToSpend",
				Url = "/dashboard/SafeToSpend"
			});
			menu.Add(new MenuData()
			{
				Title = "Homepage",
				Key = "Dashboards",
				Icon = "fe fe-home",
				Count = 4,
				Children = subMenu
			});
			return Ok(menu);
		}
		/// <summary>
		/// Creates or Updates Account
		/// </summary>
		/// <param name="json">Json Element Passed by FE</param>
		/// <returns>Updated Account Object</returns>
		private ActionResult AccountChange(JsonElement json)
		{
			Account account = JsonConvert.DeserializeObject<Account>(json.GetRawText());
			return Ok(account.AddAccount());
		}
		/// <summary>
		/// Returns a single Account object from an Id
		/// </summary>
		/// <param name="accountId">Account Id associated tot he account that is needed</param>
		/// <returns>Returns a single Account object from an Id</returns>
		private ActionResult GetAccount(string accountId)
		{
			return Ok(new AccountVM(accountId));
		}
		/// <summary>
		/// Converts date in the form of "MM/dd//yyyy" to DateTime
		/// </summary>
		/// <param name="date">date string in the form of "MM/dd/yyyy"</param>
		/// <returns>DateTime</returns>
		private DateTime DateConvert(string date)
		{
			string[] dateParts = date.Split("/");
			int month = Convert.ToInt32(dateParts[0]);
			int day = Convert.ToInt32(dateParts[1]);
			int year = Convert.ToInt32(dateParts[2]);
			return new DateTime(year,month,day);
		}
		/// <summary>
		/// Get for the reported transactions for a specified account
		/// </summary>
		/// <param name="accountId">Id for the account that is being requested</param>
		/// <param name="startDate">The start date for which to request the transactions</param>
		/// <param name="endDate">The end date for which to request the transactions</param>
		/// <returns>A List of the Reported Transactions that is associated on that account between the specified dates</returns>
		private ActionResult GetReportedTransactions(string accountId,DateTime startDate,DateTime endDate)
		{
			ReportedTransaction transaction = new ReportedTransaction();
			return Ok(transaction.GetTransactions(accountId,startDate,endDate));
		}
		/// <summary>
		/// Returns a List of accounts in the collection VM associated with a collection and a user email
		/// </summary>
		/// <param name="collectionsId">Id of the collection that was requested</param>
		/// <param name="email">email address of the user</param>
		/// <returns>Reutrns a collectiuon VM for the user</returns>
		private ActionResult GetAccounts(string collectionsId, string email)
		{
			return Ok(new CollectionVM(collectionsId,email));
		}
		private ActionResult UpdateSimulation(string json)
		{
			Simulation simulation = JsonConvert.DeserializeObject<Simulation>(json);
			simulation.Edit();
			return Ok(simulation);
		}
		private ActionResult BuildSimulation(string json,string collectionsId)
		{
			SimulationAssumptions assumptions = JsonConvert.DeserializeObject<SimulationAssumptions>(json);
			Simulation simulation = new Simulation(assumptions, collectionsId);
			simulation.BuildSimulation(assumptions);
			simulation.Scenario();
			return Ok(simulation);
		}
		private async Task<string> Verify(string token)
		{
			try
			{
				// Verify the ID token while checking if the token is revoked by passing checkRevoked
				// as true.
				bool checkRevoked = true;
				var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(
					token, checkRevoked);
				// Token is valid and not revoked.
				return decodedToken.Uid;
			}
			catch (Firebase.Auth.FirebaseAuthException ex)
			{
				return null;
			}
		}
		private async Task<FirebaseToken> validate(string token)
		{
			var auth = await FirebaseAuth.DefaultInstance
				.VerifyIdTokenAsync(token);
			if (auth.ExpirationTimeSeconds > DateTimeOffset.Now.ToUnixTimeSeconds())
			{
				string result = await Verify(token);
				if(result != null)
				{
					return auth;
				}
				else
				{
					return null;
				}
			}
			return null;
		}
		private ActionResult Index(string email)
		{
			IndexModel model = new IndexModel();
			return Ok(model.GetModel(email));
		}
		private ActionResult UnseenCount(string email)
		{
			int count = 0;
			AspNetUsers users = new AspNetUsers();
			string id = users.getUserId(email);
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				List<string> collections = _context
					.UserCollectionMapping
					.Where(x => x.Id == id)
					.Select(x => x.CollectionsId)
					.ToList();
				List<string> accounts = _context
					.Account
					.Where(Acc => collections.Contains(Acc.CollectionsId))
					.Select(x => x.Id)
					.ToList();
				foreach (string item in accounts)
				{
					count = count + _context
						.AutomatedCashFlows
						.Where(auto => item.Contains(auto.AccountId))
						.Where(x => x.Validated == false)
						.Count();
				}
				return Ok(count);
			}
		}
	}
}
