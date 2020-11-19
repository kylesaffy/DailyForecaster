using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
		public double JIBAR_3_Month { get; set; }
		public double GetSpread(Account account)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				RateInformation information = _context.RateInformation.OrderByDescending(x => x.DateEffective).FirstOrDefault();
				switch(account.FloatingType)
				{
					case "Prime":
						return account.CreditRate - information.PrimeRate;
				}
			}
			return 0;
		}
		public double GetPrime()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.RateInformation.OrderByDescending(x => x.DateEffective).Select(x => x.PrimeRate).FirstOrDefault();
			}
		}
	}
	
}
