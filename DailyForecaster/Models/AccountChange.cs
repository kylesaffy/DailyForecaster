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
		public string AccountId { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		public void Delete(string AccountId)
		{
			List<AccountChange> accounts = new List<AccountChange>();
			accounts = GetAccounts(AccountId, true);
			foreach (AccountChange item in accounts)
			{
				item.Delete();
			}
		}
		private List<AccountChange> GetAccounts(string accountId, bool ans)
		{
			if (ans)
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					return _context.AccountChange.Where(x => x.AccountId == accountId).ToList();
				}
			}
			return null;
		}
		private void Delete()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Remove(this);
				_context.SaveChanges();

			}
		}
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
		public AccountChange AddAccountChange(AutomatedCashFlow flow, Account account,int sign)
		{
			AccountChange change = new AccountChange();
			change.AccountChangeId = Guid.NewGuid().ToString();			
			change.UpdatedBalance = Math.Round(account.Available + (flow.Amount * sign), 2);
			account.Available = change.UpdatedBalance;
			change.Account = account;
			change.DateAdded = DateTime.Now;
			change.AutomatedCashFlowId = flow.ID;
			change.AccountId = account.Id;
			return change;
		}
	}
}
