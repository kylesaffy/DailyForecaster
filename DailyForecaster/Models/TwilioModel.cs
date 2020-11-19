using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace DailyForecaster.Models
{
	public class TwilioModel
	{
		static async Task SendMessage()
		{
			string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
		    string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
			TwilioClient.Init(accountSid, authToken);
			var message = await MessageResource.CreateAsync(
				body: "This is the ship that made the Kessel Run in fourteen parsecs?",
				from: new Twilio.Types.PhoneNumber("+17752549987"),
				to: new Twilio.Types.PhoneNumber("+27763105028")
			);
		}
	}
}
