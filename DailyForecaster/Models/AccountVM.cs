using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class AccountVM
	{
		public Account Account { get; set; }
		public List<Institution> Institutions { get; set; }
		public List<AccountType> AccountTypes { get; set; }
		public AccountVM()
		{ }
		public AccountVM(string accountId)
		{
			Account account = new Account();
			Account = account.GetAccount(accountId, false);
			Account.AccountIdentifier = Account.AccountIdentifier.Substring(4);
			Institution institution = new Institution();
			Institutions = institution.GetInstitutions();
			AccountTypes = AccountType.GetAccountTypes();
		}
	}
}
