using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DailyForecaster.Controllers;
using DailyForecaster.Models;
using SQLitePCL;

namespace DailyForecaster.Models
{
	public class Collections
	{
		[Required]
		public string CollectionsId { get; set; }
		[Required]
		public string Name { get; set; }
		public double TotalAmount { get; set; }
		public string DurationType { get; set; }
		public DateTime DateCreated { get; set; }
		[ForeignKey("AspNetUsers")]
		public string UserCreated { get; set; }
		public int ResetDay { get; set; }
		public AspNetUsers AspNetUsers { get; set; }
		public ICollection<UserCollectionMapping> UserCollectionMappings { get; set; }
		public ICollection<Budget> Budgets { get; set; }
		public ICollection<Account> Accounts { get; set; }
		public ICollection<Simulation> Simualtions { get; set; }
		public Collections() { }
		private Collections(string durationType, string name, string userId, int resetDate)
		{
			CollectionsId = Guid.NewGuid().ToString();
			Name = name;
			DurationType = durationType;
			AspNetUsers user = new AspNetUsers();
			UserCreated = user.getUserId(userId);
			DateCreated = DateTime.Now;
			ResetDay = resetDate;
		}
		public Collections(string collectionsId)
		{
			Collections col = new Collections();
			List<Budget> budgets = new List<Budget>();
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				col = _context.Collections.Find(collectionsId);
				budgets = _context.Budget.Where(x => x.CollectionId == collectionsId).ToList();
			}
			BudgetTransaction transaction = new BudgetTransaction();
			foreach (Budget item in budgets)
			{
				item.Collection = null;
				item.BudgetTransactions = transaction.GetBudgetTransactions(item.BudgetId);
			}
			CollectionsId = collectionsId;
			Name = col.Name;
			Accounts = col.Accounts;
			TotalAmount = col.TotalAmount;
			DurationType = col.DurationType;
			DateCreated = col.DateCreated;
			UserCreated = col.UserCreated;
			Budgets = budgets;
			ResetDay = col.ResetDay;
		}
		public int BudgetCount()
		{
			int count = this.Budgets.Count();
			if (count > 2)
			{
				count = 2;
			}
			return count;

		}
		public List<Collections> GetCollections(string userId, string type)
		{
			if (userId != "")
			{
				AspNetUsers user = new AspNetUsers();
				userId = user.getUserId(userId);
				List<Collections> collections = new List<Collections>();
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					List<string> collectonIds = _context.UserCollectionMapping
						.Where(x => x.Id == userId)
						.Select(x => x.CollectionsId)
						.ToList();
					foreach (string item in collectonIds)
					{
						switch (type)
						{
							case "Accounts":
								collections.Add(new Collections(_context.Collections.Find(item)));
								break;
							case "TransactionList":
								collections.Add(new Collections(_context.Collections.Find(item)));
								break;
							default:
								collections.Add(_context.Collections.Find(item));
								break;
						}
					}
				}
				return collections;
			}
			else
			{
				List<Collections> collections = new List<Collections>();
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					List<string> collectonIds = _context.UserCollectionMapping
						.Select(x => x.CollectionsId)
						.Distinct()
						.ToList();
					foreach (string item in collectonIds)
					{
						collections.Add(new Collections(_context.Collections.Find(item)));
					}
				}
				return collections;
			}
		}
		public Collections(Collections col)
		{
			Account account = new Account();
			Collections collection = col;
			CollectionsId = col.CollectionsId;
			Name = col.Name;
			Accounts = account.GetAccountsWithCF(col.CollectionsId);
			DurationType = col.DurationType;
			ResetDay = col.ResetDay;
			//collection.TotalAmount = collection.SumAmount();
		}
		public double SumAmount()
		{
			foreach (Account acc in Accounts)
			{
				TotalAmount += acc.NetAmount;
			}
			return TotalAmount;
		}
		public ReturnModel CreateCollection(NewCollectionsObj obj)
		{
			Collections col = new Collections(obj.durationType, obj.name, obj.User, obj.resetDate);
			UserCollectionMapping mapping = new UserCollectionMapping(col.CollectionsId, obj.User);
			ReturnModel returnModel = new ReturnModel();
			if (mapping.Id == "999")
			{
				returnModel.result = false;
				return returnModel;
			}
			try
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					_context.Collections.Add(col);
					_context.UserCollectionMapping.Add(mapping);
					_context.SaveChanges();
				}
				returnModel.result = true;
				returnModel.returnStr = col.CollectionsId;
				return returnModel;
			}
			catch (Exception e)
			{
				returnModel.result = false;
				return returnModel;
			}
		}
		public bool CollectionCycle()
		{
			List<Collections> collections = GetCollections("", "");
			DateTime date = DateTime.Now.Date;
			try
			{
				foreach (Collections item in collections)
				{
					Budget budget = item.Budgets.OrderByDescending(x => x.EndDate).FirstOrDefault();
					if (budget.EndDate == date)
					{
						budget.Duplicate(item);
					}
				}
				return true;
			}
			catch (Exception e)
			{
				ExceptionCatcher exception = new ExceptionCatcher();
				exception.Catch(e.Message);
				return false;
			}
		}
		public async Task<bool> YodleeAccountConnect()
		{
			List<Collections> collections = GetCollections("", "");
			Account account = new Account();
			bool result = true;
			foreach(Collections item in collections.Where(x=>x.Accounts.Any()))
			{
				if (item.Accounts.Where(x => x.AccountIdentifier != null).Any())
				{
					result = await account.UpdateAccounts(item.CollectionsId, item.Accounts.ToList());
					if (!result)
					{
						ExceptionCatcher catcher = new ExceptionCatcher();
						catcher.Catch("Collestions Id: " + item.CollectionsId);
					}
				}
			}
			return true;
		}
		
	}
	public class NewCollectionsObj
	{
		public string durationType { get; set; }
		public string name { get; set; }
		public string User { get; set; }
		public int resetDate { get; set; }
	}
}
