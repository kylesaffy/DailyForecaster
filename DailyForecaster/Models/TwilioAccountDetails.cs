using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class TwilioAccountDetails
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string ChatServiceSid { get; set; }
        public string SyncServiceSid { get; set; }
        public string NotificationServiceSid { get; set; }
        public TwilioAccountDetails()
		{
            AccountSid= "ACf8a1b28de03c3f2eb0f7427394ba6bcf";
            AuthToken= "6674bf976a211db2cfc3f265b1187cb3";
            ApiKey= "SKf07ab2a91d4fc5faeb2de92b510b3467";
            ChatServiceSid= "SM9962e4b05e624f82bae2ea07cac6cfb1";
            ApiSecret = "goOB7KIeJiZ8xiSI8tgRMPH2GEDnXZFb";

        }
    }
}
