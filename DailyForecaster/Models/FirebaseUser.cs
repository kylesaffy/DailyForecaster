using Microsoft.Toolkit.Extensions;
using Security.Cryptography;
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
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ProfileImage { get; set; }
		public ICollection<FirebaseLogin> FirebaseLogins { get; set; }
		public ICollection<BudgetTransaction> BudgetTransactions { get; set; }
		public ICollection<UserCollectionMapping> UserCollectionMappings { get; set; }
		public ICollection<Collections> Collections { get; set; }
		public FirebaseUser() { }
		/// <summary>
		/// Returns an exisiting user object
		/// </summary>
		/// <param name="userId">MM side userId</param>
		public FirebaseUser(string userId)
		{
			FirebaseUser user = new FirebaseUser();
			try
			{
				user = Get(new Guid(userId));
			}
			catch
			{
				user = Get(userId);
			}
			FirebaseUserId = user.FirebaseUserId;
			FirebaseId = user.FirebaseId;
			Email = user.Email;
			Phone = user.Phone;
			ProfileImage = user.ProfileImage;
		}
		public FirebaseUser GetUser(string uid)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.FirebaseUser.Where(x => x.FirebaseId == uid).FirstOrDefault();
			}
		}
		public UserNames getNames(string userId)
		{
			FirebaseUser user = Get(userId);
			return new UserNames()
			{ 
				first = user.FirstName,
				last = user.LastName
			};

		}
		public FirebaseUser(string email, string id)
		{
			FirebaseUserId = Guid.NewGuid().ToString();
			FirebaseId = id;
			Email = email;
			Save();
		}
		/// <summary>
		/// Create a new user object from server side user creation
		/// </summary>
		/// <param name="model">UserModel that is passed to from the front end</param>
		public FirebaseUser(UserModel model)
		{
			FirebaseUserId = Guid.NewGuid().ToString();
			FirebaseId = model.Uid;
			Email = model.Email;
			FirstName = model.FirstName;
			LastName = model.LastName;
			Phone = model.PhoneNumber;
			Save();
			EmailPreferences preferences = new EmailPreferences();
			preferences.Create(FirebaseUserId);
		}
		public void Update(string uid,FirebaseUser user)
		{
			FirebaseUser old = GetUser(uid);
			old.FirstName = user.FirstName;
			old.LastName = user.LastName;
			old.Phone = user.Phone;
			old.ProfileImage = user.ProfileImage;
			old.Update();
		}
		public void Update(string ImageLoc, string uid)
		{
			FirebaseUser user = GetUser(uid);
			user.ProfileImage = ImageLoc;
			Update(uid, user);
		}

		/// <summary>
		/// Tests if user exists
		/// </summary>
		/// <param name="email">Email address of the user</param>
		/// <returns>A bool of the existance of the user</returns>
		public bool Exsits(string email)
		{
			return Any(email);
		}
		/// <summary>
		///  Get function for a single user
		/// </summary>
		/// <param name="email">the email address of the user interacting with the system</param>
		/// <returns>Returns the Id of the user</returns>
		public string GetUserId(string email)
		{
			try
			{
				return Get(email).FirebaseUserId;
			}
			catch
			{
				AspNetUsers users = new AspNetUsers();
				return users.getUserId(email);
			}
		}
		/// <summary>
		/// List of all users in the Firebase environment
		/// </summary>
		/// <returns>Returns a list of all firebase users</returns>
		public List<FirebaseUser> GetUserList()
		{
			return Get();
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
		private void Update()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
				_context.SaveChanges();
			}
		}
		/// <summary>
		/// Data Layer Application for the retrieval of users by email or Id
		/// </summary>
		/// <param name="email">Email address of the user</param>
		/// <returns>Returns a single instance of a user</returns>
		private FirebaseUser Get(string email)
		{

			using(FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					if (email.IsEmail())
					{
						return _context.FirebaseUser.Where(x => x.Email == email).FirstOrDefault();
					}
					else
					{
						return _context.FirebaseUser.Where(x => x.FirebaseId == email).FirstOrDefault();
					}
				}
				catch (Exception e)
				{
					return null;
				}
			}
		}
		/// <summary>
		/// Data Layer Application for the retrieval of users by UserId 
		/// </summary>
		/// <param name="userId">UserId of the user</param>
		/// <returns>Returns a single instance of a user</returns>
		private FirebaseUser Get(Guid userId)
		{
			string UserId = userId.ToString();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.FirebaseUser.Find(UserId);
			}
		}
		/// <summary>
		/// List of all users in the Firebase environment 
		/// </summary>
		/// <returns>Returns a list of all firebase users</returns>
		private List<FirebaseUser> Get()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.FirebaseUser.ToList();
			}
		}
		/// <summary>
		/// Tests if user exists in database
		/// </summary>
		/// <param name="email">Email address of the user</param>
		/// <returns>A bool of the existance of the user in the db</returns>
		private bool Any(string email)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.FirebaseUser.Where(x => x.Email == email).Any();
			}
		}
	}
}
