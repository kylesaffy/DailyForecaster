using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class PaymentLink
	{
		public string PaymentLinkId { get; set; }
		public string AccountId { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		public int LineId { get; set; }
		/// <summary>
		/// If an object belonging to this account is identified then it is returned
		/// </summary>
		/// <param name="accountId">Account ID that the object is to belong to</param>
		/// <returns>A single object with the users linking to the account Id</returns>
		public PaymentLink GetByAccountId(string accountId)
		{
			try
			{
				using(FinPlannerContext _context = new FinPlannerContext())
				{
					return _context.PaymentLink.Where(x => x.AccountId == accountId).FirstOrDefault();
				}
			}
			catch
			{
				return null;
			}
		}
	}
}
