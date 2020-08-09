using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class IndexModel
	{
		public bool isShared { get; set; }
		public double Avaialble { get; set; }
		public double Used { get; set; }
		public double Budgeted { get; set; }
		public string Name { get; set; }
		public string CollectionsId { get; set; }
		/// <summary>
		/// The landing page is made up of a series of object this returns a list of the collection ojects high level details
		/// </summary>
		/// <param name="email">Users email address</param>
		/// <returns>Returns a list of index model objetcts</returns>
		public List<IndexModel> GetModel(string email)
		{
			List<IndexModel> model = new List<IndexModel>();
			List<string> collectionIds = new List<string>();
			UserCollectionMapping mapping = new UserCollectionMapping();
			string userId = "";
			try
			{
				FirebaseUser user = new FirebaseUser();
				userId = user.GetFirebaseUser(email);
				collectionIds = mapping.getCollectionIds(userId, "firebase");
			}
			catch
			{
				AspNetUsers user = new AspNetUsers();
				userId = user.getUserId(email);
				collectionIds = mapping.getCollectionIds(userId, "asp");
			}
			Collections collection = new Collections();
			Account account = new Account();
			Budget budget = new Budget();
			List<Collections> collections = collection.GetEagerList(collectionIds);
			foreach(Collections item in collections)
			{
				model.Add(new IndexModel
				{
					Name = item.Name,
					CollectionsId = item.CollectionsId,
					Avaialble = account.GetAvaialable(item.CollectionsId),
					Budgeted = budget.GetBudgetedAmount(item.CollectionsId),
					Used = budget.GetSpentAmount(item.CollectionsId),
					isShared = mapping.IsShared(item.CollectionsId)
				});
			}
			return model;
		}
	}
}
