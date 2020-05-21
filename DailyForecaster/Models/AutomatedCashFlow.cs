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
		private readonly FinPlannerContext _context;
		public AutomatedCashFlow(FinPlannerContext context)
		{
			_context = context;
		}
		[Required]
		public string ID { get; set; }
		[Required]
		public CFType CFType { get; set; }
		[Required]
		public CFClassification CFClassification { get; set; }
		[Required]
		public double Amount { get; set; }
		public DateTime DateBooked { get; set; }
		[Required]
		public DateTime DateCaptured { get; set; }
		[Required]
		public string SourceOfExpense { get; set; }
		public ManualCashFlow ManualCashFlow { get; set; }
		[Required]
		public string AccountId { get; set; }
		public AutomatedCashFlow()
		{ }
		public List<AutomatedCashFlow> GetAutomatedCashFlows(string AccId)
		{
			return _context.AutomatedCashFlows.Where(x => x.AccountId == AccId).ToList();
		}
	}
}
