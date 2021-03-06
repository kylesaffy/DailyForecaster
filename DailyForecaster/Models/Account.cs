﻿using DailyForecaster.Controllers;
using Microsoft.AspNetCore.Http.Connections;
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
		[Required]
		public string InstitutionId { get; set; } 		
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
		public bool isDeleted { get; set; }
		public DateTime LastUpdate { get; set; }
		public DateTime YodleeUpdate { get; set; }
		public Account() { }
		private List<Account> GetAccounts(string collectionId,bool ans)
		{
			if (ans)
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					return _context.Account
						.Where(x => x.CollectionsId == collectionId)
						.Where(x => x.isDeleted == false)
						.ToList();
				}
			}
			return null;
		}
		public void Delete()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				this.isDeleted = true;
				_context.Entry(this).State = EntityState.Modified;
				_context.SaveChanges();
			}
			AccountState state = new AccountState();
			state.Delete(this.Id);
		}
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
						.Where(x => x.CollectionsId == collecionsId && x.isDeleted == false)
						.Where(x => x.AccountType.Bank)
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
			Institution institution = new Institution();
			List<Institution> institutions = institution.GetInstitutions(); 
			foreach (Account item in accounts)
			{
				item.AccountType = types.Where(x => x.AccountTypeId == item.AccountTypeId).FirstOrDefault();
				item.Institution = institutions.Where(x => x.Id == item.InstitutionId).FirstOrDefault();
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
					.Where(x=>x.CollectionsId == collectionsIds && x.isDeleted == false)
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
		public async Task<List<Account>> GetYodleeAccounts(string providerId, string collectionsId)
		{
			YodleeAccountModel model = new YodleeAccountModel();
			List<YodleeAccountLevel> yodleeAccounts = await model.GetYodleeAccounts(collectionsId);
			List<YodleeAccountLevel> final = new List<YodleeAccountLevel>();
			List<Account> existing = GetAccounts(collectionsId);
			if (providerId == "null")
			{
				Institution institution = new Institution();
				List<string> ids = institution.GetProviders();
				foreach(YodleeAccountLevel item in yodleeAccounts)
				{
					if(!ids.Where(x=>x == item.providerId).Any() && !ids.Where(x=>x == item.providerAccountId.ToString()).Any() && !existing.Select(x => x.AccountIdentifier.Substring(4, 4)).Where(x=>x == item.accountNumber.Substring(item.accountNumber.Length-4,4)).Any())
					{
						final = yodleeAccounts.Where(x => x.providerId == item.providerId).ToList();
						break;
					}
				}
			}
			else
			{
				final = yodleeAccounts.Where(x => x.providerId == providerId).ToList();
			}
			List<Account> accounts = new List<Account>();
			foreach(YodleeAccountLevel item in final)
			{
				if(!existing.Where(x=>x.YodleeId == item.id).Any())	accounts.Add(CreateFromYodlee(item, collectionsId));
			}
			return accounts;
		}
		public Account CreateFromYodlee(YodleeAccountLevel yodleeAccount,string collectionsId)
		{
			Institution institution = new Institution();
			institution = institution.GetInstitution(Convert.ToInt64(yodleeAccount.providerId));
			YodleeAccountType type = new YodleeAccountType();
			type = type.Get(yodleeAccount.accountType, yodleeAccount.CONTAINER);
			if(type == null)
			{
				type = new YodleeAccountType()
				{
					AccountTypeId = "eYGGKtr5Xk6io9jj8tiTZA=="
				};
			}
			string InstId = "";
			if(institution != null)
			{
				InstId = institution.Id;
			}
			return new Account()
			{
				Id = Guid.NewGuid().ToString(),
				Name = yodleeAccount.accountName,
				InstitutionId = InstId,
				//Available = account.Available,
				//AccountLimit = account.AccountLimit,
				//NetAmount = account.NetAmount,
				//DebitRate = account.DebitRate,
				//CreditRate = account.CreditRate,
				//Floating = account.Floating,
				//FloatingType = account.FloatingType,
				//MonthlyFee = account.MonthlyFee,
				CollectionsId = collectionsId,
				AccountTypeId = type.AccountTypeId,
				AccountIdentifier = yodleeAccount.accountNumber.Substring(yodleeAccount.accountNumber.Length - 4, 4),
				YodleeId = yodleeAccount.id,
				//Maturity = account.Maturity,
				//MonthlyPayment = account.MonthlyPayment,
			};
		}
		public async Task<bool> UpdateAccounts(string collectionsId,List<Account> accounts)
		{
			YodleeAccountModel model = new YodleeAccountModel();
			List<YodleeAccountLevel> yodleeAccounts = await model.GetYodleeAccounts(collectionsId);
			if (yodleeAccounts != null)
			{
				try
				{
					using (FinPlannerContext _context = new FinPlannerContext())
					{

						foreach (Account item in accounts)
						{
							if (item.YodleeId == 0 && item.AccountIdentifier != null)
							{
								item.YodleeId = yodleeAccounts.Where(x => x.accountNumber.Substring(x.accountNumber.Length - 4, 4) == item.AccountIdentifier.Substring(item.AccountIdentifier.Length - 4, 4)).Select(x => x.id).FirstOrDefault();
							}
							else
							{
								YodleeAccountLevel accountLevel = yodleeAccounts.Where(x => x.accountNumber.Substring(x.accountNumber.Length - 4, 4) == item.AccountIdentifier.Substring(item.AccountIdentifier.Length - 4, 4)).FirstOrDefault();
								if (item.Institution.ProviderId == 0)
								{
									item.Institution.ProviderId = Convert.ToInt32(accountLevel.providerId);
									item.Institution.Update();
								}
								if (accountLevel != null)
								{
									item.YodleeUpdate = accountLevel.lastUpdated;
									if (item.AccountType.Bank)
									{
										if (accountLevel.availableBalance != null)
										{
											item.Available = accountLevel.availableBalance.amount;
										}
										else if (accountLevel.availableCredit != null)
										{
											item.Available = accountLevel.availableCredit.amount;
										}
										else if (accountLevel.balance != null)
										{
											item.Available = accountLevel.balance.amount;
										}
										else
										{
											item.Available = 0;
										}
									}
									else
									{
										if (accountLevel.rewardBalance != null)
										{
											if (accountLevel.rewardBalance.Where(x => x.uints == "ZAR").Any())
											{
												item.Available = accountLevel.rewardBalance.Where(x => x.uints == "ZAR").FirstOrDefault().balance;
											}
											else
											{
												item.Available = accountLevel.rewardBalance.FirstOrDefault().balance;
											}
										}
										else
										{
											if (accountLevel.availableBalance != null && accountLevel.availableBalance.amount != 0)
											{
												item.Available = -accountLevel.availableBalance.amount;
											}
											else if (accountLevel.availableCredit != null)
											{
												item.Available = -accountLevel.availableCredit.amount;
											}
											else
											{
												item.Available = -accountLevel.balance.amount;
											}
										}
									}
								}
								item.LastUpdate = DateTime.Now;
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
				catch (Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
					return false;
				}
			}
			else
			{
				return true;
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
		public bool ResetYodlee(string collectionsId)
		{
			try
			{
				List<Account> accounts = GetAccounts(collectionsId);
				foreach (Account item in accounts)
				{
					item.YodleeId = 0;
					item.SaveCahnges();
				}
				return true;
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e);
				return false;
			}
		}
		public async Task<List<Account>> AddYodleeAccount(string collectionsId)
		{
			YodleeAccountModel yodlee = new YodleeAccountModel();
			List<YodleeAccountLevel> yodleeAccounts = await yodlee.GetYodleeAccounts(collectionsId);
			List<Account> existingAccounts = GetAccountsEmpty(collectionsId);
			List<Account> newAccounts = new List<Account>();
			foreach(YodleeAccountLevel item in yodleeAccounts)
			{
				if(!existingAccounts.Where(x=>x.YodleeId == item.id).Any())
				{
					newAccounts.Add(NewAccount(item));
				}
			}
			return new List<Account>();
		}
		private Account NewAccount(YodleeAccountLevel yodlee)
		{
			Account account = new Account();
			account.YodleeId = yodlee.id;
			account.Id = Guid.NewGuid().ToString();
			return account;
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
					this.AccountType = null;
					_context.Account.Add(this);
				}
				else
				{
					Account account1 = _context.Account.Find(this.Id);
					try
					{
						account1.Update(this);
						_context.Entry(account1).State = EntityState.Modified;
					}
					catch
					{
						_context.Account.Add(this);
					}
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
					catcher.Catch(e);
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
		private void SaveCahnges()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Entry(this).State = EntityState.Modified;
				_context.SaveChanges();
			}
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
