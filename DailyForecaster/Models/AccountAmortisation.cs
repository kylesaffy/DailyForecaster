using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AccountAmortisation
	{
		public string AccountAmortisationId { get; set; }
		public string AccountId { get; set; }
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		public ICollection<MonthlyAmortisation> MonthlyAmortisations { get; set; }
		public PaymentModel PaymentModel { get; set; }
		public void Update(Account account)
		{
			if(isExists(account.Id))
			{

			}
			else
			{
				AccountAmortisation amortisation = new AccountAmortisation()
				{
					AccountId = account.Id,
					AccountAmortisationId = Guid.NewGuid().ToString()
				};
				using(FinPlannerContext _context = new FinPlannerContext())
				{
					_context.Add(amortisation);
					try
					{
						_context.SaveChanges();
					}
					catch(Exception e)
					{
						ExceptionCatcher catcher = new ExceptionCatcher();
						catcher.Catch(e.Message);
					}
				}
				PaymentModel payment = new PaymentModel(account,amortisation.AccountAmortisationId);
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					_context.Add(payment);
					try
					{
						_context.SaveChanges();
					}
					catch (Exception e)
					{
						ExceptionCatcher catcher = new ExceptionCatcher();
						catcher.Catch(e.Message);
					}
				}
				MonthlyAmortisation monthly = new MonthlyAmortisation();
				monthly.Create(account,payment,amortisation);
			}
		}
		private bool isExists(string accountId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.AccountAmortisation.Where(x => x.AccountId == accountId).Any();
			}
		}
	}
}
