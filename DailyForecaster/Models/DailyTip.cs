using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class DailyTip
	{
		public string DailyTipId { get; set; }
		public string Header { get; set; }
		public string Quote { get; set; }
		public DateTime DistributionDate { get; set; }
		public string Source { get; set; }
		public DailyTip() { }
		public DailyTip Get()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				DateTime date = DateTime.Now.Date;
				return _context.DailyTip.Where(x => x.DistributionDate == date).FirstOrDefault();
			}
		}
	}
}
