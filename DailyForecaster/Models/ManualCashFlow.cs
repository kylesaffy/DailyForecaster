using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DailyForecaster.Controllers;
using DailyForecaster.Models;
namespace DailyForecaster.Models
{
	/// <summary>
	/// Accounts for transactions that are created by users
	/// </summary>
	public class ManualCashFlow
	{
		private readonly FinPlannerContext _context;
		public ManualCashFlow(FinPlannerContext context)
		{
			_context = context;
		}
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
		public ManualCashFlow(string id)
		{
			_context.ManualCashFlows.Find(id);
		}
		public List<ManualCashFlow> GetManualCashFlows(string AccId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.ManualCashFlows.Where(x => x.AccountId == AccId).ToList();
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
