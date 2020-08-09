using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class FirebaseUser
	{
		public string FirebaseUserId { get; set; }
		public string FirebaseId { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public ICollection<FirebaseLogin> FirebaseLogins { get; set; }
		public ICollection<BudgetTransaction> BudgetTransactions { get; set; }
		public ICollection<UserCollectionMapping> UserCollectionMappings { get; set; }
		public ICollection<Collections> Collections { get; set; }
		public FirebaseUser() { }
		public FirebaseUser(string email, string id)
		{
			FirebaseUserId = Guid.NewGuid().ToString();
			FirebaseId = id;
			Email = email;
			Save();

		}
		/// <summary>
		///  Get function for a single user
		/// </summary>
		/// <param name="email">the email address of the user interacting with the system</param>
		/// <returns>Returns the Id of the user</returns>
		public string GetFirebaseUser(string email)
		{
			return Get(email).FirebaseUserId;
		}
		//===================================================================================================================
		//DLA
		//===================================================================================================================
		/// <summary>
		/// Data Layer Application for saving new users
		/// </summary>
		private void Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Add(this);
				_context.SaveChanges();
			}
		}
		/// <summary>
		/// Data Layer Application for the retrieval of users
		/// </summary>
		/// <param name="email">Email address of the user</param>
		/// <returns>Returns a single instance of a user</returns>
		private FirebaseUser Get(string email)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.FirebaseUser.Where(x => x.Email == email).FirstOrDefault();
			}
		}
	}
}
