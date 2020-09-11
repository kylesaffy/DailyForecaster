using Microsoft.EntityFrameworkCore;
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
		[ForeignKey("FirebaseUser")]
		public string FirebaseUserId { get; set; }
		public FirebaseUser FirebaseUser { get; set; }
		[ForeignKey("BudgetId")]
		public Budget Budget { get; set; }
		public AspNetUsers AspNetUsers { get; set; }
		[Required]
		public CFType CFType { get; set; }
		[Required]
		public CFClassification CFClassification { get; set; }
		public ICollection<Notes> Notes {get;set;}
		public bool Automated { get; set; }
		// public bool Deleted { get; set; }
		public BudgetTransaction() { }
		public bool UpdateBudget(List<BudgetTransaction> transactions)
		{
			string budgetId = transactions.Select(x => x.BudgetId).FirstOrDefault();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				List<BudgetTransaction> current = _context.BudgetTransactions.Where(x => x.BudgetId == budgetId).ToList();

			}
			return true;
		}
		/// <summary>
		/// Returns the amount that is expected to be spent within a particular budget
		/// </summary>
		/// <param name="budgetId">Unique Id of the budget that is under consideration</param>
		/// <returns>Returns a double of the amount that is expected to be spent</returns>
		public double ExpectedExpenses(string budgetId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					return _context
						.BudgetTransactions
						.Where(x => x.BudgetId == budgetId)
						.Where(x => x.CFClassification.Sign == -1)
						.Select(x => x.Amount)
						.Sum();
				}
				catch
				{
					return 0;
				}
			}
		}
		/// <summary>
		/// Removes a transaction if it already exists in the database
		/// </summary>
		public void Delete()
		{
			try
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{

					BudgetTransaction transaction = _context.BudgetTransactions.Find(this.BudgetTransactionId);
					_context.Remove(transaction);
					_context.SaveChanges();
				}
			}
			catch
			{

			}
		}
		/// <summary>
		/// Saves wither by updating or creating a single BudgetTransaction Object
		/// </summary>
		/// <param name="userId">Id of the user creating the transaction</param>
		public void Save(string userId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				if (this.BudgetTransactionId == null)
				{
					this.BudgetTransactionId = Guid.NewGuid().ToString();
					this.Automated = false;
					this.FirebaseUserId = userId;
					_context.Add(this);
				}
				else
				{
					BudgetTransaction transaction = _context.BudgetTransactions.Find(this.BudgetTransactionId);
					transaction = this;
					_context.Entry(transaction).State = EntityState.Modified;
				}
				_context.SaveChanges();
			}
		}
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
			try
			{
				UserId = users.getUserId(b.UserId);
			}
			catch
			{
				FirebaseUser user = new FirebaseUser();
				FirebaseUserId = user.GetFirebaseUser(UserId);
			}
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
				try
				{
					transactions = _context.BudgetTransactions.Where(x => x.BudgetId == budgetId).ToList();
				}
				catch(Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
				}
			}
			foreach(BudgetTransaction item in transactions)
			{
				item.Budget = null;
			}
			return transactions;
		}
	}
}
