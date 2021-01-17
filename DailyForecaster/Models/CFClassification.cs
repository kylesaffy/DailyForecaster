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
	/// 
	[Serializable]
	public class CFClassification
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int Sign { get; set; }

		public virtual ICollection<BudgetTransaction> BudgetTransactions { get; set; }
		public virtual ICollection<ManualCashFlow> ManualCashFlows { get; set; }
		/// <summary>
		/// Accesses the DB and returns a full list
		/// </summary>
		/// <returns>Returns a full list of CFClassifications</returns>
		public List<CFClassification> GetList()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.CFClassifications.ToList();
			}
		}
		public CFClassification() { }
		/// <summary>
		/// Constuctor that returns a single CFClassification by Id	or name
		/// </summary>
		/// <param name="id">The Id or name of the CFClassification</param>
		public CFClassification(string id)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				try {
						CFClassification temp = _context.CFClassifications.Find(id);
						Id = temp.Id;
						Name = temp.Name;
						Sign = temp.Sign;
						if (temp == null)
						{
							temp = _context.CFClassifications.Where(x => x.Name == id).FirstOrDefault();
							Id = temp.Id;
							Name = temp.Name;
							Sign = temp.Sign;
						}
					}
				catch
				{
					try
					{
						CFClassification temp = _context.CFClassifications.Where(x => x.Name == id).FirstOrDefault();
						Id = temp.Id;
						Name = temp.Name;
						Sign = temp.Sign;
					}
					catch
					{
						//Id = "";
						//Name = "";
						//Sign = 0;
					}
					
				}
			}
			
		}
	}
}
