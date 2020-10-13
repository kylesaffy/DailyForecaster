using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AccountBalance
	{
		public string AccountBalanceId { get; set; }
		public string AccountId { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		public DateTime Date { get; set; }
		public double Amount { get; set; }
		public void Delete(string AccountId)
		{
			List<AccountBalance> accounts = new List<AccountBalance>();
			accounts = GetAccounts(AccountId, true);
			foreach (AccountBalance item in accounts)
			{
				item.Delete();
			}
		}
		private List<AccountBalance> GetAccounts(string accountId, bool ans)
		{
			if (ans)
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					return _context.AccountBalance.Where(x => x.AccountId == accountId).ToList();
				}
			}
			return null;
		}
		private void Delete()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Remove(this);
				_context.SaveChanges();

			}
		}
	}
}
