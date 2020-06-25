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

namespace DailyForecaster.Controllers
{
    [Route("[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class DailyPlannerRoutingController : ControllerBase
    {
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
                List<ManualCashFlow> flows = new List<ManualCashFlow>();
                foreach (Collections item in collections)
                {
                    if (item.Accounts != null)
                    {
                        foreach (Account acc in item.Accounts)
                        {
                            flows.AddRange(acc.ManualCashFlows);
                        }
                    }
                }
                Budget budget = new Budget();
                DateTime currentDate = DateTime.Now;
                foreach (Collections item in collections)
                {
                    if (budget.BudgetCount(item.CollectionsId) > 0)
                    {
                        if (budget.DateCheck2(item.CollectionsId, currentDate))
                        {
                            budget.Duplicate(item);
                        }
                    }
                }
                foreach (ManualCashFlow item in flows)
                {
                    item.Account.ManualCashFlows = null;
                    item.Account.Collections.Accounts = null;
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
        public ActionResult GetCollections(string userId, string type)
        {
            string authHeader = this.HttpContext.Request.Headers["Authorization"];
            TokenModel tokenModel = new TokenModel();
            ClaimsPrincipal auth = tokenModel.GetPrincipal(authHeader);
            if (auth.Identity.IsAuthenticated)
            {
                AspNetUsers users = new AspNetUsers();
                new ClickTracker("GetCollections", true, false, "userId " + users.getUserId(userId) + " type " + type, auth.Identity.Name);
                if (type == "Budget")
                {
                    Collections collection = new Collections();
                    List<Collections> list = collection.GetCollections(userId, type);
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
                return Ok(cf.GetCFList(collectionsId));
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
        public ActionResult Create()
        {

            //Collections collections = new Collections();
            //string blobString = collections.Account.Institution.BlobString;
            //double total = numbers;
            return Ok(new { Name = "Success" });
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
                new ClickTracker("BudgetNew", false, true, json.GetRawText(), auth.Identity.Name);
                NewBudgetObj obj = JsonConvert.DeserializeObject<NewBudgetObj>(json.GetRawText());
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
                NewBudgetObj obj = JsonConvert.DeserializeObject<NewBudgetObj>(json.GetRawText());
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
                Budget budget = new Budget();
                Collections collections = new Collections(collectionsId);
                collections.Budgets = new List<Budget>();
                collections.Budgets.Add(budget.GetBudget(collectionsId));
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
    }
	public class ReturnModel {
        public bool result { get; set; }
        public string returnStr { get; set; }
    }

}