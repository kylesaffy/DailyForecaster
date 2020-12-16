using FirebaseAdmin.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class LogoffModel
	{
		public string LogoffModelId { get; set; }
		[ForeignKey("FirebaseUserId")]
		public string FirebaseUserId { get; set; }
		public FirebaseUser FirebaseUser { get; set; }
		public DateTime DateTime { get; set; }
		public LogoffModel() { }
		/// <summary>
		/// Checks if a user needs to be logged off and logs the user off if needed
		/// </summary>
		/// <param name="userId">MM Id of the user</param>
		/// <param name="tracker">Latest ClickTracker object recorded by MM</param>
		public async Task<bool> LogOff(string userId, string fireBaseId, ClickTracker tracker)
		{
			try
			{ 
				LogoffModel model = Get(userId);
				if (model != null)
				{
					if (model.DateTime.ToUniversalTime() < tracker.RecordDateTime.ToUniversalTime())
					{
						//FirebaseUser user = new FirebaseUser(userId);
						LogoffModel logoffModel = new LogoffModel()
						{
							LogoffModelId = Guid.NewGuid().ToString(),
							FirebaseUserId = userId,
							DateTime = await RevokeToken(fireBaseId),
						};
						logoffModel.Save();
					}
				}
				else
				{
					LogoffModel logoffModel = new LogoffModel()
					{
						LogoffModelId = Guid.NewGuid().ToString(),
						FirebaseUserId = userId,
						DateTime = await RevokeToken(fireBaseId),
					};
					logoffModel.Save();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		/// Revokes the token of a given user
		/// </summary>
		/// <param name="userId">The Firebase Id of the user</param>
		/// <returns>DateTime that ther revocation of the token occured</returns>
		public async Task<DateTime> RevokeToken(string userId)
		{
			await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(userId);
			var user = await FirebaseAuth.DefaultInstance.GetUserAsync(userId);
			return user.TokensValidAfterTimestamp;
		}
		//===================================================================================================================
		//DLA
		//===================================================================================================================
		/// <summary>
		/// Retireives the latest Logoff of a user
		/// </summary>
		/// <param name="userId">Id of the user that is being requested</param>
		/// <returns>A single instance of the most recent LogOffModel</returns>
		private LogoffModel Get(string userId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					return _context.LogoffModel.Where(x => x.FirebaseUserId == userId).OrderByDescending(x => x.DateTime).FirstOrDefault();
				}
				catch
				{
					return null;
				}
			}
		}
		/// <summary>
		/// Saves an instance of the LogOffModel
		/// </summary>
		private void Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Add(this);
				_context.SaveChanges();
			}
		}
	}
}
