using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class YodleeTransactionType
	{
		public List<YodleeTransactionCatepgory>	transactionCatepgory { get; set; }
		static private string url = "https://development.api.yodlee.com/ysl";
		public async Task<List<YodleeTransactionCatepgory>> GetYodleeTransactions(string token)
		{
			YodleeTransactionType yodleeCat = new YodleeTransactionType();
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Api-Version", "1.1");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = await client.GetAsync(url + "/transactions");
			if (response.IsSuccessStatusCode)
			{
				string str = await response.Content.ReadAsStringAsync();
				yodleeCat = JsonConvert.DeserializeObject<YodleeTransactionType>(str);
			}
			return yodleeCat.transactionCatepgory;
		}
		public async Task<List<CFType>>	YodleeTransform(string token,string collectionsId)
		{
			CFType type = new CFType();
			List<CFType> currentList = type.GetCFList(collectionsId);
			List<CFType> types = new List<CFType>();
			List<YodleeTransactionCatepgory> yodleeTransactions = await GetYodleeTransactions(token);
			foreach(YodleeTransactionCatepgory item in yodleeTransactions)
			{	  				
				if(currentList.Where(x=>x.YodleeId == item.id).Any())
				{
					types.Add(currentList.Where(x=>x.YodleeId == item.id).FirstOrDefault());
				}
				else
				{
					types.Add(new CFType { 
						Id = "39a1d903-f4e3-4e4a-986a-604bd8dff20e",
						Name = "Uncategorized",
						Custom = false,
						YodleeId = item.id,
						YodleeSGId = item.id
					});
				}
				foreach(YodleeSubCategory sub in item.detailCategory)
				{
					if (currentList.Where(x => x.YodleeId == item.id).Any())
					{
						types.Add(currentList.Where(x => x.YodleeId == item.id).FirstOrDefault());
					}
					else
					{
						types.Add(new CFType
						{
							Id = "39a1d903-f4e3-4e4a-986a-604bd8dff20e",
							Name = "Uncategorized",
							Custom = false,
							YodleeId = item.id,
							YodleeSGId = sub.id
						});
					}
				}
			}
			return types;
		}
	}
	public class YodleeTransactionCatepgory
	{

		public int id { get; set; }
		public string source { get; set; }
		public string category { get; set; }
		public string type { get; set; }
		public int hgighLevelCategory { get; set; }
		public string highLevelCategoryId { get; set; }
		public List<YodleeSubCategory> detailCategory { get; set; } 
	}
	public class YodleeSubCategory
	{
		public int id { get; set; }
		public string name { get; set; }
	}
}
