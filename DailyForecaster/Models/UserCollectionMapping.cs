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
		[ForeignKey("FirebaseUserID")]
		public FirebaseUser FirebaseUser { get; set; }
		public string Id { get; set; }
		[ForeignKey("Id")]
		public AspNetUsers AspNetUsers { get; set; }
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
		/// <param name="type">Either firebase or asp</param>
		/// <returns>Returns a string of the id's of the collections to which a single user is associated</returns>
		public List<string> getCollectionIds(string userId, string type)
		{
			switch(type)
			{
				case "firebase":
					using(FinPlannerContext _context = new FinPlannerContext())
					{
						return _context.UserCollectionMapping.Where(x => x.FirebaseUserId == userId).Select(x => x.CollectionsId).ToList();
					}
				case "asp":
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						return _context.UserCollectionMapping.Where(x => x.Id == userId).Select(x => x.CollectionsId).ToList();
					}
				default:
					return null;
			}
		}
		public UserCollectionMapping (string collectionsId,string userId)
		{
			try
			{
				AspNetUsers user = new AspNetUsers();
				userId = user.getUserId(userId);
			}
			catch
			{
				FirebaseUser user = new FirebaseUser();
				userId = user.GetFirebaseUser(userId);
			}
			if (CheckUser(collectionsId, userId))
			{
				UserCollectionMappingId = Guid.NewGuid().ToString();
				CollectionsId = collectionsId;
				Id = userId;
			}
			else
			{
				UserCollectionMappingId = "999";
				CollectionsId = "999";
				Id = "999";
			}
		}
		public int CollectionsCounter(string userId)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				AspNetUsers user = new AspNetUsers();
				string id = user.getUserId(userId);
				return _context.UserCollectionMapping.Where(x => x.Id == id).Count();
			}
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
				foreach(UserCollectionMapping item in mappings)
				{
					if(item.Id == userId || item.FirebaseUserId == userId)
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
			if (Id == "999")
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
						if (item.Id == Id||item.FirebaseUserId == Id)
						{
							ans = false;
							break;
						}
					}
				}
			}
			return ans;
		}
	}
}
