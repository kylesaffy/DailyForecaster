using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AspNetUsers
	{
		public string Email { get; set; }
		[StringLength(128)]
		[Required]
		public string Id { get; set; }
		public ICollection<UserCollectionMapping> UserCollectionMappings { get; set; }
		public ICollection<Collections> Collections { get; set; }
		public ICollection<BudgetTransaction> BudgetTransactions { get; set; } 
		public string getUserId(string userId)
		{
			return userIdGet(userId);
			//return "abc";
		}
		private string userIdGet(string userid)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AspNetUsers.Where(x => x.Email == userid).Select(x => x.Id).FirstOrDefault();
			}
		}
	}
}
