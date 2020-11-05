using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	[Serializable]
	public class RetailMerchants
	{
		public string RetailMerchantsId { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// Return a merchant based on their name
		/// </summary>
		/// <param name="name">Name of the merchant</param>
		/// <returns>A merchant object</returns>
		public RetailMerchants Get(string name)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.RetailMerchants.Where(x => x.Name == name).FirstOrDefault();
			}
		}
	}
}
