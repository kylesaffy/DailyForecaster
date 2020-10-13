using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ManualCashFlowsVM
	{
		public List<Collections> Collections { get; set; }
		public List<CFType> CFTypes { get; set; }
		public List<CFClassification> CFClassifications {get;set;}
		public List<Account> Accounts { get; set; }
		public ManualCashFlowsVM(string collectionsId, string email)
		{
			Collections collections = new Collections();
			UserInteraction userInteraction = new UserInteraction();
			if ((collectionsId == null || collectionsId == "" || collectionsId == "undefined") && email != "")
			{
				collectionsId = userInteraction.GetCollectionId(email);
			}
			else
			{
				userInteraction.CollectionsIncratment(collectionsId, email);
			}
			Collections = collections.GetCollections(email, "ManualCashFlowsVM");
			Account account = new Account();
			Accounts = account.GetAccounts(collectionsId);
			CFType type = new CFType();
			CFTypes = type.GetCFList(collectionsId);
			CFClassification classification = new CFClassification();
			CFClassifications = classification.GetList();
		}
	}
}
