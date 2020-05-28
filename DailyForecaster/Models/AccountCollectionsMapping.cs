using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	class AccountCollectionsMapping
	{
		private readonly FinPlannerContext _context;
		public AccountCollectionsMapping(FinPlannerContext context)
		{
			_context = context;
		}
		[Required]
		public string Id { get; set; }
		public string CollectionId { get; set; }
		public string AccountId { get; set; }
		public AccountCollectionsMapping() { }
		public List<Account> GetAccounts(string collectionId)
		{
			List<string> accountIds = _context.AccountCollectionsMapping
				.Where(x => x.CollectionId == collectionId)
				.Select(x => x.AccountId)
				.ToList();
			List<Account> accounts = new List<Account>();
			foreach (string item in accountIds)
			{
				accounts.Add(_context.Account.Find(item));
			}
			return accounts;
		}
	}
}
