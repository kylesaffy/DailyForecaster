using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class IncreaseModel
	{
		public string IncreaseModelId { get; set; }
		public string SimulationAssumptionsId { get; set; }
		[ForeignKey("SimulationAssumptionsId")]
		public SimulationAssumptions SimulationAssumptions { get; set; }
		public bool Increase { get; set; }
		public int IncreaseMonth { get; set; }
		public double IncreasePercentage { get; set; }
		public int LineId { get; set; }
		public string BudgetTransactionId { get; set; }
		[ForeignKey("BudgetTransactionId")]
		public BudgetTransaction BudgetTransaction { get; set; }
		public IncreaseModel() { }
		public List<IncreaseModel> Get(string assumptionsId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.IncreaseModel.Where(x => x.SimulationAssumptionsId == assumptionsId).ToList();
			}
		}
	}
}
