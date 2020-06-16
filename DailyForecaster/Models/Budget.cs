using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
		public ICollection<BudgetTransaction> BudgetTransactions { get; set; }
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
			//DateTime StartDate = obj.StartDate;
			List<BudgetTransaction> transactions = obj.BudgetTransactions;
			Collections col = new Collections(collectionId);
			DateTime EndDate = new DateTime();
			DateTime StartDate = DateTime.Now;
			switch (col.DurationType)
			{
				case "Day":
					EndDate = StartDate.AddDays(1);
					break;
				case "Week":
					int dayofweek = (int)StartDate.DayOfWeek;
					int difference = Math.Abs(dayofweek - col.ResetDay);
					StartDate = StartDate.AddDays(-difference);
					EndDate = StartDate.AddDays(7); 
					break;
				case "Month":
					int day = StartDate.Day;
					if(day == 0)
					{
						StartDate = new DateTime(StartDate.Year, StartDate.Month, 1).AddMonths(1);
						StartDate = StartDate.AddDays(-1);
					}
					else if(day > col.ResetDay)
					{
						StartDate = new DateTime(StartDate.Year, StartDate.Month, col.ResetDay);
					}
					else
					{
						StartDate = new DateTime(StartDate.Year, StartDate.Month - 1, col.ResetDay);
					}
					if(col.ResetDay == 28)
					{
						StartDate = new DateTime(StartDate.Year, StartDate.Month, 1).AddMonths(1);
						StartDate = StartDate.AddDays(-1);
					}
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
		public Budget GetBudget(string collectionsId)
		{
			int check = BudgetCount(collectionsId);
			Budget budget = new Budget();
			switch (check)
			{
				case 0:
					break;
				case 1:
					budget = getSingle(collectionsId);
					break;
				case 2:
					budget = FindBudget(collectionsId);
					break;
			}
			budget.Collection = null;
			return budget;
		}
		private Budget FindBudget(string collectionId)
		{
			Collections collection = new Collections(collectionId);
			Budget budget = new Budget();
			//Need to construct the date
			bool check = false;
			DateTime currentDate = DateTime.Now;
			switch (collection.DurationType)
			{
				case "Day":
					check = true;
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						budget = _context.Budget.Where(x => x.CollectionId == collectionId).OrderByDescending(x => x.EndDate).FirstOrDefault();
					}
					break;
				case "Week":
					int dayofweek = (int)currentDate.DayOfWeek;
					int shift = Math.Abs(dayofweek - collection.ResetDay);
					currentDate = currentDate.AddDays(-shift);
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						budget = _context.Budget.Where(x => x.CollectionId == collectionId && x.StartDate == currentDate).FirstOrDefault();
					}
					break;
				case "Month":
					DateTime resetDate = new DateTime(currentDate.Year, currentDate.Month, collection.ResetDay);
					if(currentDate > resetDate)
					{
						currentDate = resetDate;
					}
					else
					{
						currentDate = currentDate.AddMonths(-1);
					}
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						budget = _context.Budget.Where(x => x.CollectionId == collectionId && x.StartDate == currentDate).FirstOrDefault();
					}
					break;
			}
			BudgetTransaction budget1 = new BudgetTransaction();
			BudgetTransactions = budget1.GetBudgetTransactions(budget.BudgetId);
			return budget;
		}
		private int BudgetCount(string collectionsId)
		{
			Collections collections = new Collections(collectionsId);
			int count = collections.Budgets.Count();
			if(count > 2)
			{
				count = 2;
			}
			return count;
		}
		private Budget getSingle(string collectionsId)
		{
			Collections collections = new Collections(collectionsId);
			Budget budget = collections.Budgets.FirstOrDefault();
			BudgetTransaction budget1 = new BudgetTransaction();
			budget.BudgetTransactions = budget1.GetBudgetTransactions(budget.BudgetId);
			return budget;
		}
	}
	public class NewBudgetObj
	{
		public DateTime StartDate { get; set; }
		public string CollectionsId { get; set; }
		public List<BudgetTransaction> BudgetTransactions { get; set; }
		public string BudgetId { get; set; }
		public int Day { get; set; }
	}
}
