using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ReportedTransaction
	{
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
		public List<ReportedTransaction> GetTransactions(Budget budget)
		{
			DateTime startDate = budget.StartDate.AddDays(-1);
			DateTime endDate = budget.EndDate.AddDays(1);
			Account account = new Account();
			List<Account> accounts = account.GetAccounts(budget.CollectionId);
			List<ReportedTransaction> reportedTransactions = new List<ReportedTransaction>();
			foreach (Account item in accounts)
			{
				AutomatedCashFlow automatedCashFlows = new AutomatedCashFlow();
				ManualCashFlow manualCashFlow = new ManualCashFlow();					   				
				List<AutomatedCashFlow> auto = automatedCashFlows.GetAutomatedCashFlows(item.Id,startDate,endDate);
				List<ManualCashFlow> manual = manualCashFlow.GetManualCashFlows(item.Id,startDate,endDate);
				foreach (AutomatedCashFlow automated in auto)
				{
					foreach (ManualCashFlow manual1 in manual)
					{
						if (automated.Amount == manual1.Amount && automated.AccountId == manual1.AccountId && (automated.DateBooked == manual1.DateBooked || manual1.DateBooked < automated.DateBooked.AddDays(8)))
						{
							manual.Remove(manual1);
							break;
						}
					}
					reportedTransactions.Add(new ReportedTransaction(automated));
				}
				foreach (ManualCashFlow man in manual)
				{
					reportedTransactions.Add(new ReportedTransaction(man));
				} 				
			}
			return reportedTransactions;
		}
		public List<ReportedTransaction> GetTransactions(string AccountId)
		{
			AutomatedCashFlow automatedCashFlows = new AutomatedCashFlow();
			ManualCashFlow manualCashFlow = new ManualCashFlow();
			List<ReportedTransaction> reportedTransactions = new List<ReportedTransaction>();
			List<AutomatedCashFlow> auto = automatedCashFlows.GetAutomatedCashFlows(AccountId);
			List<ManualCashFlow> manual = manualCashFlow.GetManualCashFlows(AccountId);
			foreach(AutomatedCashFlow automated in auto)
			{
				foreach(ManualCashFlow manual1 in manual)
				{
					if(automated.Amount == manual1.Amount && automated.AccountId == manual1.AccountId && (automated.DateBooked == manual1.DateBooked || manual1.DateBooked < automated.DateBooked.AddDays(8) ))
					{
						manual.Remove(manual1);
						break;
					}
				}
				reportedTransactions.Add(new ReportedTransaction(automated));
			}
			foreach(ManualCashFlow man in manual)
			{
				reportedTransactions.Add(new ReportedTransaction(man));
			}
			return reportedTransactions;
		}
		private ReportedTransaction(AutomatedCashFlow auto)
		{
			CFType = auto.CFType;
			CFClassification = auto.CFClassification;
			Amount = auto.Amount;
			DateCaptured = auto.DateCaptured;
			SourceOfExpense = auto.SourceOfExpense;
		}
		private ReportedTransaction(ManualCashFlow manual)
		{
			CFType = new CFType(manual.CFTypeId);
			CFClassification = new CFClassification(manual.CFClassificationId);
			Amount = manual.Amount;
			DateCaptured = manual.DateCaptured;
			SourceOfExpense = manual.SourceOfExpense;
		}
		public ReportedTransaction() { }
	}
}
