using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class EmailRecords
	{
		public string EmailRecordsId { get; set; }
		public DateTime InteractionDate { get; set; }
		public string Message { get; set; }
		[ForeignKey("FirebaseUserId")]
		public string FirebaseUserId { get; set; }
	}
}
