// using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class BudgetVM
	{
		public Collections Collection { get; set; }
		public List<Collections> Collections { get; set; }
		public List<CFType> CFTypes { get; set; }
		public List<CFClassification> CFClassifications { get; set; }
		public string BudgetId { get; set; }
		public BudgetVM() { }
		/// <summary>
		/// Budget VM generation
		/// </summary>
		/// <param name="collectionsId">Collection Id, can be, null, "", "undefined"</param>
		/// <param name="email">Email address of the user</param>
		public BudgetVM(string collectionsId, string email)
		{
			CFType type = new CFType();
			CFClassification classification = new CFClassification();
			CFClassifications = classification.GetList();
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
			Collection = new Collections(collectionsId);
			CFTypes = type.GetCFList(collectionsId);
			Budget budget = Collection.Budgets.OrderByDescending(x => x.StartDate).FirstOrDefault();
			BudgetId = budget.BudgetId;
			foreach (BudgetTransaction item in budget.BudgetTransactions)
			{
				item.CFClassification = CFClassifications.Where(x => x.Id == item.CFClassificationId).FirstOrDefault();
				item.CFType = CFTypes.Where(x => x.Id == item.CFTypeId).FirstOrDefault();
			}
			Collection.Budgets = new List<Budget>();
			Collection.Budgets.Add(budget);
			Collections = collections.GetCollections(email, "BudgetVM");

		}
	}
}
