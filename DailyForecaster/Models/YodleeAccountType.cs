using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class YodleeAccountType
	{
		public string YodleeAccountTypeId { get; set; }
		public string YodleeDesc { get; set; }
		public string AccountTypeId { get; set; }
		[ForeignKey("AccountTypeId")]
		public AccountType AccountType { get; set; }
		public string Container { get; set; }
		public YodleeAccountType() { }
		public YodleeAccountType Get(string YodleeId, string container)
		{
			YodleeAccountType type = new YodleeAccountType() { YodleeDesc = YodleeId, Container = container};
			return type.Get();
		}
		private YodleeAccountType Get()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.YodleeAccountType.Where(x => x.YodleeDesc == this.YodleeDesc && x.Container == this.Container.ToLower()).FirstOrDefault();
			}
		}
	}
}
