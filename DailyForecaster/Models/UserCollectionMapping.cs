using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class UserCollectionMapping
	{
		[Required]
		[Key]
		public string UserCollectionMappingId { get; set; }
		[Required]
		public string CollectionsId { get; set; }
		public Collections Collections { get; set; }
		public string FirebaseUserId { get; set; }
		[ForeignKey("FirebaseUserId")]
		public FirebaseUser FirebaseUser { get; set; }
		public bool IsDelete { get; set; }
		public UserCollectionMapping() { }
		/// <summary>
		/// Checks if the collection is linked to more than one user
		/// </summary>
		/// <param name="collectionsId">The unique ID of the collection</param>
		/// <returns>Returns a bool, with true if more than one user is linked to the collection provided</returns>
		public bool IsShared(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
					.UserCollectionMapping
					.Where(x => x.CollectionsId == collectionsId)
					.Count() > 1;
			}
		}
		/// <summary>
		/// Responsible for returning the mapping of a single user to the collections to which they are assigned
		/// </summary>
		/// <param name="userId">The unique ID of the user</param>
		/// <param name="type">firebase only accepted</param>
		/// <returns>Returns a string of the id's of the collections to which a single user is associated</returns>
		public List<string> getCollectionIds(string userId, string type)
		{
			switch(type)
			{
				case "firebase":
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						return _context.UserCollectionMapping.Where(x => x.FirebaseUserId == userId).Select(x => x.CollectionsId).ToList();
					}
				//case "asp":
				//	using (FinPlannerContext _context = new FinPlannerContext())
				//	{
				//		return _context.UserCollectionMapping.Where(x => x.Id == userId).Where(x=>x.IsDelete == false).Select(x => x.CollectionsId).ToList();
				//	}
				default:
					return null;
			}
		}
		public UserCollectionMapping (string collectionsId,string userId)
		{
			string userIdNew = null;
			FirebaseUser user = new FirebaseUser();
			userIdNew = user.GetUserId(userId);
			if(userIdNew == null)
			{
				userId = user.GetUserId(userId);
				FirebaseUserId = userId;
			}
			else
			{
				userId = userIdNew;
			}
			if (CheckUser(collectionsId, userId))
			{
				UserCollectionMappingId = Guid.NewGuid().ToString();
				CollectionsId = collectionsId;
				FirebaseUserId = userId;
			}
			else
			{
				UserCollectionMappingId = "999";
				CollectionsId = "999";
			}
		}
		public int CollectionsCounter(string userId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				FirebaseUser user = new FirebaseUser();
				string id = user.GetUser(userId).FirebaseUserId;
				return _context.UserCollectionMapping.Where(x => x.FirebaseUserId == id).Count();
			}
		}
		public bool Check(string uid)
		{
			FirebaseUser user = new FirebaseUser(uid);
			if (getCollectionIds(user.FirebaseUserId, "firebase").Count() > 0) return true;
			else return false;
		}
		/// <summary>
		/// Check if the user already belong to this collection
		/// </summary>
		/// <param name="collectionId">Id of the collection in question, typically obj.name</param>
		/// <param name="userId">email of the user in question typically obj.user</param>
		/// <returns>true of false</returns>
		public bool CheckUser(string collectionId, string userId)
		{
			bool ans = true;
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				List<UserCollectionMapping> mappings = _context.UserCollectionMapping.Where(x=>x.CollectionsId == collectionId).ToList();
				//start off by assuming that the user is not linked
				//AspNetUsers aspNetUsers = new AspNetUsers();
				//userId = aspNetUsers.getUserId(userId);
				foreach (UserCollectionMapping item in mappings)
				{
					if (item.FirebaseUserId == userId || item.FirebaseUserId == userId)
					{
						ans = false;
						break;
					}
				}
			}
			return ans;
		}
		public bool CheckUser()
		{
			bool ans = true;
			if (FirebaseUserId == "999")
			{
				ans = false;
			}
			else
			{
				using (FinPlannerContext _context = new FinPlannerContext())
				{
					List<UserCollectionMapping> mappings = _context.UserCollectionMapping.Where(x => x.CollectionsId == CollectionsId).ToList();
					//start off by assuming that the user is not linked
					//AspNetUsers aspNetUsers = new AspNetUsers();
					//userId = aspNetUsers.getUserId(userId);
					foreach (UserCollectionMapping item in mappings)
					{
						if (item.FirebaseUserId == FirebaseUserId || item.FirebaseUserId == FirebaseUserId)
						{
							ans = false;
							break;
						}
					}
				}
			}
			return ans;
		}
		public void Delete(string collectionsId)
		{
			List<UserCollectionMapping> mapping = new List<UserCollectionMapping>();
			mapping = GetCollection(collectionsId);
			foreach (UserCollectionMapping item in mapping)
			{
				item.Delete();
			}
		}
		private List<UserCollectionMapping> GetCollection(string collectionId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
					.UserCollectionMapping
					.Where(x => x.CollectionsId == collectionId)
					.ToList();
			}
		}
		private void Delete()
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Remove(this);
				_context.SaveChanges();
			}
		}
	}
}
