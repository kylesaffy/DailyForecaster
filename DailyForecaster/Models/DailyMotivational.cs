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
				try
				{
					DailyMotivational motivational = _context.DailyMotivational.Where(x => x.DistributionDate == date).FirstOrDefault();
					if (motivational != null) return motivational;
					else
					{
						motivational = _context.DailyMotivational.OrderBy(x => x.DistributionDate).FirstOrDefault();
						date = DateTime.Now;
						int count = _context.DailyMotivational.Count();
						DateTime newDate = date.AddDays(count);
						motivational.DistributionDate = newDate;
						_context.Entry(motivational).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
						_context.SaveChanges();
						return motivational;
					}
				}
				catch
				{
					try
					{
						DailyMotivational motivational = _context.DailyMotivational.OrderBy(x => x.DistributionDate).FirstOrDefault();
						date = DateTime.Now;
						int count = _context.DailyMotivational.Count();
						DateTime newDate = date.AddDays(count);
						motivational.DistributionDate = newDate;
						_context.Entry(motivational).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
						_context.SaveChanges();
						return motivational;
					}
					catch(Exception e)
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
