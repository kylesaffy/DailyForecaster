using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AccountState
	{
		public string AccountStateId { get; set; }
		public string BudgetId { get; set; }
		[ForeignKey("BudgetId")]
		public Budget Budget { get; set; }
		public string AccountId { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		public double Amount { get; set; }
		public AccountState() { }
		public void Delete(string AccountId)
		{
			List<AccountState> accounts = new List<AccountState>();
			accounts = GetAccounts(AccountId, true);
			foreach (AccountState item in accounts)
			{
				item.Delete();
			}
		}
		private List<AccountState> GetAccounts(string accountId, bool ans)
		{
			if (ans)
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					return _context.AccountState.Where(x => x.AccountId == accountId).ToList();
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
		/// <summary>
		/// Instantiates an account state object as of a first instance of a simulation build
		/// </summary>
		/// <param name="account">Account object that is being replicated</param>
		/// <param name="budgetId">Budget Id and therefore location of the state</param>
		public AccountState(Account account, string budgetId)
		{
			AccountStateId = Guid.NewGuid().ToString();
			BudgetId = budgetId;
			AccountId = account.Id;
			Account = account;
			if (account.Available > account.AccountLimit)
			{
				Amount = account.Available - account.AccountLimit;
			}
			else
			{
				if (account.AccountType.Transactional)
				{
					Amount = (account.AccountLimit - account.Available) * -1;
				}
				else
				{
					Amount = account.Available * -1;
				}
			}
		}
		/// <summary>
		/// Instantiates an account state object as of a secondary instance of a simulation build
		/// </summary>
		/// <param name="account">Account object that is being replicated</param>
		/// <param name="budgetId">Budget Id and therefore location of the state</param>
		/// <param name="state">The previous state of the account</param>
		public AccountState(Account account, string budgetId, AccountState state)
		{
			AccountStateId = Guid.NewGuid().ToString();
			BudgetId = budgetId;
			AccountId = account.Id;
			Account = account;
			Account.Institution = null;
			if (account.AccountType.Transactional || state.Amount > 0)
			{
				Amount = state.Amount;
			}
			else
			{
				Amount = state.Amount + account.MonthlyPayment - account.MonthlyFee + (state.Amount * (account.CreditRate/12/100));
			}
		}
		/// <summary>
		/// Increases or deceases the specified account by an inputed amount
		/// </summary>
		/// <param name="amount">Amount that the account needs to be incremented or decremented by</param>
		public void Update(double amount)
		{
			Amount = this.Amount + amount;
		}
	}
}
