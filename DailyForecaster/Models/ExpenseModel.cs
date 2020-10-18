using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ExpenseModel
	{
		public string ExpenseModelId { get; set; }
		public string BlobLink { get; set; }
		public string ManualCashFlowId { get; set; }
		[ForeignKey("ManualCashFlowId")]
		public ManualCashFlow ManualCashFlow { get; set; }
		public string RetailBranchesId { get; set; }
		[ForeignKey("RetailBranchesId")]
		public RetailBranches RetailBranches { get; set; }
		public double Total { get; set; }
		public DateTime Date { get; set; }
		public List<ItemisedProducts> ItemisedProducts { get; set; }
		public async Task<ExpenseModel> Create(string url)
		{
			RunReader reader = new RunReader();
			reader = await reader.GetRunReader(url);
			foreach(string item in reader.Results)
			{
				switch(item)
				{
					case "Checkers":
						return Checkers(reader, url);
				}
			}
			return null;
		}
		private ExpenseModel Checkers(RunReader reader, string url)
		{
			ExpenseModel model = new ExpenseModel();
			int i = 0;
			while(reader.Results[i].ToLower() == "ins" || reader.Results[i].ToLower() == "del")
			{
				i++;
			}
			string merchant = reader.Results[i];
			i++;
			string branch = reader.Results[i];
			RetailBranches retailBranch = new RetailBranches();
			model.RetailBranches = retailBranch.Get(merchant, branch);
			model.RetailBranchesId = retailBranch.RetailBranchesId;
			model.BlobLink = url;
			model.ExpenseModelId = Guid.NewGuid().ToString();
			model.ItemisedProducts = new List<ItemisedProducts>();
			while(reader.Results[i].Substring(0,3).ToLower() != "tax")
			{ i++; }
			while(reader.Results[i].ToLower() != "total")
			{
				double temp;
			   if(double.TryParse(reader.Results[i+1].Substring(1),out temp))
				{
					double amount = Convert.ToDouble(reader.Results[i + 1].Substring(1));
					double savings = 0;
					if (reader.Results[i + 2].Length > 8)
					{
						if (reader.Results[i + 2].Substring(0, 8).ToUpper() == "XTRASAVE")
						{
							savings = Convert.ToDouble(reader.Results[i + 3].Substring(2));
						}
					}
					else if (reader.Results[i - 1].Length > 15)
					{
						if (reader.Results[i - 1].Substring(0, 14).ToUpper() == "ORIGINAL PRICE")
						{
							savings = Convert.ToDouble(reader.Results[i - 1].Substring(15).ToLower()) - amount;
						}
					}
					model.ItemisedProducts.Add(new ItemisedProducts(reader.Results[i], amount, savings, 1));
				}
				i++;
			}
			i++;
			model.Total = Convert.ToDouble(reader.Results[i].Substring(1));
			return model;
		}
	}
}