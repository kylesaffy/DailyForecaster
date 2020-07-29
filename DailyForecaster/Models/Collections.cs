using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DailyForecaster.Controllers;
using DailyForecaster.Models;
using Microsoft.EntityFrameworkCore;
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
			if(budgets.Count() == 0)
			{
				Budget budget = new Budget();
				budgets = budget.NewBudget(col);
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
		public int getUnseen(string userId)
		{ 
			return GetCollections(userId, "TransactionCount").Select(x=>x.Accounts.Select(y=>y.AutomatedCashFlows.Where(z=>z.Validated == false))).Count();
		}
		public List<Collections> GetCollections(string userId, string type)
		{
			List<Collections> collections = new List<Collections>();
			if (userId != "")
			{
				AspNetUsers user = new AspNetUsers();
				userId = user.getUserId(userId);   				
				if (type != "Index")
				{
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						List<string> collectionIds = _context.UserCollectionMapping
							.Where(x => x.Id == userId)
							.Select(x => x.CollectionsId)
							.ToList();
						foreach (string item in collectionIds)
						{
							switch (type)
							{
								case "Accounts":
									collections.Add(new Collections(_context.Collections.Find(item), 0, type));
									break;
								case "TransactionList":
									collections.Add(new Collections(_context.Collections.Find(item), 10, type));
									break;
								case "TransactionCount":
									collections.Add(new Collections(_context.Collections.Find(item), 1000, type));
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
					using(FinPlannerContext _context = new FinPlannerContext())
					{
						List<string> collectionsIds = _context.UserCollectionMapping
							.Where(x => x.Id == userId)
							.Select(x => x.CollectionsId)
							.ToList();
						collections.AddRange(GetCollections(collectionsIds,10));
						return collections;
					}
				}
			}
			else
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					collections = _context
						.Collections
						.ToList();
				}
				Account account = new Account();
				foreach(Collections item in collections)
				{
					item.Accounts = account.GetAccounts(item.CollectionsId);
				}
				//what does this do? Who involes it?
				Budget budget = new Budget();
				foreach (Collections item in collections)
				{
					item.Budgets = budget.GetBudgets(item.CollectionsId);
				}
				return collections;
			}
		}
		public List<Collections> GetCollections(List<string> collectionsIds,int count)
		{
			//get collections
			List<Collections> collections = new List<Collections>();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				collections = _context
					.Collections
					.Where(col => collectionsIds.Contains(col.CollectionsId))
					.ToList();
			}
			//get accounts
			Account account = new Account();
			List<Account> accounts = account.GetAccountIndex(collectionsIds,count);
			//assign accounts
			foreach(Collections item in collections)
			{
				item.Accounts = accounts.Where(x => x.CollectionsId == item.CollectionsId).ToList();
			}
			return collections;
		}

		public Collections(Collections col, int count,string type)
		{
			Account account = new Account();
			CollectionsId = col.CollectionsId;
			Name = col.Name;
			if (count > 0)
			{
				if (type == "Budget")
				{

				}
				else
				{
					Accounts = account.GetAccountsWithCF(col.CollectionsId, count);
				}
			}
			else
			{
				Accounts = account.GetAccounts(col.CollectionsId);
			}
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
					if (budget != null)
					{
						if (budget.EndDate <= date)
						{
							budget.Duplicate(item);
						}
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
				BankCharges(item);
			}
			return true;
		}
		public void BankCharges(Collections collection)
		{
			try
			{
				Budget budget = collection.Budgets.Where(x => x.Simulation == false).OrderByDescending(x => x.EndDate).First();
				double fees = 0;
				foreach(Account acc in collection.Accounts.Where(x=>x.AccountType.Transactional == true))
				{
					fees = fees + acc.MonthlyFee;
					double debt = acc.AccountLimit - acc.Available;
					if (debt > 0)
					{
						fees = fees + debt * (acc.DebitRate / 12 / 100);
					}
				}
				fees = Math.Round(fees, 2);
				budget.GetBudgetTransacions();
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					if (budget.BudgetTransactions.Where(x => x.CFType.Name == "Bank Charges" && x.Automated == true).Any())
					{
						BudgetTransaction transaction = budget.BudgetTransactions.Where(x => x.CFType.Name == "Bank Charges" && x.Automated == true).FirstOrDefault();
						transaction.Amount = fees;
						_context.Entry(transaction).State = EntityState.Modified;
					}
					else
					{
						CFClassification classification = new CFClassification("Expense");
						CFType type = new CFType("Bank Charges");
						BudgetTransaction transaction = new BudgetTransaction()
						{
							BudgetId = budget.BudgetId,
							Automated = true,
							BudgetTransactionId = Guid.NewGuid().ToString(),
							CFClassificationId = classification.Id,
							CFTypeId = type.Id,
							Name = "Automated Bank Charges",
							Amount = fees,
						};
						_context.Add(transaction);
					}
					_context.SaveChanges();
				}
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e.Message);
			}
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
