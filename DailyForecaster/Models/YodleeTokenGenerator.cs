using DailyForecaster.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Headers;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DailyForecaster.Models
{
	public class YodleeTokenGenerator
	{
		
	}
	public class YodleeSessionGenerator
	{
		private static string url = "https://stage.api.yodlee.uk/ysl/";
		public async Task<string> GetSession()
		{
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Cobrand-Name", "private-moneyminders-stage");
			client.DefaultRequestHeaders.Add("Api-Version", "1.1");
			StringContent content = new StringContent(JsonConvert.SerializeObject(CoBrandModel.ModelReturn()), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync(url + "/cobrand/login", content);
			if(response.IsSuccessStatusCode)
			{
				string tokenStr = await response.Content.ReadAsStringAsync();
				CoBrandSessionModel responseToken = JsonConvert.DeserializeObject<CoBrandSessionModel>(tokenStr);
				return responseToken.session.cobSession;
			}
			return null;
		}
		public async Task<string> GetUserId(string collectionId)
		{
			string session = await GetSession();
			try
			{
				YodleeModel yodlee = new YodleeModel(collectionId, "Get");
				return yodlee.loginName;
			}
			catch
			{
				YodleeModel yodlee = new YodleeModel(collectionId, "New");
				return yodlee.loginName;
			}
			return null;

		}
		public async Task<string> CreateToken(string collectionsId)
		{
			long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
			var payload = new Dictionary<string, object>();
			payload["iss"] = "9510020248";
			payload["iat"] = currentTime;
			payload["exp"] = currentTime + 1800;
			if(collectionsId != null)
			{
				payload["sub"] = GetUserId(collectionsId);
			}
			RSAParameters resParams;
			//using(var )
			return null;

		}

	}
	public class CoBrandSessionModel
	{
		public int cobrandId { get; set; }
		public string applicationId { get; set; }
		public string locale { get; set; }
		public YodleeSession session { get; set; }
	}
	public class YodleeSession
	{
		public string cobSession { get; set; }
	}
	public class CoBrandModel
	{
		public coBrand cobrand { get; set; }
		public static CoBrandModel ModelReturn()
		{
			CoBrandModel model = new CoBrandModel()
			{
				cobrand = new coBrand()
			};
			return model;
		}

	}
	public class coBrand
	{
		public static string cobrandLogin = "moneyminders-stage";
		public static string cobrandPassword = "HYS@kggcb56784";
	}
}
