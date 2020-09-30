using DailyForecaster.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
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
		public double Spread { get; set; }
		public double MonthlyFee { get; set; }
		public string CollectionsId { get; set; }
		[ForeignKey("CollectionsId")]
		public Collections Collections { get; set; }
		[ForeignKey("AccountType")]
		public string AccountTypeId { get; set; }
		public ICollection<ManualCashFlow> ManualCashFlows { get; set; }
		public ICollection<AutomatedCashFlow> AutomatedCashFlows { get; set; }
		public ICollection<AccountBalance> AccountBalances { get; set; }
		public AccountType AccountType { get; set; }
		public virtual List<ReportedTransaction> ReportedTransactions { get; set; }
		public string AccountIdentifier { get; set; }
		public int YodleeId { get; set; }
		public DateTime Maturity { get; set; }
		public AccountAmortisation AccountAmortisation { get; set; }
		public double MonthlyPayment { get; set; }
		public bool isDummy { get; set; }
		public string SimulationId { get; set; }
		[ForeignKey("SimulationId")]
		public Simulation Simulation { get; set; }
		public Account() { }
		/// <summary>
		/// Available amount on all acounts within a collection
		/// </summary>
		/// <param name="collecionsId">Unique id of collection</param>
		/// <returns>Returns double of the amount available within the collection of accounts</returns>
		public double GetAvaialable(string collecionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					return _context
						.Account
						.Where(x => x.CollectionsId == collecionsId)
						.Where(x => x.AccountType.Transactional)
						.Select(x => x.Available)
						.Sum();
				}
				catch
				{
					return 0;
				}
			}
		}
		public Account(string id)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				Account account = _context.Account.Find(id);
				Id = account.Id;
				Name = account.Name;
				InstitutionId = account.InstitutionId;
				Available = account.Available;
				AccountLimit = account.AccountLimit;
				NetAmount = account.NetAmount;
				DebitRate = account.DebitRate;
				CreditRate = account.CreditRate;
				Floating = account.Floating;
				FloatingType = account.FloatingType;
				MonthlyFee = account.MonthlyFee;
				CollectionsId = account.CollectionsId;
				AccountTypeId = account.AccountTypeId;
				AccountIdentifier = account.AccountIdentifier;
				YodleeId = account.YodleeId;
				Maturity = account.Maturity;
				MonthlyPayment = account.MonthlyPayment;
			}
		}
		public void GetTransactions()
		{
			ReportedTransaction reportedTransaction = new ReportedTransaction();
			this.ReportedTransactions = reportedTransaction.GetTransactions(this.Id);
		}
		public Account GetAccount(string id, bool transactions)
		{
			Account account = new Account();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					account = _context.Account.Find(id);
				}
				catch (Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
				}
			}
			if (transactions)
			{
				account.GetTransactions();
			}
			if(account.AccountType == AccountType.GetAccountTypes().Where(x=>x.Name == "Home Loan"))
			{
				account.Available = 0;
			}
			return account;
		}
		/// <summary>
		/// Returns a list of accounts associated with a list of collection
		/// </summary>
		/// <param name="collectionsIds">The list collections that is being requested</param>
		/// <param name="count">The number of transactions that is requested</param>
		/// <returns>A List of Account objects</returns>
		public List<Account> GetAccountIndex(List<string> collectionsIds,int count)
		{

			//get accounts
			List<Account> accounts = new List<Account>();
			//get accountTypes
			//AccountType type = new AccountType();
			//List<AccountType> accountTypes = type.GetAccountTypes();
			foreach (string item in collectionsIds)
			{
				accounts.AddRange(GetAccountsEmpty(item));
			}
			if (count > 0)
			{
				//get transactions
				ReportedTransaction reportedTransaction = new ReportedTransaction();
				List<ReportedTransaction> transactions = reportedTransaction.GetTransactions(accounts.Select(x => x.Id).ToList(), count, collectionsIds);
				//assign transactions
				foreach (Account item in accounts)
				{
					item.ReportedTransactions = transactions.Where(x => x.AccountId == item.Id).ToList();
					item.AccountType = AccountType.GetAccountTypes().Where(x => x.AccountTypeId == item.AccountTypeId).FirstOrDefault();
					item.AccountType.Accounts = null;
				}
			}
			return accounts;
		}
		public List<Account> GetAccounts(string collectionsId)
		{
			List<Account> accounts = new List<Account>();
			accounts = GetAccountsEmpty(collectionsId, false);
			List<AccountType> types = AccountType.GetAccountTypes();
			foreach (Account item in accounts)
			{
				item.AccountType = types.Where(x => x.AccountTypeId == item.AccountTypeId).FirstOrDefault();
			}
			return accounts;
		}
		/// <summary>
		/// New convention for retrieving Account data
		/// </summary>
		/// <param name="collectionsIds">The collection Id associated with the accounts</param>
		/// <param name="dummy">Are dummy accounts to be included</param>
		/// <returns>List of accounts associated with the collection Id</returns>
		private List<Account> GetAccountsEmpty(string collectionsIds, bool dummy = false)
		{		   			
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
					.Account
					.Where(x=>x.CollectionsId == collectionsIds)
					.Where(x => x.isDummy == dummy)
					.ToList();
			}
			
		}
		public List<Account> GetAccountsSim(string collectionsId,string simulationId)
		{
			//AccountType type = new AccountType();
			//List<AccountType> accountTypes = type.GetAccountTypes();
			List<Account> acc = new List<Account>();
			acc.AddRange(GetAccountsEmpty(collectionsId));
			acc.AddRange(GetAccountsEmpty(collectionsId, true).Where(x => x.SimulationId == simulationId));
			foreach(Account item in acc)
			{
				item.AccountType = AccountType.GetAccountTypes().Where(x => x.AccountTypeId == item.AccountTypeId).FirstOrDefault();
			}
			return acc;
		}
		public async Task<bool> UpdateAccounts(string collectionsId,List<Account> accounts)
		{
			YodleeAccountModel model = new YodleeAccountModel();
			List<YodleeAccountLevel> yodleeAccounts = await model.GetYodleeAccounts(collectionsId);
			try
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					foreach (Account item in accounts)
					{
						if (item.YodleeId == 0 && item.AccountIdentifier != null)
						{
							item.YodleeId = yodleeAccounts.Where(x => x.accountNumber == item.AccountIdentifier).Select(x => x.id).FirstOrDefault();
						}
						else
						{
							YodleeAccountLevel accountLevel = yodleeAccounts.Where(x => x.accountNumber == item.AccountIdentifier).FirstOrDefault();
							if (accountLevel != null)
							{
								if (accountLevel.availableBalance != null)
								{
									item.Available = accountLevel.availableBalance.amount;
								}
								else if(accountLevel.availableCredit != null)
								{
									item.Available = accountLevel.availableCredit.amount;
								}
								else
								{
									item.Available = accountLevel.balance.amount;
								}
							}
						}
						_context.Entry(item).State = EntityState.Modified;
						AccountBalance balance = new AccountBalance()
						{
							AccountBalanceId = Guid.NewGuid().ToString(),
							AccountId = item.Id,
							Amount = item.Available,
							Date = DateTime.Now.Date
						};
						_context.Add(balance);
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
		/// <summary>
		/// List of accounts associated to a single Collection
		/// </summary>
		/// <param name="collectionsId">The collection Id that is being requested</param>
		/// <param name="transactions">Are transactions to be included</param>
		/// <param name="email">email address of the user</param>
		/// <returns>Returns a list of accounts associated to the collection Id</returns>
		public List<Account> GetAccounts(string collectionsId,bool transactions = true,string email = "")
		{
			if((collectionsId == null || collectionsId == "" || collectionsId == "undefined") && email != "")
			{
				UserInteraction userInteraction = new UserInteraction();
				collectionsId = userInteraction.GetCollectionId(email);
			}
			else
			{
				if(email != "")
				{
					UserInteraction userInteraction = new UserInteraction();
					userInteraction.CollectionsIncratment(collectionsId, email);
				}
			}
			List<Account> accounts = GetAccountsEmpty(collectionsId);
			Institution institution = new Institution();
			List<Institution> institutions = institution.GetInstitutions();
			//AccountType type = new AccountType();
			//List<AccountType> types = type.GetAccountTypes();
			foreach(Account item in accounts)
			{
				item.Institution = institutions.Where(x => x.Id == item.InstitutionId).FirstOrDefault();
				item.AccountType = AccountType.GetAccountTypes().Where(x => x.AccountTypeId == item.AccountTypeId).FirstOrDefault();
				if (transactions)
				{
					item.GetTransactions();
				}
				item.AccountType.Accounts = null;
			}
			return accounts;
	}
		
		public List<Account> GetAccountsWithCF(string collectionsId, int count)
		{
			CFClassification classification = new CFClassification();
			List<CFClassification> classifications = classification.GetList();
			List<Account> accounts = GetAccountsEmpty(collectionsId);
			Institution institution = new Institution();
			List<Institution> institutions = institution.GetInstitutions();
			//AccountType type = new AccountType();
			//List<AccountType> types = type.GetAccountTypes();
			ManualCashFlow manual = new ManualCashFlow();
			AutomatedCashFlow automated = new AutomatedCashFlow();
			Collections collection = new Collections();
			foreach (Account item in accounts)
			{
				item.Institution = institutions
					.Where(x => x.Id == item.InstitutionId)
					.FirstOrDefault();
				item.AccountType = AccountType
					.GetAccountTypes()
					.Where(x => x.AccountTypeId == item.AccountTypeId)
					.FirstOrDefault();
				item.AccountType.Accounts = null;
				item.ManualCashFlows = manual.GetManualCashFlows(item.Id,count);
				item.AutomatedCashFlows = automated.GetAutomatedCashFlows(item.Id, count);
				foreach(ManualCashFlow flow in item.ManualCashFlows)
				{
					flow.CFClassification = classifications.Where(x => x.Id == flow.CFClassificationId).FirstOrDefault(); ;
					flow.CFClassification.ManualCashFlows = null;
				}
				foreach (AutomatedCashFlow flow in item.AutomatedCashFlows)
				{
					flow.CFClassification = classifications.Where(x => x.Id == flow.CFClassificationId).FirstOrDefault(); ;
					flow.CFClassification.ManualCashFlows = null;
				}
				item.Collections = collection.GetCollections(collectionsId);
				//foreach(ManualCashFlow flow in item.ManualCashFlows)
				//{
				//	flow.Account.ManualCashFlows = null;
				//}
			}
			return accounts;
		}
		public Account AddAccount()
		{
			ReturnModel model = new ReturnModel();
			this.AccountIdentifier = "xxxx" + this.AccountIdentifier;
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				if (this.Id == null)
				{
					this.Id = Guid.NewGuid().ToString();
					_context.Account.Add(this);
				}
				else
				{
					Account account1 = _context.Account.Find(this.Id);
					account1.Update(this);
					_context.Entry(account1).State = EntityState.Modified;
				}
				try
				{
					_context.SaveChanges();
					model.result = true;
				}
				catch (Exception e)
				{
					model.result = false;
					model.returnStr = e.Message;
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
				}
			}
			return this;
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
			return model;
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
			this.Maturity = account.Maturity;
			this.MonthlyPayment = account.MonthlyPayment;
			this.isDummy = account.isDummy;
			if (account.Floating)
			{
				RateInformation information = new RateInformation();
				this.Spread = information.GetSpread(account);
			}
			else
			{
				this.Spread = 0;
			}
			AccountType type = new AccountType();
			if(type.isAmortised(account.AccountTypeId) && account.Maturity != DateTime.MinValue && account.MonthlyPayment != 0)
			{
				AccountAmortisation amortisation = new AccountAmortisation();
				amortisation.Update(account);
			}
		}
		
	}
}
