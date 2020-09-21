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
		public bool Firebase { get; set; }
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
			Firebase = false;
			Save(this);
		}
		public ClickTracker(string location, bool get, bool post, string data, string userId,bool part)
		{
			ClickTrackerId = Guid.NewGuid().ToString();
			// AspNetUsers user = new AspNetUsers();
			UserId = userId;
			Location = location;
			GET = get;
			POST = post;
			RecordDateTime = DateTime.Now;
			RequestData = data;
			Firebase = part;
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
		/// <summary>
		/// Checks the activity of all users, and if needed loggs the user off
		/// </summary>
		/// <returns>If successful returns true</returns>
		public async Task<bool> ActivityTracker()
		{
			FirebaseUser firebase = new FirebaseUser();
			List<FirebaseUser> users = firebase.GetUserList();
			DateTime current = DateTime.Now.ToUniversalTime();
			bool result = false;
			foreach(FirebaseUser user in users)
			{
				ClickTracker tracker = Get(user.FirebaseUserId);
				if (tracker != null)
				{
					DateTime recordDateTime = tracker.RecordDateTime.AddMinutes(15).ToUniversalTime();
					if (current > recordDateTime)
					{
						LogoffModel model = new LogoffModel();
						result = await model.LogOff(user.FirebaseUserId, user.FirebaseId, tracker);
					}
				}
				else
				{
					LogoffModel model = new LogoffModel();
					model.LogOff(user.FirebaseUserId, user.FirebaseId, tracker);
				}
			}
			return result;
		}
		//===================================================================================================================
		//DLA
		//===================================================================================================================
		/// <summary>
		/// Retrieve the latest user interaction
		/// </summary>
		/// <param name="userId">Firebase UserId</param>
		/// <returns>Clicktrack object of the latest interaction that is recorded by the backend</returns>
		private ClickTracker Get(string userId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					return _context.ClickTracker.Where(x => x.UserId == userId).OrderByDescending(x => x.RecordDateTime).FirstOrDefault();
				}
				catch (Exception e)
				{
					return null;
				}
			}
		}
	}
}
