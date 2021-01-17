using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class SimulationVM
	{
		public Simulation Simulation { get; set; }
		public List<SimulationAssessment> SimulationAssessments { get; set; }
		public SimulationVM() { }
		public SimulationVM(string id)
		{
			Simulation simulation = new Simulation();
			Simulation = simulation.Get(id);
			Budget budget = new Budget();
			BudgetTransaction transaction = new BudgetTransaction();
			Simulation.Budgets = budget.GetBudgetsBySim(id);
			foreach(Budget item in Simulation.Budgets)
			{
				item.BudgetTransactions = transaction.GetBudgetTransactions(item.BudgetId);
			}
			SimulationAssessments = new List<SimulationAssessment>();
			CFClassification classification = new CFClassification();
			List<CFClassification> classifications = classification.GetList();
			string income = classifications.Where(x => x.Sign == 1).FirstOrDefault().Id;
			string expense = classifications.Where(x => x.Sign == -1).FirstOrDefault().Id;
			AccountState state = new AccountState();
			List<AccountState> states = new List<AccountState>();
			// List<BudgetTransaction> transactions = new List<BudgetTransaction>();
			foreach (Budget item in Simulation.Budgets)
			{
				states = state.Get(item.BudgetId);
				SimulationAssessments.Add(new SimulationAssessment(item, income, expense,states));
			}
		}
	}
	public class SimulationAssessment
	{
		public string Key { get; set; }
		public string Date { get; set; }
		public string BudgetIncome { get; set; }
		public string BudgetExpense { get; set; }
		public string NetBudget { get; set; }
		public string NetWorth { get; set; }
		public SimulationAssessment() { }
		public SimulationAssessment(Budget budget, string income, string expense,List<AccountState> states)
		{
			double exp = budget.BudgetTransactions.Where(x => x.CFClassificationId == expense).Sum(x => x.Amount);
			double inc = budget.BudgetTransactions.Where(x => x.CFClassificationId == income).Sum(x => x.Amount);
			double net = inc - exp;
			Key = budget.BudgetId;
			Date = budget.StartDate.ToShortDateString();
			BudgetIncome = "R " + inc.ToString("N2");
			BudgetExpense = "R " + exp.ToString("N2");
			NetBudget = "R " + net.ToString("N2");
			NetWorth = "R " + states.Sum(x => x.Amount).ToString("N2");
		}
	}

}
