using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DailyForecaster.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text.Json.Serialization;
// using Newtonsoft.Json;
using FirebaseAuth = FirebaseAdmin.Auth.FirebaseAuth;
using Newtonsoft.Json;

namespace DailyForecaster.Controllers
{
	[Route("[controller]")]
	[EnableCors("AllowOrigin")]
	[ApiController]
	public class RoutingController : ControllerBase
	{
		private readonly IOptions<MyConfig> config;



		private readonly TwilioAccountDetails _twilioAccountDetails;
		// I’ve injected twilioAccountDetails into the constructor

		public RoutingController(IOptions<TwilioAccountDetails> twilioAccountDetails)
		{
			// We want to know if twilioAccountDetails is null so we throw an exception if it is           
			_twilioAccountDetails = twilioAccountDetails.Value ?? throw new ArgumentException(nameof(twilioAccountDetails));
		}
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
			try
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
					string misc = "";
					if (this.HttpContext.Request.Headers["miscellaneous"] != "")
					{
						misc = this.HttpContext.Request.Headers["miscellaneous"].ToString();
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
						SmartHelper helper = new SmartHelper();
						switch (route)
						{
							case "UnseenCount":
								return UnseenCount(auth.Uid);
							case "Index":
								return Index(auth.Claims["email"].ToString());
							case "GetAccounts":
								return GetAccounts(collectionsId, auth.Claims["email"].ToString());
							case "GetReportedTransaction":
								return GetReportedTransactions(accountId, startDate, endDate);
							case "GetAccount":
								return GetAccount(accountId);
							case "GetCollectionsMenu":
								return GetCollectionsMenu(auth.Uid);
							case "EditBudget":
								return EditBudget(auth.Claims["email"].ToString(), collectionsId);
							case "SafeToSpend":
								return SafeToSpend(auth.Claims["email"].ToString(), collectionsId);
							case "ShareCollection":
								return ShareCollection(collectionsId);
							case "ManualCashFlows":
								return ManualCashFlows(auth.Claims["email"].ToString(), collectionsId);
							case "GetUnseenTransactions":
								return GetUnseenTransactions(auth.Claims["email"].ToString());
							case "GetYodleeToken":
								return await GetYodleeToken(collectionsId, auth.Uid);
							case "GetUser":
								return GetUser(auth.Uid);
							case "SmartHelp":
								return SmartHelp(auth.Uid);
							case "GetCalculator":
								return GetCalculator();
							case "TwilioToken":
								return TwilioToken(auth.Uid);
							case "WelcomePage":
								return Ok(helper.WelcomePage());
							case "CollectionCount":
								return CollectionCount(auth.Uid);
							case "GetYodleeAccounts":
								return await GetYodleeAccounts(misc, collectionsId);
							case "CheckYodleeInclusion":
								return CheckYodleeInclusion(auth.Uid);
							case "NewToAccounts":
								return Ok(helper.NewToAccounts());
							case "GetNewSimulationAssumptions":
								return GetNewSimulationAssumptions(auth.Uid);
							case "GetSimulations":
								return GetSimulations(auth.Uid);
							case "GetSim":
								return GetSim(misc);
							case "GetBudget":
								return GetBudget(misc);
							case "UpdateYodlee":
								return await UpdateInclude(misc, collectionsId,auth.Uid);
							case "GetInclude":
								return GetInclude(collectionsId);
						}
					}
				}
				return Ok();
			}
			catch
			{
				return Ok("logoff");
			}
		}
		//[Route("Delete")]
		//[HttpDelete]
		//public async Task<ActionResult> Delete()
		//{
		//	string authHeader = this.HttpContext.Request.Headers["Authorization"];
		//	string route = this.HttpContext.Request.Headers["Route"];
		//	string collectionsId = "";
		//	if (this.HttpContext.Request.Headers["collectionsId"] != "")
		//	{
		//		collectionsId = this.HttpContext.Request.Headers["collectionsId"];
		//	}
		//	FirebaseToken auth = await validate(authHeader);
		//	if (auth != null)
		//	{
		//		FirebaseUser firebaseUser = new FirebaseUser();
		//		new ClickTracker(this.HttpContext.Request.Headers["Route"],
		//		false,
		//		true,
		//		"collectionsId: " + this.HttpContext.Request.Headers["collectionsId"] + ", accountId: " + this.HttpContext.Request.Headers["accountId"] + ", startDate: " + this.HttpContext.Request.Headers["startDate"] + ", endDate: " + this.HttpContext.Request.Headers["startDate"] + ", email: " + auth.Claims["email"].ToString(),
		//		firebaseUser.GetUserId(auth.Claims["email"].ToString()),
		//		true);

		//		if (auth.ExpirationTimeSeconds > DateTimeOffset.Now.ToUnixTimeSeconds())
		//		{
		//			switch (route)
		//			{
		//				case "DeleteCollection":
		//					return DeleteCollection(collectionsId);
		//			}
		//		}
		//	}
		//	return null;
		//}
		
		[Route("Post")]
		[HttpPost]
		public async Task<ActionResult> Post([FromBody] JsonElement json)
		{
			try
			{
				//string stringData = this.HttpContext.Request..Content.ReadAsStringAsync().Result;
				var reader = await this.HttpContext.Request.BodyReader.ReadAsync();
				Request.BodyReader.AdvanceTo(reader.Buffer.Start, reader.Buffer.End);
				string body = Encoding.UTF8.GetString(reader.Buffer.FirstSpan);
				string authHeader = this.HttpContext.Request.Headers["Authorization"];
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
				FirebaseToken auth = await validate(authHeader);
				if (auth != null)
				{
					FirebaseUser firebaseUser = new FirebaseUser();
					new ClickTracker(this.HttpContext.Request.Headers["Route"],
					false,
					true,
					"collectionsId: " + this.HttpContext.Request.Headers["collectionsId"] + ", accountId: " + this.HttpContext.Request.Headers["accountId"] + ", startDate: " + this.HttpContext.Request.Headers["startDate"] + ", endDate: " + this.HttpContext.Request.Headers["startDate"] + ", email: " + auth.Claims["email"].ToString() +
					", body: " + json, firebaseUser.GetUserId(auth.Claims["email"].ToString()),
					true);

					if (auth.ExpirationTimeSeconds > DateTimeOffset.Now.ToUnixTimeSeconds())
					{
						switch (route)
						{
							case "NewUser":
								new FirebaseUser(auth.Claims["email"].ToString(), auth.Uid);
								return Ok();
							case "BuildSimulation":
								return BuildSimulation(json.GetRawText(), collectionsId, auth.Uid);
							case "AccountChange":
								return await AccountChange(json);
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
							case "SaveTransaction":
								return SaveTransaction(json, auth.Uid);
							case "CreateCollection":
								return CreateCollection(json, auth.Uid, auth.Claims["email"].ToString());
							//return Ok();
							case "LinkShare":
								return LinkShare(json, auth.Uid);
							case "ManualCashFlows":
								return ManualCashFlows(json, auth.Claims["email"].ToString());
							case "UpdateUser":
								return UpdateUser(json, auth.Uid);
							case "NewCFType":
								return NewCFType(json, collectionsId);
							case "AddScheduledTransaction":
								return AddScheduledTransaction(json);
							case "Calculator":
								return Calculator(json);
							case "TwilioToken":
								return TwilioToken(auth.Uid);
							case "CreateInclusion":
								return CreateInclusion(json, collectionsId);
							case "ManualTransfer":
								return ManualTransfer(json);
						}
					}
				}
				return Ok();
			}
			catch
			{
				return Ok("logoff");
			}
		}
		[Route("PostFile")]
		[HttpPost]
		public async Task<IActionResult> PostFile()
		{
			try
			{
				string authHeader = this.HttpContext.Request.Headers["Authorization"];
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
				string cfTypeId = "";
				if (this.HttpContext.Request.Headers["cfTypeId"] != "")
				{
					cfTypeId = this.HttpContext.Request.Headers["cfTypeId"];
				}
				FirebaseToken auth = await validate(authHeader);
				if (auth != null)
				{
					FirebaseUser firebaseUser = new FirebaseUser();
					new ClickTracker(this.HttpContext.Request.Headers["Route"],
					false,
					true,
					"collectionsId: " + this.HttpContext.Request.Headers["collectionsId"] + ", accountId: " + this.HttpContext.Request.Headers["accountId"] + ", startDate: " + this.HttpContext.Request.Headers["startDate"] + ", endDate: " + this.HttpContext.Request.Headers["startDate"] + ", email: " + auth.Claims["email"].ToString(),
					firebaseUser.GetUserId(auth.Claims["email"].ToString()),
					true);

					if (auth.ExpirationTimeSeconds > DateTimeOffset.Now.ToUnixTimeSeconds())
					{
						switch (route)
						{
							case "InvoiceUpload":
								var files = Request.Form.Files[0];
								try
								{
									if (CloudStorageAccount.TryParse("DefaultEndpointsProtocol=https;AccountName=storageaccountmoney9367;AccountKey=AtoBz3bP/esi7HTyWqg3ySyGgoIolYp376gYMjKlsaiwqNaOaORIjHSUL0RqoXw0Il4epqjP31j/LkXwZLb+PQ==;EndpointSuffix=core.windows.net", out CloudStorageAccount storageAccount))
									{
										CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
										CloudBlobContainer container = blobClient.GetContainerReference("invoices");
										CloudBlockBlob blockBlob = container.GetBlockBlobReference(files.FileName);
										await blockBlob.UploadFromStreamAsync(files.OpenReadStream());
										string loc = "https://storageaccountmoney9367.blob.core.windows.net/invoices/" + files.FileName;
										ExpenseModel model = new ExpenseModel();
										ReturnModel returnModel = await model.Build(loc, accountId, auth.Uid);
										return Ok(true);
									}
									else
									{
										return Ok(false);
									}
								}
								catch (Exception e)
								{
									ExceptionCatcher catcher = new ExceptionCatcher();
									//catcher.Catch(e.Message);
									return Ok(false);
								}
							case "InvoiceUploadPartial":
								var files1 = Request.Form.Files[0];
								try
								{
									if (CloudStorageAccount.TryParse("DefaultEndpointsProtocol=https;AccountName=storageaccountmoney9367;AccountKey=AtoBz3bP/esi7HTyWqg3ySyGgoIolYp376gYMjKlsaiwqNaOaORIjHSUL0RqoXw0Il4epqjP31j/LkXwZLb+PQ==;EndpointSuffix=core.windows.net", out CloudStorageAccount storageAccount))
									{
										CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
										CloudBlobContainer container = blobClient.GetContainerReference("invoices");
										CloudBlockBlob blockBlob = container.GetBlockBlobReference(files1.FileName);
										await blockBlob.UploadFromStreamAsync(files1.OpenReadStream());
										string loc = "https://storageaccountmoney9367.blob.core.windows.net/invoices/" + files1.FileName;
										ExpenseModel model = new ExpenseModel();
										ReturnModel returnModel = await model.BuildPartial(loc, accountId, auth.Uid, cfTypeId);
										return Ok(true);
									}
									else
									{
										return Ok(false);
									}
								}
								catch (Exception e)
								{
									ExceptionCatcher catcher = new ExceptionCatcher();
									//catcher.Catch(e.Message);
									return Ok(false);
								}
							case "UploadProfileImage":
								var file = Request.Form.Files[0];
								try
								{
									if (CloudStorageAccount.TryParse("DefaultEndpointsProtocol=https;AccountName=storageaccountmoney9367;AccountKey=3XosEFLMi9B0r4V03Krn0d1yb6Ecwj6SJm8FcwzckmI9FOwUDG0/ZgPx0ZkXgkEFyh+CuiZqGZ3WJOmVnqRMxg==;EndpointSuffix=core.windows.net", out CloudStorageAccount storageAccount))
									{
										CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
										CloudBlobContainer container = blobClient.GetContainerReference("profileimage");
										string name = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
										CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
										await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
										FirebaseUser user = new FirebaseUser();
										user = user.GetUser(auth.Uid);
										user.Update(name, auth.Uid);
										return Ok(true);
									}
									else
									{
										return Ok(false);
									}
								}
								catch (Exception e)
								{
									ExceptionCatcher catcher = new ExceptionCatcher();
									//catcher.Catch(e.Message);
									return Ok(false);
								}
						}
					}
				}
				return Ok();
			}
			catch (Exception e)
			{
				return Ok("logoff");
			}

		}
		[Route("CheckUser")]
		[HttpGet]
		public async Task<ActionResult> CheckUser(string token)
		{
			try
			{
				FirebaseToken auth = await validate(token);
				return Ok(true);
			}
			catch
			{
				return Ok(false);
			}
		}
		[Route("GetTest")]
		[HttpGet]
		public ActionResult GetTest()
		{
			return Test();
		}
		private ActionResult Test()
		{
			string collectionsId = "d2dd81a4-3d19-4ea3-9243-da49e6a7e54e";
			BudgetTransactionComparison comparison = new BudgetTransactionComparison();
			return Ok(comparison.Get(collectionsId));
		}
		[Route("Create")]
		[HttpPost]
		public async Task<ActionResult> Create([FromBody] JsonElement json)
		{
			UserModel model = System.Text.Json.JsonSerializer.Deserialize<UserModel>(json.GetRawText());
			return Ok(await model.CreateUser(model));
		}
		[Route("PostTest")]
		[HttpPost]
		public async Task<ActionResult> PostTest([FromBody] JsonElement json)
		{
			string route = this.HttpContext.Request.Headers["Route"];
			string collectionsId = "";
			if (this.HttpContext.Request.Headers["collectionsId"] != "")
			{
				collectionsId = this.HttpContext.Request.Headers["collectionsId"];
			}
			{
				switch (route)
				{
					case "UpdateSimulation":
						return UpdateSimulation(json.GetRawText());
					case "AccountChange":
						return await AccountChange(json);
				}
			}
			return Ok();
		}
		private ActionResult ManualTransfer(JsonElement json)
		{
			ManualCashFlow manual = new ManualCashFlow();
			TransferObject transfer = JsonConvert.DeserializeObject<TransferObject>(json.GetRawText());
			return Ok(manual.AddTransfer(transfer));
		}
		private ActionResult GetInclude(string collectionsId)
		{
			return Ok(new IncludeYodlee(collectionsId));
		}
		private async Task<ActionResult> UpdateInclude(string misc, string collectionsId, string uid)
		{
			bool ans = Convert.ToBoolean(misc);
			IncludeYodlee includeYodlee = new IncludeYodlee(collectionsId);
			return Ok(await includeYodlee.Update(ans, uid));
		}
		private ActionResult GetBudget(string budgetId)
		{
			return Ok(new BudgetVM(budgetId));
		}
		private ActionResult GetSim(string id)
		{
			return Ok(new SimulationVM(id));
		}
		private ActionResult GetNewSimulationAssumptions(string uid)
		{
			return Ok(new SimulationAssumptionView(uid));
		}
		private ActionResult CreateInclusion(JsonElement json, string collectionsId)
		{
			bool result = JsonConvert.DeserializeObject<SelectionModel>(json.GetRawText()).Selection;
			IncludeYodlee include = new IncludeYodlee();
			include.Create(collectionsId, result);
			return Ok();
		}
		private ActionResult CheckYodleeInclusion(string uid)
		{
			IncludeYodlee includeYodlee = new IncludeYodlee();
			return Ok(includeYodlee.Exists(uid));
		}
		private async Task<ActionResult> GetYodleeAccounts(string id, string collectionsId)
		{
			Account account = new Account();
			return Ok(await account.GetYodleeAccounts(id, collectionsId));
		}
		private ActionResult CollectionCount(string uid)
		{
			UserCollectionMapping mapping = new UserCollectionMapping();
			if (mapping.CollectionsCounter(uid) > 0) return Ok(false);
			else return Ok(true);
		}
		private ActionResult TwilioToken(string identity)
		{
			if (identity == null) return null;
			TwilioTokenGenerator _tokenGenerator = new TwilioTokenGenerator();
			var token = _tokenGenerator.Generate(identity);
			return Ok(new { identity, token });
		}
		private ActionResult GetSimulations(string uid)
		{
			Simulation simulation = new Simulation();
			return Ok(simulation.GetSimulations(uid));
		}
		private ActionResult GetCalculator()
		{
			CalculatorModels model = new CalculatorModels();
			return Ok(model.Get());
		}
		private ActionResult Calculator(JsonElement json)
		{
			CalculatorModels model = System.Text.Json.JsonSerializer.Deserialize<CalculatorModels>(json.GetRawText());
			return Ok(model.Calculate());
		}
		private ActionResult AddScheduledTransaction(JsonElement json)
		{

			ScheduledTransactions transactions = System.Text.Json.JsonSerializer.Deserialize<tempScheduledTransaction>(json.GetRawText()).t;
			return Ok(transactions.Create());
		}
		public ActionResult GetUnseenTransactions(string email)
		{
			return Ok(new UnseenModel(email));
		}
		private ActionResult NewCFType(JsonElement json, string collectionsId)
		{
			CFType type = new CFType();
			type.CreateCFType(collectionsId, System.Text.Json.JsonSerializer.Deserialize<Newcftype>(json.GetRawText()).NewCFType);
			return Ok(type.GetCFList(collectionsId));
		}
		private ActionResult SmartHelp(string uid)
		{
			return Ok(new SmartHelper(uid));
		}
		private ActionResult UpdateUser(JsonElement json, string uid)
		{
			ProfileModel model = System.Text.Json.JsonSerializer.Deserialize<ProfileModel>(json.GetRawText());
			if(model.FirebaseUser == null)
			{
				model = JsonConvert.DeserializeObject<ProfileModel>(json.GetRawText());
			}
			return Ok(model.Update());
		}
		private ActionResult GetUser(string uid)
		{
			return Ok(new ProfileModel(uid));
		}
		private async Task<ActionResult> GetYodleeToken(string collectionsId, string uid)
		{
			YodleeModel yodleeModel = new YodleeModel();
			string token = await yodleeModel.getToken(collectionsId, uid);
			return Ok(token);
		}
		private ActionResult ManualCashFlows(JsonElement json, string email)
		{
			return Ok(new ManualCashFlow(System.Text.Json.JsonSerializer.Deserialize<ManualCashFlow>(json.GetRawText()), email));
		}
		private ActionResult ManualCashFlows(string email, string collectionsId)
		{
			return Ok(new ManualCashFlowsVM(collectionsId, email));
		}
		private ActionResult LinkShare(JsonElement json, string userId)
		{
			CollectionSharing sharing = new CollectionSharing();
			NewCollectionsObj obj = System.Text.Json.JsonSerializer.Deserialize<NewCollectionsObj>(json.GetRawText());
			obj.User = userId;
			return Ok(sharing.AddUserToCollection(obj));
		}
		private ActionResult ShareCollection(string collectionId)
		{
			return Ok(new CollectionSharing(collectionId));
		}
		/// <summary>
		/// Saves changes either to automated cash flows or to budgeted transactions
		/// </summary>
		/// <param name="json">the JSON version of the object</param>
		/// <param name="userId">The firebase userId</param>
		/// <returns>Updated version of the object</returns>
		private ActionResult SaveTransaction(JsonElement json, string userId)
		{
			try
			{
				ReportedTransaction transaction = System.Text.Json.JsonSerializer.Deserialize<ReportedTransaction>(json.GetRawText());
				if (transaction.Id == null)
				{
					transaction = JsonConvert.DeserializeObject<ReportedTransaction>(json.GetRawText());
				}
				if (transaction.AutomatedCashFlow != null)
				{
					AutomatedCashFlow flow = transaction.AutomatedCashFlow;
					flow.CFType = null;
					flow.CFTypeId = transaction.CFType.Id;
					flow.SourceOfExpense = transaction.SourceOfExpense;
					flow = flow.Save(flow);
					return Ok(new ReportedTransaction(flow, new Account(flow.AccountId)));
				}
				else
				{
					AutomatedCashFlow automated = System.Text.Json.JsonSerializer.Deserialize<AutomatedCashFlow>(json.GetRawText());
					if (automated.ID == null)
					{
						automated = JsonConvert.DeserializeObject<AutomatedCashFlow>(json.GetRawText());
					}
					automated.Save(automated);
					return Ok();
				}
			}
			catch
			{
				return BudgetChange(json, userId);
			}
		}
		private ActionResult CreateCollection(JsonElement json, string userId, string email)
		{
			NewCollectionsObj obj = System.Text.Json.JsonSerializer.Deserialize<NewCollectionsObj>(json.GetRawText());
			Collections collections = new Collections();
			return Ok(collections.CreateCollection(obj, userId, email));
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
			List<SplitTransactions> splits = System.Text.Json.JsonSerializer.Deserialize<List<SplitTransactions>>(json.GetRawText());
			List<SplitTransactions> newList = new List<SplitTransactions>();
			foreach (SplitTransactions item in splits)
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
			BudgetTransaction transaction = System.Text.Json.JsonSerializer.Deserialize<BudgetTransaction>(json.GetRawText());
			if(transaction.BudgetId == null)
			{
				transaction = JsonConvert.DeserializeObject<BudgetTransaction>(json.GetRawText());
			}
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
			try
			{
				FirebaseUser user = new FirebaseUser();
				userId = user.GetUserId(userId);
				BudgetTransaction transaction = System.Text.Json.JsonSerializer.Deserialize<BudgetTransaction>(json.GetRawText());
				if(transaction.BudgetId == null || transaction.CFTypeId == null)
				{
					transaction = JsonConvert.DeserializeObject<BudgetTransaction>(json.GetRawText());
				}
				transaction.Budget = null;
				transaction.Save(userId);
				return Ok(transaction);
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e);
				return Ok();
			}
		}
		/// <summary>
		/// Returns a VM that contains BudgetTransactionComparison, a list of the reportedTransactions and the budget item in question
		/// </summary>
		/// <param name="email">Email Address of the user</param>
		/// <param name="collectionsId">Id of the collection that the user is interacting with</param>
		/// <returns>A safe to spend object that contains all of the elements that are needed</returns>
		private ActionResult SafeToSpend(string email, string collectionsId)
		{
			try
			{
				SafeToSpendVM vm = new SafeToSpendVM(collectionsId, email);
				return Ok(vm);
			}
			catch (Exception e)
			{																																		   
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e.Message);
				return Ok();
			}

		}
		private ActionResult EditBudget(JsonElement json)
		{
			Collections collection = System.Text.Json.JsonSerializer.Deserialize<Collections>(json.GetRawText());

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
		/// <param name="uid">The users Id as defined by Google</param>
		/// <returns>The required data to populate the FE left Menu</returns>
		private ActionResult GetCollectionsMenu(string uid)
		{
			UserCollectionMapping mapping = new UserCollectionMapping();
			if (mapping.Check(uid))
			{
				IncludeYodlee include = new IncludeYodlee();
				List<MenuData> menu = new List<MenuData>();
				List<MenuData> subMenu = new List<MenuData>();
				List<MenuData> resources = new List<MenuData>();
				List<MenuData> Calculators = new List<MenuData>();
				List<MenuData> Simulations = new List<MenuData>();
				menu.Add(new MenuData()
				{
					Category = true,
					Title = "Dashboards"
				});
				Simulations.Add(new MenuData()
				{
					Title = "Homepage",
					Key = "Homepage",
					Url = "/simulations/homepage"
				});
				Simulations.Add(new MenuData()
				{
					Title = "New Simulation",
					Key = "New Simulation",
					Url = "/simulations/NewSimulation"
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
				subMenu.Add(new MenuData()
				{
					Title = "New Collection",
					Key = "NewCollection",
					Url = "/dashboard/NewCollection"
				});
				subMenu.Add(new MenuData()
				{
					Title = "Manual Transaction",
					Key = "ManualTransactions",
					Url = "/dashboard/ManualTransactions"
				});
				subMenu.Add(new MenuData()
				{
					Title = "Add Account",
					Key = "AddAccounts",
					Url = "/dashboard/AddAccount"
				});
				if (include.isIncluded(uid))
				{
					subMenu.Add(new MenuData()
					{
						Title = "Automated Account",
						Key = "AutomatedLink",
						Url = "/dashboard/AutomatedLink"
					});
				}
				menu.Add(new MenuData()
				{
					Title = "Budget Dashboard",
					Key = "Dashboards",
					Icon = "fe fe-home",
					Children = subMenu
				});
				menu.Add(new MenuData()
				{
					Title = "Simulation Dashboard",
					Key = "Simulations",
					Icon = "fe fe-cpu",
					Children = Simulations
				});
				menu.Add(new MenuData()
				{
					Title = "Resources Dashboard",
					Key = "Resources",
					Icon = "fe fe-book-open",
					Children = resources
				});
				resources.Add(new MenuData()
				{
					Title = "Calculators",
					Key = "Homepage",
					Children = Calculators
				});
				Calculators.Add(new MenuData()
				{
					Title = "Bond Calculator",
					Key = "BondCalculators",
					Url = "/dashboard/Bond"
				});
				Calculators.Add(new MenuData()
				{
					Title = "Loan Calculator",
					Key = "LoanCalculators",
					Url = "/dashboard/Loan"
				});
				Calculators.Add(new MenuData()
				{
					Title = "Savings Calculator",
					Key = "SavingsCalculators",
					Url = null
				});
				return Ok(menu);
			}
			else
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
					Title = "New Collection",
					Key = "NewCollection",
					Url = "/dashboard/NewCollection"
				});
				menu.Add(new MenuData()
				{
					Title = "Budget Dashboard",
					Key = "Dashboards",
					Icon = "fe fe-home",
					Children = subMenu
				});
				return Ok(menu);
			}
		}
		/// <summary>
		/// Creates or Updates Account
		/// </summary>
		/// <param name="json">Json Element Passed by FE</param>
		/// <returns>Updated Account Object</returns>
		public async Task<ActionResult> AccountChange(JsonElement json)
		{
			try
			{
				Account account = System.Text.Json.JsonSerializer.Deserialize<AccountObj>(json.GetRawText()).Account;
				Account account1 = JsonConvert.DeserializeObject<AccountObj>(json.GetRawText()).Account;
				if(account1 == null)
				{
					account1 = JsonConvert.DeserializeObject<Account>(json.GetRawText());
				}
				string id = "";
				if (account != null)
				{
					account.AddAccount();
					id = account.CollectionsId;
				}
				else
				{
					account1.AddAccount();
					id = account1.CollectionsId;
				}
				Collections collections = new Collections(id);
				Account acc = new Account();
				collections.Accounts = acc.GetAccounts(id);
				return Ok(await acc.UpdateAccounts(id, collections.Accounts.ToList()));
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e);
				return Ok();
			}
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
			return new DateTime(year, month, day);
		}
		/// <summary>
		/// Get for the reported transactions for a specified account
		/// </summary>
		/// <param name="accountId">Id for the account that is being requested</param>
		/// <param name="startDate">The start date for which to request the transactions</param>
		/// <param name="endDate">The end date for which to request the transactions</param>
		/// <returns>A List of the Reported Transactions that is associated on that account between the specified dates</returns>
		private ActionResult GetReportedTransactions(string accountId, DateTime startDate, DateTime endDate)
		{
			ReportedTransaction transaction = new ReportedTransaction();
			return Ok(transaction.GetTransactions(accountId, startDate, endDate));
		}
		/// <summary>
		/// Returns a List of accounts in the collection VM associated with a collection and a user email
		/// </summary>
		/// <param name="collectionsId">Id of the collection that was requested</param>
		/// <param name="email">email address of the user</param>
		/// <returns>Reutrns a collectiuon VM for the user</returns>
		private ActionResult GetAccounts(string collectionsId, string email)
		{
			CollectionVM VM = new CollectionVM(collectionsId, email);
			return Ok(VM);
		}
		private ActionResult UpdateSimulation(string json)
		{
			Simulation simulation = System.Text.Json.JsonSerializer.Deserialize<Simulation>(json);
			simulation.Edit();
			return Ok(simulation);
		}
		private ActionResult BuildSimulation(string json, string collectionsId, string userId)
		{
			try
			{
				FirebaseUser user = new FirebaseUser();
				userId = user.GetUserId(userId);
				SimulationAssumptionsModel assumptions = JsonConvert.DeserializeObject<SimulationAssumptionsModel>(json);
				foreach(BudgetTransaction item in assumptions.BudgetTransactions)
				{
					item.Save(userId);
				}
				Simulation simulation = new Simulation(assumptions.SimulationAssumptions, collectionsId);
				simulation.BuildSimulation(assumptions.SimulationAssumptions, userId);
				simulation.Scenario();
				return Ok(simulation);
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e);
				return Ok();
			}
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
				if (result != null)
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
		private ActionResult UnseenCount(string uid)
		{
			return Ok(new UserProfileModel(uid));
		}
	}
	class Newcftype
	{
		public string NewCFType { get; set; }
	}
	public class SelectionModel
	{
		public bool Selection { get; set; }
	}
	public class AccountObj
	{
		public Account Account { get; set; }
	}
	public class tempScheduledTransaction
	{
		public ScheduledTransactions t { get; set; }
	}
}
