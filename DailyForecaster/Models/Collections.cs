using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
		public List<Account> Accounts { get; set; }
		public double TotalAmount { get; set; }
		public string DurationType { get; set; }
		public DateTime DateCreated { get; set; }
		[ForeignKey("AspNetUsers")]
		public string UserCreated { get; set; }
		public AspNetUsers AspNetUsers { get; set; }
		public ICollection<UserCollectionMapping> UserCollectionMappings { get; set; }
		public ICollection<Budget> Budgets { get; set; }
		public Collections() { }
		private Collections(string durationType,string name,string userId)
		{
			CollectionsId = Guid.NewGuid().ToString();
			Name = name;
			DurationType = durationType;
			AspNetUsers user = new AspNetUsers();
			UserCreated = user.getUserId(userId);
			DateCreated = DateTime.Now; 
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
					if (type == "Accounts")
					{
						collections.Add(new Collections(_context.Collections.Find(item)));
					}
					else
					{
						collections.Add(_context.Collections.Find(item));
					}
				}
			}
			return collections;
		}
		public Collections(Collections col)
		{
			Collections collection = col;
			AccountCollectionsMapping mapping = new AccountCollectionsMapping();
			collection.Accounts = mapping.GetAccounts(col.CollectionsId);
			collection.TotalAmount = collection.SumAmount();
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
			Collections col = new Collections(obj.durationType, obj.name,obj.User);
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
	}
}
