using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
		public Simulation() { }
		/// <summary>
		/// Instantiates a Simulation object and saves it
		/// </summary>
		/// <param name="assumptions">The asumtpions object that underpins the Simulation</param>
		/// <param name="collectionsId">The collection that the simulation is being conducted under</param>
		public Simulation(SimulationAssumptions assumptions, string collectionsId)
		{
			assumptions.SimulationAssumptionsId = Guid.NewGuid().ToString();
			SimulationAssumptions = assumptions;
			SimulationName = assumptions.SimualtionName;
			SimulationId = "temp";
			CollectionsId = collectionsId;
			SimulationId = Save();
		}
		/// <summary>
		/// Saves the current Simulation
		/// </summary>
		private string Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				if (this.SimulationId == "temp")	 
				{
					this.SimulationId = Guid.NewGuid().ToString();
					_context.Add(this.SimulationAssumptions);
					_context.Add(this);
				}
				else
				{
					_context.Entry(this).State = EntityState.Modified;
				}
				//_context.SaveChanges();
			}
			return this.SimulationId;
		}
		/// <summary>
		/// Builds the simulation using the connected references from the db pick-up
		/// </summary>
		/// <returns>Returns the initial build of the simulation</returns>
		public Simulation BuildSimulation(SimulationAssumptions assumptions)
		{
			this.SimulationAssumptions = assumptions;
			this.Build();
			this.Collections.Accounts = null;
			this.SimulationAssumptions.Simulation = null;
			return this;
		}
		private void Build()
		{
			this.Collections = new Collections(this.CollectionsId);
			// Get base information
			Budget budget = Collections.Budgets.Where(x => x.Simulation == false).OrderByDescending(x => x.StartDate).FirstOrDefault();
			this.Collections.Budgets = null;
			Account account = new Account();
			this.Collections.Accounts = account.GetAccounts(this.CollectionsId);
			foreach(Account acc in Collections.Accounts)
			{
				acc.ReportedTransactions = null;
				acc.AutomatedCashFlows = null;
				acc.ManualCashFlows = null;
				acc.Institution = null;
			}
			//this.SimulationAssumptions = new SimulationAssumptions(SimulationAssumptionsId);
			this.Budgets = new List<Budget>();
			CFClassification cf = new CFClassification();
			List<CFClassification> cfList = cf.GetList();
			CFType type = new CFType();
			List<CFType> typeList = type.GetCFList(CollectionsId);
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
					// Set Simulation Object
					Simulation simulation = new Simulation();
					// Loop for the number of Months
					for (int i = 0; i < SimulationAssumptions.NumberOfMonths; i++)
					{
						Budget sim = new Budget(CollectionsId, date.AddMonths(i), date.AddMonths(i + 1), true);
						sim.AccountStates = new List<AccountState>();
						// If first month, then pull BudgetTransactions from actual BudgetTransactions ELSE pull from previous month
						if (i == 0)
						{
							sim.BudgetTransactions = budget.BudgetTransactions.Where(x=>x.Automated == false).ToList();
							foreach(BudgetTransaction item in sim.BudgetTransactions)
							{
								item.CFClassification = cfList.Where(x => x.Id == item.CFClassificationId).FirstOrDefault();
								item.CFType = typeList.Where(x => x.Id == item.CFTypeId).FirstOrDefault();
							}
							foreach(Account item in Collections.Accounts)
							{
								sim.AccountStates.Add(new AccountState(item,sim.BudgetId));
							}
						}
						else
						{
							sim.BudgetTransactions = this.Budgets[i - 1].BudgetTransactions.Where(x => x.Automated == false).ToList();
							foreach (BudgetTransaction item in sim.BudgetTransactions)
							{
								item.CFClassification = cfList.Where(x => x.Id == item.CFClassificationId).FirstOrDefault();
							}
							foreach (Account item in this.Collections.Accounts)
							{
								sim.AccountStates.Add(new AccountState(item, sim.BudgetId, this
									.Budgets[i - 1]
									.AccountStates
									.Where(x => x.AccountId == item.Id)
									.FirstOrDefault()));
							}
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
									transaction.Amount = this
										.Budgets[i - 1]
										.BudgetTransactions.Where(x => x.Name == transaction.Name)
										.Select(x => x.Amount)
										.FirstOrDefault() 
										* (1 + SimulationAssumptions.IncreasePercentage);
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
						//=======================================================================================================================================================
						// Cashflow Handling
						//=======================================================================================================================================================
						if(i > 0)
						{
							//double fees = 0;
							//foreach(Account acc in Collections.Accounts.Where(x=>x.AccountType.Transactional))
							//{
							//	fees = fees + acc.MonthlyFee;
							//	double debt = acc.AccountLimit - acc.Available;
							//	if (debt > 0)
							//	{
							//		fees = fees + debt * (acc.CreditRate / 12 / 100);
							//	}
							//}
							//fees = Math.Round(fees, 2);
							//CFClassification classification = new CFClassification("Expense");
							//CFType typeT = new CFType("Bank Charges");
							//sim.BudgetTransactions.Add(new BudgetTransaction
							//{
							//	BudgetId = sim.BudgetId,
							//	Automated = true,
							//	BudgetTransactionId = Guid.NewGuid().ToString(),
							//	CFClassificationId = classification.Id,
							//	CFClassification = classification,
							//	CFType = typeT,
							//	CFTypeId = typeT.Id,
							//	Name = "Automated Bank Charges",
							//	Amount = fees,
							//});
							sim.BudgetTransactions.Add(AddAutomated(sim.BudgetId));
							sim.AccountStates
								.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
								.OrderByDescending(x => x.Account.CreditRate)
								.FirstOrDefault()
								.Update(sim.BudgetTransactions.Sum(x => x.Amount * x.CFClassification.Sign));
							
						}
						this.Budgets.Add(sim);
					}

					break;
			}
		}
		/// <summary>
		/// Calculation and addition of the Automated Bank Charges field
		/// </summary>
		/// <param name="budgetId">The Id of the budget in question</param>
		/// <returns>New Budget Transaction with a completed Automated Bank Charges Calculation</returns>
		private BudgetTransaction AddAutomated(string budgetId)
		{
			double fees = 0;
			foreach (Account acc in Collections.Accounts.Where(x => x.AccountType.Transactional))
			{
				fees = fees + acc.MonthlyFee;
				double debt = acc.AccountLimit - acc.Available;
				if (debt > 0)
				{
					fees = fees + debt * (acc.CreditRate / 12 / 100);
				}
			}
			fees = Math.Round(fees, 2);
			CFClassification classification = new CFClassification("Expense");
			CFType typeT = new CFType("Bank Charges");
			return new BudgetTransaction
			{
				BudgetId = budgetId,
				Automated = true,
				BudgetTransactionId = Guid.NewGuid().ToString(),
				CFClassificationId = classification.Id,
				CFClassification = classification,
				CFType = typeT,
				CFTypeId = typeT.Id,
				Name = "Automated Bank Charges",
				Amount = fees,
			};
		}
		public void Edit()
		{
			Account acc = new Account();
			Collections.Accounts = acc.GetAccountsSim(CollectionsId, SimulationId);
			for(int i = 0; i < this.SimulationAssumptions.NumberOfMonths;i++)
			{
				BudgetTransaction automated = Budgets[i].BudgetTransactions.Where(x=>x.Automated).FirstOrDefault();
				Budgets[i].BudgetTransactions.Remove(automated);
				Budgets[i].BudgetTransactions.Add(AddAutomated(Budgets[i].BudgetId));
				Budgets[i].AccountStates
								.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
								.OrderByDescending(x => x.Account.CreditRate)
								.FirstOrDefault()
								.Update(Budgets[i].BudgetTransactions.Sum(x => x.Amount * x.CFClassification.Sign));
			}
		}
		private void Scenario()
		{
			switch(SimulationAssumptions.Type)
			{
				case "Payment":
					Payment();
					break;
			}
		}
		private void Payment()
		{
			CFClassification cf = new CFClassification();
			List<CFClassification> cfList = cf.GetList();
			CFType type = new CFType();
			List<CFType> typeList = type.GetCFList(CollectionsId);
			if (SimulationAssumptions.Recurring)
			{
				for (int i = 0; i < SimulationAssumptions.NumberOfMonths; i++)
				{
					Budgets[i].BudgetTransactions.Add(new BudgetTransaction
					{
						Amount = SimulationAssumptions.Amount,
						Automated = true,
						BudgetId = Budgets[i].BudgetId,
						BudgetTransactionId = Guid.NewGuid().ToString(),
						CFClassificationId = cfList.Where(x=>x.Sign == 1).FirstOrDefault().Id,
						CFClassification = cfList.Where(x => x.Sign == 1).FirstOrDefault(),
						CFTypeId = "999",
						CFType = typeList.Where(x=>x.Id == "999").FirstOrDefault(),
						Name = "Recurring Payment",
					});
					Budgets[i].BudgetTransactions.Add(new BudgetTransaction
					{
						Amount = SimulationAssumptions.Amount,
						Automated = true,
						BudgetId = Budgets[i].BudgetId,
						BudgetTransactionId = Guid.NewGuid().ToString(),
						CFClassificationId = cfList.Where(x => x.Sign == 1).FirstOrDefault().Id,
						CFClassification = cfList.Where(x => x.Sign == -1).FirstOrDefault(),
						CFTypeId = "999",
						CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
						Name = "Recurring Payment",
					});
					Budgets[i].AccountStates
						.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
						.OrderByDescending(x => x.Account.CreditRate)
						.FirstOrDefault()
						.Update(-SimulationAssumptions.Amount);
					Budgets[i].AccountStates
						.Where(x => x.AccountId == SimulationAssumptions.AccountId)
						.FirstOrDefault()
						.Update(SimulationAssumptions.Amount);
				}
			}
			else
			{
				for(int i = 0; i < SimulationAssumptions.NumberOfMonths;i++)
				{
					if(SimulationAssumptions.ChangeDate == Budgets[i].StartDate)
					{
						Budgets[i].BudgetTransactions.Add(new BudgetTransaction
						{
							Amount = SimulationAssumptions.Amount,
							Automated = true,
							BudgetId = Budgets[i].BudgetId,
							BudgetTransactionId = Guid.NewGuid().ToString(),
							CFClassificationId = cfList.Where(x => x.Sign == 1).FirstOrDefault().Id,
							CFClassification = cfList.Where(x => x.Sign == 1).FirstOrDefault(),
							CFTypeId = "999",
							CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
							Name = "Recurring Payment",
						});
						Budgets[i].BudgetTransactions.Add(new BudgetTransaction
						{
							Amount = SimulationAssumptions.Amount,
							Automated = true,
							BudgetId = Budgets[i].BudgetId,
							BudgetTransactionId = Guid.NewGuid().ToString(),
							CFClassificationId = cfList.Where(x => x.Sign == 1).FirstOrDefault().Id,
							CFClassification = cfList.Where(x => x.Sign == -1).FirstOrDefault(),
							CFTypeId = "999",
							CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
							Name = "Recurring Payment",
						});
						Budgets[i].AccountStates
							.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
							.OrderByDescending(x => x.Account.CreditRate)
							.FirstOrDefault()
							.Update(-SimulationAssumptions.Amount);
						Budgets[i].AccountStates
							.Where(x => x.AccountId == SimulationAssumptions.AccountId)
							.FirstOrDefault()
							.Update(SimulationAssumptions.Amount);
					}
				}
			}
		}
		/// <summary>
		/// Looks to te database to source and find a particular Simulation via the Simulation Id
		/// </summary>
		/// <param name="SimulationId">Id of the simulatiion that is being requested</param>
		/// <returns>A Simulation for a given Id</returns>
		private Simulation GetSimulation(string SimulationId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
					.Simulation
					.Find(SimulationId);
			}
		}
	}
}
