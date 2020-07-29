using DailyForecaster.Migrations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class YodleeAccountModel
	{
		static private string url = "https://development.api.yodlee.com/ysl";
		public List<YodleeAccountLevel>	account { get; set; }
		public async Task<List<YodleeAccountLevel>> GetYodleeAccounts(string collectionsId)
		{
			YodleeAccountModel accounts = new YodleeAccountModel();
			YodleeModel yodlee = new YodleeModel();
			string token = await yodlee.getToken(collectionsId, "");
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Api-Version", "1.1");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = await client.GetAsync(url + "/accounts");
			if (response.IsSuccessStatusCode)
			{
				string str = await response.Content.ReadAsStringAsync();
				accounts = JsonConvert.DeserializeObject<YodleeAccountModel>(str);
			}
			return accounts.account;
		}
		public async Task<bool> UpdateYodlee()
		{
			Collections collection = new Collections();
			YodleeModel yodlee = new YodleeModel();
			List<Collections> collections = collection.GetCollections("", "");
			bool result = true;
			foreach(string id in collections.Where(x=>x.Accounts.Count()>0).Select(x=>x.CollectionsId))
			//foreach(string id in collections.Where(x=>x.CollectionsId == "f687f366-d162-4a04-89c7-8e0ad123f9cf").Select(x=>x.CollectionsId))
			{
				if(!result)
				{
					return result;
				}
				string token = await yodlee.getToken(id, "");
				List<string> providers = await GetProviders(token);
				foreach(string provider in providers.Distinct())
				{
					HttpClient client = new HttpClient();
					client.DefaultRequestHeaders.Add("Api-Version", "1.1");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
					HttpResponseMessage response = await client.PutAsync(url + "/providerAccounts?providerAccountIds=" + provider,new StringContent(""));
					if(!response.IsSuccessStatusCode)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}
		public async Task<List<string>> GetProviders(string token)
		{
			List<string> providers = new List<string>();
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Api-Version", "1.1");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = await client.GetAsync(url + "/accounts");
			if (response.IsSuccessStatusCode)
			{
				string str = await response.Content.ReadAsStringAsync();
				YodleeAccountModel accounts = JsonConvert.DeserializeObject<YodleeAccountModel>(str);
				if (accounts.account != null)
				{
					foreach (YodleeAccountLevel item in accounts.account)
					{
						providers.Add(item.providerAccountId.ToString());
					}
				}
			}
			return providers;
		}
	}
	public class YodleeAccountLevel
	{
		public string CONTAINER { get; set; }
		public int providerAccountId { get; set; }
		public string accountName { get; set; }
		public string accountStatus { get; set; }
		public bool isAsset { get; set; }
		public string aggregationSource { get; set; }
		public int id { get; set; }
		public DateTime lastUpdated { get; set; }
		public string providerId { get; set; }
		public string providerName { get; set; }
		public string accountType { get; set; }
		public bool isManual { get; set; }
		public DateTime createdDate { get; set; }
		public bool includeInNetWorth { get; set; }
		public string displayedName { get; set; }
		public string accountNumber { get; set; }
		public YodleeBalance currentBalance { get; set; }
		public YodleeBalance balance { get; set; }
		public YodleeBalance cash { get; set; }
		public YodleeBalance runningBalance { get; set; }
		public YodleeBalance availableCredit { get; set; }
		public YodleeBalance availableBalance { get; set; }
		public List<YodleeDataSet> dataset { get; set; }
		
	}
	public class YodleeBalance
	{
		public double amount { get; set; }
		public string currency { get; set; }
	}
	public class YodleeDataSet
	{
		public string name { get; set; }
		public string additionalStatus { get; set; }
		public string updateEligibility { get; set; }
		public DateTime lastUpdated { get; set; }
		public DateTime lastUpdateAttempt { get; set; }
		public DateTime nextUpdateScheduled { get; set; }
	}
}
