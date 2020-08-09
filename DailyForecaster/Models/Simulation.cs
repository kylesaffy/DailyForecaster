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
		public void Build()
		{
			switch (this.Collections.DurationType)
			{
				case "Month":
					// Get Date of starting
					DateTime date = DateTime.Now;
					date = new DateTime(date.Year, date.Month, this.Collections.ResetDay);
					if (date > DateTime.Now)
					{
						date.AddMonths(1);
					}
					// Get base information
					Budget budget = Collections.Budgets.Where(x => x.Simulation == false).OrderByDescending(x => x.StartDate).FirstOrDefault();
					// Set Simulation Object
					Simulation simulation = new Simulation();
					// Loop for the number of Months
					for (int i = 0; i < SimulationAssumptions.NumberOfMonths; i++)
					{
						Budget sim = new Budget(CollectionsId, date.AddMonths(i), date.AddMonths(i + 1), true);
						// If first month, then pull BudgetTransactions from actual BudgetTransactions ELSE pull from previous month
						if (i == 0)
						{
							sim.BudgetTransactions = budget.BudgetTransactions;
						}
						else
						{
							sim.BudgetTransactions = this.Budgets[i - 1].BudgetTransactions;
						}
						//=======================================================================================================================================================
						// Bonus Handling
						//=======================================================================================================================================================
						// Add Bonus multiple if the correct month
						if (SimulationAssumptions.Bonus && SimulationAssumptions.BonusMonth == date.AddMonths(i).Month)
						{
							foreach(BudgetTransaction transaction in sim.BudgetTransactions.Where(x=>x.CFType.Name == "Salary"))
							{
								transaction.Amount = transaction.Amount * SimulationAssumptions.BonusAmount;
							}
						}
						// Strip Bonus Amount out if following Month
						if (SimulationAssumptions.Bonus && SimulationAssumptions.BonusMonth == date.AddMonths(i+1).Month)
						{
							foreach (BudgetTransaction transaction in sim.BudgetTransactions.Where(x => x.CFType.Name == "Salary"))
							{
								transaction.Amount = this.Budgets[i - 1].BudgetTransactions.Where(x=>x.Name == transaction.Name).Select(x=>x.Amount).FirstOrDefault();
							}
						}
						//=======================================================================================================================================================
						// Increase Handling
						//=======================================================================================================================================================
						// Add Increase if correct month (only add apportionment of previous month if not bonus month)
						if (SimulationAssumptions.Increase && SimulationAssumptions.IncreaseMonth == date.AddMonths(i).Month)
						{
							// if Bonus month == Increase month then use previous month
							if (SimulationAssumptions.IncreaseMonth == SimulationAssumptions.BonusMonth)
							{
								foreach (BudgetTransaction transaction in sim.BudgetTransactions.Where(x => x.CFType.Name == "Salary"))
								{
									transaction.Amount = this.Budgets[i - 1].BudgetTransactions.Where(x => x.Name == transaction.Name).Select(x => x.Amount).FirstOrDefault() * (1 + SimulationAssumptions.IncreasePercentage);
								}
							}
							else
							{
								foreach (BudgetTransaction transaction in sim.BudgetTransactions.Where(x => x.CFType.Name == "Salary"))
								{
									transaction.Amount = transaction.Amount * (1+ SimulationAssumptions.IncreasePercentage);
								}
							}
						}
						//=======================================================================================================================================================
						// Inflation Handling
						//=======================================================================================================================================================
						if(date.AddMonths(i).Month == 1)
						{
							foreach(BudgetTransaction item in sim.BudgetTransactions.Where(x=>x.CFType.Infaltion))
							{
								item.Amount = item.Amount * 1.06;
							}
						}
					}

					break;
			}
		}
	}
}
