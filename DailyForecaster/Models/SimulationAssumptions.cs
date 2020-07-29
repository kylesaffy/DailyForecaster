using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class SimulationAssumptions
	{
		[Required]
		public string SimulationAssumptionsId { get; set; }
		[Required]
		public int NumberOfMonths { get; set; }
		[Required]
		public bool Bonus { get; set; }
		public int BonusMonth { get; set; }
		public double BonusAmount { get; set; }
		[Required]
		public bool Increase { get; set; }
		public int IncreaseMonth { get; set; }
		public double IncreasePercentage { get; set; }
		public bool Recurring { get; set; }
		public CFClassification CFClassification { get; set; }
		public CFType CFType { get; set; }

		public Simulation Simulation { get; set; }
	}
}
