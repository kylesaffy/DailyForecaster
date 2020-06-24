using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AccountChange
	{
		public string AccountChangeId { get; set; }
		public double UpdatedBalance { get; set; }
		public DateTime DateAdded { get; set; }
		public string ManualCashFlowId { get; set; }
		[ForeignKey("ManualCashFlowId")]
		public ManualCashFlow ManualCashFlow { get; set; }
		public string AutomatedCashFlowId { get; set; }
		[ForeignKey("AutomatedCashFlowId")]
		public AutomatedCashFlow AutomatedCashFlow { get; set; }
		public void AddAccountChange(ManualCashFlow flow)
		{
			AccountChange change = new AccountChange();
			change.AccountChangeId = Guid.NewGuid().ToString();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				Account account = _context.Account.Find(flow.AccountId);
				CFClassification classification = _context.CFClassifications.Find(flow.CFClassificationId);
				change.UpdatedBalance = Math.Round(account.Available + (flow.Amount * classification.Sign),2);
				account.Available = change.UpdatedBalance;
				change.DateAdded = DateTime.Now;
				change.ManualCashFlowId = flow.Id;
				_context.Entry(account).State = EntityState.Modified;
				_context.AccountChange.Add(change);
				_context.SaveChanges();
			}
		}
	}
}
