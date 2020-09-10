using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class CollectionVM
	{
		public List<Collections> Collections { get; set; }
		public List<Account> Accounts { get; set; }
		public string Name { get; set; }
		public CollectionVM () { }
		/// <summary>
		/// Collections VM generation
		/// </summary>
		/// <param name="collectionsId">Collection Id, can be, null, "", "undefined"</param>
		/// <param name="email">Email address of the user</param>
		public CollectionVM(string collectionsId,string email)
		{
			Account account = new Account();
			Collections collections = new Collections();
			Collections = collections.GetCollections(email,"CollectionsVM");
			if ((collectionsId == null || collectionsId == "" || collectionsId == "undefined") && email != "")
			{
				UserInteraction userInteraction = new UserInteraction();
				Collections collection = new Collections();
				string collectionId = userInteraction.GetCollectionId(email);
				Name = collection.GetCollections(collectionId).Name;
				Accounts = account.GetAccounts(collectionsId, false, email);
			}
			else
			{
				if (email != "")
				{
					Accounts = account.GetAccounts(collectionsId, false, email);
					Collections collection = new Collections();
					UserInteraction userInteraction = new UserInteraction();
					userInteraction.CollectionsIncratment(collectionsId, email);
					Name = collection.GetCollections(collectionsId).Name;
				}
			}
		}
	}
}
