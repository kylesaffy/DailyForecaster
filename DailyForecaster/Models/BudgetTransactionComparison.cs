using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
			ReportedTransactions = CleanNonTransactional(reportedTransaction.GetTransactions(BudgetItem));
			List<ReportedTransaction> reportedTransactions = CleanNonTransactional(reportedTransaction.GetTransactions(BudgetItem));
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
		public List<ReportedTransaction> CleanNonTransactional(List<ReportedTransaction> reportedTransactions)
		{
			List<Account> accounts = reportedTransactions.Select(x => x.Account).Distinct().ToList();
			List<ReportedTransaction> copy = new List<ReportedTransaction>(reportedTransactions.ToArray());
			CFType type = new CFType();
			List<CFType> types = new List<CFType>();
			types.Add(new CFType("Loans"));
			types.Add(new CFType("Car Loan"));
			//4 phases
			//Convert interst on transactional account to bank charges
			CFType tempType = reportedTransactions.Select(x => x.CFType).Where(x => x.Name == "Bank Charges").FirstOrDefault();
			foreach (ReportedTransaction item in copy.Where(x=>x.Account.AccountType.Transactional == true && x.CFType.Name == "Interest"))
			{
				item.CFType = tempType;
			}
			//change bank charges and interest to the same CF Classification
			CFClassification tempClassification = reportedTransactions.Select(x => x.CFClassification).Where(x => x.Sign == -1).FirstOrDefault();
			foreach (ReportedTransaction item in copy.Where(x => x.Account.AccountType.Transactional == true && x.CFType.Name == "Bank Charges" && x.CFClassification.Sign == 1))
			{
				item.CFClassification = tempClassification;
				item.Amount = item.Amount * -1;
			}
			// convert transfer to non tranasactional items to payments
			foreach (ReportedTransaction item in copy.Where(x=>x.Account.AccountType.Transactional && x.CFType.Id == "999"))
			{
				if(reportedTransactions.Where(x=>x.Account.AccountType.Transactional == false && x.CFType.Id == "999" && x.Amount == item.Amount).Any())
				{
					ReportedTransaction transaction = reportedTransactions.Where(x => x.Account.AccountType.Transactional == false && x.CFType.Id == "999" && x.Amount == item.Amount).FirstOrDefault();
					switch (transaction.Account.AccountType.Name)
					{
						case "Personal Loan":
							item.CFType = types.Where(x => x.Name == "Loans").FirstOrDefault();
							break;
						case "Car Loan":
							item.CFType = types.Where(x => x.Name == "Car Loan").FirstOrDefault();
							break;
					}
				}
			}
			// strip out the non tranasactional accounts 
			foreach (Account item in accounts.Where(x => x.AccountType.Transactional == false))
			{
				foreach (ReportedTransaction t in reportedTransactions.Where(x => x.AccountId == item.Id))
				{
					copy.Remove(t);
				}
			}
			foreach(ReportedTransaction item in copy)
			{
				item.Account = null;
			}
			return copy;
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
