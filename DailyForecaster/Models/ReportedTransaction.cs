using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ReportedTransaction
	{
		public string Id { get; set; }
		public CFType CFType { get; set; }
		[Required]
		public CFClassification CFClassification { get; set; }
		[Required]
		public double Amount { get; set; }
		[Required]
		public DateTime DateCaptured { get; set; }
		[Required]
		public string SourceOfExpense { get; set; }
		public ManualCashFlow ManualCashFlow { get; set; }
		[Required]
		public string AccountId { get; set; }
		public Account Account { get; set; }
		public DateTime DateBooked { get; set; }
		public bool Validated { get; set; }
		public List<ReportedTransaction> GetTransactions(List<string> accountIds,int count,List<string> collectionsIds)
		{
			//get transaction
			List<ManualCashFlow> manualCashFlows = new List<ManualCashFlow>();
			List<AutomatedCashFlow> automatedCashFlows = new List<AutomatedCashFlow>();
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				manualCashFlows = _context
					.ManualCashFlows
					.Where(cf => accountIds.Contains(cf.AccountId))
					.Where(x => x.AutomatedCashFlow == null)
					.OrderByDescending(x=>x.DateBooked)
					.Take(count)
					.ToList();
				automatedCashFlows = _context
					.AutomatedCashFlows
					.Where(cf => accountIds.Contains(cf.AccountId))
					.OrderByDescending(x => x.DateBooked)
					.Take(count)
					.ToList();
			}
			//get type and classification lists
			CFType type = new CFType();
			CFClassification classification = new CFClassification();
			List<CFType> types = type.GetCFList(collectionsIds);
			List<CFClassification> classifications = classification.GetList();
			//reconfigure cash flows
			List<ReportedTransaction> transactions = new List<ReportedTransaction>();
			foreach(AutomatedCashFlow item in automatedCashFlows)
			{
				transactions.Add(new ReportedTransaction(item,types,classifications));
			}
			foreach (ManualCashFlow item in manualCashFlows)
			{
				transactions.Add(new ReportedTransaction(item, types, classifications));
			}
			return transactions;
		}
		/// <summary>
		/// Reported transactions for a given period 
		/// </summary>
		/// <param name="budget">The budget that is under examination</param>
		/// <returns>Reutns a list of reported transactions for the budget</returns>
		public List<ReportedTransaction> GetTransactions(Budget budget)
		{
			DateTime startDate = budget.StartDate.AddDays(-3);
			DateTime endDate = budget.EndDate.AddDays(3);
			Account account = new Account();
			List<Account> accounts = account.GetAccounts(budget.CollectionId);
			List<ReportedTransaction> reportedTransactions = new List<ReportedTransaction>();
			foreach (Account item in accounts)
			{
				List<ReportedTransaction> transactions = item.ReportedTransactions.Where(x => x.DateBooked > startDate && x.DateBooked < endDate).ToList();
				foreach(ReportedTransaction t in transactions)
				{
					t.AccountId = item.Id;
					t.Account = item;
				}
				reportedTransactions.AddRange(transactions);
				//AutomatedCashFlow automatedCashFlows = new AutomatedCashFlow();
				//ManualCashFlow manualCashFlow = new ManualCashFlow();					   				
				//List<AutomatedCashFlow> auto = automatedCashFlows.GetAutomatedCashFlows(item.Id,startDate,endDate);
				//List<ManualCashFlow> manual = manualCashFlow.GetManualCashFlows(item.Id,startDate,endDate);
				//manual = manual.Where(x => x.AutomatedCashFlowId == null).ToList();
				//foreach (AutomatedCashFlow automated in auto)
				//{			 					
				//	reportedTransactions.Add(new ReportedTransaction(automated,item));
				//}
				//foreach (ManualCashFlow man in manual)
				//{
				//	reportedTransactions.Add(new ReportedTransaction(man,item));
				//} 				
			}
			foreach(ReportedTransaction item in reportedTransactions)
			{
				item.Account.AccountType = accounts.Where(x => x.AccountTypeId == item.Account.AccountTypeId).Select(x => x.AccountType).FirstOrDefault();
			}
			return reportedTransactions;
		}
		public List<ReportedTransaction> GetTransactions(string AccountId)
		{
			AutomatedCashFlow automatedCashFlows = new AutomatedCashFlow();
			ManualCashFlow manualCashFlow = new ManualCashFlow();
			List<ReportedTransaction> reportedTransactions = new List<ReportedTransaction>();
			List<AutomatedCashFlow> auto = automatedCashFlows.GetAutomatedCashFlows(AccountId,10000);
			Account account = new Account();
			account = account.GetAccount(AccountId,false);
			List<ManualCashFlow> manual = manualCashFlow.GetManualCashFlows(AccountId);
			manual = manual.Where(x => x.AutomatedCashFlowId == null).ToList();
			foreach (AutomatedCashFlow automated in auto)
			{
				reportedTransactions.Add(new ReportedTransaction(automated,account));
			}
			foreach (ManualCashFlow man in manual)
			{
				reportedTransactions.Add(new ReportedTransaction(man,account));
			}
			return reportedTransactions;
		}
		private ReportedTransaction(AutomatedCashFlow auto, Account account)
		{
			CFType = new CFType(auto.CFTypeId);
			CFClassification = new CFClassification(auto.CFClassificationId);
			Amount = auto.Amount;
			DateCaptured = auto.DateCaptured;
			SourceOfExpense = auto.SourceOfExpense;
			Account = account;
			Account.ManualCashFlows = null;
			Account.AutomatedCashFlows = null;
			DateBooked = auto.DateBooked;
			Validated = auto.Validated;

		}
		private ReportedTransaction(AutomatedCashFlow auto,List<CFType> types,List<CFClassification> classifications)
		{
			CFType = types.Where(x=>x.Id == auto.CFTypeId).FirstOrDefault();
			CFClassification = classifications.Where(x => x.Id == auto.CFClassificationId).FirstOrDefault();
			Amount = auto.Amount;
			DateCaptured = auto.DateCaptured;
			SourceOfExpense = auto.SourceOfExpense;
			AccountId = auto.AccountId;
			DateBooked = auto.DateBooked;
			Validated = auto.Validated;

		}
		private ReportedTransaction(ManualCashFlow manual, List<CFType> types, List<CFClassification> classifications)
		{
			CFType = types.Where(x => x.Id == manual.CFTypeId).FirstOrDefault();
			CFClassification = classifications.Where(x => x.Id == manual.CFClassificationId).FirstOrDefault();
			Amount = manual.Amount;
			DateCaptured = manual.DateCaptured;
			SourceOfExpense = manual.SourceOfExpense;
			AccountId = manual.AccountId;
			DateBooked = manual.DateBooked;
			Validated = true;
		}
		private ReportedTransaction(ManualCashFlow manual, Account account)
		{
			CFType = new CFType(manual.CFTypeId);
			CFClassification = new CFClassification(manual.CFClassificationId);
			Amount = manual.Amount;
			DateCaptured = manual.DateCaptured;
			SourceOfExpense = manual.SourceOfExpense;
			Account = account;
			Account.ManualCashFlows = null;
			Account.AutomatedCashFlows = null;
			DateBooked = manual.DateBooked;
			Validated = true;
		}
		public ReportedTransaction() { }
	}
}
