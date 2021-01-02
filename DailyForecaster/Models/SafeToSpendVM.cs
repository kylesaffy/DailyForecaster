using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class SafeToSpendVM
	{
		public List<Collections> Collections { get; set; }
		public BudgetTransactionComparison BudgetTransactionComparison { get; set; }
		public List<CFType> CFTypes { get; set; }
		public string Name { get; set; }
		public string CollectionsId { get; set; }
		public SafeToSpendVM() { }
		/// <summary>
		/// Safe to Spend VM generation
		/// </summary>
		/// <param name="collectionsId">Collection Id, can be, null, "", "undefined"</param>
		/// <param name="email">Email address of the user</param>
		public SafeToSpendVM(string collectionsId, string email)
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
			BudgetTransactionComparison = new BudgetTransactionComparison(collectionsId);
			Collections = collections.GetCollections(email, "SafeToSpendVM");
			CFType type = new CFType();
			CFTypes = type.GetCFList(collectionsId);
			Name = Collections.Where(x => x.CollectionsId == collectionsId).Select(x=>x.Name).FirstOrDefault();
			CollectionsId = collectionsId;
		}
	}
}
