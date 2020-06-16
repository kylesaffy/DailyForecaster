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

namespace DailyForecaster.Controllers
{
    [Route("[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class DailyPlannerRoutingController : ControllerBase
    {
        [Route("BudgetCheck")]
        [HttpGet]
        public ActionResult BudgetCheck(string collectionId)
        {
            Budget budget = new Budget();
            return Ok(budget.BudgetCheck(collectionId));

        }
        [Route("GetCollections")]
        [HttpGet]
        public ActionResult GetCollections(string userId, string type)
        {
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
        [Route("GetCollection")]
        [HttpGet]
        public ActionResult GetCollection(string collectionsId)
        {
            Collections collections = new Collections(collectionsId);
            foreach(Budget item in collections.Budgets)
			{
                item.Collection = null;
			}
            return Ok(collections);
        }
        [Route("GetAccounts")]
        [HttpGet]
        public ActionResult GetAccounts(string collectionsId)
		{
            Account account = new Account();
            return Ok(account.GetAccounts(collectionsId));
		}
        [Route("GetAccountType")]
        [HttpGet]
        public ActionResult GetAccountType()
        {
            AccountType accountType = new AccountType();
            return Ok(accountType.GetAccountTypes());
        }
        [Route("AssigningSharedCollection")]
        [HttpPost]
        public ActionResult AssigningSharedCollection([FromBody] JsonElement json)
        {
            NewCollectionsObj obj = JsonConvert.DeserializeObject<NewCollectionsObj>(json.GetRawText());
            CollectionSharing sharing = new CollectionSharing();
            return Ok(sharing.AddUserToCollection(obj));
        }
        [Route("SetSharedCollection")]
        [HttpPost]
        public ActionResult SetSharedCollection(string collectionId)
        {
            CollectionSharing collectionSharing = new CollectionSharing(collectionId);
            return Ok(new ReturnModel() { result = true, returnStr = collectionSharing.CollectionSharingId });
        }
        [Route("CollectionsCount")]
        [HttpGet]
        public ActionResult CollectionsCount(string userId)
        {
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
        [Route("GetCFType")]
        [HttpGet]
        public ActionResult GetCFType()
        {
            CFType cf = new CFType();
            return Ok(cf.GetCFList());
        }
        [Route("GetClassification")]
        [HttpGet]
        public ActionResult GetClassification()
        {
            CFClassification cf = new CFClassification();
            return Ok(cf.GetList());
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
            NewBudgetObj obj = JsonConvert.DeserializeObject<NewBudgetObj>(json.GetRawText());
            Budget budget = new Budget();
            bool ans = budget.Create(obj);
            return Ok(ans);
        }
        [Route("BudgetEdit")]
        [HttpPost]
        //[ResponseType(typeof(ManualCashFlow))]
        public ActionResult BudgetEdit([FromBody] JsonElement json)
        {
            NewBudgetObj obj = JsonConvert.DeserializeObject<NewBudgetObj>(json.GetRawText());
            Budget budget = new Budget();
            bool ans = budget.Edit(obj);
            return Ok(ans);
        }
        [Route("NewCollection")]
        [HttpPost]
        //[ResponseType(typeof(ManualCashFlow))]
        public ActionResult NewCollection([FromBody] JsonElement json)
        {
            NewCollectionsObj obj = JsonConvert.DeserializeObject<NewCollectionsObj>(json.GetRawText());
            Collections collections = new Collections();
            return Ok(collections.CreateCollection(obj));
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
            Budget budget = new Budget();
            budget = budget.GetBudget(collectionsId);
            return Ok(budget);
		}
        [Route("getInstitutions")]
        [HttpGet]
        public ActionResult getInstitutions()
		{
            Institution institution = new Institution();
            return Ok(institution.GetInstitutions());
		}
        [Route("AddAccount")]
        [HttpPost]
        public ActionResult AddAccount([FromBody] JsonElement json)
		{
            Account account = JsonConvert.DeserializeObject<Account>(json.GetRawText());
            ReturnModel returnModel = account.AddAccount(account);
            return Ok(returnModel);
		}
        //[Route("BudgetEdit")]
        //[HttpGet]
        //public ActionResult BudgetEdit(string collectionsId, DateTime date)
        //{
        //    return Ok();
        //}
    }
	public class ReturnModel {
        public bool result { get; set; }
        public string returnStr { get; set; }
    }

}