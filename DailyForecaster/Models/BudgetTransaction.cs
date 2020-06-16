using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class BudgetTransaction
	{
		[Required]
		public string BudgetTransactionId { get; set; }
		[Required]
		public string BudgetId { get; set; }
		public string Name { get; set; }
		public double Amount { get; set; }
		[ForeignKey("CFType")]
		public string CFTypeId { get; set; }
		[ForeignKey("CFClassification")]
		public string CFClassificationId { get; set; }
		[ForeignKey("AspNetUsers")]
		public string UserId { get; set; }
		[ForeignKey("BudgetId")]
		public Budget Budget { get; set; }
		public AspNetUsers AspNetUsers { get; set; }
		[Required]
		public CFType CFType { get; set; }
		[Required]
		public CFClassification CFClassification { get; set; }
		public BudgetTransaction() { }
		public List<BudgetTransaction> CreateBudgetTransactions(List<BudgetTransaction> transactions, string BudgetId)
		{
			List<BudgetTransaction> newTransactions = new List<BudgetTransaction>();
			foreach(BudgetTransaction item in transactions)
			{
				newTransactions.Add(new BudgetTransaction(item, BudgetId));
			}
			return newTransactions;
		}
		private BudgetTransaction(BudgetTransaction b, string budgetId)
		{
			BudgetTransactionId = Guid.NewGuid().ToString();
			BudgetId = budgetId;
			Amount = b.Amount;
			Name = b.Name;
			CFTypeId = b.CFTypeId;
			CFClassificationId = b.CFClassificationId;
			AspNetUsers users = new AspNetUsers();
			UserId = users.getUserId(b.UserId);
			//using (FinPlannerContext _context = new FinPlannerContext())
			//{
			//	CFType = _context.CFTypes.Find(CFTypeId);
			//	CFClassification = _context.CFClassifications.Find(CFClassificationId);
			//}
		}
		public List<BudgetTransaction> GetBudgetTransactions(string budgetId)
		{
			List<BudgetTransaction> transactions = new List<BudgetTransaction>();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				transactions = _context.BudgetTransactions.Where(x => x.BudgetId == budgetId).ToList();
			}
			foreach(BudgetTransaction item in transactions)
			{
				item.Budget = null;
			}
			return transactions;
		}
	}
}
