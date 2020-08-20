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
		/// <summary>
		/// Saves user interaction
		/// </summary>
		/// <param name="location">Name of the function being called</param>
		/// <param name="get">Boolean, Get</param>
		/// <param name="post">Boolean, Post</param>
		/// <param name="data">Data passed</param>
		/// <param name="userId">User that is interaction</param>
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
