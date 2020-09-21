using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class EmailPreferences
	{
		public string EmailPreferencesId { get; set; }
		[ForeignKey("FirebaseUserId")]
		public string FirebaseUserId { get; set; }
		public FirebaseUser FirebaseUser { get; set; }
		public DateTime LastInteraction { get; set; }
		public string InteractionRecord { get; set; }
		public bool LoginNotification { get; set; }
		public bool DailyCommunication { get; set; }
	}
}
