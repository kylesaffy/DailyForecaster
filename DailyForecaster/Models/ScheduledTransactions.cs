using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ScheduledTransactions
	{
		public string ScheduledTransactionsId { get; set; }
		public int Day { get; set; }
		public double Amount { get; set; }
		public string CFTypeId { get; set; }
		[ForeignKey("FTypeId")]
		public CFType CFType { get; set; }
		public string CFClassificationId { get; set; }
		[ForeignKey("CFClassificationId")]
		public CFClassification CFClassification { get; set; }
		public string Description { get; set; }
		public string AccountId { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		public ScheduledTransactions() { }
		public ScheduledTransactions Create()
		{
			return this.Save();
		}
		public void Check()
		{
			ManualCashFlow manual = new ManualCashFlow();
			int today = GetDay();
			List<ScheduledTransactions> transactions = GetScheduledTransactionsList(today);
			if (transactions.Count > 0)
			{
				foreach (ScheduledTransactions item in transactions)
				{
					manual.AddScheduledTransactions(item, new CFType(item.CFTypeId), new CFClassification(item.CFClassificationId));
				}
			}
		}
		private List<ScheduledTransactions> GetScheduledTransactionsList(int day)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.ScheduledTransactions.Where(x => x.Day == day).ToList();
			}
		}
		private int GetDay()
		{
			int day = DateTime.Now.Day;
			if (day > 27)
			{
				if (DateTime.Now.AddDays(1).Day == 1)
				{
					day = 999;
				}
			}
			return day;
		}
		private ScheduledTransactions Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				if(this.ScheduledTransactionsId == null)
				{
					this.ScheduledTransactionsId = Guid.NewGuid().ToString();
					_context.Add(this);
				}
				else
				{
					_context.Entry(this).State = EntityState.Modified;
				}
			}
			return this;
		}
	}
}
