using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class DailyMotivational
	{
		public string DailyMotivationalId { get; set; }
		public string URL { get; set; }
		public string Source { get; set; }
		public DateTime DistributionDate { get; set; }
		public DailyMotivational() { }
		public DailyMotivational Get()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				DateTime date = DateTime.Now.Date;
				return _context.DailyMotivational.Where(x => x.DistributionDate == date).FirstOrDefault();
			}
		}
	}
}
