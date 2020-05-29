using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DailyForecaster.Models;

namespace DailyForecaster.Models
{
	public class Collections
	{
		private readonly FinPlannerContext _context;
		public Collections(FinPlannerContext context)
		{
			_context = context;
		}
		[Required]
		public string CollectionsId { get; set; }
		[Required]
		public string Name { get; set; }
		public List<Account> Accounts { get; set; }
		public double TotalAmount { get; set; }
		public Collections() { }
		public List<Collections> GetCollections(string userId)
		{
			List<string> collectonIds = _context.UserCollectionMapping
				.Where(x=>x.UserId == userId)
				.Select(x=>x.CollectionId)
				.ToList();
			List<Collections> collections = new List<Collections>();
			foreach(string item in collectonIds)
			{
				collections.Add(new Collections(_context.Collections.Find(item)));
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
		
	}
	
}
