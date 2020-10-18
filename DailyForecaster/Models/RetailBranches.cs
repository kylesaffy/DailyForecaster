using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class RetailBranches
	{
		public string RetailBranchesId { get; set; }
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
		public string ShopNumber { get; set; }
		public string Complex { get; set; }
		public string StreetNumber { get; set; }
		public string StreetName { get; set; }
		public string Suburb { get; set; }
		public string City { get; set; }
		public int PostalCode { get; set; }
		public string GoogleAddress { get; set; }
		public string RetailMerchantsId { get; set; }
		[ForeignKey("RetailMerchantsId")]
		public RetailMerchants RetailMerchants { get; set; }
		/// <summary>
		/// Finds a retail branch nd a retail merchant by names
		/// </summary>
		/// <param name="merchantName">Name of the merchant</param>
		/// <param name="branch">string that contains the name of the branch</param>
		/// <returns>a RetaiBranch object</returns>
		public RetailBranches Get(string merchantName,string branch)
		{
			RetailMerchants merchant = new RetailMerchants();
			merchant = merchant.Get(merchantName);
			List<RetailBranches> branches = new List<RetailBranches>();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				branches = _context.RetailBranches.Where(x => x.RetailMerchantsId == merchant.RetailMerchantsId).ToList();
			}
			RetailBranches retailBranch = NameFinder(branches, branch);
			retailBranch.RetailMerchants = merchant;
			return retailBranch;
		}
		/// <summary>
		/// Finds the branch name by decreasing the branch string until a match is found
		/// </summary>
		/// <param name="branches">List of branches belonging to a defined merchant</param>
		/// <param name="branch">string that contains the name of the merchant</param>
		/// <returns>a RetailBranch object</returns>
		public RetailBranches NameFinder(List<RetailBranches> branches, string branch)
		{
			RetailBranches retailBranches = new RetailBranches();
			for(int i = branch.Length;i>0; i--)
			{
				if(branches.Where(x=>x.Name.ToLower() == branch.Substring(0,i).ToLower()).Any())
				{
					retailBranches = branches.Where(x => x.Name == branch.Substring(0, i)).FirstOrDefault();
					break;
				}
			}
			return retailBranches;
		}
	}
}
