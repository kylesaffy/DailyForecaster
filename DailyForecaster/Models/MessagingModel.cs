using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	[DataContract]
	public class MessagingModel
	{
		[DataMember]
		public string MessagingModelId { get; set; }
		[DataMember]
		public string Message { get; set; }
		[DataMember]
		public DateTime DateTimeCreated { get; set; }
		[DataMember]
		public DateTime DateTimeRead { get; set; }
		[DataMember]
		public string RecipientId { get; set; }
		[DataMember]
		[ForeignKey("RecipientId")]
		public FirebaseUser Recipient { get; set; }
		[DataMember]
		public string SenderId { get; set; }
		[DataMember]
		[ForeignKey("SenderId")]
		public FirebaseUser Sender { get; set; }
		[DataMember]
		public bool Delivered { get; set; }
		[DataMember]
		public bool Read { get; set; }
	}
}
