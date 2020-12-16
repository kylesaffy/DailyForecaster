using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class PredictionModel
	{
		public Budget Budget { get; set; }
		public List<CashFlowPredication> CashFlowPredications { get; set; }
		public PredictionModel() { }
		public PredictionModel(string collectionsId)
		{
			Budget = new Budget(collectionsId);
			List<ReportedTransaction> historical = GetHistoricalTransactions(Budget.CollectionId).Where(x => x.DateBooked < Budget.StartDate).ToList();
			BudgetTransaction budgetTransaction = new BudgetTransaction();
			Budget.BudgetTransactions = new List<BudgetTransaction>();
			Budget.BudgetTransactions = budgetTransaction.GetBudgetTransactions(Budget.BudgetId);
			CashFlowPredications = new List<CashFlowPredication>();
			CashFlowPredication predication = new CashFlowPredication();
			foreach (BudgetTransaction item in Budget.BudgetTransactions)
			{
				List<ReportedTransaction> transactions = historical.Where(x => x.Amount < item.Amount * 1.02 && x.Amount > item.Amount * 0.98).Where(x => x.CFTypeId == item.CFTypeId && x.CFClassificationId == item.CFClassificationId ).ToList();
				if (transactions.Count() > 0)
				{
					predication.GetPredication(transactions,Budget.StartDate);
					if (predication.TransactionDate > DateTime.Now.Date)
					{
						predication.CFType = new CFType(item.CFTypeId);
						predication.BudgetTransaction = item;
						CashFlowPredications.Add(predication);
					}
				}
				predication = new CashFlowPredication();
			}
		}
		public List<ReportedTransaction> GetHistoricalTransactions(string collectionsId)
		{
			ReportedTransaction transaction = new ReportedTransaction();
			Account account = new Account();
			List<Account> accounts = account.GetAccounts(collectionsId);
			List<ReportedTransaction> transactions = new List<ReportedTransaction>();
			foreach (Account item in accounts)
			{
				transactions.AddRange(transaction.GetTransactions(item.Id));
			}
			return transactions.ToList();
		}
	}
	public class CashFlowPredication
	{
		public DateTime TransactionDate { get; set; }
		public CFType CFType { get; set; }
		public CFClassification CFClassification { get; set; }
		public Account Account { get; set; }
		public BudgetTransaction BudgetTransaction { get; set; }
		public double Amount { get; set; }
		public CashFlowPredication GetPredication(List<ReportedTransaction> transactions,DateTime date)
		{
			DateTime latestDate = transactions.OrderByDescending(x => x.DateBooked).Select(x => x.DateBooked).FirstOrDefault();
			CFClassification classification = new CFClassification();
			List<CFClassification> classifications = classification.GetList();
			List<int> dates = transactions.Select(x => x.DateBooked).ToList().Select(x => x.Day).Distinct().ToList();
			int mode = dates.GroupBy(v => v)
						.OrderByDescending(g => g.Count())
						.First()
						.Key;
			try
			{
				int testDate = 0;
				testDate = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1).Day;
				if (mode > testDate)
				{
					switch (testDate)
					{
						case 30:
							mode = 30;
							break;
						case 28:
							mode = 28;
							break;
					}
				}
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e);
			}
			

			string accountId = transactions.OrderByDescending(x => x.DateBooked).Select(x => x.Account.Id).FirstOrDefault();
			Account = new Account();
			this.TransactionDate = new DateTime(date.Year, date.Month, mode).AddMonths(1);
			this.CFClassification = classifications.Where(x => x.Id == transactions.Select(x => x.CFClassification.Id).FirstOrDefault()).FirstOrDefault();
			this.Account = Account.GetAccount(accountId, false);
			this.Amount = transactions.OrderByDescending(x => x.DateBooked).Select(x => x.Amount).FirstOrDefault();

			return this;
		}

	}
}
