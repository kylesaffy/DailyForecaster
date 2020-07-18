using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AutomatedLog
	{
		public string Id { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public string ProcessName { get; set; }
		public bool result { get; set; }
		public void SaveLog(DateTime start, DateTime end, string processName, bool result)
		{
			AutomatedLog log = new AutomatedLog()
			{
				Id = Guid.NewGuid().ToString(),
				Start = start,
				End = end,
				ProcessName = processName,
				result = result
			};
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.AutomatedLog.Add(log);
				_context.SaveChanges();
			}
		}
	}
	
}
