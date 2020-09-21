using FirebaseAdmin.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
    public class UserModel
    {
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string PhoneNumber { get; set; }
        public string Password {get;set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Uid { get; set; }
        public async Task<bool> CreateUser(UserModel model)
		{
            UserRecordArgs args = new UserRecordArgs()
            {
                Email = "user@example.com",
                EmailVerified = false,
                PhoneNumber = "+11234567890",
                Password = "secretPassword",
                DisplayName = model.FirstName + " " + model.LastName,
            };
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
            model.Uid = userRecord.Uid;
            new FirebaseUser(model);
            return true;

        }
	}
   
}
