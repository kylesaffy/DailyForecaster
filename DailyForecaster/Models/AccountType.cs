using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AccountType
	{
		public string AccountTypeId { get; set; }
		public string Name { get; set; }
		public ICollection<Account> Accounts { get; set; }
		public List<AccountType> GetAccountTypes()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AccountType.ToList();
			}
		}
	}
}
