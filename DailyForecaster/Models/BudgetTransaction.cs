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
		public ICollection<Notes> Notes {get;set;}
		public bool Automated { get; set; }
		public BudgetTransaction() { }
		public List<BudgetTransaction> CreateBudgetTransactions(List<BudgetTransaction> transactions, string BudgetId, string collectionsId)
		{
			List<BudgetTransaction> newTransactions = new List<BudgetTransaction>();
			foreach(BudgetTransaction item in transactions)
			{
				newTransactions.Add(new BudgetTransaction(item, BudgetId,collectionsId));
			}
			return newTransactions;
		}
		public BudgetTransaction(BudgetTransaction b, string budgetId,string collectionsId)
		{
			BudgetTransactionId = Guid.NewGuid().ToString();
			BudgetId = budgetId;
			Amount = b.Amount;
			Name = b.Name;
			CFType type = new CFType();
			List<CFType> list = type.GetCFList(collectionsId);
			if (list.Any(x=>x.Id == b.CFTypeId))
			{
				CFTypeId = b.CFTypeId;
			}
			else
			{
				type = type.CreateCFType(collectionsId, b.CFTypeId);
				CFTypeId = type.Id;
			}
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
