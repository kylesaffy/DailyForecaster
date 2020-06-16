using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class Institution
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string ContactNumber { get; set; }
		[Required]
		public string WebLink { get; set; }
		public string Email { get; set; }
		public string BlobString { get; set; }
		public List<Institution> GetInstitutions()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.Institution.ToList();
			}
		}
	}
}
