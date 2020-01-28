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
		public string Id;
		[Required]
		public CFType CFType;
		[Required]
		public CFClassification CFClassification;
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
		public string UserId;
		public ManualCashFlow(string cfId,string cfClass, double amount, DateTime dateBooked, string source, string userID)
		{
			CFType = _context.CFTypes.Find(cfId);
			CFClassification = _context.CFClassifications.Find(cfClass);
			Amount = amount;
			DateBooked = dateBooked;
			DateCaptured = DateTime.Now;
			SourceOfExpense = source;
			Id = Guid.NewGuid().ToString();
			UserId = userID;
		}
	}
}
