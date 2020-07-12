using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class YodleeTransactionModel
	{
		static private string url = "https://development.api.yodlee.com/ysl";
		public List<YodleeTransactionLevel> transaction { get; set; }
		public async Task<List<YodleeTransactionLevel>> GetYodleeTransactions(string collectionsId,string token)
		{
			YodleeTransactionModel transactions = new YodleeTransactionModel();
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Api-Version", "1.1");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = await client.GetAsync(url + "/transactions");
			if (response.IsSuccessStatusCode)
			{
				string str = await response.Content.ReadAsStringAsync();
				transactions = JsonConvert.DeserializeObject<YodleeTransactionModel>(str);
			}
			return transactions.transaction;
		}
	}
	public class YodleeTransactionLevel
	{
		public string CONTAINER { get; set; }
		public int id { get; set; }
		public YodleeBalance amount { get; set; }
		public string baseType { get; set; }
		public string categoryType { get; set; }
		public int categoryId { get; set; }
		public string category { get; set; }
		public string categorySource { get; set; }
		public int highLevelCategoryId { get; set; }
		public DateTime createdDate { get; set; }
		public DateTime lastUpdated { get; set; }
		public YodleeDescription description { get; set; }
		public bool isManual { get; set; }
		public string sourceType { get; set; }
		public DateTime date { get; set; }		
		public DateTime transactionDate { get; set; }
		public string status { get; set; }
		public int accountId { get; set; }
		public YodleeBalance runningBalance { get; set; }
		public YodleeMerchant merchant { get; set; }
	}
	public class YodleeDescription
	{
		public string description { get; set; }
		public string original { get; set; }
		public string simple { get; set; }
	}
	public class YodleeMerchant
	{
		public int id { get; set; }
		public string name { get; set; }
		public string source { get; set; }
	}
}
