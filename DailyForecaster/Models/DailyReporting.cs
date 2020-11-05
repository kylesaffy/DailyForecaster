using DailyForecaster.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class DailyReporting
	{
		public DateTime LastInteraction { get; set; }
		public string FirstName { get; set; }
		public string Email { get; set; }
		public DailyMotivational DailyMotivational { get; set; }
		public DailyTip DailyTip { get; set; }
		public List<AutomatedCashFlow> AutomatedCashFlows { get; set; }
		public List<string> CollectionIds { get; set; }

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
			CollectionIds = collectionIds;
			DateTime dateTime = DateTime.Now;
			List<AutomatedCashFlow> flows = new List<AutomatedCashFlow>();
			List<BudgetTransactionComparison> comparisons = new List<BudgetTransactionComparison>();
			foreach (string item in collectionIds)
			{
				BudgetTransactionComparison comparison = new BudgetTransactionComparison(item);
				if (comparison.ReportedTransactions != null)
				{
					if (comparison.ReportedTransactions.Count > 0)
					{
						if (comparison.ReportedTransactions.Select(x => x.AutomatedCashFlow).ToList()[0] != null)
						{
							comparisons.Add(comparison);
							flows.AddRange(comparison.ReportedTransactions.Where(x => x.AutomatedCashFlow.DateCaptured.Date == dateTime.Date).Select(x => x.AutomatedCashFlow));
						}
					}
				}
			}
			AutomatedCashFlows = flows;
			DailyMotivational motivational = new DailyMotivational();
			DailyMotivational = motivational.Get();
			DailyTip tip = new DailyTip();
			DailyTip = tip.Get();
		}
	}
}
