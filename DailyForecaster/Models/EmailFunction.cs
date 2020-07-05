using DailyForecaster.Controllers;
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
		private static string password = ",3{hE]&WRP*5";
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
			try
			{
				client.Send(message);
			}
			catch(SmtpException e)
			{
				string error = e.Message;
			}
		}
		
	}
}
