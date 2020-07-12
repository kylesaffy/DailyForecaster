using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;

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

		public ManualCashFlow ManualCashFlow { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		[Required]
		public string AccountId { get; set; }
		public AutomatedCashFlow()
		{ }
		public List<AutomatedCashFlow> GetAutomatedCashFlows(string AccId)
		{
			using (FinPlannerContext _context = new FinPlannerContext()) {
				return _context.AutomatedCashFlows.Where(x => x.AccountId == AccId).ToList();
			}
		}
		public List<AutomatedCashFlow> GetAutomatedCashFlows(string AccId,DateTime startDate,DateTime endDate)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AutomatedCashFlows.Where(x => x.AccountId == AccId).Where(x => x.DateBooked > startDate && x.DateBooked < endDate).ToList();
			}
		}
		public async Task<bool>	UpdateTrandactions()
		{
			YodleeTransactionModel transactionModel = new YodleeTransactionModel();
			Collections collection = new Collections();
			YodleeTransactionType transactionType = new YodleeTransactionType();
			List<Collections> collections = collection.GetCollections("", "");
			foreach (Collections item in collections.Where(x => x.Accounts.Any()))
			{
				if (item.Accounts.Where(x => x.AccountIdentifier != null).Any())
				{

					YodleeModel yodlee = new YodleeModel();
					string token = await yodlee.getToken(item.CollectionsId, "");
					List<YodleeTransactionLevel> transactions = await transactionModel.GetYodleeTransactions(item.CollectionsId,token);
					List<CFType> yodleeTypes = await transactionType.YodleeTransform(token, item.CollectionsId);
					foreach(Account account in item.Accounts.Where(x=>x.YodleeId != 0))
					{
						List<ManualCashFlow> manualFlows = account.ManualCashFlows.ToList();
						foreach(YodleeTransactionLevel transaction in transactions.Where(x=>x.accountId == account.YodleeId))
						{
							if(!account.AutomatedCashFlows.Where(x=>x.YodleeId == transaction.id).Any())
							{
								ManualCashFlow manualCashFlow = manualFlows
									.Where(x => x.AutomatedCashFlowId == null)
									.Where(x => x.Amount == transaction.amount.amount && x.CFClassification.Name.ToLower() == transaction.categoryType.ToLower() && x.DateBooked > transaction.transactionDate.AddDays(-2) && x.DateBooked < transaction.transactionDate.AddDays(5))
									.FirstOrDefault();
								AddTransaction(transaction,account.Id,yodleeTypes,manualCashFlow);
								if (manualCashFlow != null)
								{
									manualFlows.Remove(manualCashFlow);
								}
							}
						}
					}
				}
			}		   
			return true;
		}
		public bool AddTransaction(YodleeTransactionLevel transaction,string accId,List<CFType> types,ManualCashFlow manual)
		{
			CFClassification classification = new CFClassification();
			List<CFClassification> classifications = classification.GetList();
			AutomatedCashFlow automated = new AutomatedCashFlow()
			{
				ID = Guid.NewGuid().ToString(),
				AccountId = accId,
				Amount = transaction.amount.amount,
				CFClassificationId = GetCFClassification(transaction, classifications),
				CFTypeId = types.Where(x => x.YodleeId == transaction.id).Select(x => x.Id).FirstOrDefault(),
				DateBooked = transaction.transactionDate,
				DateCaptured = transaction.createdDate,
				SourceOfExpense = GetSource(transaction),
				YodleeId = transaction.id
			};
			manual.AutomatedCashFlowId = automated.ID;
			try
			{
				using(FinPlannerContext _context = new FinPlannerContext())
				{
					_context.AutomatedCashFlows.Add(automated);
					_context.Entry(manual).State = EntityState.Modified;
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
	}
}
