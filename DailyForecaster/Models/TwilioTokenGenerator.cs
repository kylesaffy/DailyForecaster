using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Twilio.Jwt.AccessToken;

namespace DailyForecaster.Models
{
	public interface ITokenGenerator
	{
		string Generate(string identity);
	}
	public class TwilioTokenGenerator : ITokenGenerator
	{
        public TwilioTokenGenerator() { }
        public string Generate(string identity)
        {
            TwilioAccountDetails Configuration = new TwilioAccountDetails();
            var grants = new HashSet<IGrant>
                {
                    new ChatGrant {ServiceSid = Configuration.ChatServiceSid}
                };

            var token = new Token(
                Configuration.AccountSid,
                Configuration.ApiKey,
                Configuration.ApiSecret,
                identity,
                grants: grants);

            return token.ToJwt();
        }
    }
}
