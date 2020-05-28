using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class BudgetTransaction
	{
		[Required]
		public string TransactionId { get; set; }
		[Required]
		public string BudgetId { get; set; }
		public double Amount { get; set; }
		[Required]
		public CFType CFType { get; set; }
		[Required]
		public CFClassification	CFClassification { get; set; }
	}
}
