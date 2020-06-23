using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class Notes
	{
		[Required]
		public string NotesId { get; set; }
		[Required]
		public string BudgetTransactionId { get; set; }
		[ForeignKey("BudgetTransactionId")]
		public BudgetTransaction BudgetTransaction { get; set; }
		[Required]
		public string Body { get; set; }
	}
}
