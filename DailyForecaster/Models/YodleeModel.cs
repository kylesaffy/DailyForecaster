using DailyForecaster.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class YodleeModel
	{
		static private string url = "https://development.api.yodlee.com/ysl";
		static private string ClientID = "vXEJJjy2aVQUIdIZqAAKqIw7QPyo73DO";
		static private string secret = "1cRBWjCGGxEsrTbL";
		static private string ADMIN = "8f1954be-d622-45a1-9bd1-2adbd813a832_ADMIN";
		public string Id { get; set; }
		public string loginName { get; set; }
		public string CollectionsId { get; set; }
		[ForeignKey("CollectionsId")]
		public Collections Collection { get; set; }
		public YodleeModel() { }
		public YodleeModel(string collectionsId,string instruction)
		{
			if (instruction == "New")
			{
				Id = Guid.NewGuid().ToString();
				loginName = Guid.NewGuid().ToString("N");
				CollectionsId = collectionsId;	 			
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					_context.YodleeModel.Add(this);
					_context.SaveChanges();
				}
			}
			if(instruction == "Get")
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					YodleeModel getM = _context.YodleeModel.Where(x => x.CollectionsId == collectionsId).FirstOrDefault();
					loginName = getM.loginName;
				}
			}
		}
		
		public async Task<string> getToken(string collectionsId, string userId)
		{
			YodleeAuthReturn authReturn = new YodleeAuthReturn();
			ReturnModel returnModel = new ReturnModel();
			if(Check(collectionsId))
			{
				authReturn = await authReturn.GetToken(GetLoginName(collectionsId));
				if(authReturn == null)
				{
					ExceptionCatcher exceptionCatcher = new ExceptionCatcher();
					exceptionCatcher.Catch("auth return null ");
				}
				returnModel.returnStr = authReturn.token.accessToken;
			}
			else
			{
				if (userId != "")
				{
					returnModel = await Register(userId, collectionsId);
				}
			}
			return returnModel.returnStr;
		}
		
		private bool Check(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.YodleeModel.Where(x => x.CollectionsId == collectionsId).Any();
			}
		}
		private string GetLoginName(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.YodleeModel.Where(x => x.CollectionsId == collectionsId).Select(x => x.loginName).FirstOrDefault();
			}
		}
		private async Task<ReturnModel> Register(string userId, string collectionsId)
		{
			YodleeAuthReturn auth = await GetAdminAuth();
			ReturnModel returnModel = new ReturnModel();
			if(auth.token.accessToken != null)
			{
				YodleeModel yodleeModel = new YodleeModel();
				if (!Check(collectionsId))
				{
					yodleeModel = new YodleeModel(collectionsId, "New");
				}
				else
				{
					yodleeModel = new YodleeModel(collectionsId, "Get");
				}
				AspNetUsers user = new AspNetUsers();
				UserNames userNames = user.getNames(userId);
				RegisterModel registerModel = new RegisterModel(yodleeModel.loginName, userNames);

				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Add("Api-Version", "1.1"); 
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.token.accessToken);
				StringContent content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync(url + "/auth/register", content);
				if (response.IsSuccessStatusCode)
				{
					YodleeAuthReturn authReturn = new YodleeAuthReturn();
					authReturn = await authReturn.GetToken(yodleeModel.loginName);
					returnModel.result = true;
					returnModel.returnStr = authReturn.token.accessToken;
				}
				else
				{
					string tokenStr = await response.Content.ReadAsStringAsync();
					ExceptionCatcher exceptionCatcher = new ExceptionCatcher();
					exceptionCatcher.Catch(tokenStr);
					returnModel.result = false;
				}
			}
			else
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch("No Token generated");
				returnModel.result = false;
			}
			return returnModel;
		}
		private async Task<YodleeAuthReturn> GetAdminAuth()
		{
			YodleeAuthReturn authReturn = new YodleeAuthReturn();
			authReturn = await authReturn.GetToken(ADMIN);
			return authReturn;
		}
		private class YodleeAuthReturn
		{
			public YodleeToken token { get; set; }
			public YodleeAuthReturn() { }
			public async Task<YodleeAuthReturn> GetToken(string loginName)
			{
				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Add("loginName", loginName);
				client.DefaultRequestHeaders.Add("Api-Version", "1.1");
				var dict = new Dictionary<string, string>();
				dict.Add("clientId", ClientID);
				dict.Add("secret", secret);
				dict.Add("Content-Type", "application/x-www-form-urlencoded");
				var content = new FormUrlEncodedContent(dict);
				HttpResponseMessage response = await client.PostAsync(url + "/auth/token",content);
				if (response.IsSuccessStatusCode)
				{
					string tokenStr = await response.Content.ReadAsStringAsync();
					YodleeAuthReturn responseToken = JsonConvert.DeserializeObject<YodleeAuthReturn>(tokenStr);
					return responseToken;
				}
				else
				{
					string tokenStr = await response.Content.ReadAsStringAsync();
					ExceptionCatcher exceptionCatcher = new ExceptionCatcher();
					exceptionCatcher.Catch(tokenStr);
				}
				return null;
			}
			
		}
		private class YodleeToken
		{
			public string accessToken { get; set; }
			public DateTime issuedAt { get; set; }
			public int expiresIn { get; set; }
		}
		private class RegisterModel
		{
			public UserModel user { get; set; }
			public RegisterModel(string loginname, UserNames userN) 
			{
				user = new UserModel(loginname, userN);
			}
		}
		private class UserModel
		{
			public string loginName { get; set; }
			public UserNames name { get; set; }
			public Preferences preferences { get; set; }
			public UserModel(string loginname,UserNames user)
			{
				loginName = loginname;
				name = user;
				preferences = new Preferences()
				{
					currency = "ZAR",
					locale = "en_ZA"
				};
			}
		}
		private class Preferences
		{
			public string currency { get; set; }
			public string locale { get; set; }
		}
		
	}
	
}
