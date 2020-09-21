using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class SplitTransactions
	{
		public string SplitTransactionsId { get; set; }
		public double Amount { get; set; }
		[ForeignKey("CFTYpeID")]
		public CFType CFType { get; set; }
		public string CFTYpeID { get; set; }
		public string Name { get; set; }
		public string AutomatedCashFlowId { get; set; }
		[ForeignKey("AutomatedCashFlowId")]
		public AutomatedCashFlow AutomatedCashFlow { get; set; }
		public SplitTransactions() { }
		/// <summary>
		/// Modifies the data appropriately, either by adding it to the DB or updating it on the DB 
		/// </summary>
		/// <returns>Updated object</returns>
		public SplitTransactions UpdateSplit()
		{
			if(this.SplitTransactionsId == null || this.SplitTransactionsId == "")
			{
				SplitTransactions split = new SplitTransactions()
				{
					SplitTransactionsId = Guid.NewGuid().ToString(),
					Amount = this.Amount,
					CFTYpeID = this.CFTYpeID,
					Name = this.Name,
					AutomatedCashFlowId = this.AutomatedCashFlowId
				};
				Add(split);
				return split;
			}
			else
			{
				Update();
				return this;
			}
		}
		/// <summary>
		/// Replace Autmoted Cash Flows with Transaction Splits
		/// </summary>
		/// <param name="flows">List of AutomateCashFlows that need to be investigated</param>
		/// <returns>List of Automated Cash flows with the splits injected</returns>
		public List<AutomatedCashFlow> GetTransactions (List<AutomatedCashFlow> flows)
		{
			List<string> Ids = flows.Select(x => x.ID).ToList();		 			
			List<SplitTransactions> splits = GetTransactions(Ids);
			foreach (string item in splits.Select(x=>x.AutomatedCashFlowId).Distinct())
			{
				AutomatedCashFlow flow = flows.Where(x => x.ID == item).FirstOrDefault();
				flows.Remove(flow);
				foreach(SplitTransactions transaction in splits)
				{
					flows.Add(new AutomatedCashFlow
					{
						AccountId = flow.AccountId,
						Amount = transaction.Amount,
						CFClassificationId = flow.CFClassificationId,
						CFTypeId = transaction.CFTYpeID,
						DateBooked = flow.DateBooked,
						DateCaptured = flow.DateCaptured,
						ID = transaction.SplitTransactionsId,
						SourceOfExpense = transaction.Name,
						Validated = flow.Validated,
						YodleeId = flow.YodleeId,
						Split = true,
						AutomatedCashFlowsId = flow.AutomatedCashFlowsId,
						EmbededAutomatedCashFlow = AutomatedCashFlow
					});
				}
			}
			return flows;
		}
		//===================================================================================================================
		//DLA
		//===================================================================================================================
		/// <summary>
		/// Returns split transations list from Automtated Cash Flows 
		/// </summary>
		/// <param name="Ids">List of Automated Cash Flow Ids</param>
		/// <returns>List of SplitTransaction Objects</returns>
		private List<SplitTransactions> GetTransactions(List<string> Ids)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
				   .SplitTransactions
				   .Where(auto => Ids.Contains(auto.AutomatedCashFlowId))
				   .ToList();
			}
		}
		/// <summary>
		/// Adds current object to the data base context
		/// </summary>
		/// <param name="split">Object to be added to the data base context</param>
		private void Add(SplitTransactions split)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Add(split);
				_context.SaveChanges();
			}
		}
		/// <summary>
		/// Updates current object in the data base context
		/// </summary>
		private void Update()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Entry(this).State = EntityState.Modified;
				_context.SaveChanges();
			}
		}
	}
}
