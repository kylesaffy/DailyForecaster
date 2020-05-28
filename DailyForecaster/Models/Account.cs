using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
		[Required]
		public Institution Institution { get; set; }
		public double Available { get; set; }
		public double AccountLimit {get;set;}
		public double NetAmount { get; set; }
		public List<ReportedTransaction> GetTransactions()
		{
			ReportedTransaction reportedTransaction = new ReportedTransaction();
			return reportedTransaction.GetTransactions(this.Id);
		}
	}
}
