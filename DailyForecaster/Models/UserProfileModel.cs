using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class UserProfileModel
	{
		public int Count { get; set; }
		public string ProfileImage { get; set; }
		public FirebaseUser User { get; set; }
		public UserProfileModel() { }
		public UserProfileModel(string uid)
		{
			FirebaseUser user = new FirebaseUser();
			user = user.GetUser(uid);
			Count = GetCount(user.Email);
			if (user.ProfileImage != null)
			{
				ProfileImage = "https://storageaccountmoney9367.blob.core.windows.net/profileimage/" + user.ProfileImage;
			}
			else
			{
				ProfileImage = null;
			}
			User = user;
		}
		private int GetCount(string email)
		{
			int count = 0;
			FirebaseUser user = new FirebaseUser();
			string id = user.GetUserId(email);
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				List<string> collections = _context
					.UserCollectionMapping
					.Where(x => x.FirebaseUserId == id)
					.Select(x => x.CollectionsId)
					.ToList();
				List<string> accounts = _context
					.Account
					.Where(Acc => collections.Contains(Acc.CollectionsId))
					.Select(x => x.Id)
					.ToList();
				foreach (string item in accounts)
				{
					count = count + _context
						.AutomatedCashFlows
						.Where(auto => item.Contains(auto.AccountId))
						.Where(x => x.Validated == false)
						.Count();
				}
			}
			return count;
		}
	}
}
