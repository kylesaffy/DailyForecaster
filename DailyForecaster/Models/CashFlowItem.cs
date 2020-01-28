using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace DailyForecaster
{
	/// <summary>
	/// This is the name of the cash flow type as well as whether or not this is a a custom or default
	/// </summary>
	public class CFType
	{
		[Required]
		public string Id;
		[Required]
		public bool Custom;
		[Required]
		public string Name;
		public string ClientReference;
		public CFType()
		{
			Id = Guid.NewGuid().ToString();
			Custom = true;
			Name = "Undefined";
		}
	}
	/// <summary>
	/// Idetifies whether or not this is a Income or Expense
	/// </summary>
	public class CFClassification
	{
		[Key]
		[Required]
		public string ID;
		[Required]
		public string Name;
	}
	/// <summary>
	/// Sub class to the actual Cash flow Item
	/// </summary>
	public class CashFlowItem
	{
		[Key]
		[Required]
		public string ID;
		[Required]
		public CFType CFType;
		[Required]
		public CFClassification CFClassification;
	}
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
		public ManualCashFlow(CashFlowItem cf,double amount,DateTime dateBooked,string source)
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
	/// <summary>
	/// Accounts for transactions that are captured from an automated source
	/// </summary>
	public class AutomatedCashFlow
	{
		[Key]
		[Required]
		public string ID;
		[Required]
		public CashFlowItem CashFlowItem;
		[Required]
		public double Amount;
		public DateTime DateBooked;
		[Required]
		public DateTime DateCaptured;
		[Required]
		public string SourceOfExpense;
		public ManualCashFlow ManualCashFlow;
	}
}
