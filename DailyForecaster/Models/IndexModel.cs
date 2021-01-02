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
		public bool Account { get; set; }
		public bool breach { get; set; }
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
				userId = user.GetUserId(email);
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
				try
				{
					IndexModel temp = new IndexModel();
					temp.Name = item.Name;
					temp.CollectionsId = item.CollectionsId;
					temp.Avaialble = account.GetAvaialable(item.CollectionsId);
					temp.Budgeted = Convert.ToDouble(Math.Round(budget.GetBudgetedAmount(item.CollectionsId), 2).ToString("N2"));
					temp.Used = Convert.ToDouble(Math.Round(getSepent(item.CollectionsId), 2).ToString("N2"));
					if (temp.Budgeted > temp.Used) temp.breach = false;
					else temp.breach = true;
					temp.isShared = mapping.IsShared(item.CollectionsId);
					model.Add(temp);
				}
				catch (Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
				}
			}
			return model;
		}
		public double getSepent(string collectionsId)
		{
			Budget budget = new Budget(collectionsId);
			AutomatedCashFlow automated = new AutomatedCashFlow();
			ManualCashFlow manual = new ManualCashFlow();
			return automated.GetSpent(collectionsId,budget) + manual.GetSpent(collectionsId,budget);
		}
	}
}
