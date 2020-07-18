using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DailyForecaster.Controllers;
using DailyForecaster.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyForecaster.Models
{
	/// <summary>
	/// Accounts for transactions that are created by users
	/// </summary>
	public class ManualCashFlow
	{
		[Required]
		public string Id { get; set; }
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
		[Required]
		public DateTime DateBooked { get; set; }
		[Required]
		public DateTime DateCaptured { get; set; }
		[Required]
		public string SourceOfExpense { get; set; } //Possibly make this a class - Account or Cash
		[Required]
		public bool Expected { get; set; }
		public string Description {get;set;}
		public string ExpenseLocation { get; set; }
		public string PhotoBlobLink { get; set; }
		public string UserId { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		[Required]
		public string AccountId {get;set;}
		[ForeignKey("AutomatedCashFlowId")]
		public AutomatedCashFlow AutomatedCashFlow { get; set; }
		public string AutomatedCashFlowId { get; set; }
		public bool isDeleted { get; set; }
		public ManualCashFlow(ManualCashFlow flow)
		{
			AspNetUsers users = new AspNetUsers();
			CFTypeId = flow.CFTypeId;
			CFClassificationId = flow.CFClassificationId;
			Amount = flow.Amount;
			DateBooked = flow.DateBooked;
			DateCaptured = DateTime.Now;
			SourceOfExpense = flow.SourceOfExpense;
			Expected = flow.Expected;
			Id = Guid.NewGuid().ToString();
			UserId = users.getUserId(flow.UserId);
			isDeleted = false;
			ExpenseLocation = flow.ExpenseLocation;
			AccountId = flow.AccountId;
			Description = flow.Description;
			ExpenseLocation = flow.ExpenseLocation;
		}
		public ManualCashFlow(CFType cfId, CFClassification cfClass, double amount, DateTime dateBooked, string source, string userID, bool exp, string el)
		{
			CFType = cfId;
			CFClassification = cfClass;
			Amount = amount;
			DateBooked = dateBooked;
			DateCaptured = DateTime.Now;
			SourceOfExpense = source;
			Expected = exp;
			Id = Guid.NewGuid().ToString();
			UserId = userID;
			isDeleted = false;
			ExpenseLocation = el;
		}		  		
		public List<ManualCashFlow> GetManualCashFlows(string AccId,int count = 10)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
					.ManualCashFlows
					.Where(x => x.AccountId == AccId)
					.OrderByDescending(x=>x.DateBooked)
					.Take(count)
					.ToList();
			}
		}
		public List<ManualCashFlow> GetManualCahFlowsUnseen(List<string> accountsStr)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				List<ManualCashFlow> cfs = _context
					.ManualCashFlows
					.Where(man => accountsStr.Contains(man.AccountId))
					.Where(x=>x.AutomatedCashFlowId == null)
					.ToList();
				return cfs;
			}
		}
		public List<ManualCashFlow> GetManualCashFlows(string accountId, DateTime startDate, DateTime endDate)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.ManualCashFlows.Where(x => x.AccountId == accountId).Where(x => x.DateBooked > startDate && x.DateBooked < endDate).ToList();
			}
		}
		public ManualCashFlow()
		{

		}
		public void updateTransaction(string manId,string autoId)
		{
			try
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					ManualCashFlow man = _context.ManualCashFlows.Find(manId);
					man.AutomatedCashFlowId = autoId;
					_context.Entry(man).State = EntityState.Modified;
					_context.SaveChanges();
				}
			}
			catch(Exception e)
			{
				ExceptionCatcher exceptionCatcher = new ExceptionCatcher();
				exceptionCatcher.Catch(e.Message);
			}
		}
		public ReturnModel AddTransfer(TransferObject obj)
		{
			CFClassification classification = new CFClassification();
			List<CFClassification> classifications = classification.GetList();
			ManualCashFlow to = new ManualCashFlow()
			{
				Id = Guid.NewGuid().ToString(),
				CFClassificationId = classifications.Where(x => x.Sign == 1).Select(x => x.Id).FirstOrDefault(),
				CFTypeId = "999",
				Amount = obj.Amount,
				SourceOfExpense = "Transfer",
				Description = "Transfer",
				DateBooked = obj.DateBooked,
				DateCaptured = DateTime.Now,
				AccountId = obj.TransferTo,
			};
			ManualCashFlow from = new ManualCashFlow()
			{
				Id = Guid.NewGuid().ToString(),
				CFClassificationId = classifications.Where(x => x.Sign == -1).Select(x => x.Id).FirstOrDefault(),
				CFTypeId = "999",
				SourceOfExpense = "Transfer",
				Description = "Transfer",
				Amount = obj.Amount,
				DateBooked = obj.DateBooked,
				DateCaptured = DateTime.Now,
				AccountId = obj.TransferFrom,
			};
			to.Save();
			from.Save();
			return new ReturnModel() { result = true };
		}
		public ReturnModel Save()
		{
			ManualCashFlow flow = new ManualCashFlow(this);
			ReturnModel returnModel = new ReturnModel();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Add(flow);
				try
				{
					_context.SaveChanges();
					AccountChange change = new AccountChange();
					change.AddAccountChange(flow);
					returnModel.result = true;
					return returnModel;
				}
				catch(Exception e)
				{
					returnModel.returnStr = e.Message;
					returnModel.result = false;
					return returnModel;
				}
			}
		}
	}
	public class tempManualCashFlow
	{
		public string CFType { get; set; }
		public string CFClassification { get; set; }
		public double Amount { get; set; }
		public DateTime DateBooked { get; set; }
		public string SourceOfExpense { get; set; } //Possibly make this a class - Account or Cash
		public bool Expected { get; set; }
		public string ExpenseLocation { get; set; }
		public string PhotoBlobLink { get; set; }
		public string UserId { get; set; }
		public string accountId { get; set; }
	}
}
