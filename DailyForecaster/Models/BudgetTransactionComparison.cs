using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class BudgetTransactionComparison
	{
		public virtual Budget BudgetItem { get; set; }
		public virtual List<ReportedTransaction> ReportedTransactions {get;set;}
		public List<TransactionComparison> TransactionComparisons { get; set; }
		public BudgetTransactionComparison() { }
		public BudgetTransactionComparison(string collectionsId)
		{
			Budget budget = new Budget();
			BudgetItem = budget.GetBudget(collectionsId);
			ReportedTransaction reportedTransaction = new ReportedTransaction();
			ReportedTransactions = reportedTransaction.GetTransactions(BudgetItem);
			List<ReportedTransaction> reportedTransactions = reportedTransaction.GetTransactions(BudgetItem);
			List<BudgetTransaction> budgetTransactions = BudgetItem.BudgetTransactions.ToList();
			List<TransactionComparison> transactionList = new List<TransactionComparison>();
			foreach(BudgetTransaction item in BudgetItem.BudgetTransactions)
			{
				item.CFType = new CFType(item.CFTypeId);
				item.CFClassification = new CFClassification(item.CFClassificationId);
				double spent = ReportedTransactions
					.Where(x => x.CFType.Id == item.CFTypeId && x.CFClassification.Id == item.CFClassificationId)
					.Select(x=>x.Amount)
					.Sum();
				ReportedTransaction transaction = reportedTransactions.Where(x => x.CFType.Id == item.CFTypeId && x.CFClassification.Id == item.CFClassificationId).FirstOrDefault();
				reportedTransactions.Remove(transaction);
				double budgeted = budgetTransactions
					.Where(x => x.CFClassificationId == item.CFClassificationId && x.CFTypeId == item.CFTypeId)
					.Select(x=>x.Amount)
					.Sum();
				transactionList
					.Add(new TransactionComparison(spent, budgeted, item));
			}
			foreach(ReportedTransaction item in reportedTransactions)
			{
				item.CFType = item.CFType;
				item.CFClassification = item.CFClassification;
				double spent = ReportedTransactions
					.Where(x => x.CFType.Id == item.CFType.Id && x.CFClassification.Id == item.CFClassification.Id)
					.Select(x => x.Amount)
					.Sum();
				transactionList
					.Add(new TransactionComparison(spent, 0, item));
			}
			TransactionComparisons = transactionList
				.GroupBy(p => new { p.CFClassificationId, p.CFTypeId })
				.Select(g => g.First())
				.ToList();
		}
	}
	public class TransactionComparison
	{
		public virtual CFClassification CFClassification { get; set; }
		public string CFClassificationId { get; set; }
		public virtual CFType CFType { get; set; }
		public string CFTypeId { get; set; }
		public double Budgeted { get; set; }
		public double Spent { get; set; }
		public double Remaining { get; set; }
		public TransactionComparison() { }
		public TransactionComparison(double spent, double budgeted, BudgetTransaction transaction)
		{
			CFClassification = new CFClassification(transaction.CFClassificationId);
			CFType = new CFType(transaction.CFTypeId);
			Budgeted = budgeted;
			Spent = spent;
			Remaining = budgeted - spent;
			CFClassificationId = transaction.CFClassificationId;
			CFTypeId = transaction.CFTypeId;
		}
		public TransactionComparison(double spent, double budgeted, ReportedTransaction transaction)
		{
			CFClassification = transaction.CFClassification;
			CFType = transaction.CFType;
			Budgeted = budgeted;
			Spent = spent;
			Remaining = budgeted - spent;
			CFClassificationId = transaction.CFClassification.Id;
			CFTypeId = transaction.CFType.Id;
		}
	}
}
