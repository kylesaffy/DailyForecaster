using DailyForecaster.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class EmailFunction
	{
		private static string FromEmail = "admin@moneyminders.co.za";
		private static string SMTPServer = "mail.moneyminders.co.za";
		private static int port = 587;
		private static string password = "j^NBx+pqX@BD";
		public string Body { get; set; }
		public string To { get; set; }
		public string Subject { get; set; }
		public ReturnModel SendEmail(string Name)
		{
			try
			{
				SendEmail(this.To, this.Body, this.Subject,Name);
				return new ReturnModel() { result = true };
			}
			catch (Exception e)
			{
				return new ReturnModel() { result = false, returnStr = e.Message };
			}
		}
		public void EmailError(string type, string url  = null, string product = null)
		{
			string to = "kylesaffy@moneyminders.co.za";
			string body = "";
			if (url != null)
			{
				body = "There was an error with the reading following url:" + Environment.NewLine + url;
			}
			else
			{
				body = "There was an error with the reading following product: " + Environment.NewLine + product;
			}
			string subject = "Error with reading of slip of type: " + type;
			//SendEmail(to, body, subject, "Kyle");
		}
		public ReturnModel DailyEmailSend(string userId)
		{
			try
			{
				DailyReporting daily = new DailyReporting(userId);

				string text = "";
				var webRequest = WebRequest.Create(@"https://storageaccountmoney9367.blob.core.windows.net/emailimages/imported-from-beefreeio.html");
				using (var response = webRequest.GetResponse())
				using (var content = response.GetResponseStream())
				using (var reader = new StreamReader(content))
				{
					text = reader.ReadToEnd();
				};
				//message.Body = text.Replace("[guest_name]", Name);
				string transactionsStr = "";
				CFType type = new CFType();
				List<CFType> types = type.GetCFList(daily.CollectionIds);
				if (daily.AutomatedCashFlows.Count > 0)
				{
					foreach (AutomatedCashFlow item in daily.AutomatedCashFlows)
					{
						item.CFType = types.Where(x => x.Id == item.CFTypeId).FirstOrDefault();
						transactionsStr = transactionsStr + "</br></br><p><b>Source</b>: " + item.SourceOfExpense + " - <b>Category</b> " + item.CFType.Name + " - <b>R</b> " + item.Amount.ToString("N2") + "</p>";
					}
				}
				else
				{
					transactionsStr = "No new transactions were picked up";
				}
				text = text.Replace("{transactions}", transactionsStr);
				text = text.Replace("{Last Login}", daily.LastInteraction.ToLongDateString());
				text = text.Replace("{Title}", daily.DailyTip.Header);
				text = text.Replace("{Body}", daily.DailyTip.Quote);
				text = text.Replace("{motivational}", daily.DailyMotivational.URL);
				SendEmail(daily.Email, text, "Your Daily Grind", daily.FirstName);
				return new ReturnModel() { result = true };
			}
			catch (Exception e)
			{
				return new ReturnModel() { result = false, returnStr = e.Message };
			}
		}
		private void SendEmail(string ToEmail, string Message, string Subject,string Name)
		{
			SmtpClient client = new SmtpClient(SMTPServer, port)
			{
				UseDefaultCredentials = false,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Credentials = new NetworkCredential(FromEmail, password),
				EnableSsl = true,
				Timeout = 200000
			};
			MailAddress from = new MailAddress(FromEmail, "MoneyMinders", Encoding.UTF8);
			MailAddress to = new MailAddress(ToEmail);
			MailMessage message = new MailMessage(from, to);
			if (Subject == "Confirm your account")
			{
				string text = "";
				var webRequest = WebRequest.Create(@"https://storageaccountmoney9367.blob.core.windows.net/emailimages/EmailValidation.txt");
				using(var response = webRequest.GetResponse())
				using(var content = response.GetResponseStream())
				using (var reader = new StreamReader(content))
				{
					text = reader.ReadToEnd();
				};					
				text = text.Replace("www.google.com", Message);
				message.Body = text.Replace("[guest_name]", Name);

			}
			else if (Subject == "Login")
			{
				string text = "";
				var webRequest = WebRequest.Create(@"https://storageaccountmoney9367.blob.core.windows.net/emailimages/Login.txt");
				using (var response = webRequest.GetResponse())
				using (var content = response.GetResponseStream())
				using (var reader = new StreamReader(content))
				{
					text = reader.ReadToEnd();
				};
			}
			else
			{
				message.Body = Message;
			}
			message.Subject = Subject;
			message.IsBodyHtml = true;
			message.SubjectEncoding = Encoding.UTF8;
			EmailStore emailStore = new EmailStore() {
				Body = message.Body,
				To = to.ToString(),
				From = from.ToString(),
				EmailDate = DateTime.Now,
				EmailStoreId = Guid.NewGuid().ToString(),
				Subject = Subject
			};
			emailStore.Save();
			try
			{						 
				client.Send(message);
			}
			catch(SmtpException e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e.Message);
			}
		}
		
	}
	public class EmailStore
	{
		public string EmailStoreId { get; set; }
		public string To { get; set; }
		public string From { get; set; }
		public string Body { get; set; }
		public DateTime EmailDate { get; set; }
		public string Subject { get; set; }
		public void Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.EmailStore.Add(this);
				try
				{
					_context.SaveChanges();
				}
				catch(Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e.Message);
				}
			}
		}
	}
}
