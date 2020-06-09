using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class Budget
	{
		[Required]
		public string BudgetId { get; set; }
		[Required]
		public DateTime StartDate { get; set; }
		[Required]
		public DateTime EndDate { get; set; }
		[Required]
		public string CollectionId { get; set; }
		[ForeignKey("CollectionId")]
		public Collections Collection { get; set; }
		public Budget() { }
		public Budget(string collectionsId, DateTime startDate, DateTime endDate)
		{
			BudgetId = Guid.NewGuid().ToString();
			StartDate = startDate;
			CollectionId = collectionsId;
			EndDate = endDate;
		}
		public bool BudgetCheck(string collectionId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				int cnt = _context.Budget.Where(x => x.CollectionId == collectionId).Count();
				if(cnt > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public bool Edit(NewBudgetObj obj)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				Budget budget = _context.Budget.Find(obj.BudgetId);
				List<BudgetTransaction> transactions = _context.BudgetTransactions.Where(x => x.BudgetId == obj.BudgetId).ToList();
				//split the transactions into two lists Present/Updated and New
				List<BudgetTransaction> newList = new List<BudgetTransaction>();
				BudgetTransaction b = new BudgetTransaction();
				bool check = false;
				foreach(BudgetTransaction item in obj.BudgetTransactions)
				{
					check = false;
					foreach(BudgetTransaction ex in transactions)
					{
						if(item.CFClassificationId == ex.CFClassificationId && item.CFTypeId == ex.CFTypeId)
						{
							check = true;
							ex.Amount = item.Amount;
							_context.Entry(ex).State = EntityState.Modified;
							break;
						}
					}
					if(!check)
					{
						newList.Add(item);
					}
				}
				newList = b.CreateBudgetTransactions(newList,obj.BudgetId);
				_context.AddRange(newList);
				try
				{
					_context.SaveChanges();
					return true;
				}
				catch(Exception e)
				{
					return false;
				}
			}
		}
		public bool Create(NewBudgetObj obj)
		{
			string collectionId = obj.CollectionsId;
			DateTime StartDate = obj.StartDate;
			List<BudgetTransaction> transactions = obj.BudgetTransactions;
			Collections col = new Collections();
			using (FinPlannerContext _context = new FinPlannerContext()) 
			{
				col = _context.Collections.Find(collectionId);
			}
			DateTime EndDate = new DateTime();
			switch (col.DurationType)
			{
				case "Day":
					EndDate = StartDate.AddDays(1);
					break;
				case "Week":
					EndDate = StartDate.AddDays(7);
					break;
				case "Month":
					EndDate = StartDate.AddMonths(1);
					break;
			}
			if (DateCheck(collectionId, EndDate))
			{
				
				Budget budget = new Budget(collectionId, StartDate, EndDate);
				BudgetTransaction t = new BudgetTransaction();
				List<BudgetTransaction> list = t.CreateBudgetTransactions(transactions,budget.BudgetId);
				try
				{
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						_context.Budget.Add(budget);
						foreach(BudgetTransaction item in list)
						{
							_context.BudgetTransactions.Add(item);
						}
						_context.SaveChanges();
					}
					return true;
				}
				catch(Exception e)
				{
					return false;
				}
			}
			else
			{
				return false;
			}

		}
		private bool DateCheck(string collectionsId, DateTime endDate)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				int count = _context.Budget.Where(x => x.CollectionId == collectionsId && x.StartDate < endDate).Count();
				if(count > 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
	}
	public class NewBudgetObj
	{
		public DateTime StartDate { get; set; }
		public string CollectionsId { get; set; }
		public List<BudgetTransaction> BudgetTransactions { get; set; }
		public string BudgetId { get; set; }
	}
}
