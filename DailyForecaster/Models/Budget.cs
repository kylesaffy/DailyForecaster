using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class Budget
	{
		[Required]
		public string BudgetId { get; set; }
		[Required]
		public DateTime StartDate { get; set; }
		[Required]
		public DateTime EndDFate { get; set; }
		[Required]
		public string CollectionId { get; set; }
	}
}
