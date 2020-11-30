using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class SmartHelper
	{
		public string Html { get; set; }
		public SmartHelper() { }
		public string WelcomePage()
		{
			string text = "";
			var webRequest = WebRequest.Create(@"https://storageaccountmoney9367.blob.core.windows.net/cloud-store/WelcomePage.html");
			using (var response = webRequest.GetResponse())
			using (var content = response.GetResponseStream())
			using (var reader = new StreamReader(content))
			{
				text = reader.ReadToEnd();
			};
			return text;
		}
		public string NewToAccounts()
		{
			string text = "";
			var webRequest = WebRequest.Create(@"https://storageaccountmoney9367.blob.core.windows.net/cloud-store/NewToAccounts.html");
			using (var response = webRequest.GetResponse())
			using (var content = response.GetResponseStream())
			using (var reader = new StreamReader(content))
			{
				text = reader.ReadToEnd();
			};
			return text;
		}
		public SmartHelper(string uid)
		{
			UserCollectionMapping mapping = new UserCollectionMapping();
			List<string> collectionIds = new List<string>();
			FirebaseUser user = new FirebaseUser();
			user = user.GetUser(uid);
			collectionIds = mapping.getCollectionIds(user.FirebaseUserId, "firebase");
			if(collectionIds.Count ==0)
			{
				Html = "<div><h4>First thing is first! MoneyMinders works on the notion of Collections:</h4><p>By Collections we mean a collection of accounts that you as the user sets. Collections contain the budgets, accounts, and all of your transactions on those accounts. Collections<b> cannot</b> talk to each other. They are completely isolated entities. You as the creater of a collection can choose to share a specific collection with someone else. Each share code can only be used once. After that if you want to share the collection with another person you will need to create another sharing link. While on that note, you can share a collection with as many people as you want. We do inform you of which of your collections are shared by other users.</p><p> We will display this icon on your home page if the collection is shared.</p><div><i class='fe fe-users font-size-50'></i></div></br><p>If the collection is only seen by you then you will see this icon</p><div><i class='fe fe-user font-size-50'></i></div><p>You can create as many collections as you want as well.</p><p>If you feel that something is not clear or that you need more information from us, please feel free to email us on<a href= 'mailto:admin@moneyminders.co.za' > admin@moneyminders</a>.We love hearing feedback from you and are excited to share our ideas with you.</p><p>In orer to create a collection, please click on the Homepage or New Collection on the left menu, otherwise if you aren't sure where these are you can click <a href='#/dashboard/NewCollection'><u>here</u></a></p></div>";
			}
			else
			{
				Collections collection = new Collections();
				Account account = new Account();
				Budget budget = new Budget();
				List<Account> accounts = new List<Account>();
				List<Collections> collections = collection.GetEagerList(collectionIds);
				bool check = false;
				foreach(Collections item in collections)
				{
					accounts.AddRange(account.GetAccounts(item.CollectionsId));
					if(accounts.Count == 0)
					{
						Html = "<div><h4>Well done! You have created a collection!</h4><p> Now onto the next step, let's add <b>Accounts</b> to your " + item.Name;
						Html = Html + " Collection</p><p>Now we need to add your accounts!</p><p>This is a two part process;<ol><li>The first part is required in order to ensure that any transaction that occurs in the collection can be assigned to an account. Because all accounts behave differently we need as much information in order to ensure that we can as closely approximate your financial position as possible. Accounts can be added <a href='#/dashboard/AddAccount'><u>here</u></a></p></li><li>The second is to link these accounts through our automated linking facility. This allows our third party provider to <b>only</b> read your transactional data. From there our propriatry algorithms link and map these transactions only to the accounts that you have linked to this collection. This feature is highly recommended! Please click <a href='#/dashboard/AutomatedLink'><u>here</u></a> in order to start this process</li></ol></div>";
						check = true;
						break;
					}
				}
				if(!check)
				{
					foreach (Collections item in collections)
					{
						if (budget.BudgetCheck(item.CollectionsId))
						{
							Html = "<div><h4>We are so glad to see that you are so committed to this journey!</h4><p>We recognize that everyone has their own plan for their money as is their right! You worked hard for your money and therefore have the right to spend it the way you want to!</p><p><b>However</b>, Benjamin Franklin said it well when he said \"<i>If you fail to plan, you are planning to fail</i>.\"</p><p>We at MoneyMinders have found this to true in our own lives, and in fact was our motivation for building this!</p><p>So lets head over to the Budget page and put together a budget in order to benchmark your spending of that hard earned cash versus what you expected to spend it on.</p><p>Let's create a budget for your <b>" + item.Name + "</b> collection. Click <a href='#/dashboard/budget'><u>here</u></a> for quick acsess</p></div>";
							check = true;
							break;
						}
						//if (accounts.Where(x=>x.YodleeId == 0).Any())
						//{
						//	string acc = "";
						//	if (accounts.Count > 1)
						//	{
						//		acc = "accounts";
						//	}
						//	else
						//	{
						//		acc = "an account";
						//	}
						//	Html = "<div><h4>Great Job on adding " + acc + " to your collection!</h4><p>Did you know that we can safely and securely scan your accounts on a daily basis to check what transactions have gone through?</p><p>By partnering with a global leader in FinTech we are able to ensure that your bank accounts safety is at front of mind! With over 27 million users you can rest assured that your data is in safe hands!</p><p>This process will allow us to gather all the information that we need, rather than you capturing each transaction in the maticulous detail that we need.</p></div>";
						//	check = true;
						//}
						//else
						//{
						//	Html = "<div><h4>We are so glad to see that you are so committed to this journey!</h4><p>We recognize that everyone has their own plan for their money as is their right! You worked hard for your money and therefore have the right to spend it the way you want to!</p><p><b>However</b>, Benjamin Franklin said it well when he said \"<i>If you fail to plan, you are planning to fail</i>.\"</p><p>We at MoneyMinders have found this to true in our own lives, and in fact was our motivation for building this!</p><p>So lets head over to the Budget page and put together a budget in order to benchmark your spending of that hard earned cash versus what you expected to spend it on.</p></div>";
						//}
					}
					if(!check)
					{

					}
				}
			}

		}
	}
}
