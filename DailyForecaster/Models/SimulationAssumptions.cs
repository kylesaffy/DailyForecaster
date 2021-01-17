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
		public List<BonusModel> BonusModels { get; set; }
		public List<IncreaseModel> IncreaseModels { get; set; }
		public SimulationAssumptions() { }
		/// <summary>
		/// Return a specific instance of the object
		/// </summary>
		/// <param name="Id">Id of the instance</param>
		public SimulationAssumptions(string Id)
		{

			SimulationAssumptions a = Get(Id);
			SimulationAssumptionsId = a.SimulationAssumptionsId;
			NumberOfMonths = a.NumberOfMonths;
			Recurring = a.Recurring;
			ChangeDate = a.ChangeDate;
			if(a.CFClassificationId != null) CFClassification = new CFClassification(a.CFClassificationId);
			CFClassificationId = a.CFClassificationId;
			if(a.CFTypeId != null) CFType = new CFType(a.CFTypeId);
			CFTypeId = a.CFTypeId;
			Type = a.Type;
			SimualtionName = a.SimualtionName;
			BonusModels = a.BonusModels;
			IncreaseModels = a.IncreaseModels;
		}
		/// <summary>
		/// Builds a get object for Simulation Assumptions
		/// </summary>
		/// <param name="Id">Id of the simulation assumption model needed</param>
		/// <returns>A single Simulation Assupmtion object</returns>
		private SimulationAssumptions Get(string Id)
		{
			SimulationAssumptions assumptions = new SimulationAssumptions();
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				assumptions = _context.SimulationAssumptions.Find(Id);
			}
			BonusModel bonus = new BonusModel();
			IncreaseModel increase = new IncreaseModel();
			assumptions.BonusModels = bonus.Get(Id);
			assumptions.IncreaseModels = increase.Get(Id);
			return assumptions;
		}
	}
	public class SimulationAssumptionsModel
	{
		public SimulationAssumptions SimulationAssumptions { get; set; }
		public List<BudgetTransaction> BudgetTransactions { get; set; }
	}
	public class SimulationAssumptionView
	{
		public SimulationAssumptions SimulationAssumptions { get; set; }
		public List<Collections> Collections { get; set; }
		public List<PaymentLink> PaymentLinks { get; set; }
		public List<BudgetTransaction> Salary { get; set; }
		public SimulationAssumptionView() { }
		public SimulationAssumptionView(string uid)
		{
			Collections collections = new Collections();
			FirebaseUser user = new FirebaseUser(uid);
			Collections = collections.GetCollections(user.Email, "Simulations");
			Account account = new Account();
			PaymentLink link = new PaymentLink();
			Budget budget = new Budget();
			BudgetTransaction transaction = new BudgetTransaction();
			foreach (Collections item in Collections)
			{
				item.Accounts = account.GetAccounts(item.CollectionsId).Where(x=>x.AccountType.Amortised).ToList();
				foreach(Account acc in item.Accounts)
				{
					link = new PaymentLink();
					link = link.GetByAccountId(acc.Id);
					if (link != null)
					{
						PaymentLinks.Add(link);
					}
				}
				budget = budget.GetBudget(item.CollectionsId);
				budget.BudgetTransactions = transaction.GetBudgetTransactions(budget.BudgetId).Where(x => (x.CFTypeId == "bc86f797-2f81-4467-80c4-a6387099d0b0" && x.CFClassificationId == "d1b1528a-b753-4bf3-bec5-3d9acd8c8b4f") || ((x.CFTypeId == "43383c91-51c2-4b14-a88c-96f28f9a01de" || x.CFTypeId == "44d90f1f-3061-451b-9bec-2e81a1feec32" || x.CFTypeId == "a310a05f-fa7c-4a89-b8fb-7f6ab917dea4") && x.CFClassificationId == "2e2db03d-55f2-43dc-b950-1eff6077cf19")).ToList();
				item.Budgets = new List<Budget>();
				item.Budgets.Add(budget);
			}
			SimulationAssumptions = new SimulationAssumptions();
		}
	}
}
