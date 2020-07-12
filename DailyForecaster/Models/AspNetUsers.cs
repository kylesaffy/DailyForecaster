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
		public string firstName { get; set; }
		public string lastName { get; set; }
		public ICollection<UserCollectionMapping> UserCollectionMappings { get; set; }
		public ICollection<Collections> Collections { get; set; }
		public ICollection<BudgetTransaction> BudgetTransactions { get; set; } 
		public string getUserId(string userId)
		{
			return userIdGet(userId);
			//return "abc";
		}
		public UserNames getNames(string userId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					AspNetUsers user = _context.AspNetUsers.Where(x => x.Email == userId).FirstOrDefault();
					return new UserNames() { first = user.firstName, last = user.lastName };
				}
				catch (Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
					return new UserNames();
				}
			}
		}
		private string userIdGet(string userid)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					return _context.AspNetUsers.Where(x => x.Email == userid).Select(x => x.Id).FirstOrDefault();
				}
				catch(Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
					return "error";
				}
			}
		}
	}
	public class UserNames
	{
		public string first { get; set; }
		public string last { get; set; }
	}
}
