using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class FirebaseLogin
	{
		public string FirebaseLoginId { get; set; }
		public string FirebaseUserId { get; set; }
		[ForeignKey("FirebaseUserId")]
		public FirebaseUser FirebaseUser {get;set;}
		public DateTime LoginDate { get; set; }
		public FirebaseLogin() { }
		public FirebaseLogin(string Id)
		{
			FirebaseLoginId = Guid.NewGuid().ToString();
			FirebaseUserId = Id;
			LoginDate = DateTime.Now;
		}
		private void Save()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Add(this);
				_context.SaveChanges();
			}
		}
	}
}
