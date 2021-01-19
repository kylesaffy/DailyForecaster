using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class YodleeErrorManager
	{
		public string YodleeErrorManagerId { get; set; }
		public string ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
		public string ReferenceCode { get; set; }
		public string CollectionsId { get; set; }
		public string  Function { get; set; }
		public string YodleeFail { get; set; }
		public YodleeErrorManager() { }
		/// <summary>
		/// Saves a error from a response string 
		/// </summary>
		/// <param name="id">CollectionsId where the error occured</param>
		/// <param name="location">The MM Function that was being executed</param>
		/// <param name="process">The Yodlee process that MM was trying to action</param>
		public void Save(string id, string location, string process)
		{
			this.YodleeErrorManagerId = Guid.NewGuid().ToString();
			this.CollectionsId = id;
			this.Function = location;
			this.YodleeFail = process;
			this.Save();
		}
		/// <summary>
		/// Saves a new and complete YodleeErrorManager Function
		/// </summary>
		private void Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.YodleeErrorManager.Add(this);
				_context.SaveChanges();
			}
		}
	}
}
