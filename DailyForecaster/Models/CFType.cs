using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	/// <summary>
	/// This is the name of the cash flow type as well as whether or not this is a a custom or default
	/// </summary>
	public class CFType
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public bool Custom { get; set; }
		[Required]
		public string Name { get; set; }
		public string ClientReference { get; set; }
		public virtual ICollection<BudgetTransaction> BudgetTransactions { get; set; }
		public List<CFType> GetCFList()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.CFTypes.ToList();
			}
		}
	}
}
