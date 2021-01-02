using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class UnseenModel
	{
		public List<AutomatedCashFlow> AutomatedCashFlows { get; set; }
		public List<ManualCashFlow> ManualCashFlows { get; set; }
		public List<CFClassification> CFClassifications { get; set; }
		public List<CFType> CFTypes { get; set; }
		public List<Account> Accounts { get; set; }
		public UnseenModel(string email)
		{
			FirebaseUser users = new FirebaseUser();
			string id = users.GetUserId(email);
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				List<string> collections = _context
					.UserCollectionMapping
					.Where(x => x.FirebaseUserId == id)
					.Select(x => x.CollectionsId)
					.ToList();
				List<string> accountsStr = _context
					.Account
					.Where(Acc => collections.Contains(Acc.CollectionsId))
					.Select(x => x.Id)
					.ToList();
				Account account = new Account();
				Accounts = new List<Account>();
				foreach (string item in collections)
				{
					Accounts
						.AddRange(account.GetAccounts(item));
				}
				AutomatedCashFlow automatedCash = new AutomatedCashFlow();
				ManualCashFlow manualCash = new ManualCashFlow();
				CFType type = new CFType();
				CFClassification classification = new CFClassification();
				AutomatedCashFlows = automatedCash
					.GetAutomatedCashFlowsUnseen(accountsStr);
				foreach(AutomatedCashFlow item in AutomatedCashFlows)
				{
					item.Account = Accounts.Where(x => x.Id == item.AccountId).FirstOrDefault();
				}
				ManualCashFlows = manualCash
					.GetManualCahFlowsUnseen(accountsStr)
					.Where(x=>x.DateCaptured > DateTime.Now.AddDays(-90))
					.ToList();
				CFTypes = type
					.GetCFList(collections)
					.GroupBy(x => x.Id)
					.Select(x => x.First())
					.ToList();
				CFClassifications = classification
					.GetList();
			}
		}
	}
}
