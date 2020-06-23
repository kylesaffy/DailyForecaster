using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;
using System.Linq;
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
		private Collections(string durationType,string name,string userId, int resetDate)
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
		public List<Collections> GetCollections(string userId,string type)
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
		public Collections(Collections col)
		{
			Account account = new Account();
			Collections collection = col;
			CollectionsId = col.CollectionsId;
			Name = col.Name;
			Accounts = account.GetAccountsWithCF(col.CollectionsId);
			DurationType = col.DurationType;
			//collection.TotalAmount = collection.SumAmount();
		}
		public double SumAmount()
		{
			foreach(Account acc in Accounts)
			{
				TotalAmount += acc.NetAmount; 
			}
			return TotalAmount;
		}
		public ReturnModel CreateCollection(NewCollectionsObj obj)
		{
			Collections col = new Collections(obj.durationType, obj.name,obj.User, obj.resetDate);
			UserCollectionMapping mapping = new UserCollectionMapping(col.CollectionsId,obj.User);
			ReturnModel returnModel = new ReturnModel();
			if(mapping.Id == "999")
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
		
		
	}
	public class NewCollectionsObj
	{
		public string durationType { get; set; }
		public string name { get; set; }
		public string User { get; set; }
		public int resetDate { get; set; }
	}
}
