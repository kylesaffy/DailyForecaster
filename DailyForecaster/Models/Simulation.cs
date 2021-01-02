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
			foreach(BonusModel model in assumptions.BonusModels)
			{
				model.BonusModelId = Guid.NewGuid().ToString();
				model.SimulationAssumptionsId = assumptions.SimulationAssumptionsId;
			}
			foreach (IncreaseModel model in assumptions.IncreaseModels)
			{
				model.IncreaseModelId = Guid.NewGuid().ToString();
				model.SimulationAssumptionsId = assumptions.SimulationAssumptionsId;
			}
			SimulationAssumptions = assumptions;
			SimulationName = assumptions.SimualtionName;
			SimulationId = "temp";
			CollectionsId = collectionsId;
			SimulationId = Save();
			Collections = new Collections(collectionsId);
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
		public Simulation BuildSimulation(SimulationAssumptions assumptions, string userId, string collectionsId)
		{
			this.SimulationAssumptions = assumptions;
			this.Collections = new Collections(collectionsId);
			this.BuildSim(userId);
			this.Collections.Accounts = null;
			this.SimulationAssumptions.Simulation = null;
			return this;
		}
		/// <summary>
		/// Builds a Simulation from a basic set of Simualtion Assumptions
		/// </summary>
		/// <param name="userId">Id of the user that is requesting the build</param>
		private void BuildSim(string userId)
		{
			string budgetId = BuildBudgets();
			int count = 0;
			List<Account> accounts = GetAccounts();
			List<AccountState> states = new List<AccountState>();
			foreach(Budget item in this.Budgets)
			{
				states = SetAccountStates(item.BudgetId, count, states);
				item.BudgetTransactions = BuildBudgetTransactions(budgetId,item.StartDate.Month, item.BudgetId, userId,count, states);
				states = UpdateStates(states, accounts, item.BudgetTransactions.ToList());
				item.AccountStates = states;
				budgetId = item.BudgetId;
				count++;
			}
		}
		public void Recreate(string uid)
		{
			Budget budget = new Budget();
			this.Budgets = budget.GetBudgetsBySim(this.SimulationId);
			this.CollectionsId = this.Budgets.First().CollectionId;
			this.SimulationAssumptions = new SimulationAssumptions(this.SimulationAssumptionsId);
			FirebaseUser user = new FirebaseUser();
			string userId = user.GetUserId(uid);
			this.Edit(userId);
		}
		/// <summary>
		/// Recomputes the simulation
		/// </summary>
		private void Edit(string userId)
		{
			List<Account> accounts = GetAccounts();
			AccountState state = new AccountState();
			List<AccountState> oldStates = new List<AccountState>();
			BudgetTransaction transaction = new BudgetTransaction();
			CFType type = new CFType();
			List<CFType> types = type.GetCFList(this.CollectionsId);
			foreach(Budget item in this.Budgets)
			{
				List<AccountState> states = state.Get(item.BudgetId);
				if(oldStates.Count > 0)
				{
					foreach (AccountState s in states)
					{
						s.Amount = oldStates.Where(x => x.AccountId == s.AccountId).FirstOrDefault().Amount;
						s.Update(0);
					}
				}
				else
				{
					foreach(AccountState s in states)
					{
						s.Amount = state.GetAmount(accounts.Where(x=>x.Id == s.AccountId).FirstOrDefault());
						s.Update(0);
					}
				}
				item.BudgetTransactions = transaction.GetBudgetTransactions(item.BudgetId);
				int counter = 0;
				foreach(BudgetTransaction t in item.BudgetTransactions)
				{
					double amount = 0;
					if (t.CFTypeId == "bc86f797-2f81-4467-80c4-a6387099d0b0") amount = Math.Round(BuildTransaction(t, item.StartDate.Month, counter, states, types, item.BudgetId,userId ,accounts, t.LineId),2);
					else amount = Math.Round(BuildTransaction(t, 0, counter, states, types, item.BudgetId, userId, accounts, t.LineId), 2);
					t.Amount = amount;
					t.Save(userId);
					counter++;
				}
				states = UpdateStates(states, accounts, item.BudgetTransactions.ToList());
				item.AccountStates = states;
			}
		}
		/// <summary>
		/// Finds the correct account to update accordning to the cost of having an account as overdrawn
		/// </summary>
		/// <param name="states">The current set of account states</param>
		/// <param name="accounts">The set of Accounts</param>
		/// <param name="transactions">The current set of BudgetTransactions</param>
		/// <returns>Updated set of Account states</returns>
		private List<AccountState> UpdateStates(List<AccountState> states, List<Account> accounts, List<BudgetTransaction> transactions)
		{
			var costs = new List<Tuple<string, double>>();
			foreach(Account item in accounts.Where(x=>x.AccountType.Bank).Where(x=>x.CreditRate > 0))
			{
				double cost = AccountCost(item.CreditRate, item.AccountLimit, states.Where(x => x.AccountId == item.Id).Select(x => x.Amount).FirstOrDefault());
				costs.Add(Tuple.Create(item.Id, cost));
			}
			string maxId = costs.OrderByDescending(x => x.Item2).Select(x => x.Item1).FirstOrDefault();
			double expenses = transactions.Where(x => x.CFClassification.Sign == -1).Sum(x=>x.Amount);
			double income = transactions.Where(x => x.CFClassification.Sign == 1).Sum(x => x.Amount);
			states.Where(x => x.AccountId == maxId).FirstOrDefault().Update(income - expenses);
			return states;
		}
		/// <summary>
		/// Calcuates the interest portion of the acocunt costs
		/// </summary>
		/// <param name="rate">Prevailing Debit Rate on the account</param>
		/// <param name="limit">Limit on the account</param>
		/// <param name="accountAvailable">Amount available on the account</param>
		/// <returns>Double of the cost of the account attributalbe to the interest</returns>
		private double AccountCost(double rate, double limit , double accountAvailable)
		{
			double overdrawn = -1 * (accountAvailable - limit);
			return overdrawn*((rate/100)/12);
		}
		/// <summary>
		/// Creates the list of account states for the current budget
		/// </summary>
		/// <param name="budgetId">budgetId of the current budget</param>
		/// <param name="count">The current count in the for loop</param>
		/// <param name="statesOld">Previous account states</param>
		/// <returns>List of account states for the current budget</returns>
		private List<AccountState> SetAccountStates(string budgetId,int count,List<AccountState> statesOld)
		{
			List<Account> accounts = GetAccounts();
			List<AccountState> states = new List<AccountState>();
			foreach(Account item in accounts)
			{
				states.Add(AccountState(item, budgetId, count, statesOld.Where(x => x.AccountId == item.Id).FirstOrDefault()));
			}
			return states;
		}
		/// <summary>
		/// Creates the new account states objects
		/// </summary>
		/// <param name="account">Current account that is being assessed</param>
		/// <param name="budgetId">budgetId of the current budget</param>
		/// <param name="count">The current count in the for loop</param>
		/// <param name="state">old account state object</param>
		/// <returns>New/Updated account state object</returns>
		private AccountState AccountState(Account account,string budgetId, int count,AccountState state)
		{
			if(count > 0)
			{

				return new AccountState(account, budgetId,state);
			}
			else
			{
				return new AccountState(account, budgetId);
			}
		}
		/// <summary>
		/// Returns a list of accounts associated with the collection
		/// </summary>
		/// <returns></returns>
		private List<Account> GetAccounts()
		{
			Account account = new Account();
			return account.GetAccounts(this.CollectionsId);
		}
		/// <summary>
		/// Builds the Budget Transaction Elements of a budget
		/// </summary>
		/// <param name="budgetId">budgetId of the previous budget</param>
		/// <param name="month">date month integer of the current month</param>
		/// <param name="newBudgetId">budgetId of the new/current budget</param>
		/// <param name="userId">Id of the user requesting the simulation build</param>
		/// <returns>A list of budget transactions of the current budget</returns>
		private List<BudgetTransaction> BuildBudgetTransactions(string budgetId, int month, string newBudgetId, string userId,int counter, List<AccountState> states)
		{
			BudgetTransaction transaction = new BudgetTransaction();
			List<BudgetTransaction> transactions = transaction.GetBudgetTransactions(budgetId);
			List<BudgetTransaction> newTransactions = new List<BudgetTransaction>();
			List<Account> accounts = GetAccounts();
			CFType type = new CFType();
			List<CFType> types = type.GetCFList(this.CollectionsId);
			foreach(BudgetTransaction t in transactions.Where(x=>x.CFTypeId != "593ad400-f06a-4580-ab67-6ad287a89be9"))
			{
				double amount = BuildTransaction(t, month, counter, states, types, newBudgetId, userId, accounts, t.LineId);
				BudgetTransaction newT = (new BudgetTransaction()
				{
					Amount = Math.Round(amount, 2),
					CFTypeId = t.CFTypeId,
					CFClassificationId = t.CFClassificationId,
					AccountId = t.AccountId,
					Automated = t.Automated,
					BudgetId = newBudgetId,
					FirebaseUserId = userId,
					LineId = t.LineId,
					Name = t.Name,
				});
				newT.Save(userId);
				newTransactions.Add(newT);
			}
			newTransactions.Add(BankCharges(accounts, newBudgetId, states, userId));
			return newTransactions;
		}
		/// <summary>
		/// A single BudgetTransaction object for Bank Charges
		/// </summary>
		/// <param name="accounts">List of the accounts in the collection</param>
		/// <param name="budgetId">Id of the new budget Object</param>
		/// <param name="states">List of the current account states</param>
		/// <param name="userId">Id of the user requesting the simulation build</param>
		/// <returns>A saved </returns>
		private BudgetTransaction BankCharges(List<Account> accounts,string budgetId,List<AccountState> states,string userId)
		{
			double fees = 0;
			foreach (Account acc in accounts.Where(x => x.AccountType.Bank))
			{
				fees = fees + acc.MonthlyFee;
				double debt = -states.Where(x=>x.AccountId == acc.Id).Select(x=>x.Amount).FirstOrDefault();
				if (debt > 0)
				{
					fees = fees + debt * (acc.CreditRate / 12 / 100);
				}
			}
			fees = Math.Round(fees, 2);	
			CFClassification classification = new CFClassification("Expense");
			CFType type = new CFType("Bank Charges");
			BudgetTransaction transaction = new BudgetTransaction()
			{
				BudgetId = budgetId,
				Automated = true,
				CFClassificationId = classification.Id,
				CFTypeId = type.Id,
				Name = "Automated Bank Charges",
				Amount = fees,
			};
			transaction.Save(userId);
			return transaction;
		}
		private void NonBankFees(List<AccountState> states, List<Account> accounts)
		{
			foreach(AccountState state in states)
			{

			}
		}
		/// <summary>
		/// Builds the individual BudgetTransaction amount
		/// </summary>
		/// <param name="t">Previous BudgetTransaction object</param>
		/// <param name="month">month number of the current budget period</param>
		/// <param name="counter">counter of the number of transactions</param>
		/// <param name="states">List of account states</param>
		/// <param name="types">list of CFTypes</param>
		/// <param name="budgetId">Id of the new budget</param>
		/// <param name="userId">Id ofthe user that is requesting the simulation build</param>
		/// <returns>A single budget transaction amount</returns>
		private double BuildTransaction(BudgetTransaction t, int month, int counter,List<AccountState> states, List<CFType> types, string budgetId, string userId,List<Account> accounts, int lineId)
		{
			double amount = 0;
			switch (t.CFTypeId)
			{
				case "bc86f797-2f81-4467-80c4-a6387099d0b0": //Salary/Bonus
					amount = BuildSalary(t.Amount, month, counter, t.LineId) + BuildBonus(t.Amount, month, lineId);
					break;
				case "43383c91-51c2-4b14-a88c-96f28f9a01de": //Loan payment
					if (t.AccountId != null) amount = BuildLoanPayment(t.Amount, states.Where(x => x.AccountId == t.AccountId).FirstOrDefault(),accounts.Where(x=>x.Id == t.AccountId).FirstOrDefault());
					else amount = t.Amount;
					break;
				case "44d90f1f-3061-451b-9bec-2e81a1feec32": //Car payment
					if (t.AccountId != null) amount = BuildLoanPayment(t.Amount, states.Where(x => x.AccountId == t.AccountId).FirstOrDefault(), accounts.Where(x => x.Id == t.AccountId).FirstOrDefault());
					else amount = t.Amount;					
					break;
				case "a310a05f-fa7c-4a89-b8fb-7f6ab917dea4": //Home payment
					if (t.AccountId != null) amount = BuildLoanPayment(t.Amount, states.Where(x => x.AccountId == t.AccountId).FirstOrDefault(), accounts.Where(x => x.Id == t.AccountId).FirstOrDefault());
					else amount = t.Amount;
					break;
				default: // Everything else
					amount = BuildPayment(t.Amount, month, types.Where(x => x.Id == t.CFTypeId).Select(x => x.Infaltion).FirstOrDefault());					
					break;
			}
			return amount;
		}
		/// <summary>
		/// Ensures that a loan payment being made is not overpaid
		/// </summary>
		/// <param name="Amount">Amount that is expected to be paid to the loan</param>
		/// <param name="state">State of the account that is being paid</param>
		/// <param name="account">The account of that is being paid</param>
		/// <returns>Amount that is to be paid to the account</returns>
		private double BuildLoanPayment(double Amount, AccountState state, Account account)
		{
			double amount = 0;
			if (state != null)
			{
				if (Amount > Math.Abs(state.Amount))
				{
					amount = state.Amount + 1;
				}
				else
				{
					if (account == null)
					{
						amount = Amount;
					}
					else
					{
						double interest = Math.Abs(state.Amount * ((account.CreditRate / 100) / 12));
						amount = Amount - interest - account.MonthlyFee;
					}
				}
				state.Update(amount);
			}
			return Amount;
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
		private double BuildBonus(double Amount, int month, int lineId)
		{
			double amount = 0;
			if (this
				.SimulationAssumptions
				.BonusModels
				.Where(x => x.LineId == lineId)
				.Select(x => x.Bonus)
				.FirstOrDefault()
				&& this
				.SimulationAssumptions
				.BonusModels
				.Where(x => x.LineId == lineId)
				.Select(x => x.BonusMonth)
				.FirstOrDefault() == month
				)
			{
				amount = Amount * this
					.SimulationAssumptions
					.BonusModels
					.Where(x => x.LineId == lineId)
					.Select(x => x.BonusAmount)
					.FirstOrDefault();
			}
			return amount;
		}
		/// <summary>
		/// Build the salary amount
		/// </summary>
		/// <param name="Amount">Previous Salary</param>
		/// <param name="month">Current Month</param>
		/// <param name="counter">Simulation Month Counter</param>
		/// <returns>New Salary Amount</returns>
		private double BuildSalary(double Amount, int month, int counter, int lineId)
		{
			double amount = 0;
			if (this
				.SimulationAssumptions
				.BonusModels
				.Where(x=>x.LineId == lineId)
				.Select(x=>x.Bonus)
				.FirstOrDefault()
				)
			{
				if (counter > 0 && this
					.SimulationAssumptions
					.BonusModels.Where(x => x.LineId == lineId).
					Select(x => x.BonusMonth)
					.FirstOrDefault() + 1 == month
					)
				{
					double a = this
						.SimulationAssumptions
						.BonusModels
						.Where(x => x.LineId == lineId)
						.Select(x => x.BonusAmount)
						.FirstOrDefault();
					Amount = Amount/(1+a);
				}
			}
			if (this
				.SimulationAssumptions
				.IncreaseModels
				.Where(x => x.LineId == lineId)
				.Select(x => x.Increase)
				.FirstOrDefault())
			{
				if (month == this
					.SimulationAssumptions
					.IncreaseModels
					.Where(x => x.LineId == lineId)
					.Select(x => x.IncreaseMonth)
					.FirstOrDefault())
				{
					amount = Amount * (1 + this
						.SimulationAssumptions
						.IncreaseModels
						.Where(x => x.LineId == lineId)
						.Select(x => x.IncreasePercentage)
						.FirstOrDefault());
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
			int day = 0;
			if (day != 28) day = this.Collections.ResetDay;
			else day = new DateTime(budget.StartDate.Year, budget.StartDate.Month + 1, 1).AddDays(-1).Day;
			DateTime initiationDate = setDate(new DateTime(budget.StartDate.Year, budget.StartDate.Month, day));
			DateTime endingDate = setDate(budget.EndDate);
			this.Budgets = new List<Budget>();
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
					if (this.Collections.ResetDay == 28)
					{
						if (date.Month != 12) date = new DateTime(date.Year,date.Month + 1, 1).AddDays(-1);
						else new DateTime(date.Year + 1, 1, 1).AddDays(-1);
					}

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
						//if (SimulationAssumptions.Bonus && SimulationAssumptions.BonusMonth == date.AddMonths(i).Month)
						//{
						//	foreach(BudgetTransaction transaction in sim.BudgetTransactions.Where(x=>x.CFType.Name == "Salary"))
						//	{
						//		transaction.Amount = transaction.Amount * SimulationAssumptions.BonusAmount;
						//	}
						//}
						//// Strip Bonus Amount out if following Month
						//if (SimulationAssumptions.Bonus && SimulationAssumptions.BonusMonth == date.AddMonths(i+1).Month)
						//{
						//	foreach (BudgetTransaction transaction in sim.BudgetTransactions.Where(x => x.CFType.Name == "Salary"))
						//	{
						//		transaction.Amount = this.Budgets[i - 1].BudgetTransactions.Where(x=>x.Name == transaction.Name).Select(x=>x.Amount).FirstOrDefault();
						//	}
						//}
						////=======================================================================================================================================================
						//// Increase Handling
						////=======================================================================================================================================================
						//// Add Increase if correct month (only add apportionment of previous month if not bonus month)
						//if (SimulationAssumptions.Increase && SimulationAssumptions.IncreaseMonth == date.AddMonths(i).Month)
						//{
						//	// if Bonus month == Increase month then use previous month
						//	if (SimulationAssumptions.IncreaseMonth == SimulationAssumptions.BonusMonth)
						//	{
						//		foreach (BudgetTransaction transaction in sim.BudgetTransactions.Where(x => x.CFType.Name == "Salary"))
						//		{
						//			transaction.Amount = this
						//				.Budgets[i - 1]
						//				.BudgetTransactions.Where(x => x.Name == transaction.Name)
						//				.Select(x => x.Amount)
						//				.FirstOrDefault() 
						//				* (1 + SimulationAssumptions.IncreasePercentage);
						//		}
						//	}
						//	else
						//	{
						//		foreach (BudgetTransaction transaction in sim.BudgetTransactions.Where(x => x.CFType.Name == "Salary"))
						//		{
						//			transaction.Amount = transaction.Amount * (1+ SimulationAssumptions.IncreasePercentage);
						//		}
						//	}
						//}
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
			foreach (Account acc in Collections.Accounts.Where(x => x.AccountType.Bank))
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
		public void EditOld()
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
