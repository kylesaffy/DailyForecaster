using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class Simulation
	{
		[Required]
		public string SimulationId { get; set; }
		[Required]
		public string SimulationName { get; set; }
		[Required]
		public string CollectionsId { get; set; }
		[ForeignKey("CollectionsId")]
		public Collections Collections { get; set; }
		public List<Budget> Budgets { get; set; }
		public string SimulationAssumptionsId { get; set; }
		[ForeignKey("SimulationAssumptionsId")]
		public SimulationAssumptions SimulationAssumptions { get; set; }
	}
}
