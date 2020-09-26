using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using DailyForecaster.Controllers;
using DailyForecaster.Migrations;
using System.Runtime.InteropServices.WindowsRuntime;

namespace DailyForecaster.Models
{
	/// <summary>
	/// Accounts for transactions that are captured from an automated source
	/// </summary>
	public class AutomatedCashFlow
	{
		
		[Required]
		public string ID { get; set; }
		[Required]
		public string CFTypeId { get; set; }
		[ForeignKey("CFTypeId")]
		public CFType CFType { get; set; }
		[Required]
		public string CFClassificationId { get; set; }
		[ForeignKey("CFClassificationId")]
		public CFClassification CFClassification { get; set; }
		[Required]
		public double Amount { get; set; }
		public DateTime DateBooked { get; set; }
		[Required]
		public DateTime DateCaptured { get; set; }
		[Required]
		public string SourceOfExpense { get; set; }
		public int YodleeId { get; set; }
		public bool Validated { get; set; }
		public bool Split { get; set; }
		public string AutomatedCashFlowsId { get; set; }
		[ForeignKey("AutomatedCashFlowsId")]
		public AutomatedCashFlow EmbededAutomatedCashFlow { get; set; }
		public ManualCashFlow ManualCashFlow { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		[Required]
		public string AccountId { get; set; }
		ICollection<ReportedTransaction> ReportedTransactions { get; set; }
		public AutomatedCashFlow()
		{ }		
		/// <summary>
		/// Saves and returns updated object
		/// </summary>
		/// <param name="flow">Object to be saved and updated</param>
		/// <returns>Updated version of the object that was being saved</returns>
		public AutomatedCashFlow Save(AutomatedCashFlow flow)
		{
			if(flow.ID != null)
			{
				flow.Save();
				return Get(flow.ID);
			}
			return null;
		}
		/// <summary>
		/// Retrieve object with specified Id
		/// </summary>
		/// <param name="Id">Id of the object needed</param>
		/// <returns>Single object with ID specified</returns>
		private AutomatedCashFlow Get(string Id)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AutomatedCashFlows.Find(Id);
			}
		}	
		/// <summary>
		/// Save this Object
		/// </summary>
		private void Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Entry(this).State = EntityState.Modified;
				_context.SaveChanges();
			}
		}
		/// <summary>
		/// Returns a list of the most recent transactions on a specified account of a designated count
		/// </summary>
		/// <param name="AccId">The account with which the transactions are associated</param>
		/// <param name="count">The count of the transactions needed, defaulted to 10</param>
		/// <returns></returns>
		public List<AutomatedCashFlow> GetAutomatedCashFlows(string AccId, int count = 10)
		{
			using (FinPlannerContext _context = new FinPlannerContext()) {
				SplitTransactions split = new SplitTransactions();
				return split.GetTransactions(_context
					.AutomatedCashFlows
					.Where(x => x.AccountId == AccId)
					.OrderByDescending(x=>x.DateBooked)
					.Take(count)
					.ToList());
			}
		}
		/// <summary>
		/// returns a list of transactions within a group of collections for a specified date
		/// </summary>
		/// <param name="collectionsId">list of collection ids</param>
		/// <param name="date">date that the transactions we captured on</param>
		/// <returns>List of Trasnasctions within a group of collections captured on a specified date</returns>
		public List<AutomatedCashFlow> Get(List<string> collectionsId,DateTime date)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
						.AutomatedCashFlows
						.Where(flows => collectionsId.Contains(flows.Account.CollectionsId))
						.Where(x=>x.DateCaptured.Date == date.Date)
						.ToList();
			}
		}
		/// <summary>
		/// Returns all of the associated automated cash flows for a collection
		/// </summary>
		/// <param name="collectionsId">Id of the collection being queried</param>
		/// <param name="budget">budget defining the period</param>
		/// <returns>double of the non transfer sum of the transactions for the month</returns>
		public double GetSpent(string collectionsId, Budget budget)
		{
			DateTime startDate = budget.StartDate.AddDays(-3);
			DateTime endDate = budget.EndDate.AddDays(3);
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				double amount = _context
					.AutomatedCashFlows
					.Where(x => x.Account.CollectionsId == collectionsId && x.DateBooked > startDate &&  x.DateBooked < endDate && x.CFClassification.Sign == -1)
					.Where(x => x.CFTypeId != "999")
					.Sum(x=>x.Amount);
				return amount;
			}
		}
		public List<AutomatedCashFlow> GetAutomatedCashFlowsUnseen(List<string> accountsStr)
		{	  			
			using (FinPlannerContext _context = new FinPlannerContext())
			{	  				
				List<AutomatedCashFlow> cfs = _context
					.AutomatedCashFlows
					.Where(auto => accountsStr.Contains(auto.AccountId))
					.Where(x=>x.Validated==false)
					.ToList();	 				
				return cfs;
			}	  			
		}
		public void TransactionClassifier()
		{
			Collections collection = new Collections();
			CFClassification classification = new CFClassification();
			CFType type = new CFType();
			List<Collections> collections = collection.GetCollections("", "");
			List<CFClassification> classifications = classification.GetList();
			List<AutomatedCashFlow> collectionCF = new List<AutomatedCashFlow>();
			List<AutomatedCashFlow> Uncategorized = new List<AutomatedCashFlow>();
			type = type.GetUncategorized();
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				foreach (Collections item in collections.Where(x => x.Accounts.Any()))
				{
					collectionCF = new List<AutomatedCashFlow>();
					Uncategorized = new List<AutomatedCashFlow>();
					foreach (Account acc in item.Accounts)
					{
						collectionCF.AddRange(_context.AutomatedCashFlows.Where(x => x.Validated && x.CFTypeId != type.Id && x.AccountId == acc.Id).ToList());
						Uncategorized.AddRange(_context.AutomatedCashFlows.Where(x => x.Validated == false && x.CFTypeId == type.Id && x.AccountId == acc.Id).ToList());
					}
					foreach (AutomatedCashFlow cashFlow in Uncategorized)
					{
						//Find Exact Matches
						if (collectionCF.Where(x => x.Validated == true && x.SourceOfExpense == cashFlow.SourceOfExpense).Any())
						{
							cashFlow.CFTypeId = collectionCF.Where(x => x.Validated == true && x.SourceOfExpense == cashFlow.SourceOfExpense).Select(x => x.CFTypeId).FirstOrDefault();
							_context.Entry(cashFlow).State = EntityState.Modified;
						}
					}
				}
				_context.SaveChanges();
			}
		}
		public List<AutomatedCashFlow> GetAutomatedCashFlows(string AccId,DateTime startDate,DateTime endDate)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AutomatedCashFlows.Where(x => x.AccountId == AccId).Where(x => x.DateBooked > startDate && x.DateBooked < endDate).ToList();
			}
		}
		public ReturnModel UpdateAutomated(string manualcashflow)
		{
			CFType type = new CFType();
			AutomatedCashFlow automatedCashFlow = getTransaction(this.ID);
			Account account = new Account(automatedCashFlow.AccountId);
			string collectionsId = account.CollectionsId;
			List<CFType> types = type.GetCFList(collectionsId);
			types.Add(new CFType { Id = "999"});
			if(manualcashflow != null)
			{
				ManualCashFlow mancf = new ManualCashFlow();
				mancf.updateTransaction(manualcashflow,this.ID);
			}
			if (!types.Any(x=>x.Id == this.CFTypeId))
			{
				type = type.CreateCFType(collectionsId, this.CFTypeId);
				automatedCashFlow.CFTypeId = type.Id;
			}
			else
			{
				automatedCashFlow.CFTypeId = this.CFTypeId; ;
			}
			automatedCashFlow.Validated = this.Validated;
			automatedCashFlow.SourceOfExpense = this.SourceOfExpense;
			return updateTransaction(automatedCashFlow);
		}
		private ReturnModel updateTransaction(AutomatedCashFlow automatedCashFlow)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					_context.Entry(automatedCashFlow).State = EntityState.Modified;
					_context.SaveChanges();
					return new ReturnModel() { result = true };
				}
				catch (Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
					return new ReturnModel() { result = false, returnStr = e.Message };
				}
			}
		}
		private AutomatedCashFlow getTransaction(string Id)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AutomatedCashFlows.Find(Id);
			}
		}
		public async Task<bool>	UpdateTransactions()
		{
			//Invoke Classes
			CFClassification classification = new CFClassification();			
			YodleeTransactionModel transactionModel = new YodleeTransactionModel();
			Collections collection = new Collections();
			YodleeTransactionType transactionType = new YodleeTransactionType();
			ManualCashFlow manualCash = new ManualCashFlow();
			AutomatedCashFlow automatedCashFlow = new AutomatedCashFlow();
			//Get Static Lists
			List<Collections> collections = collection.GetCollections("", "");
			List<CFClassification> classifications = classification.GetList();

			foreach (Collections item in collections.Where(x => x.Accounts.Any()))
			{
				AutomateReturnList returnList = new AutomateReturnList();
				returnList.automateReturns = new List<AutomateReturn>();
				if (item.Accounts.Where(x => x.AccountIdentifier != null).Any())
				{

					YodleeModel yodlee = new YodleeModel();
					string token = await yodlee.getToken(item.CollectionsId, "");
					List<YodleeTransactionLevel> transactions = await transactionModel.GetYodleeTransactions(item.CollectionsId,token);
					if (transactions != null)
					{
						DateTime smallest = transactions.Select(x => x.transactionDate).Min();
						if (smallest > DateTime.MinValue)
						{
							smallest = smallest.AddDays(-3);
						}
						List<CFType> yodleeTypes = await transactionType.YodleeTransform(token, item.CollectionsId);
						foreach (Account account in item.Accounts.Where(x => x.YodleeId != 0))
						{
							Account tempAccount = account;
							List<ManualCashFlow> manualFlows = manualCash.GetManualCashFlows(account.Id);
							foreach (ManualCashFlow m in manualFlows)
							{
								m.CFClassification = classifications.Where(x => x.Id == m.CFClassificationId).FirstOrDefault();
							}
							account.AutomatedCashFlows = automatedCashFlow.GetAutomatedCashFlows(account.Id, smallest, DateTime.Now.AddDays(1));
							foreach (YodleeTransactionLevel transaction in transactions.Where(x => x.accountId == account.YodleeId))
							{
								if (!account.AutomatedCashFlows.Where(x => x.YodleeId == transaction.id).Any())
								{
									ManualCashFlow manualCashFlow = manualFlows
										.Where(x => x.AutomatedCashFlowId == null)
										.Where(x => x.Amount == transaction.amount.amount && x.CFClassification.Name.ToLower() == transaction.categoryType.ToLower() && x.DateBooked > transaction.transactionDate.AddDays(-2) && x.DateBooked < transaction.transactionDate.AddDays(5))
										.FirstOrDefault();
									try
									{
										returnList.automateReturns.Add(AddTransaction(transaction, account.Id, yodleeTypes, manualCashFlow, classifications, tempAccount));
									}
									catch (Exception e)
									{
										ExceptionCatcher catcher = new ExceptionCatcher();
										catcher.Catch(e.Message + ";" + JsonConvert.SerializeObject(transaction) + ";" + JsonConvert.SerializeObject(manualCashFlow) + "");
									}
									if (manualCashFlow != null)
									{
										manualFlows.Remove(manualCashFlow);
									}
								}
							}
							//item.Accounts.Where(x=>x.Id == account.Id).FirstOrDefault() = tempAccount;
						}
						try
						{
							List<AccountChange> accChangeList = new List<AccountChange>();
							List<AutomatedCashFlow> cashFlows = new List<AutomatedCashFlow>();
							List<ManualCashFlow> manualCashFlows = new List<ManualCashFlow>();
							//ac 
							foreach (AutomateReturn returnItem in returnList.automateReturns)
							{
								if (returnItem.AccountChange.AccountChangeId != "")
								{
									returnItem.AccountChange.Account = null;
									returnItem.AccountChange.AutomatedCashFlow = null;
									accChangeList.Add(returnItem.AccountChange);
								}
								returnItem.AutomatedCashFlow.Account = null;
								returnItem.AutomatedCashFlow.CFClassification = null;
								cashFlows.Add(returnItem.AutomatedCashFlow);
								if (returnItem.ManualCashFlow.Id != "")
								{
									manualCashFlows.Add(returnItem.ManualCashFlow);
								}
							}
							using (FinPlannerContext _context = new FinPlannerContext())
							{
								_context.AccountChange.AddRange(accChangeList);
								_context.AutomatedCashFlows.AddRange(cashFlows);
								foreach (ManualCashFlow manual in manualCashFlows)
								{
									_context.Entry(manual).State = EntityState.Modified;
								}
								_context.SaveChanges();
							}
						}
						catch (Exception e)
						{
							ExceptionCatcher catcher = new ExceptionCatcher();
							catcher.Catch(e.Message);
						}
					}
				}
			}		   
			return true;
		}
		public AutomateReturn AddTransaction(YodleeTransactionLevel transaction,string accId,List<CFType> types,ManualCashFlow manual,List<CFClassification> classifications, Account account)
		{
			
			AutomateReturn automateReturn = new AutomateReturn();
			automateReturn.AutomatedCashFlow = new AutomatedCashFlow()
			{
				ID = Guid.NewGuid().ToString(),
				AccountId = accId,
				Amount = transaction.amount.amount,
				CFClassificationId = GetCFClassification(transaction, classifications),
				CFTypeId = types.Where(x => x.YodleeId == transaction.categoryId).Select(x => x.Id).FirstOrDefault(),
				DateBooked = transaction.transactionDate,
				DateCaptured = transaction.createdDate,
				SourceOfExpense = GetSource(transaction),
				YodleeId = transaction.id
			};
			if(automateReturn.AutomatedCashFlow.DateBooked == DateTime.MinValue)
			{
				automateReturn.AutomatedCashFlow.DateBooked = automateReturn.AutomatedCashFlow.DateCaptured;
			}
			
			if (manual != null)
			{
				automateReturn.ManualCashFlow = manual;
				automateReturn.ManualCashFlow.AutomatedCashFlowId = automateReturn.AutomatedCashFlow.ID;
				automateReturn.AutomatedCashFlow.CFTypeId = manual.CFTypeId;
				automateReturn.AccountChange = new AccountChange() { AccountChangeId = "" };
				automateReturn.AutomatedCashFlow.Validated = true;
			}
			else
			{
				AccountChange change = new AccountChange();
				automateReturn.AccountChange = change.AddAccountChange(automateReturn.AutomatedCashFlow, account, GetCFClassificationSign(transaction, classifications));
				automateReturn.ManualCashFlow = new ManualCashFlow() { Id = "" };
				automateReturn.AutomatedCashFlow.Validated = false;
			}
			return automateReturn;
		}
		public string GetSource(YodleeTransactionLevel transaction)
		{
			if(transaction.description.simple == null)
			{
				return transaction.description.original;
			}
			else
			{
				return transaction.description.simple;
			}
		}
		public string GetCFClassification(YodleeTransactionLevel transaction, List<CFClassification> classifications)
		{
			if (transaction.baseType == "DEBIT")
			{
				return classifications.Where(x => x.Name == "Expense").Select(x => x.Id).FirstOrDefault();
			}
			else
			{
				return classifications.Where(x => x.Name == "Income").Select(x => x.Id).FirstOrDefault();
			}
		}
		public int GetCFClassificationSign(YodleeTransactionLevel transaction, List<CFClassification> classifications)
		{
			if (transaction.baseType == "DEBIT")
			{
				return classifications.Where(x => x.Name == "Expense").Select(x => x.Sign).FirstOrDefault();
			}
			else
			{
				return classifications.Where(x => x.Name == "Income").Select(x => x.Sign).FirstOrDefault();
			}
		}

	}
	public class AutomateReturnList
	{
		public List<AutomateReturn> automateReturns { get; set; }
	}
	public class AutomateReturn
	{
		public AutomatedCashFlow AutomatedCashFlow { get; set; }
		public AccountChange AccountChange { get; set; }
		public ManualCashFlow ManualCashFlow { get; set; }
	}
}
