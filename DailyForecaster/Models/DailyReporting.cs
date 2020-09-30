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
			//AutomatedCashFlow automated = new AutomatedCashFlow();
			DateTime dateTime = DateTime.Now;
			List<AutomatedCashFlow> flows = new List<AutomatedCashFlow>();
			//Budget budget = new Budget();
			List<BudgetTransactionComparison> comparisons = new List<BudgetTransactionComparison>();
			foreach (string item in collectionIds)
			{
				BudgetTransactionComparison comparison = new BudgetTransactionComparison(item);
				comparisons.Add(comparison);
				flows.AddRange(comparison.ReportedTransactions.Where(x => x.DateCaptured == dateTime).Select(x => x.AutomatedCashFlow));
			}

			//List<Budget> budgets = budget.
		}
	}
}
