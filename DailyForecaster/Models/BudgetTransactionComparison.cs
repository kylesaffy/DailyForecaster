using Google.Apis.Util;
using Microsoft.EntityFrameworkCore.Storage;
using Org.BouncyCastle.Bcpg.OpenPgp;
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
		public BudgetTransactionComparison Get(string collectionsId)
		{
			BudgetTransactionComparison comparison = new BudgetTransactionComparison();
			Budget budget = new Budget(collectionsId);
			BudgetTransaction budgetTransaction = new BudgetTransaction();
			budget.BudgetTransactions = budgetTransaction.GetBudgetTransactions(budget.BudgetId);
			comparison.BudgetItem = budget;
			ReportedTransaction reportedTransaction = new ReportedTransaction();
			comparison.ReportedTransactions = CleanNonTransactional(reportedTransaction.Get(budget));
			List<ReportedTransaction> reportedTransactions = new List<ReportedTransaction>(comparison.ReportedTransactions.ToArray());
			List<BudgetTransaction> budgetTransactions = comparison.BudgetItem.BudgetTransactions.ToList();
			List<TransactionComparison> transactionList = new List<TransactionComparison>();
			CFType type = new CFType();
			List<CFType> types = type.GetCFList(budget.CollectionId);
			CFClassification cf = new CFClassification();
			List<CFClassification> classifications = cf.GetList();
			TransactionGroups groups = new TransactionGroups();
			List<TransactionGroups> transactionGroups = groups.GetGroups(budgetTransactions, reportedTransactions,classifications,types);
			foreach(TransactionGroups item in transactionGroups)
			{
				double spent = reportedTransactions
					.Where(x => x.CFType.Id == item.CFType.Id && x.CFClassification.Id == item.CFClassification.Id)
					.Select(x => x.Amount)
					.Sum();
				List<ReportedTransaction> transactions = reportedTransactions
					.Where(x => x.CFType.Id == item.CFType.Id && x.CFClassification.Id == item.CFClassification.Id)
					.ToList();
				
				double budgeted = budgetTransactions
									.Where(x => x.CFClassificationId == item.CFClassification.Id && x.CFTypeId == item.CFType.Id)
									.Select(x => x.Amount)
									.Sum();
				BudgetTransaction transaction = budgetTransactions.Where(x => x.CFClassificationId == item.CFClassification.Id && x.CFTypeId == item.CFType.Id).FirstOrDefault();
				if (transaction != null)
				{
					transactionList
						.Add(new TransactionComparison(spent, budgeted, transaction));
					foreach (ReportedTransaction t in transactions)
					{
						reportedTransactions.Remove(t);
					}
				}
			}
			foreach (ReportedTransaction item in reportedTransactions.Where(X=>X.CFType.Id != "999"))
			{
				item.CFType = item.CFType;
				item.CFClassification = item.CFClassification;
				double spent = reportedTransactions
					.Where(x => x.CFType.Id == item.CFType.Id && x.CFClassification.Id == item.CFClassification.Id)
					.Select(x => x.Amount)
					.Sum();
				List<ReportedTransaction> transactions = reportedTransactions
					.Where(x => x.CFType.Id == item.CFType.Id && x.CFClassification.Id == item.CFClassification.Id)
					.ToList();
				transactionList
					.Add(new TransactionComparison(spent, 0, item));
			}
			comparison.TransactionComparisons = transactionList
				.GroupBy(p => new { p.CFClassificationId, p.CFTypeId })
				.Select(g => g.First())
				.ToList();
			return comparison;
		}
		public BudgetTransactionComparison(string collectionsId)
		{
			BudgetTransactionComparison comparison = Get(collectionsId);
			BudgetItem = comparison.BudgetItem;
			ReportedTransactions = comparison.ReportedTransactions;
			TransactionComparisons = comparison.TransactionComparisons;
		}
		public List<ReportedTransaction> CleanNonTransactional(List<ReportedTransaction> reportedTransactions)
		{
			List<Account> accounts = reportedTransactions.Select(x => x.Account).Distinct().ToList();
			List<ReportedTransaction> copy = new List<ReportedTransaction>(reportedTransactions.ToArray());
			CFType type = new CFType();
			List<CFType> types = new List<CFType>();
			types.Add(new CFType("Loans"));
			types.Add(new CFType("Car Loan"));
			types.Add(new CFType("Home Loan"));
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
			foreach (ReportedTransaction item in copy.Where(x=>x.Account.AccountType.Transactional && x.CFClassification.Sign == -1 && x.CFType.Id == "999"))
			{
				List<ReportedTransaction> transactions = reportedTransactions.Where(x => x.Amount == item.Amount).ToList();
				if(reportedTransactions.Where(x=>x.CFType.Id == "999" && (x.Account.AccountType.Transactional == false || (x.Account.AccountType.Name == "Home Loan" && x.CFClassification.Sign == 1)) &&  x.Amount == item.Amount).Any())
				{
					ReportedTransaction transaction = reportedTransactions.Where(x => x.CFType.Id == "999" && (x.Account.AccountType.Transactional == false || (x.Account.AccountType.Name == "Home Loan" && x.CFClassification.Sign == 1)) && x.Amount == item.Amount).FirstOrDefault();
					switch (transaction.Account.AccountType.Name)
					{
						case "Personal Loan":
							item.CFType = types.Where(x => x.Name == "Loans").FirstOrDefault();
							break;
						case "Car Loan":
							item.CFType = types.Where(x => x.Name == "Car Loan").FirstOrDefault();
							break;
						case "Home Loan":
							item.CFType = types.Where(x => x.Name == "Home Loan").FirstOrDefault();
							break;
					}
				}
			}
			// strip out the non tranasactional accounts 
			foreach (Account item in accounts.Where(x => x.AccountType.Transactional == false))
			{
				List<ReportedTransaction> ToBeRemoved = new List<ReportedTransaction>();
				foreach (ReportedTransaction t in copy.Where(x => x.Account.Id == item.Id))
				{
					ToBeRemoved.Add(t);
				}
				foreach(ReportedTransaction t in ToBeRemoved)
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
	public class TransactionGroups
	{
		public CFClassification CFClassification { get; set; }
		public CFType CFType { get; set; }
		public TransactionGroups() { }
		public List<TransactionGroups> GetGroups(List<BudgetTransaction> budgets, List<ReportedTransaction> reported,List<CFClassification> classifications, List<CFType> types)
		{
			foreach(BudgetTransaction item in budgets)
			{
				item.CFType = types.Where(x => x.Id == item.CFTypeId).FirstOrDefault();
				item.CFClassification = classifications.Where(x => x.Id == item.CFClassificationId).FirstOrDefault();
			}
			List<TransactionGroups> income = new List<TransactionGroups>();
			foreach(BudgetTransaction item in budgets.Where(x=>x.CFClassification.Sign == 1 && x.CFType.Id != "999"))
			{
				income.Add(new TransactionGroups
				{
					CFClassification = item.CFClassification,
					CFType = item.CFType
				});
			}
			foreach (ReportedTransaction item in reported.Where(x => x.CFClassification.Sign == 1 && x.CFType.Id != "999"))
			{
				income.Add(new TransactionGroups
				{
					CFClassification = item.CFClassification,
					CFType = item.CFType
				});
			}
			income = income
				.GroupBy(x=>x.CFType.Id)
				.Select(x=>x.First())
				.ToList();
			List<TransactionGroups> expense = new List<TransactionGroups>();
			foreach (BudgetTransaction item in budgets.Where(x => x.CFClassification.Sign == -1 && x.CFType.Id != "999"))
			{
				expense.Add(new TransactionGroups
				{
					CFClassification = item.CFClassification,
					CFType = item.CFType
				});
			}
			foreach (ReportedTransaction item in reported.Where(x => x.CFClassification.Sign == -1 && x.CFType.Id != "999"))
			{
				expense.Add(new TransactionGroups
				{
					CFClassification = item.CFClassification,
					CFType = item.CFType
				});
			}
			expense = expense
				.GroupBy(x => x.CFType.Id)
				.Select(x => x.First())
				.ToList();
			income.AddRange(expense);
			return income;
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
			Spent = Math.Round(spent,2);
			Remaining = Math.Round(budgeted - spent,2);
			CFClassificationId = transaction.CFClassificationId;
			CFTypeId = transaction.CFTypeId;
		}
		public TransactionComparison(double spent, double budgeted, ReportedTransaction transaction)
		{
			CFClassification = transaction.CFClassification;
			CFType = transaction.CFType;
			Budgeted = budgeted;
			Spent = Math.Round(spent,2);
			Remaining = Math.Round(budgeted - spent,2);
			CFClassificationId = transaction.CFClassification.Id;
			CFTypeId = transaction.CFType.Id;
		}
	}
}
