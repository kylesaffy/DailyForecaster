using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AccountType
	{
		public string AccountTypeId { get; set; }
		public string Name { get; set; }
		[Required]
		public bool Transactional { get; set; }
		public bool Bank { get; set; }
		public bool Amortised { get; set; }
		public ICollection<Account> Accounts { get; set; }
		/// <summary>
		/// Returns a list of all of the account types
		/// </summary>
		/// <returns>A List of all of the account types</returns>
		public static List<AccountType> GetAccountTypes()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AccountType.ToList();
			}
		}
		public bool isAmortised(string AccountTypeId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AccountType.Where(x => x.AccountTypeId == AccountTypeId).Select(x => x.Amortised).FirstOrDefault();
			}
		}
	}
}
