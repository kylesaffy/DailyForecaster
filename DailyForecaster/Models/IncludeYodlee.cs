using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class IncludeYodlee
	{
		public string IncludeYodleeId { get; set; }
		public string CollectionsId { get; set; }
		[ForeignKey("CollectionsId")]
		public Collections Collections { get; set; }
		public bool Included { get; set; }
		public IncludeYodlee() { }
		public IncludeYodlee (string collectionsId, bool included)
		{
			IncludeYodleeId = Guid.NewGuid().ToString();
			CollectionsId = collectionsId;
			Included = included;
		}
		public IncludeYodlee (string collectionsId)
		{
			IncludeYodlee yodlee = Get(collectionsId);
			IncludeYodleeId = yodlee.IncludeYodleeId;
			Included = yodlee.Included;
			CollectionsId = yodlee.CollectionsId;
		}
		public async Task<bool> Update(bool included,string userId)
		{
			ReturnModel returnModel = new ReturnModel();
			YodleeModel model = new YodleeModel();
			if (included)
			{
				returnModel = await model.Register(userId, this.CollectionsId);
			}
			else
			{
				returnModel = await model.Unregister(this.CollectionsId);
			}
			if(returnModel.result)
			{
				if(!included)
				{
					Account account = new Account();
					if(account.ResetYodlee(this.CollectionsId))
					{
						this.Included = included;
						Update();
					}
				}
				else
				{
					this.Included = included;
					Update();
				}
				
			}
			return returnModel.result;
		}
		private void Update()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
				_context.SaveChanges();
			}
		}
		private List<string> GetCollections(string uid)
		{
			FirebaseUser user = new FirebaseUser(uid);
			UserCollectionMapping mapping = new UserCollectionMapping();
			return mapping.getCollectionIds(user.FirebaseUserId, "firebase");
		}
		public bool Exists(string uid)
		{
			return Exisits(GetCollections(uid));
		}
		public void Check(string uid)
		{
			Check(GetCollections(uid));			
		}
		public bool isIncluded(string uid)
		{
			int counter = 0;
			//Check(uid);
			List<string> collectionIds = GetCollections(uid);
			foreach(string item in collectionIds)
			{
				if (Exisits(item))
				{
					if (Get(item).Included)
					{
						counter++;
					}
				}
			}
			if(counter > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		private void Check(List<string> collectionIds)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				foreach (string item in collectionIds)
				{
					if (!_context.IncludeYodlee.Where(x => x.CollectionsId == item).Any())
					{
						Create(item);
					}
				}
			}
		}
		private bool Exisits(string collectionId)
		{
			int count = 0;
			using (FinPlannerContext _context = new FinPlannerContext())
			{						
				if (_context.IncludeYodlee.Where(x => x.CollectionsId == collectionId).Any())
				{
					count++;
				}
			}
			if (count > 0) return true;
			else return false;
		}
		private bool Exisits(List<string> collectionIds)
		{
			int count = 0;
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				foreach (string item in collectionIds)
				{
					if (_context.IncludeYodlee.Where(x => x.CollectionsId == item).Any())
					{
						count++;
					}
				}
			}
			if (count > 0) return true;
			else return false;
		}
		public void Create(string collectionsId, bool included = true)
		{
			IncludeYodlee include = new IncludeYodlee(collectionsId, included);
			include.Save();
		}
		private void Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Add(this);
				_context.SaveChanges();
			}
		}
		private IncludeYodlee Get(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.IncludeYodlee.Where(x => x.CollectionsId == collectionsId).FirstOrDefault();
			}
		}
	}
}
