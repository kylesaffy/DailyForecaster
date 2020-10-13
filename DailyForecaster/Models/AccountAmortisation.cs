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
		public void Delete(string AccountId)
		{
			List<AccountAmortisation> accounts = new List<AccountAmortisation>();
			accounts = GetAccounts(AccountId, true);
			MonthlyAmortisation amortisation = new MonthlyAmortisation();
			foreach (AccountAmortisation item in accounts)
			{
				amortisation.Delete(item.AccountAmortisationId);
				item.Delete();
			}
		}
		private List<AccountAmortisation> GetAccounts(string accountId, bool ans)
		{
			if (ans)
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					return _context.AccountAmortisation.Where(x => x.AccountId == accountId).ToList();
				}
			}
			return null;
		}
		private void Delete()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Remove(this);
				_context.SaveChanges();

			}
		}
		public List<MonthlyAmortisation> CalculateMonthly(Account account)
		{
			PaymentModel payment = new PaymentModel(account, "temp");
			MonthlyAmortisation monthly = new MonthlyAmortisation();
			AccountAmortisation amortisation = Get(account.Id, "AccountId");
			return monthly.Calculate(account, payment, amortisation);
		}
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
		private AccountAmortisation Get(string Id,string type)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				switch (type)
				{
					case "AmortId":
						return _context.AccountAmortisation.Find(Id);
					case "AccountId":
						return _context.AccountAmortisation.Where(x => x.AccountId == Id).FirstOrDefault();
					default:
						return _context.AccountAmortisation.Find(Id);

				}
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
