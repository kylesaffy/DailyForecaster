using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AccountBalance
	{
		public string AccountBalanceId { get; set; }
		public string AccountId { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		public DateTime Date { get; set; }
		public double Amount { get; set; }
	}
}
