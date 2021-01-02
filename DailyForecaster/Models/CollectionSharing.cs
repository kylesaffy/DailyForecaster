using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class CollectionSharing
	{
		public string CollectionSharingId { get; set; }
		public string CollectionId { get; set; }
		public int count { get; private set; }
		public CollectionSharing() { }
		public CollectionSharing(string collectionId)
		{
			CollectionSharingId = Guid.NewGuid().ToString();
			CollectionId = collectionId;
			count = 0;
			SavedCollection(this);
		}
		private void CountIncrament()
		{
			count++;
		}
		private void SavedCollection(CollectionSharing collectionSharing)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				_context.CollectionSharing.Add(collectionSharing);
				_context.SaveChanges();
			}
		}
		
		private CollectionSharing ShareCounter(string id)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				CollectionSharing sharing = _context.CollectionSharing.Find(id);
				return sharing;
			}
		}
		/// <summary>
		/// Add user and collection to collection mapping table
		/// </summary>
		/// <param name="obj"> New Collection Object containing user eamil as obj.user and collectionsharing id as obj.name</param>
		/// <returns></returns>
		public bool AddUserToCollection(NewCollectionsObj obj)
		{
		// 3 checks
			// 1 - is count == 0
			CollectionSharing sharing = ShareCounter(obj.name);
			// 2 - is the user different from the other users-- already mapped into UserCollectionsMapping
				UserCollectionMapping mapping = new UserCollectionMapping(sharing.CollectionId, obj.User);
			if (sharing.count == 0 && mapping.CheckUser())
			{
				// set the counter to +1
				sharing.CountIncrament();
				try
				{
					using(FinPlannerContext _context = new FinPlannerContext())
					{
						_context.Entry(sharing).State = EntityState.Modified;
						_context.UserCollectionMapping.Add(mapping);
						_context.SaveChanges();
					}
					return true;
				}
				catch(Exception e)
				{
					return false;
				}
			}
			else
			{
				return false;
			}

		}
		public string AddUserToCollection(string uid, string sharingId)
		{
			// 3 checks
			// 1 - is count == 0
			CollectionSharing sharing = ShareCounter(sharingId);
			// 2 - is the user different from the other users-- already mapped into UserCollectionsMapping
			UserCollectionMapping mapping = new UserCollectionMapping(sharing.CollectionId, uid);
			if (sharing.count == 0 && mapping.CheckUser())
			{
				// set the counter to +1
				sharing.CountIncrament();

				try
				{
					using (FinPlannerContext _context = new FinPlannerContext())
					{
						_context.Entry(sharing).State = EntityState.Modified;
						_context.UserCollectionMapping.Add(mapping);
						_context.SaveChanges();
					}
					return sharing.CollectionId;
				}
				catch (Exception e)
				{
					ExceptionCatcher catcher = new ExceptionCatcher();
					catcher.Catch(e);
					return null;
				}
			}
			else
			{
				return null;
			}

		}
	}
}
