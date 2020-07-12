using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ExceptionCatcher
	{
		public string Id { get; set; }
		public DateTime DateTime { get; set; }
		public string Exception { get; set; }
		public ExceptionCatcher() { }
		public void Catch(string e)
		{
			ExceptionCatcher catcher = new ExceptionCatcher()
			{
				Id = Guid.NewGuid().ToString(),
				DateTime = DateTime.Now,
				Exception = e
			};
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.ExceptionCatcher.Add(catcher);
				_context.SaveChanges();
			}
		}
	}
}
