using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ClickTracker
	{
		public string ClickTrackerId { get; set; }
		public string Location { get; set; }
		public bool GET { get; set; }
		public bool POST { get; set; }
		public DateTime RecordDateTime { get; set; }
		public string RequestData { get; set; }
		public string UserId { get; set; }
		public ClickTracker() { }
		public ClickTracker(string location, bool get, bool post,string data,string userId)
		{
			ClickTrackerId = Guid.NewGuid().ToString();
			AspNetUsers user = new AspNetUsers();
			UserId = user.getUserId(userId);
			Location = location;
			GET = get;
			POST = post;
			RecordDateTime = DateTime.Now;
			RequestData = data;
			Save(this);
		}
		private void Save(ClickTracker tracker)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.ClickTracker.Add(tracker);
				_context.SaveChanges();
			}
		}
	}
}
