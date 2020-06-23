using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace DailyForecaster.Models
{
	/// <summary>
	/// Idetifies whether or not this is a Income or Expense
	/// </summary>
	public class CFClassification
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int Sign { get; set; }

		public virtual ICollection<BudgetTransaction> BudgetTransactions { get; set; }
		public virtual ICollection<ManualCashFlow> ManualCashFlows { get; set; }
		public List<CFClassification> GetList()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.CFClassifications.ToList();
			}
		}
		public CFClassification() { }
		public CFClassification(string id)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				CFClassification temp = _context.CFClassifications.Find(id);
				Id = temp.Id;
				Name = temp.Name;
				Sign = temp.Sign;
			}
			
		}
	}
}
