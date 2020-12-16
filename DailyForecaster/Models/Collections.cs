using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
		/// <summary>
		/// Returns the first collections Id from the user mapping table
		/// </summary>
		/// <param name="userId">Id of the user that the data is being collected for</param>
		/// <returns>The string of the first collections Id from the user mapping table</returns>
		public string GetId(string userId)
		{
			UserCollectionMapping mapping = new UserCollectionMapping();
			try
			{
				return mapping.getCollectionIds(userId, "firebase").FirstOrDefault();
			}
			catch
			{
				return mapping.getCollectionIds(userId,"asp").FirstOrDefault();
			}
		}
		/// <summary>
		/// Exlpicit eager articulated list of collection objects
		/// </summary>
		/// <param name="collectionsIds">List of the collection Ids</param>
		/// <returns>Returns a list of collection from the string of Ids supplied</returns>
		public List<Collections> GetEagerList(List<string> collectionsIds)
		{
			List<Collections> collections = new List<Collections>();
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				foreach (string id in collectionsIds)
				{
					try
					{
						collections.Add(_context
							.Collections
							.Where(x => x.CollectionsId == id)
							.FirstOrDefault());
					}
					catch(Exception e)
					{
						ExceptionCatcher catcher = new ExceptionCatcher();
						catcher.Catch(e.Message);
					}
				}
			}
			return collections;
		}
		/// <summary>
		/// Returns a collection object
		/// </summary>
		/// <param name="collectionsId">Collection Id of the object required</param>
		/// <returns>Returns a collation object</returns>
		public Collections GetCollections(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
					.Collections
					.Find(collectionsId);
			}
		}
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
		private Collections(NewCollectionsObj obj, string UserId)
		{
			CollectionsId = Guid.NewGuid().ToString();
			Name = obj.name;
			DurationType = obj.durationType;
			// FirebaseUserId = new FirebaseUser(UserId).FirebaseUserId;
			DateCreated = DateTime.Now;
			ResetDay = obj.resetDate;
		}
		/// <summary>
		/// Returns a populated collections object
		/// </summary>
		/// <param name="collectionsId">Id of the collection object</param>
		public Collections(string collectionsId)
		{
			Collections col = new Collections();
			List<Budget> budgets = new List<Budget>();
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				col = _context.Collections.Find(collectionsId);
				budgets = _context.Budget.Where(x => x.CollectionId == collectionsId).ToList();
			}
			if (budgets.Count() == 0)
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
			return GetCollections(userId, "TransactionCount").Select(x => x.Accounts.Select(y => y.AutomatedCashFlows.Where(z => z.Validated == false))).Count();
		}
		/// <summary>
		/// Type depenedent list of collections that are returned for a given user
		/// </summary>
		/// <param name="email">Email address of the user, can be ""</param>
		/// <param name="type">The type that is required, "", Index, CollectionsVM</param>
		/// <returns>List of collections objects</returns>
		public List<Collections> GetCollections(string email, string type)
		{
			List<Collections> collections = new List<Collections>();
			if (email != "")
			{
				FirebaseUser user = new FirebaseUser();
				string userId = user.GetUserId(email);
				if (type == "Index")
				{
					AspNetUsers user1 = new AspNetUsers();
					userId = user1.getUserId(email);
				}
				if (type != "Index" && !(type == "CollectionsVM" || type == "BudgetVM" || type == "SafeToSpendVM" || type == "ManualCashFlowsVM"))
				{
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						List<string> collectionIds = _context.UserCollectionMapping
							.Where(x => x.FirebaseUserId == userId)
							.Select(x => x.CollectionsId)
							.ToList();
						foreach (string item in collectionIds)
						{
							switch (type)
							{
								case "Accounts":
									collections.Add(new Collections(_context.Collections.Find(item), 0, type));
									break;
								case "DailyReport":
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
				else if(type == "CollectionsVM" || type == "BudgetVM" || type == "SafeToSpendVM" || type == "ManualCashFlowsVM" || type == "Simulations")
				{
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						List<string> collectionIds = _context.UserCollectionMapping
							.Where(x => x.FirebaseUserId == userId)
							.Select(x => x.CollectionsId)
							.ToList();
						collections = _context
							.Collections
							.Where(col => collectionIds.Contains(col.CollectionsId))
							.ToList();
					}
					Account account = new Account();
					foreach (Collections item in collections)
					{
						item.Accounts = account.GetAccounts(item.CollectionsId);
					}
					return collections;
				}
				else
				{
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						List<string> collectionsIds = _context.UserCollectionMapping
							.Where(x => x.FirebaseUserId == userId)
							.Select(x => x.CollectionsId)
							.ToList();
						collections.AddRange(GetCollections(collectionsIds, 10));
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
				foreach (Collections item in collections)
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
		public List<Collections> GetCollections(List<string> collectionsIds, int count)
		{
			//get collections
			List<Collections> collections = new List<Collections>();
			try
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					collections = _context
						.Collections
						.Where(col => collectionsIds.Contains(col.CollectionsId))
						.ToList();
				}
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e.Message);
			}
			//get accounts
			Account account = new Account();
			List<Account> accounts = account.GetAccountIndex(collectionsIds, count);
			//assign accounts
			foreach (Collections item in collections)
			{
				item.Accounts = accounts.Where(x => x.CollectionsId == item.CollectionsId).ToList();
			}
			return collections;
		}

		public Collections(Collections col, int count, string type)
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
			if (mapping.FirebaseUserId == "999")
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
		public ReturnModel CreateCollection(NewCollectionsObj obj, string userId, string email)
		{
			UserCollectionMapping mapping = new UserCollectionMapping();
			Collections col = new Collections(obj,userId);
			if (obj.User != null)
			{
				mapping = new UserCollectionMapping(col.CollectionsId, obj.User);
			}
			else
			{
				mapping = new UserCollectionMapping(col.CollectionsId, userId);
			}
			ReturnModel returnModel = new ReturnModel();
			if (mapping.FirebaseUserId == "999")
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
				UserInteraction interaction = new UserInteraction();
				interaction.CollectionsIncratment(col.CollectionsId, email);
				return returnModel;
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e);
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
			foreach (Collections item in collections.Where(x => x.Accounts.Any()))
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
				Budget budget = collection.Budgets.Where(x => x.SimulationBool == false).OrderByDescending(x => x.EndDate).First();
				double fees = 0;
				foreach (Account acc in collection.Accounts.Where(x => x.AccountType.Bank))
				{
					fees = fees + acc.MonthlyFee;
					double debt = acc.AccountLimit - acc.Available;
					if (debt > 0)
					{
						fees = fees + debt * (acc.CreditRate / 12 / 100);
					}
				}
				fees = Math.Round(fees, 2);
				budget.GetBudgetTransacions();
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					if (budget.BudgetTransactions.Where(x => x.CFType.Name == "Bank Charges" && (x.Automated == true || x.Name == "Automated Bank Charges")).Any())
					{
						BudgetTransaction transaction = budget.BudgetTransactions.Where(x => x.CFType.Name == "Bank Charges" && (x.Automated == true || x.Name == "Automated Bank Charges")).FirstOrDefault();
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