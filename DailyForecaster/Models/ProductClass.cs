using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ProductClassModel
	{
		public string ProductClassModelId { get; set; }
		public string Name { get; set; }
		public string SuperClass { get; set; }
		/// <summary>
		/// Returns a ProductClassModel by ID
		/// </summary>
		/// <param name="id">ID of the ProductClassModel</param>
		/// <returns>a ProductClassModel</returns>
		public ProductClassModel Get(string id)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.ProductClassModel.Find(id);
			}
		}
	}
}
