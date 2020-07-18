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

		public ManualCashFlow ManualCashFlow { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		[Required]
		public string AccountId { get; set; }
		public AutomatedCashFlow()
		{ }
		public List<AutomatedCashFlow> GetAutomatedCashFlows(string AccId, int count = 10)
		{
			using (FinPlannerContext _context = new FinPlannerContext()) {
				return _context
					.AutomatedCashFlows
					.Where(x => x.AccountId == AccId)
					.OrderByDescending(x=>x.DateBooked)
					.Take(count)
					.ToList();
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
		public List<AutomatedCashFlow> GetAutomatedCashFlows(string AccId,DateTime startDate,DateTime endDate)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AutomatedCashFlows.Where(x => x.AccountId == AccId).Where(x => x.DateBooked > startDate && x.DateBooked < endDate).ToList();
			}
		}
		public ReturnModel UpdateAutomated(string manualcashflow)
		{
			AutomatedCashFlow automatedCashFlow = getTransaction(this.ID);
			if(manualcashflow != null)
			{
				ManualCashFlow mancf = new ManualCashFlow();
				mancf.updateTransaction(manualcashflow,this.ID);
			}
			
			automatedCashFlow.CFTypeId = this.CFTypeId; ;
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
					DateTime smallest = transactions.Select(x => x.transactionDate).Min();
					if (smallest > DateTime.MinValue)
					{
						smallest = smallest.AddDays(-3);
					}
					List<CFType> yodleeTypes = await transactionType.YodleeTransform(token, item.CollectionsId);
					foreach(Account account in item.Accounts.Where(x=>x.YodleeId != 0))
					{
						Account tempAccount = account;
						List<ManualCashFlow> manualFlows = manualCash.GetManualCashFlows(account.Id);
						foreach(ManualCashFlow m in manualFlows)
						{
							m.CFClassification = classifications.Where(x => x.Id == m.CFClassificationId).FirstOrDefault();
						}
						account.AutomatedCashFlows = automatedCashFlow.GetAutomatedCashFlows(account.Id,smallest, DateTime.Now.AddDays(1));
						foreach(YodleeTransactionLevel transaction in transactions.Where(x=>x.accountId == account.YodleeId))
						{
							if(!account.AutomatedCashFlows.Where(x=>x.YodleeId == transaction.id).Any())
							{
								ManualCashFlow manualCashFlow = manualFlows
									.Where(x => x.AutomatedCashFlowId == null)
									.Where(x => x.Amount == transaction.amount.amount && x.CFClassification.Name.ToLower() == transaction.categoryType.ToLower() && x.DateBooked > transaction.transactionDate.AddDays(-2) && x.DateBooked < transaction.transactionDate.AddDays(5))
									.FirstOrDefault();
								try
								{
									returnList.automateReturns.Add(AddTransaction(transaction, account.Id, yodleeTypes, manualCashFlow, classifications, tempAccount));
								}
								catch(Exception e)
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
			
			
			if (manual != null)
			{
				automateReturn.ManualCashFlow = manual;
				automateReturn.ManualCashFlow.AutomatedCashFlowId = automateReturn.AutomatedCashFlow.ID;
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
