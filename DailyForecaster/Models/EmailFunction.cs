using DailyForecaster.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
		public ReturnModel SendEmail()
		{
			try
			{
				SendEmail(this.To, this.Body, this.Subject);
				return new ReturnModel() { result = true };
			}
			catch (Exception e)
			{
				return new ReturnModel() { result = false, returnStr = e.Message };
			}
		}
		private void SendEmail(string ToEmail, string Message, string Subject)
		{
			SmtpClient client = new SmtpClient(SMTPServer, port)
			{
				UseDefaultCredentials = false,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Credentials = new NetworkCredential(FromEmail, password),
				EnableSsl = true,
				Timeout = 200000
			};
			MailAddress from = new MailAddress(FromEmail, "Admin", System.Text.Encoding.UTF8);
			MailAddress to = new MailAddress(ToEmail);
			MailMessage message = new MailMessage(from, to);
			message.Body = Message;
			message.Subject = Subject;
			message.IsBodyHtml = true;
			message.SubjectEncoding = System.Text.Encoding.UTF8;
			EmailStore emailStore = new EmailStore() {
				Body = Message,
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
