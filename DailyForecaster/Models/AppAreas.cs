using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AppAreas
	{
		public string AppAreasId { get; set; }
		public string Name { get; set; }
		public static List<AppAreas> GetAppAreas()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
					.AppAreas
					.ToList();
			}
		}
	}
}
