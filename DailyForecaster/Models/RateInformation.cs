using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class RateInformation
	{
		public string RateInformationId { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateEffective { get; set; }
		public double RepoRate { get; set; }
		public double PrimeRate { get; set; }
	}
}
