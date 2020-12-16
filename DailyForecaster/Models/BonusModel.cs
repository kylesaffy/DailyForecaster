using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class BonusModel
	{
		public string BonusModelId { get; set; }
		public string SimulationAssumptionsId { get; set; }
		[ForeignKey("SimulationAssumptionsId")]
		public SimulationAssumptions SimulationAssumptions { get; set; }
		public bool Bonus { get; set; }
		public int BonusMonth { get; set; }
		public double BonusAmount { get; set; }
		public int LineId { get; set; }
		public string BudgetTransactionId { get; set; }
		[ForeignKey("BudgetTransactionId")]
		public BudgetTransaction BudgetTransaction { get; set; }
		public BonusModel() { }
		public List<BonusModel> Get(string assumptionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.BonusModel.Where(x => x.SimulationAssumptionsId == assumptionsId).ToList();
			}
		}
	}
}
