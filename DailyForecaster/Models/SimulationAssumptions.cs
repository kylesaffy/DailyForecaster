using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
		public double Amount { get; set; }
		public string AccountId { get; set; }
		public DateTime ChangeDate { get; set; }
		public string CFClassificationId { get; set; }
		[ForeignKey("CFClassificationId")]
		public CFClassification CFClassification { get; set; }
		public string CFTypeId { get; set; }
		[ForeignKey("CFTypeId")]
		public CFType CFType { get; set; }
		public Simulation Simulation { get; set; }
		public string Type { get; set; }
		public string SimualtionName { get; set; }
		public SimulationAssumptions() { }
		/// <summary>
		/// Return a specific instance of the object
		/// </summary>
		/// <param name="Id">Id of the instance</param>
		public SimulationAssumptions(string Id)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				SimulationAssumptions a = _context.SimulationAssumptions.Find(Id);
				SimulationAssumptionsId = a.SimulationAssumptionsId;
				NumberOfMonths = a.NumberOfMonths;
				Bonus = a.Bonus;
				BonusMonth = a.BonusMonth;
				BonusAmount = a.BonusAmount;
				Increase = a.Increase;
				IncreaseMonth = a.IncreaseMonth;
				IncreasePercentage = a.IncreasePercentage;
				Recurring = a.Recurring;
				ChangeDate = a.ChangeDate;
				CFClassification = new CFClassification(a.CFClassificationId);
				CFClassificationId = a.CFClassificationId;
				CFType = new CFType(a.CFTypeId);
				CFTypeId = a.CFTypeId;
				Type = a.Type;
				SimualtionName = a.SimualtionName;
			}
		}
	}
}
