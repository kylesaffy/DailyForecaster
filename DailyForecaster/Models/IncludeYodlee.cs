﻿using System;
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
		private List<string> GetCollections(string uid)
		{
			FirebaseUser user = new FirebaseUser(uid);
			UserCollectionMapping mapping = new UserCollectionMapping();
			return mapping.getCollectionIds(user.FirebaseUserId, "firebase");
		}
		public void Check(string uid)
		{
			Check(GetCollections(uid));			
		}
		public bool isIncluded(string uid)
		{
			int counter = 0;
			Check(uid);
			List<string> collectionIds = GetCollections(uid);
			foreach(string item in collectionIds)
			{
				if(Get(item).Included)
				{
					counter++;
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
