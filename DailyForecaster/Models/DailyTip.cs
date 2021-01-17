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
				try
				{
					DailyTip tip = _context.DailyTip.Where(x => x.DistributionDate == date).FirstOrDefault();
					if (tip != null) return tip;
					else
					{
						tip = _context.DailyTip.OrderBy(x => x.DistributionDate).FirstOrDefault();
						date = DateTime.Now;
						int count = _context.DailyMotivational.Count();
						DateTime newDate = date.AddDays(count);
						tip.DistributionDate = newDate;
						_context.Entry(tip).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
						_context.SaveChanges();
						return tip;
					}
				}
				catch
				{
					try
					{
						DailyTip tip = _context.DailyTip.OrderBy(x => x.DistributionDate).FirstOrDefault();
						date = DateTime.Now;
						int count = _context.DailyMotivational.Count();
						DateTime newDate = date.AddDays(count);
						tip.DistributionDate = newDate;
						_context.Entry(tip).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
						_context.SaveChanges();
						return tip;
					}
					catch (Exception  e)
					{
						ExceptionCatcher catcher = new ExceptionCatcher();
						catcher.Catch(e);
						return null;
					}
				}
			}
		}
	}
}
