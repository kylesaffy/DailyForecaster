using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DailyForecaster.Models;
namespace DailyForecaster.Models
{
	/// <summary>
	/// Accounts for transactions that are created by users
	/// </summary>
	public class ManualCashFlow
	{
		[Key]
		[Required]
		public string ID;
		[Required]
		public CashFlowItem CashFlowItem;
		[Required]
		public double Amount;
		[Required]
		public DateTime DateBooked;
		[Required]
		public DateTime DateCaptured;
		[Required]
		public string SourceOfExpense; //Possibly make this a class - Account or Cash
		[Required]
		public bool Expected;
		public string ExpenseLocation;
		public string PhotoBlobLink;
		public ManualCashFlow(CashFlowItem cf, double amount, DateTime dateBooked, string source)
		{
			CashFlowItem = cf;
			Amount = amount;
			DateBooked = dateBooked;
			DateCaptured = DateTime.Now;
			SourceOfExpense = source;
			ID = Guid.NewGuid().ToString();
		}
		public ManualCashFlow() { }
	}
}
