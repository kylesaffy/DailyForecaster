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
		private readonly FinPlannerContext _context;
		public ManualCashFlow(FinPlannerContext context)
		{
			_context = context;
		}
		[Required]
		public string Id { get; set; }
		[Required]
		public CFType CFType { get; set; }
		[Required]
		public CFClassification CFClassification { get; set; }
		[Required]
		public double Amount { get; set; }
		[Required]
		public DateTime DateBooked { get; set; }
		[Required]
		public DateTime DateCaptured { get; set; }
		[Required]
		public string SourceOfExpense { get; set; } //Possibly make this a class - Account or Cash
		[Required]
		public bool Expected { get; set; }
		public string Description {get;set;}
		public string ExpenseLocation { get; set; }
		public string PhotoBlobLink { get; set; }
		public string UserId { get; set; }
		public string AccountId {get;set;}
		public bool isDeleted { get; set; }
		public ManualCashFlow(string cfId,string cfClass, double amount, DateTime dateBooked, string source, string userID,bool exp,string el)
		{
			CFType = _context.CFTypes.Where(x=>x.Id==cfId).FirstOrDefault();
			CFClassification = _context.CFClassifications.Find(cfClass);
			Amount = amount;
			DateBooked = dateBooked;
			DateCaptured = DateTime.Now;
			SourceOfExpense = source;
			Expected = exp;
			Id = Guid.NewGuid().ToString();
			UserId = userID;
			isDeleted = false;
			ExpenseLocation = el;
		}
		public ManualCashFlow(CFType cfId, CFClassification cfClass, double amount, DateTime dateBooked, string source, string userID, bool exp, string el)
		{
			CFType = cfId;
			CFClassification = cfClass;
			Amount = amount;
			DateBooked = dateBooked;
			DateCaptured = DateTime.Now;
			SourceOfExpense = source;
			Expected = exp;
			Id = Guid.NewGuid().ToString();
			UserId = userID;
			isDeleted = false;
			ExpenseLocation = el;
		}
		public ManualCashFlow(tempManualCashFlow cf)
		{
			new ManualCashFlow(cf.CFType, cf.CFClassification, cf.Amount, cf.DateBooked, cf.SourceOfExpense, cf.UserId, cf.Expected, cf.ExpenseLocation);
		}
		public ManualCashFlow(string id)
		{
			_context.ManualCashFlows.Find(id);
		}
	}
	public class tempManualCashFlow
	{
		public string CFType { get; set; }
		public string CFClassification { get; set; }
		public double Amount { get; set; }
		public DateTime DateBooked { get; set; }
		public string SourceOfExpense { get; set; } //Possibly make this a class - Account or Cash
		public bool Expected { get; set; }
		public string ExpenseLocation { get; set; }
		public string PhotoBlobLink { get; set; }
		public string UserId { get; set; }
	}
}
