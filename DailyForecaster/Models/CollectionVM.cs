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
		public List<Institution> Institutions { get; set; }
		public List<AccountType> AccountTypes { get; set; }
		public IncludeYodlee IncludeYodlee { get; set; }
		public PredictionModel PredictionModel { get; set; }
		public string Name { get; set; }
		public string Id { get; set; }
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
				collection = collection.GetCollections(collectionId);
				Accounts = account.GetAccounts(collectionsId, false, email);
				Id = collection.CollectionsId;
				Name = collection.Name;
				IncludeYodlee = new IncludeYodlee(collection.CollectionsId);
				PredictionModel = new PredictionModel(collection.CollectionsId);
			}
			else
			{
				if (email != "")
				{
					Accounts = account.GetAccounts(collectionsId, false, email);
					Collections collection = new Collections();
					UserInteraction userInteraction = new UserInteraction();
					userInteraction.CollectionsIncratment(collectionsId, email);
					collection = collection.GetCollections(collectionsId);
					Id = collection.CollectionsId;
					Name = collection.Name;
					IncludeYodlee = new IncludeYodlee(collection.CollectionsId);
					PredictionModel = new PredictionModel(collection.CollectionsId);
				}
			}
			Institution institution = new Institution();
			Institutions = institution.GetInstitutions();
			AccountTypes = AccountType.GetAccountTypes();
		}
	}
}
