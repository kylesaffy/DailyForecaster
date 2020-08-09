using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DailyForecaster.Models;
using System.Web.Http.Description;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Primitives;
using System.Net.Http;

namespace DailyForecaster.Controllers
{
    [Route("[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class DailyPlannerRoutingController : ControllerBase
    {
        private static string googleAPI = "https://www.google.com/recaptcha/api/siteverify";
        private static string reCAPTCHAsecret = "6LeYfa4ZAAAAAOnPNDisNyX18flb1MSZvs8xyoTL";
        [Route("AccountDetails")]
        [HttpGet]
        public ActionResult AccountDetails(string id)
		{
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("AccountDetails", true, false, "AccountId " + id, auth.Identity.Name);
                Account account = new Account();
                return Ok(account.GetAccount(id,true));
            }
            return Ok("");
        }
        [Route("BasicEmail")]
        [HttpPost]
        public ActionResult BasicEmail([FromBody] JsonElement json, string Name)
		{
            EmailFunction email = JsonConvert.DeserializeObject<EmailFunction>(json.GetRawText());
            return Ok(email.SendEmail(Name));
		}
        [Route("SafeToSpend")]
        [HttpGet]
        public ActionResult SafeToSpend(string collectionsId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("SafeToSpend", true, false, "collectionsId " + collectionsId, auth.Identity.Name);
                return Ok(new BudgetTransactionComparison(collectionsId));
            }
            return Ok("");
        }
        [Route("GetTransactions")]
        [HttpGet]
        public ActionResult GetTransactions(string id, DateTime startDate, DateTime endDate)
		{
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                GetTransactionsObj obj = new GetTransactionsObj()
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Id = id
                };
                ReportedTransaction transaction = new ReportedTransaction();
                List<ReportedTransaction> transactions = transaction.GetTransactions(obj.Id);
                transactions = transactions.Where(x => x.DateCaptured <= obj.EndDate && x.DateCaptured >= obj.StartDate).Where(x=>x.CFType.Id != "999").OrderByDescending(x=>x.DateBooked).ToList();
                return Ok(transactions);
            }
            return Ok("");
        }
        [Route("GetIndex")]
        [HttpGet]
        public ActionResult GetIndex(string userId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                AspNetUsers users = new AspNetUsers();
                new ClickTracker("GetIndex", true, false, "userId " + users.getUserId(userId), auth.Identity.Name);
                Collections collection = new Collections();
                List<Collections> collections = collection.GetCollections(userId, "TransactionList");
                List<ReportedTransaction> flows = new List<ReportedTransaction>();
                ReportedTransaction reportedTransaction = new ReportedTransaction();
                foreach(Collections item in collections)
				{
                    foreach(Account acc in item.Accounts)
					{
                        flows.AddRange(reportedTransaction.GetTransactions(acc.Id));
					}
				}
                return Ok(flows);
            }
            return Ok("");
        }
        [Route("BudgetCheck")]
        [HttpGet]
        public ActionResult BudgetCheck(string collectionId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("BudgetCheck", true, false, "collectionId " + collectionId, auth.Identity.Name);
                Budget budget = new Budget();
                return Ok(budget.BudgetCheck(collectionId));
            }
            return Ok("");
        }
        [Route("GetCollections")]
        [HttpGet]
        public ActionResult GetCollections(string type)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                AspNetUsers users = new AspNetUsers();
                new ClickTracker("GetCollections", true, false, "userId " + users.getUserId(auth.Identity.Name) + " type " + type, auth.Identity.Name);
                if (type == "Index")
                {
                    Collections collection = new Collections();
                    List<Collections> list = collection.GetCollections(auth.Identity.Name, type);
                    return Ok(list);
                }
                else if (type == "Transactions")
                {
                    return Ok();
                }
                return Ok();
            }
            return Ok();
        }
        [Route("GetCollection")]
        [HttpGet]
        public ActionResult GetCollection(string collectionsId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("GetCollection", true, false, "collectionsId " + collectionsId, auth.Identity.Name);
                Collections collections = new Collections(collectionsId);
                foreach (Budget item in collections.Budgets)
                {
                    item.Collection = null;
                }
                return Ok(collections);
            }
            return Ok("");
        }
        [Route("GetAccounts")]
        [HttpGet]
        public ActionResult GetAccounts(string collectionsId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("GetAccounts", true, false, "collectionsId " + collectionsId, auth.Identity.Name);
                Account account = new Account();
                return Ok(account.GetAccounts(collectionsId));
            }
            return Ok("");
        }
        [Route("GetAccountType")]
        [HttpGet]
        public ActionResult GetAccountType()
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("GetAccountType", true, false, "", auth.Identity.Name);
                AccountType accountType = new AccountType();
                return Ok(accountType.GetAccountTypes());
            }
            return Ok("");
        }
        [Route("AssigningSharedCollection")]
        [HttpPost]
        public ActionResult AssigningSharedCollection([FromBody] JsonElement json)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("AssigningSharedCollection", false, true, json.GetRawText(), auth.Identity.Name);
                NewCollectionsObj obj = JsonConvert.DeserializeObject<NewCollectionsObj>(json.GetRawText());
                CollectionSharing sharing = new CollectionSharing();
                return Ok(sharing.AddUserToCollection(obj));
            }
            return Ok("");
        }
        [Route("AddTransfer")]
        [HttpPost]
        public ActionResult AddTransfer([FromBody] JsonElement json)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("AddTransfer", false, true, json.GetRawText(), auth.Identity.Name);
                TransferObject obj = JsonConvert.DeserializeObject<TransferObject>(json.GetRawText());
                ManualCashFlow manualCashFlow = new ManualCashFlow();
                return Ok(manualCashFlow.AddTransfer(obj));
            }
            return Ok("");
        }
        [Route("SaveManualCashFlow")]
        [HttpPost]
        public ActionResult SaveManualCashFlow([FromBody] JsonElement json)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("SaveManualCashFlow", false, true, json.GetRawText(), auth.Identity.Name);
                ManualCashFlow obj = JsonConvert.DeserializeObject<ManualCashFlow>(json.GetRawText());
                return Ok(obj.Save());
            }
            return Ok("");
        }
        [Route("SetSharedCollection")]
        [HttpPost]
        public ActionResult SetSharedCollection(string collectionId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
                if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("SetSharedCollection", false, true,"collectionId " + collectionId, auth.Identity.Name);
                CollectionSharing collectionSharing = new CollectionSharing(collectionId);
                return Ok(new ReturnModel() { result = true, returnStr = collectionSharing.CollectionSharingId });
            }
            return Ok("");
        }
        [Route("CollectionsCount")]
        [HttpGet]
        public ActionResult CollectionsCount(string userId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                AspNetUsers users = new AspNetUsers();
                new ClickTracker("CollectionsCount", true, false, "userId " + users.getUserId(userId), auth.Identity.Name);
                UserCollectionMapping mapping = new UserCollectionMapping();
                if (mapping.CollectionsCounter(userId) > 0)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            return Ok("");
        }
        [Route("GetCFType")]
        [HttpGet]
        public ActionResult GetCFType(string collectionsId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("GetCFType", true, false, "collectionsId " + collectionsId, auth.Identity.Name);
                CFType cf = new CFType();
                return Ok(cf.GetCFList(collectionsId).Where(x=>x.Id != "39a1d903-f4e3-4e4a-986a-604bd8dff20e"));
            }
            return Ok("");
        }
        [Route("GetClassification")]
        [HttpGet]
        public ActionResult GetClassification()
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("GetClassification", true, false, "", auth.Identity.Name);
                CFClassification cf = new CFClassification();
                return Ok(cf.GetList());
            }
            return Ok("");
        }
        [Route("Create")]
        [HttpPost]
        //[ResponseType(typeof(ManualCashFlow))]
        public async Task<ActionResult> Create()
        {
            //YodleeModel model = new YodleeModel();
            //ReturnModel returnModel = await model.Register();
            //return Ok(returnModel);
            return Ok("Sucess");
        }
        [Route("getYodleeToken")]
        [HttpGet]
        public async Task<ActionResult> getYodleeToken(string collectionsId)
		{
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("GetYodleeToken", true, false, collectionsId, auth.Identity.Name);
                YodleeModel yodleeModel = new YodleeModel();
                string token = await yodleeModel.getToken(collectionsId, auth.Identity.Name);
                return Ok(token);
            }
            return Ok("");
        }
        [Route("BudgetNew")]
        [HttpPost]
        //[ResponseType(typeof(ManualCashFlow))]
        public ActionResult BudgetNew([FromBody] JsonElement json)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("BudgetNew/Update", false, true, json.GetRawText(), auth.Identity.Name);
                string str = json.GetRawText();
                NewBudgetObj obj = JsonConvert.DeserializeObject<NewBudgetObj>(str);
                Budget budget = new Budget();
                bool ans = budget.Create(obj);
                return Ok(ans);
            }
            return Ok("");
        }
        [Route("BudgetEdit")]
        [HttpPost]
        //[ResponseType(typeof(ManualCashFlow))]
        public ActionResult BudgetEdit([FromBody] JsonElement json)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("BudgetEdit", false, true, json.GetRawText(), auth.Identity.Name);
                string str = json.GetRawText();
                NewBudgetObj obj = JsonConvert.DeserializeObject<NewBudgetObj>(str);
                Budget budget = new Budget();
                bool ans = budget.Edit(obj);
                return Ok(ans);
            }
            return Ok("");
        }
        [Route("NewCollection")]
        [HttpPost]
        //[ResponseType(typeof(ManualCashFlow))]
        public ActionResult NewCollection([FromBody] JsonElement json)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("NewCollection", false, true, json.GetRawText(), auth.Identity.Name);
                NewCollectionsObj obj = JsonConvert.DeserializeObject<NewCollectionsObj>(json.GetRawText());
                Collections collections = new Collections();
                return Ok(collections.CreateCollection(obj));
            }
            return Ok("");
        }
        //      [Route("BudgetCount")]
        //      [HttpGet]
        //      public ActionResult BudgetCount(string collectionsId,DateTime? startDate)
        //{
        //          Budget budget = new Budget();
        //          return Ok(budget.BudgetCount(collectionsId));
        //}
        [Route("BudgetEdit")]
        [HttpGet]
        public ActionResult BudgetEdit(string collectionsId)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("BudgetEdit", true, false, "collectionsId " + collectionsId, auth.Identity.Name);
                Collections collections = new Collections(collectionsId);
                return Ok(collections);
            }
            return Ok("");
        }
        [Route("getInstitutions")]
        [HttpGet]
        public ActionResult getInstitutions()
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("getInstitutions", true, false, "", auth.Identity.Name);
                Institution institution = new Institution();
                return Ok(institution.GetInstitutions());
            }
            return Ok("");
        }
        [Route("AddAccount")]
        [HttpPost]
        public ActionResult AddAccount([FromBody] JsonElement json)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("AddAccount", false, true, json.GetRawText(), auth.Identity.Name);
                Account account = JsonConvert.DeserializeObject<Account>(json.GetRawText());
                ReturnModel returnModel = account.AddAccount(account);
                return Ok(returnModel);
            }
            return Ok("");
            //[Route("BudgetEdit")]
            //[HttpGet]
            //public ActionResult BudgetEdit(string collectionsId, DateTime date)
            //{
            //    return Ok();
            //}
        }
        [Route("BotCheck")]
        [HttpPost]
        public async Task<ActionResult> BotCheck(string token)
		{
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(googleAPI + "?secret=" + reCAPTCHAsecret + "&response=" + token);
            return Ok(await response.Content.ReadAsStringAsync());
		}
        [Route("UnseenUpdate")]
        [HttpPost]
        public ActionResult UnseenUpdate(string manualcashflow, [FromBody] JsonElement json)
		{
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("UnseenUpdate", false, true, json.GetRawText(), auth.Identity.Name);
                AutomatedCashFlow automatedCashFlow = JsonConvert.DeserializeObject<AutomatedCashFlow>(json.GetRawText());
				ReturnModel returnModel = automatedCashFlow.UpdateAutomated(manualcashflow);
                return Ok(returnModel);
            }
            return Ok("");
        }
        [Route("GetUser")]
        [HttpGet]
        public ActionResult GetUser()
		{
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                AspNetUsers users = new AspNetUsers();
                return Ok(users.getUserId(auth.Identity.Name));
            }
            return Ok("");
		}
        [Route("GetUnseen")]
        [HttpGet]
        public ActionResult GetUnseen()
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            ClaimsPrincipal auth = new ClaimsPrincipal();
            TokenModel tokenModel = new TokenModel();
            if (authHeader == "497633e2-8572-4711-8748-894ba8886b41")
            {
                authHeader = tokenModel.generateToken("kylesaffy@gmail.com");
            }
            auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                int count = 0;
                AspNetUsers users = new AspNetUsers();
                string id = users.getUserId(auth.Identity.Name);
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
            return Ok("");
        }
        [Route("GetUnseenTransactions")]
        [HttpGet]
        public ActionResult GetUnseenTransactions()
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                new ClickTracker("GetUnseenTransactions", true, false, "", auth.Identity.Name);
                UnseenModel unseen = new UnseenModel(auth.Identity.Name);
                return Ok(unseen);
            }
            return Ok("");
        }
        [Route("GetCFTypeUnseen")]
        [HttpGet]
        public ActionResult GetCFTypeUnseen()
		{
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                AspNetUsers users = new AspNetUsers();
                string id = users.getUserId(auth.Identity.Name);
                using (FinPlannerContext _context = new FinPlannerContext())
                {
                    List<string> collections = _context
                            .UserCollectionMapping
                            .Where(x => x.Id == id)
                            .Select(x => x.CollectionsId)
                            .ToList();
                    List<CFType> types = new List<CFType>();
                    foreach (string item in collections)
                    {
                        CFType type = new CFType();
                        types.AddRange(type.GetCFList(item));
                    }
                    types = types
                        .GroupBy(x => x.Id)
                        .Select(g => g.First())
                        .ToList();
                    return Ok(types);
                }
            }
            return Ok("");
        }
    }
	public class ReturnModel {
        public bool result { get; set; }
        public string returnStr { get; set; }
    }

}