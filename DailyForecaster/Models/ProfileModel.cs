using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ProfileModel
	{
		public FirebaseUser FirebaseUser { get; set; }
		public EmailPreferences EmailPreferences { get; set; }
		public ProfileModel() { }
		public ProfileModel(string uid)
		{
			FirebaseUser user = new FirebaseUser();
			EmailPreferences preferences = new EmailPreferences();
			FirebaseUser = user.GetUser(uid);
			EmailPreferences = preferences.Get(FirebaseUser);
		}
		public ProfileModel Update()
		{
			FirebaseUser user = this.FirebaseUser;
			user.Update(user.FirebaseId,user);
			ProfileModel model = new ProfileModel();
			model.FirebaseUser = user.GetUser(user.FirebaseId);
			EmailPreferences preferences = this.EmailPreferences;
			model.EmailPreferences = preferences.Update();
			return model;
		}
	}
}
