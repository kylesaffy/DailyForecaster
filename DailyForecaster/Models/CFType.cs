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
		public virtual ICollection<BudgetTransaction> BudgetTransactions { get; set; }
		public List<CFType> GetCFList(string collectionsId)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context.CFTypes.Where(x=>x.ClientReference == collectionsId || x.Custom == false).OrderBy(x=>x.Name).ToList();
			}
		}
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
		public CFType(string id)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				CFType temp = _context.CFTypes.Find(id);
				Id = temp.Id;
				Custom = temp.Custom;
				Name = temp.Name;
				ClientReference = temp.ClientReference;
			}
		}
		public CFType CreateCFType(string collectionsId, string name)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				CFType type = new CFType();
				type.Id = Guid.NewGuid().ToString();
				type.Custom = true;
				type.Name = name;
				type.ClientReference = collectionsId;
				_context.CFTypes.Add(type);
				_context.SaveChanges();
				return type;
			}
		}
	}
}
