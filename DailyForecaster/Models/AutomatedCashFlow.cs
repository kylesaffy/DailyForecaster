using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace DailyForecaster.Models
{
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
