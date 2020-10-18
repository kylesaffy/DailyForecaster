using DailyForecaster.Controllers;
using Org.BouncyCastle. Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
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
using System.IO;
using System.Net;
using Org.BouncyCastle.Crypto;

namespace DailyForecaster.Models
{
	public class YodleeTokenGenerator
	{
		private static string url = "https://stage.api.yodlee.uk/ysl/";
		public async Task<string> CreateToken(string loginName)
		{
			//Generate key pair
			RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048);
			var rsaKeyPair = DotNetUtilities.GetRsaKeyPair(RSA);
			var writer = new StringWriter();
			var pemWriter = new PemWriter(writer);
			pemWriter.WriteObject(rsaKeyPair.Public);
			//pemWriter.WriteObject(rsaKeyPair.Private);
			string publicKey = writer.ToString();
			string issuerId = await getKey(publicKey);
			long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
			var payload = new Dictionary<string, object>();
			payload["iss"] = issuerId;
			payload["iat"] = currentTime;
			payload["exp"] = currentTime + 1800;
			if (loginName != null && loginName != "")
			{
				payload["sub"] = loginName;
			}
			RSAParameters rsaParams;
			writer = new StringWriter();
			pemWriter = new PemWriter(writer);
			pemWriter.WriteObject(rsaKeyPair.Private);
			using (var streamReader = new StringReader(writer.ToString()))
			//string path = @"C:\privateKey.pem";
			//var file = new FileInfo(path);
			//using (var streamReader = file.OpenText())
			{
				var pemReader = new PemReader(streamReader);

				RsaPrivateCrtKeyParameters privkey = null;

				AsymmetricCipherKeyPair keyPair;

				try
				{
					var pem = new PemReader(streamReader);
					var o = pem.ReadObject();
					keyPair = (AsymmetricCipherKeyPair)o;
					//privkey = (RsaPrivateCrtKeyParameters)o;

					privkey = (RsaPrivateCrtKeyParameters)keyPair.Private;

					rsaParams = DotNetUtilities.ToRSAParameters(privkey);
					using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
					{
						rsa.ImportParameters(rsaParams);
						try
						{
							string jwt = Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS512);
							return jwt;
						}
						catch (Exception e)
						{
							return e.InnerException.Message;
						}
					}
				}
				catch (Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
					return null;
				} 				
			}
			
		}
		private async Task<string> getKey(string key)
		{
			var byteArray = Encoding.ASCII.GetBytes(coBrand.cobrandLogin + ":" + coBrand.cobrandPassword);
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
			client.DefaultRequestHeaders.Add("Cobrand-Name", "private-moneyminders-stage");
			client.DefaultRequestHeaders.Add("Api-Version", "1.1");
			StringContent content = new StringContent(JsonConvert.SerializeObject(new APIKey() { publicKey = key }), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync(url + "/auth/apiKey", content);
			if(response.IsSuccessStatusCode)
			{
				string tokenStr = await response.Content.ReadAsStringAsync();
				APIKeyModel responseToken = JsonConvert.DeserializeObject<APIKeyModel>(tokenStr);
				return responseToken.apiKey[0].key;
			}
			return null;
		}
		public async Task<string> GetUserId(string collectionId)
		{
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
		}
	}
	public class YodleeSessionGenerator
	{
		private static string url = "https://stage.api.yodlee.uk/ysl/";
		public async Task<string> GetSession()
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://storageaccountmoney9367.blob.core.windows.net/cloud-store/privateKey.pem");
			HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
			string key = "";
			using (var streamReader = new StreamReader(resp.GetResponseStream()))
			{
				key = await streamReader.ReadToEndAsync();
			}  				
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
		}
		public string CreateToken(string collectionsId)
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
			RSAParameters rsaParams;
			using var fs = new FileStream("https://storageaccountmoney9367.blob.core.windows.net/cloud-store/privateKey.txt", FileMode.Open, FileAccess.Read);
			using(var streamReader = new StreamReader(fs, Encoding.UTF8))
			{
				var pemReader = new PemReader(streamReader);

				RsaPrivateCrtKeyParameters privkey = null;
				Object obj = pemReader.ReadObject();
				if (obj != null)
				{
					privkey = (RsaPrivateCrtKeyParameters)obj;
				}
				rsaParams = DotNetUtilities.ToRSAParameters(privkey);
			}
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
			{
				rsa.ImportParameters(rsaParams);

				return Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS512);
			}
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
	public class APIKey
	{
		public string publicKey { get; set; }
	}
	public class YodleeAPIKey
	{
		public string expiresIn { get; set; }
		public string createdDate { get; set; }
		public string publicKey { get; set; }
		public string key { get; set; }
	}
	public class APIKeyModel
	{
		public List<YodleeAPIKey> apiKey { get; set; }
	}

}
