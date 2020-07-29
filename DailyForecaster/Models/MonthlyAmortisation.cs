using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class MonthlyAmortisation
	{
		public string MonthlyAmortisationId { get; set; }
		public string AccountAmortisationId { get; set; }
		[ForeignKey("AccountAmortisationId")]
		public AccountAmortisation AccountAmortisation { get; set; }
		public DateTime Date { get; set; }
		public double Open { get; set; }
		public double Interest { get; set; }
		public double Payment { get; set; }
		public double Capital { get; set; }
		public double Additional { get; set; }
		public double Close { get; set; }
		
		public void Create(Account account, PaymentModel payment, AccountAmortisation amortisation)
		{
			DateTime end = account.Maturity;
			DateTime start = DateTime.Now;
			start = new DateTime(start.Year, start.Month, end.Day);
			List<MonthlyAmortisation> monthlies = new List<MonthlyAmortisation>();
			double amount = account.Available;
			for (int i = 0; end.Date > start.AddMonths(i).Date; i++)
			{
				MonthlyAmortisation monthly = new MonthlyAmortisation
				{
					MonthlyAmortisationId = Guid.NewGuid().ToString(),
					AccountAmortisationId = amortisation.AccountAmortisationId,
					Date = start.AddMonths(i),
					Open = amount					
				};
				monthly.Interest = amount * (account.CreditRate / 12 / 100);
				monthly.Payment = payment.CostOfLoan;
				monthly.Capital = monthly.Payment - monthly.Interest;
				monthly.Additional = payment.AdditionalLoan;
				monthly.Close = amount - monthly.Capital - monthly.Additional;
				monthlies.Add(monthly);
				amount = monthly.Close;
				if(amount < 0)
				{
					break;
				}
			}
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.AddRange(monthlies);
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
		}
		private int DateDifference(DateTime start, DateTime end)
		{
			int i = 0;
			
			return i;
		}
	}
}
