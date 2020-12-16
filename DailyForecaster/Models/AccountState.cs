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
		/// <summary>
		/// Gets a list of accountState objects associated to a budget
		/// </summary>
		/// <param name="budgetId">Id of the budget that the accountstates are linked to</param>
		/// <returns>List of accountStates for a particular budget</returns>
		public List<AccountState> Get(string budgetId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AccountState.Where(x => x.BudgetId == budgetId).ToList();
			}
		}
		/// <summary>
		/// Get AccountState object using budgetId and AccountId
		/// </summary>
		/// <param name="accountId">Id of the Associated Account</param>
		/// <param name="budgetId">Id of the Associated Budget</param>
		/// <returns>A Single AccountState object according to the account and budget</returns>
		public AccountState Get(string accountId, string budgetId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AccountState.Where(x => x.BudgetId == budgetId && x.AccountId == accountId).FirstOrDefault();
			}
		}
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
			BudgetId = budgetId;
			AccountId = account.Id;
			Account = account;
			if (account.Available > account.AccountLimit && account.AccountType.Transactional && account.AccountType.Bank)
			{
				Amount = account.Available - account.AccountLimit;
			}
			else
			{
				if (account.AccountType.Transactional && account.AccountType.Bank)
				{
					Amount = Math.Abs(account.AccountLimit - account.Available) * -1;
				}
				else
				{
					if (account.AccountLimit != 0)
					{
						Amount = Math.Abs(account.Available) * -1;
					}
					else
					{
						Amount = account.Available;
					}
				}
			}
			Save();
		}
		/// <summary>
		/// Instantiates an account state object as of a secondary instance of a simulation build
		/// </summary>
		/// <param name="account">Account object that is being replicated</param>
		/// <param name="budgetId">Budget Id and therefore location of the state</param>
		/// <param name="state">The previous state of the account</param>
		public AccountState(Account account, string budgetId, AccountState state)
		{
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
			Save();
		}
		/// <summary>
		/// Increases or deceases the specified account by an inputed amount
		/// </summary>
		/// <param name="amount">Amount that the account needs to be incremented or decremented by</param>
		public void Update(double amount)
		{
			Amount = Math.Round(this.Amount + amount,2);
			Save();
		}
		private void Save()
		{
			this.Account = null;
			this.Budget = null;
			this.Amount = Math.Round(this.Amount, 2);
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				if(this.AccountStateId == null)
				{
					this.AccountStateId = Guid.NewGuid().ToString();
					_context.Add(this);
				}
				else
				{
					_context.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
				}
				_context.SaveChanges();
			}
		}
	}
}
