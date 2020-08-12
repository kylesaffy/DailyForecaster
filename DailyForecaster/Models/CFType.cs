using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	/// <summary>
	/// This is the name of the cash flow type as well as whether or not this is a a custom or default
	/// </summary>
	public class CFType
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public bool Custom { get; set; }
		[Required]
		public string Name { get; set; }
		public string ClientReference { get; set; }
		public int YodleeId { get; set; }
		public int YodleeSGId { get; set; }
		public bool Infaltion { get; set; }
		public virtual ICollection<BudgetTransaction> BudgetTransactions { get; set; }
		/// <summary>
		/// Returns a list of CFTYpes for that collection
		/// </summary>
		/// <param name="collectionsId">Collection ID which is pulling the request</param>
		/// <returns>Returns a list of all CFTYpes applicatble to that collection</returns>
		public List<CFType> GetCFList(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.CFTypes.Where(x=>x.ClientReference == collectionsId || x.Custom == false ).OrderBy(x=>x.Name).ToList();
			}
		}
		/// <summary>
		///    Returns a list of CFTYpes for a group collections
		/// </summary>
		/// <param name="collectionsIds"> A List of Collection ID's which is pulling the request</param>
		/// <returns>Returns a list of all CFTYpes applicatble to those collections</returns>
		public List<CFType> GetCFList(List<string> collectionsIds)
		{
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				List<CFType> type = _context
					.CFTypes
					.Where(x =>	x.Custom == false)
					.ToList();
				type.AddRange(_context
					.CFTypes
					.Where(t => collectionsIds.Contains(t.ClientReference))
					.ToList()
					);
				return type;
			}
		}
		public CFType() { }
		/// <summary>
		/// Constructor returning the CFType of a particular CFType
		/// </summary>
		/// <param name="id">Either the Id or the name of a CFType</param>
		public CFType(string id)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				try
				{
					CFType temp = _context.CFTypes.Find(id);
					Id = temp.Id;
					Custom = temp.Custom;
					Name = temp.Name;
					ClientReference = temp.ClientReference;
					if(temp == null)
					{
						temp = _context.CFTypes.Where(x => x.Name == id).Where(x => x.Custom == false).FirstOrDefault();
						Id = temp.Id;
						Custom = temp.Custom;
						Name = temp.Name;
						ClientReference = temp.ClientReference;
					}
				}
				catch
				{
					try
					{
						CFType temp = _context.CFTypes.Where(x => x.Name == id).Where(x => x.Custom == false).FirstOrDefault();
						Id = temp.Id;
						Custom = temp.Custom;
						Name = temp.Name;
						ClientReference = temp.ClientReference;
					}
					catch
					{
						Id = "";
						Custom = false;
						Name = "";
						ClientReference = "";
					}
				}
			}
		}
		/// <summary>
		/// Add a custom CFType to the dataset
		/// </summary>
		/// <param name="collectionsId">Id of the collection creating the custom type</param>
		/// <param name="name">The custom name that is being provided</param>
		/// <returns>The new CFType that has been created</returns>
		public CFType CreateCFType(string collectionsId, string name)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				CFType type = new CFType();
				type.Id = Guid.NewGuid().ToString();
				type.Custom = true;
				type.Name = name;
				type.Infaltion = true;
				type.ClientReference = collectionsId;
				_context.CFTypes.Add(type);
				_context.SaveChanges();
				return type;
			}
		}
	}
}
