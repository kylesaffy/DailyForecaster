using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class UserInteraction
	{
		public string UserInteractionId { get; set; }
		public string AppAreasId { get; set; }
		[ForeignKey("AppAreasId")]
		public AppAreas AppAreas { get; set; }
		public string FirebaseUserId { get; set; }
		[ForeignKey("FirebaseUserId")]
		public FirebaseUser FirebaseUser { get; set; }
		public DateTime Date { get; set; }
		public int Count { get; set; }
		public string AreaObejctId { get; set; }
		public string GetCollectionId(string email)
		{
			FirebaseUser user = new FirebaseUser();
			string userId = user.GetFirebaseUser(email);
			AppAreas app = AppAreas.GetAppAreas().Where(x => x.Name == "Collections").FirstOrDefault();
			try
			{
				return GetUserInteraction(userId, app.AppAreasId).AreaObejctId;
			}
			catch
			{
				Collections collections = new Collections();
				return collections.GetId(userId);
			}
		}
		/// <summary>
		/// Adds an increment and a userInteraction object to the database
		/// </summary>
		/// <param name="collectionsId">collactions Id that is being incremented</param>
		/// <param name="email">email address of the user that the collection is bveing incremented against</param>
		public void CollectionsIncratment(string collectionsId,string email)
		{
			FirebaseUser user = new FirebaseUser();
			string userId = user.GetFirebaseUser(email);
			AppAreas app = AppAreas.GetAppAreas().Where(x => x.Name == "Collections").FirstOrDefault();
			Incrament(collectionsId, userId, app);
		}
		/// <summary>
		///  Returns the most recenet User Intereation object based on the App Area Id and the User Id
		/// </summary>
		/// <param name="Id">Id of the object in the area that the user is engaging</param>
		/// <param name="userId">Id of the user that is engaging</param>
		/// <param name="app">App object that is being engaged</param>
		private void Incrament(string Id, string userId, AppAreas app)
		{
			Save(app.AppAreasId, userId, GetUserInteraction(Id, userId, app.AppAreasId).Count, Id);
		}
		private UserInteraction GetUserInteraction(string collectionsId,string userId,string appAreaId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				UserInteraction interaction = _context
					.UserInteraction
					.Where(x => x.FirebaseUserId == userId && x.AreaObejctId == collectionsId)
					.Where(x => x.AppAreasId == appAreaId)
					.OrderByDescending(x => x.Count)
					.FirstOrDefault();
				if (interaction == null)
				{
					interaction = new UserInteraction()
					{
						Count = 0
					};
				}
				return interaction;
			}
	}
		/// <summary>
		/// Returns the most recenet User Intereation object based on the App Area Id and the User Id
		/// </summary>
		/// <param name="userId">Id of the User in question</param>
		/// <param name="appAreaId">Area of the App that is being queried</param>
		/// <returns>The most recent UserInteraction Object</returns>
		private UserInteraction GetUserInteraction(string userId, string appAreaId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				UserInteraction interaction = _context
					.UserInteraction
					.Where(x => x.FirebaseUserId == userId)
					.Where(x => x.AppAreasId == appAreaId)
					.OrderByDescending(x => x.Count)
					.FirstOrDefault();
				if(interaction == null)
				{
					interaction = new UserInteraction()
					{
						Count = 0
					};
				}
				return interaction;
			}					   		
			
		}
		/// <summary>
		/// Saves a new UserInteraction Object
		/// </summary>
		/// <param name="appAreasId">The area that the user is interacting with</param>
		/// <param name="userId">The Id of the user that is interacting</param>
		/// <param name="count">Count of the last interaction</param>
		/// <param name="Id">Id of the object that is being interacted with</param>
		private void Save(string appAreasId, string userId,int count,string Id)
		{
			UserInteraction userInteraction = new UserInteraction()
			{
				AppAreasId = appAreasId,
				Count = count + 1,
				AreaObejctId = Id,
				Date = DateTime.Now,
				FirebaseUserId = userId,
				UserInteractionId = Guid.NewGuid().ToString()
			};
			try
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					_context.Add(userInteraction);
					_context.SaveChanges();
				}
			}
			catch (Exception e)
			{
				ExceptionCatcher catcher = new ExceptionCatcher();
				catcher.Catch(e.Message);
			}
		}
	}
}
