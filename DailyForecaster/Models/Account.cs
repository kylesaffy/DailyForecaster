using DailyForecaster.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class Account
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public string Name { get; set; }
		[ForeignKey("InstitutionId")]
		public string InstitutionId { get; set; } 		
		[Required]
		public Institution Institution { get; set; }
		public double Available { get; set; }
		public double AccountLimit {get;set;}
		public double NetAmount { get; set; }
		public double DebitRate { get; set; }
		public double CreditRate { get; set; }
		public bool Floating { get; set; }
		public string FloatingType { get; set; }
		public double MonthlyFee { get; set; }
		public string CollectionsId { get; set; }
		[ForeignKey("CollectionsId")]
		public Collections Collections { get; set; }
		[ForeignKey("AccountType")]
		public string AccountTypeId { get; set; }
		public ICollection<ManualCashFlow> ManualCashFlows { get; set; }
		public ICollection<AutomatedCashFlow> AutomatedCashFlows { get; set; }
		public AccountType AccountType { get; set; }
		public virtual List<ReportedTransaction> ReportedTransactions { get; set; }
		public string AccountIdentifier { get; set; }
		public int YodleeId { get; set; }
		public void GetTransactions()
		{
			ReportedTransaction reportedTransaction = new ReportedTransaction();
			this.ReportedTransactions = reportedTransaction.GetTransactions(this.Id);
		}
		public Account GetAccount(string id)
		{
			Account account = new Account();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				account = _context.Account.Find(id);
			}
			account.GetTransactions();
			return account;
		}
		public async Task<bool> UpdateAccounts(string collectionsId,List<Account> accounts)
		{
			YodleeAccountModel model = new YodleeAccountModel();
			List<YodleeAccountLevel> yodleeAccounts = await model.GetYodleeAccounts(collectionsId);
			try
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					foreach (Account item in accounts.Where(x=>x.YodleeId == 0 && x.AccountIdentifier != null))
					{
							item.YodleeId = yodleeAccounts.Where(x => x.accountNumber == item.AccountIdentifier).Select(x => x.id).FirstOrDefault();
							_context.Entry(item).State = EntityState.Modified;
					}
					_context.SaveChanges();	
				}
				return true;
			}
			catch(Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e.Message);
				return false;
			}
		}
		public List<Account> GetAccounts(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				List<Account> accounts = _context.Account.Where(x => x.CollectionsId == collectionsId).ToList();
				foreach(Account item in accounts)
				{
					item.Institution = _context.Institution.Find(item.InstitutionId);
					item.AccountType = _context.AccountType.Find(item.AccountTypeId);
					item.GetTransactions();
					item.AccountType.Accounts = null;
				}
				return accounts;
			}
		}
		public List<Account> GetAccountsWithCF(string collectionsId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				List<Account> accounts = _context.Account.Where(x => x.CollectionsId == collectionsId).ToList();
				foreach (Account item in accounts)
				{
					item.Institution = _context.Institution.Find(item.InstitutionId);
					item.AccountType = _context.AccountType.Find(item.AccountTypeId);
					item.AccountType.Accounts = null;
					item.ManualCashFlows = _context.ManualCashFlows.Where(x => x.AccountId == item.Id).OrderByDescending(x=>x.DateBooked).Take(10).ToList();
					item.AutomatedCashFlows = _context.AutomatedCashFlows.Where(x => x.AccountId == item.Id).OrderByDescending(x=>x.DateBooked).Take(30).ToList();
					foreach(ManualCashFlow flow in item.ManualCashFlows)
					{
						flow.CFClassification = _context.CFClassifications.Find(flow.CFClassificationId);
						flow.CFClassification.ManualCashFlows = null;
					}
					item.Collections = _context.Collections.Find(item.CollectionsId);
					//foreach(ManualCashFlow flow in item.ManualCashFlows)
					//{
					//	flow.Account.ManualCashFlows = null;
					//}
				}
				return accounts;
			}
		}
		public ReturnModel AddAccount(Account account)
		{
			ReturnModel model = new ReturnModel();
			account.AccountIdentifier = "xxxx" + account.AccountIdentifier;
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				if (account.Id == null)
				{
					account.Id = Guid.NewGuid().ToString();
					_context.Account.Add(account);
				}
				else
				{
					Account account1 = _context.Account.Find(account.Id);
					account1.Update(account);
					_context.Entry(account1).State = EntityState.Modified;
				}
				try
				{
					_context.SaveChanges();
					model.result = true;
				}
				catch(Exception e)
				{
					model.result = false;
					model.returnStr = e.Message;
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
				}
			}
			return model;;
		}
		private void Update(Account account)
		{
			this.AccountLimit = account.AccountLimit;
			this.AccountTypeId = account.AccountTypeId;
			this.Available = account.Available;
			this.CreditRate = account.CreditRate;
			this.DebitRate = account.DebitRate;
			this.Floating = account.Floating;
			this.FloatingType = account.FloatingType;
			this.InstitutionId = account.InstitutionId;
			this.MonthlyFee = account.MonthlyFee;
			this.Name = account.Name;
			this.NetAmount = account.NetAmount;
			this.AccountIdentifier = account.AccountIdentifier;
		}
	}
}
