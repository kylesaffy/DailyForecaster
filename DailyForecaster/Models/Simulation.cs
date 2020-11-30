using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
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
		public string Notes { get; set; }
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
		public Simulation Get(string id)
		{
			return GetSimulation(id);
		}
		public List<Simulation>	GetSimulations(string uid)
		{
			FirebaseUser user = new FirebaseUser(uid);
			Collections collection = new Collections();
			List<string> collections = collection.GetCollections(user.Email, "").Select(x => x.CollectionsId).ToList(); ;
			return GetSimulations(collections);
		}
		private List<Simulation> GetSimulations(List<string> collectionIds)
		{
			List<Simulation> simulations = new List<Simulation>();
			try
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					simulations = _context
						.Simulation
						.Where(col => collectionIds.Contains(col.CollectionsId))
						.ToList();
				}
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e);
			}
			if(simulations.Count > 0)
			{
				foreach (Simulation item in simulations)
				{
					item.Collections = new Collections();
					item.Collections = item.Collections.GetCollections(item.CollectionsId);
					Budget budget = new Budget();
					item.Budgets = budget.GetBudgets(item.CollectionsId,true);
					item.Budgets = item.Budgets.Where(x => x.SimulationId == item.SimulationId).ToList();
				}
			}
			return simulations;
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
				_context.SaveChanges();
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
		private void BuildSim()
		{
			string budgetId = BuildBudgets();
		}
		private List<BudgetTransaction> BuildBudgetTransactions(string budgetId, int month, string newBudgetId, string userId)
		{
			BudgetTransaction transaction = new BudgetTransaction();
			List<BudgetTransaction> transactions = transaction.GetBudgetTransactions(budgetId);
			List<BudgetTransaction> newTransactions = new List<BudgetTransaction>();
			CFType type = new CFType();
			List<CFType> types = type.GetCFList(this.CollectionsId);
			int count = 0;
			foreach(BudgetTransaction t in transactions)
			{
				switch(t.CFTypeId)
				{
					case "bc86f797-2f81-4467-80c4-a6387099d0b0":
						newTransactions.Add(new BudgetTransaction()
						{
							Amount = BuildSalary(t.Amount, month) + BuildBonus(t.Amount, month),
							BudgetId = newBudgetId,
							CFClassificationId = t.CFClassificationId,
							CFTypeId = t.CFTypeId,
							Name = t.Name
						});
						break;
					default:
						newTransactions.Add(new BudgetTransaction()
						{
							Amount = BuildPayment(t.Amount,month,types.Where(x=>x.Id == t.CFTypeId).Select(x=>x.Infaltion).FirstOrDefault()),
							BudgetId = newBudgetId,
							CFClassificationId = t.CFClassificationId,
							CFTypeId = t.CFTypeId,
							Name = t.Name
						});
						break;
				}
				newTransactions[count].Save(userId);
				count++;
			}
			return newTransactions;
		}
		/// <summary>
		/// Builds the new transaction amount
		/// </summary>
		/// <param name="Amount">Prevoius Amount</param>
		/// <param name="month">Current month</param>
		/// <param name="inflation">Is this an inflation based amount</param>
		/// <returns>New transaction amount</returns>
		private double BuildPayment(double Amount, int month, bool inflation)
		{
			double amount = 0;
			if (month == 1 && inflation) amount = Amount * (1 + 0.06);
			else amount = Amount;
			return amount;
		}
		/// <summary>
		/// Builds the Bonus Amount
		/// </summary>
		/// <param name="Amount">Previous Salary</param>
		/// <param name="month">Current Month</param>
		/// <returns>Bonus Amount if the bonus occurs</returns>
		private double BuildBonus(double Amount, int month)
		{
			double amount = 0;
			if (this.SimulationAssumptions.Bonus && this.SimulationAssumptions.BonusMonth == month) amount = Amount * this.SimulationAssumptions.BonusAmount;
			return amount;
		}
		/// <summary>
		/// Build the salary amount
		/// </summary>
		/// <param name="Amount">Previous Salary</param>
		/// <param name="month">Current Month</param>
		/// <returns>New Salary Amount</returns>
		private	double BuildSalary(double Amount, int month)
		{
			double amount = 0;
			if (this.SimulationAssumptions.Increase)
			{
				if (month == this.SimulationAssumptions.IncreaseMonth)
				{
					amount = Amount * (1 + this.SimulationAssumptions.IncreasePercentage);
				}
				else
				{
					amount = Amount;
				}
			}
			else
			{
				amount = Amount;
			}
			return amount;			
		}
		/// <summary>
		/// Build the budgets for the simulation
		/// </summary>
		private string BuildBudgets()
		{
			Budget budget = new Budget(this.CollectionsId);
			DateTime initiationDate = setDate(budget.StartDate);
			DateTime endingDate = setDate(budget.EndDate);
			for(int i = 0; i < this.SimulationAssumptions.NumberOfMonths; i ++)
			{
				this.Budgets.Add(new Budget(this.CollectionsId,initiationDate,endingDate,true,this.SimulationId));
				initiationDate = setDate(initiationDate);
				endingDate = setDate(endingDate);
			}
			return budget.BudgetId;
		}
		/// <summary>
		/// Set the date for the budgets
		/// </summary>
		/// <param name="date">pervious date</param>
		/// <returns>new date</returns>
		public DateTime setDate(DateTime date)
		{
			switch (this.Collections.DurationType.ToLower())
			{
				case "month":
					date = date.AddMonths(1);
					break;
				case "week":
					date = date.AddDays(7);
					break;
				default:
					break;
			}
			return date;
		}
		private void Build()
		{
			this.Collections = new Collections(this.CollectionsId);
			// Get base information
			Budget budget = Collections.Budgets.Where(x => x.SimulationBool == false).OrderByDescending(x => x.StartDate).FirstOrDefault();
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
						Budget sim = new Budget(CollectionsId, date.AddMonths(i), date.AddMonths(i + 1), true, this.SimulationId);
						sim.AccountStates = new List<AccountState>();
						// If first month, then pull BudgetTransactions from actual BudgetTransactions ELSE pull from previous month
						if (i == 0)
						{
							sim.BudgetTransactions = budget.BudgetTransactions.Where(x=>x.Automated == false).ToList();
							foreach(BudgetTransaction item in sim.BudgetTransactions)
							{
								item.BudgetTransactionId = null;
								item.BudgetId = sim.BudgetId;
								item.Save("");
								// item.CFClassification = cfList.Where(x => x.Id == item.CFClassificationId).FirstOrDefault();
								// item.CFType = typeList.Where(x => x.Id == item.CFTypeId).FirstOrDefault();
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
								item.BudgetTransactionId = null;
								item.CFClassification = null;
								item.CFType = null;
								item.BudgetId = sim.BudgetId;
								item.Save("");
								// item.CFType = typeList.Where(x => x.Id == item.CFTypeId).FirstOrDefault();
								// item.CFClassification = cfList.Where(x => x.Id == item.CFClassificationId).FirstOrDefault();
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
		public void Scenario()
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
					CashFlowRouting(SimulationAssumptions.Amount, i, typeList, cfList, "Recurring Payment");
					//Budgets[i].BudgetTransactions.Add(new BudgetTransaction
					//{
					//	Amount = SimulationAssumptions.Amount,
					//	Automated = true,
					//	BudgetId = Budgets[i].BudgetId,
					//	BudgetTransactionId = Guid.NewGuid().ToString(),
					//	CFClassificationId = cfList.Where(x=>x.Sign == 1).FirstOrDefault().Id,
					//	CFClassification = cfList.Where(x => x.Sign == 1).FirstOrDefault(),
					//	CFTypeId = "999",
					//	CFType = typeList.Where(x=>x.Id == "999").FirstOrDefault(),
					//	Name = "Recurring Payment",
					//});
					//Budgets[i].BudgetTransactions.Add(new BudgetTransaction
					//{
					//	Amount = SimulationAssumptions.Amount,
					//	Automated = true,
					//	BudgetId = Budgets[i].BudgetId,
					//	BudgetTransactionId = Guid.NewGuid().ToString(),
					//	CFClassificationId = cfList.Where(x => x.Sign == 1).FirstOrDefault().Id,
					//	CFClassification = cfList.Where(x => x.Sign == -1).FirstOrDefault(),
					//	CFTypeId = "999",
					//	CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
					//	Name = "Recurring Payment",
					//});
					//Budgets[i].AccountStates
					//	.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
					//	.OrderByDescending(x => x.Account.CreditRate)
					//	.FirstOrDefault()
					//	.Update(-SimulationAssumptions.Amount);
					//Budgets[i].AccountStates
					//	.Where(x => x.AccountId == SimulationAssumptions.AccountId)
					//	.FirstOrDefault()
					//	.Update(SimulationAssumptions.Amount);
				}
			}
			else
			{
				for(int i = 0; i < SimulationAssumptions.NumberOfMonths;i++)
				{
					if(SimulationAssumptions.ChangeDate == Budgets[i].StartDate)
					{
						//Budgets[i].BudgetTransactions.Add(new BudgetTransaction
						//{
						//	Amount = SimulationAssumptions.Amount,
						//	Automated = true,
						//	BudgetId = Budgets[i].BudgetId,
						//	BudgetTransactionId = Guid.NewGuid().ToString(),
						//	CFClassificationId = cfList.Where(x => x.Sign == 1).FirstOrDefault().Id,
						//	CFClassification = cfList.Where(x => x.Sign == 1).FirstOrDefault(),
						//	CFTypeId = "999",
						//	CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
						//	Name = "Once off Payment",
						//});
						//Budgets[i].BudgetTransactions.Add(new BudgetTransaction
						//{
						//	Amount = SimulationAssumptions.Amount,
						//	Automated = true,
						//	BudgetId = Budgets[i].BudgetId,
						//	BudgetTransactionId = Guid.NewGuid().ToString(),
						//	CFClassificationId = cfList.Where(x => x.Sign == -1).FirstOrDefault().Id,
						//	CFClassification = cfList.Where(x => x.Sign == -1).FirstOrDefault(),
						//	CFTypeId = "999",
						//	CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
						//	Name = "Once Off Payment",
						//});
						//Budgets[i].AccountStates
						//	.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
						//	.OrderByDescending(x => x.Account.CreditRate)
						//	.FirstOrDefault()
						//	.Update(-SimulationAssumptions.Amount);
						//Budgets[i].AccountStates
						//	.Where(x => x.AccountId == SimulationAssumptions.AccountId)
						//	.FirstOrDefault()
						//	.Update(SimulationAssumptions.Amount);
						CashFlowRouting(SimulationAssumptions.Amount, i,typeList,cfList,"Once off Payment");
					}
				}
			}
		}
		/// <summary>
		/// Smart cash flow modelling used to ensure that there are sufficient funds available
		/// </summary>
		/// <param name="amount">Amount that need to be moved</param>
		/// <param name="i">The count of the budget</param>
		/// <param name="typeList">List of CFType</param>
		/// <param name="cfList">List of CFClassification</param>
		/// <param name="transName">Name of the transaction</param>
		private void CashFlowRouting(double amount, int i,List<CFType> typeList, List<CFClassification> cfList, string transName)
		{
			if (SimulationAssumptions.Type == "Payment")
			{
				// Is the amount that needs to be updated available in the account that we are looking for
				if (amount < Budgets[i]
					.AccountStates
					.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
					.OrderByDescending(x => x.Account.CreditRate)
					.Select(x => x.Account.AccountLimit)
					.FirstOrDefault()
					+
					Budgets[i]
					.AccountStates
					.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
					.OrderByDescending(x => x.Account.CreditRate)
					.Select(x => x.Amount)
					.FirstOrDefault())
				{
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
						Name = transName,
					});
					Budgets[i].BudgetTransactions.Add(new BudgetTransaction
					{
						Amount = SimulationAssumptions.Amount,
						Automated = true,
						BudgetId = Budgets[i].BudgetId,
						BudgetTransactionId = Guid.NewGuid().ToString(),
						CFClassificationId = cfList.Where(x => x.Sign == -1).FirstOrDefault().Id,
						CFClassification = cfList.Where(x => x.Sign == -1).FirstOrDefault(),
						CFTypeId = "999",
						CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
						Name = transName,
					});
					Budgets[i]
						.AccountStates
						.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
						.OrderByDescending(x => x.Account.CreditRate)
						.FirstOrDefault()
						.Update(-SimulationAssumptions.Amount);
					Budgets[i].AccountStates
						.Where(x => x.AccountId == SimulationAssumptions.AccountId)
						.FirstOrDefault()
						.Update(SimulationAssumptions.Amount);
				}
				else
				{
					double buildUp = 0;
					foreach(AccountState account in Budgets[i].AccountStates
						.Where(x => x.Amount < 0 && x.Account.AccountType.Transactional)
						.OrderByDescending(x => x.Account.CreditRate))
					{
						if(buildUp < amount)
						{
							double available = account
								.Account
								.AccountLimit
								+
								account.Amount;
							if(available + buildUp <= amount)
							{
								account.Update(-available);
								buildUp = buildUp + available;
							}
							else
							{
								double required = amount - buildUp;
								account.Update(-required);
								break;
							}
						}
						else
						{
							break;
						}
					}
					Notes note = new Notes()
					{
						NotesId = "123"
					};
					if (buildUp < amount)
					{
						note.Body = "The full set of funds required were not available, therefore we allocated the maximum amount available R" + buildUp.ToString("N2");
						note.NotesId = Guid.NewGuid().ToString();
					}  					
					Budgets[i].AccountStates
						.Where(x => x.AccountId == SimulationAssumptions.AccountId)
						.FirstOrDefault()
						.Update(buildUp);
					Budgets[i].BudgetTransactions.Add(new BudgetTransaction
					{
						Amount = buildUp,
						Automated = true,
						BudgetId = Budgets[i].BudgetId,
						BudgetTransactionId = Guid.NewGuid().ToString(),
						CFClassificationId = cfList.Where(x => x.Sign == 1).FirstOrDefault().Id,
						CFClassification = cfList.Where(x => x.Sign == 1).FirstOrDefault(),
						CFTypeId = "999",
						CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
						Name = transName,
					});
					string btId = Guid.NewGuid().ToString();
					Budgets[i].BudgetTransactions.Add(new BudgetTransaction
					{
						Amount = buildUp,
						Automated = true,
						BudgetId = Budgets[i].BudgetId,
						BudgetTransactionId = btId,
						CFClassificationId = cfList.Where(x => x.Sign == -1).FirstOrDefault().Id,
						CFClassification = cfList.Where(x => x.Sign == -1).FirstOrDefault(),
						CFTypeId = "999",
						CFType = typeList.Where(x => x.Id == "999").FirstOrDefault(),
						Name = transName,
						Notes = new List<Notes>()
					});
					if(note.NotesId != "123")
					{
						note.BudgetTransactionId = btId;
						Budgets[i].BudgetTransactions.Where(x => x.BudgetTransactionId == btId).First().Notes.Add(note);
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
