﻿using DailyForecaster.Controllers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class YodleeModel
	{
		static private string url = "https://api.yodlee.uk/ysl";
		static private string ClientID = "vXEJJjy2aVQUIdIZqAAKqIw7QPyo73DO";
		static private string secret = "1cRBWjCGGxEsrTbL";
		static private string ADMIN = "8f1954be-d622-45a1-9bd1-2adbd813a832_ADMIN";
		public string Id { get; set; }
		public string loginName { get; set; }
		public string Key { get; set; }
		public string CollectionsId { get; set; }
		[ForeignKey("CollectionsId")]
		public Collections Collection { get; set; }
		public YodleeModel() { }
		public List<YodleeModel> Get()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.YodleeModel.ToList();
			}
		}
		public YodleeModel Get(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.YodleeModel.Where(x => x.CollectionsId == collectionsId).FirstOrDefault();
			}
		}
		/// <summary>
		/// Either creates and or returns a yodlee model object depending on the instruction passed
		/// </summary>
		/// <param name="collectionsId">The Id of the collection</param>
		/// <param name="instruction">Can either be New or Get</param>
		public YodleeModel(string collectionsId,string instruction)
		{
			if (instruction == "New")
			{
				Id = Guid.NewGuid().ToString();
				loginName = Guid.NewGuid().ToString("N");
				CollectionsId = collectionsId;
				//Key = new RSAOpenSsl(2048).ToString();
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
					//Key = getM.Key;
				}
			}
		}
		public void Update()
		{
			//this.Key = new RSAOpenSsl(2048).ToString();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Entry(this).State = EntityState.Modified;
				_context.SaveChanges();
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
		public async Task<ReturnModel> RemoveAutoUpdates(string collectionsId)
		{
			string token = await getToken(collectionsId, "");
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Api-Version", "1.1");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = await client.DeleteAsync(@"https://api.yodlee.uk/ysl/configs/notifications/events/AUTO_REFRESH_UPDATES");
			if (response.IsSuccessStatusCode)
			{
				YodleeModel yodleeModel = Get(collectionsId);
				yodleeModel.Remove();
				return new ReturnModel() { result = true };
			}
			else
			{
				return new ReturnModel() { result = false, returnStr = await response.Content.ReadAsStringAsync() };
			}
		}
		public async Task<ReturnModel> Unregister(string collectionsId)
		{
			string token = await getToken(collectionsId, "");
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Api-Version", "1.1");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = await client.DeleteAsync(url + "/user/unregister");
			if (response.IsSuccessStatusCode)
			{
				YodleeModel yodleeModel = Get(collectionsId);
				yodleeModel.Remove();
				return new ReturnModel() { result = true };
			}
			else
			{
				return new ReturnModel() { result = false };
			}
		}
		private void Remove()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Remove(this);
				_context.SaveChanges();
			}
		}
		public async Task<ReturnModel> Register(string userId, string collectionsId)
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
				FirebaseUser user = new FirebaseUser();
				UserNames userNames = new UserNames();
				try
				{
					userNames = user.getNames(userId);
				}
				catch
				{
					userNames = new UserNames()
					{
						first = "",
						last = ""
					};
				}
				if(userNames.first == null)
				{
					userNames = new UserNames()
					{
						first = "",
						last = ""
					};
				}
				RegisterModel registerModel = new RegisterModel(yodleeModel.loginName, userNames);

				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Add("Api-Version", "1.1"); 
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.token.accessToken);
				StringContent content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync(url + "/user/register", content);
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
			authReturn = await authReturn.GetToken("");
			return authReturn;
		}
		private class YodleeAuthReturn
		{
			public YodleeToken token { get; set; }
			public YodleeAuthReturn() { }
			public async Task<YodleeAuthReturn> GetToken(string loginName)
			{
				YodleeTokenGenerator tokenGenerator = new YodleeTokenGenerator();
				string token = await tokenGenerator.CreateToken(loginName);
				YodleeAuthReturn authReturn = new YodleeAuthReturn();
				YodleeToken yodleeToken = new YodleeToken();
				yodleeToken.accessToken = token;
				authReturn.token = yodleeToken;
				return authReturn;
				//HttpClient client = new HttpClient();
				//client.DefaultRequestHeaders.Add("loginName", loginName);
				//client.DefaultRequestHeaders.Add("Api-Version", "1.1");
				//var dict = new Dictionary<string, string>();
				//dict.Add("clientId", ClientID);
				//dict.Add("secret", secret);
				//dict.Add("Content-Type", "application/x-www-form-urlencoded");
				//var content = new FormUrlEncodedContent(dict);
				//HttpResponseMessage response = await client.PostAsync(url + "/auth/token", content);
				//if (response.IsSuccessStatusCode)
				//{
				//	string tokenStr = await response.Content.ReadAsStringAsync();
				//	YodleeAuthReturn responseToken = JsonConvert.DeserializeObject<YodleeAuthReturn>(tokenStr);
				//	return responseToken;
				//}
				//else
				//{
				//	string tokenStr = await response.Content.ReadAsStringAsync();
				//	ExceptionCatcher exceptionCatcher = new ExceptionCatcher();
				//	exceptionCatcher.Catch(tokenStr);
				//}
				//return null;
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
