using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class EmailPreferences
	{
		public string EmailPreferencesId { get; set; }
		[ForeignKey("FirebaseUserId")]
		public string FirebaseUserId { get; set; }
		public FirebaseUser FirebaseUser { get; set; }
		public DateTime LastInteraction { get; set; }
		public string InteractionRecord { get; set; }
		public bool LoginNotification { get; set; }
		public bool DailyCommunication { get; set; }
		public EmailPreferences() { }
		private EmailPreferences(string id)
		{
			EmailPreferencesId = Guid.NewGuid().ToString();
			FirebaseUserId = id;
			LoginNotification = true;
			DailyCommunication = true;
		}
		public void CheckPrefrences(FirebaseUser user)
		{
			if(!Check(user.FirebaseUserId))
			{
				Create(user.FirebaseUserId);
			}
		}
		public bool CheckDaily(FirebaseUser user)
		{
			return Get(user.FirebaseUserId).DailyCommunication;
		}
		public EmailPreferences Get(FirebaseUser user)
		{
			return Get(user.FirebaseUserId);
		}
		public void Create(string id)
		{
			EmailPreferences preferences = new EmailPreferences(id);
			preferences.Save();
		}
		private void Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Add(this);
				_context.SaveChanges();
			}
		}
		/// <summary>
		/// Get Email preference object by FirebaseUserId
		/// </summary>
		/// <param name="id">FirebaseUserId</param>
		/// <returns>Single Email Prefence Object</returns>
		private EmailPreferences Get(string id)
		{							
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.EmailPrefernces.Where(x => x.FirebaseUserId == id).FirstOrDefault();
			}
			
		}
		/// <summary>
		/// Get Email preference object by Id
		/// </summary>
		/// <param name="id">Object Id</param>
		/// <returns>Single Email Prefence Object</returns>
		private EmailPreferences GetById(string id)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.EmailPrefernces.Find(id);
			}

		}
		public bool Check(string id)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.EmailPrefernces.Where(x => x.FirebaseUserId == id).Any();
			}
		}
		public EmailPreferences Update()
		{
			EmailPreferences oldPreferences = GetById(this.EmailPreferencesId);
			oldPreferences.DailyCommunication = this.DailyCommunication;
			oldPreferences.LoginNotification = this.LoginNotification;
			SaveChanges();
			return Get(this.EmailPreferencesId);
		}
		private void SaveChanges()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Entry(this).State = EntityState.Modified;
				_context.SaveChanges();
			}
		}
	}
}
