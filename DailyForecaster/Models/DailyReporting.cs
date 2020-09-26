using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class DailyReporting
	{
		public DateTime LastInteraction { get; set; }
		public string FirstName { get; set; }
		public string Email { get; set; }
		public DailyReporting() { }
		public DailyReporting(string userId)
		{
			ClickTracker tracker = new ClickTracker();
			LastInteraction = tracker.GetLatest(userId);
			FirebaseUser user = new FirebaseUser(userId);
			FirstName = user.FirstName;
			Email = user.Email;
			UserCollectionMapping mapping = new UserCollectionMapping();
			List<string> collectionIds = mapping.getCollectionIds(userId, "firebase");
			//Collections collection = new Collections();
			//List<Collections> collections = collection.GetEagerList(collectionIds);
			AutomatedCashFlow automated = new AutomatedCashFlow();
			DateTime dateTime = DateTime.Now;
			List<AutomatedCashFlow> flows = automated.Get(collectionIds, dateTime);
			Budget budget = new Budget();
			//List<Budget> budgets = budget.
		}
	}
}
