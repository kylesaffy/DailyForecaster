using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class TransferObject
	{
		public string TransferFrom { get; set; }
		public string TransferTo { get; set; }
		public double Amount { get; set; }
		public DateTime DateBooked { get; set; }
	}
}
