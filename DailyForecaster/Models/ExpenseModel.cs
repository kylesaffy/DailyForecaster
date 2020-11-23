using Azure.Storage.Blobs.Models;
using DailyForecaster.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace DailyForecaster.Models
{
	[Serializable]
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
		
		public async Task<ReturnModel> BuildPartial(string url, string accountId, string user, string cftypeId)
		{
			ExpenseModel model = await CreatePartial(url);
			if (model != null)
			{
				if (model.BlobLink != "Error")
				{
					if (model.BlobLink != "Merchant not found")
					{
						CFClassification classification = new CFClassification();
						CFType type = new CFType(cftypeId);
						List<CFClassification> classifications = classification.GetList();
						ManualCashFlow cf = new ManualCashFlow(type, classifications.Where(x => x.Sign == -1).FirstOrDefault(), model.Total, DateTime.Now, model.RetailBranches.Name, user, true, model.RetailBranches.RetailMerchants.Name, accountId);
						cf.CFClassification = null;
						cf.Save();
						model.ManualCashFlowId = cf.Id;
						model.Date = DateTime.Now;
						model.RetailBranchesId = model.RetailBranches.RetailBranchesId;
						model.RetailBranches = null;
						ExpenseModel saveModel = model.DeepClone();
						saveModel.Save();
						return new ReturnModel() { result = true };
					}
					else
					{
						return new ReturnModel() { result = false, returnStr = "Merchant not found, we are investigating this merchant and will update our system. We will inform you once this has been corrected. We will create this transaction for you" };
					}
				}
			}
			return new ReturnModel() { result = false, returnStr = "Oops something went wrong, please try a different pricture" };

		}
		public async Task<ReturnModel> Build(string url, string accountId,string user)
		{
			ExpenseModel model = await Create(url);
			if (model != null)
			{
				if (model.BlobLink != "Error")
				{
					if (model.BlobLink != "Merchant not found")
					{
						if (model.ItemisedProducts.Where(x => x.ItemisedProductsId != null).Any())
						{
							CFType type = Weighting(model);
							CFClassification classification = new CFClassification();
							List<CFClassification> classifications = classification.GetList();
							ManualCashFlow cf = new ManualCashFlow(type, classifications.Where(x => x.Sign == -1).FirstOrDefault(), model.Total, DateTime.Now, model.RetailBranches.Name, user, true, model.RetailBranches.RetailMerchants.Name, accountId);
							cf.CFClassification = null;
							cf.Save();
							model.ManualCashFlowId = cf.Id;
							model.Date = DateTime.Now;
							model.RetailBranchesId = model.RetailBranches.RetailBranchesId;
							model.RetailBranches = null;
							foreach (ItemisedProducts item in model.ItemisedProducts.Where(x => x.ItemisedProductsId != null))
							{
								item.ExpenseModelId = model.ExpenseModelId;
								item.ProductsModel = null;
							}
							ExpenseModel saveModel = model.DeepClone();
							saveModel.ItemisedProducts = saveModel.ItemisedProducts.Where(x => x.ItemisedProductsId != null).ToList();
							saveModel.Save();
						}
						List<ItemisedProducts> items = new List<ItemisedProducts>();
						if (model.ItemisedProducts.Where(x => x.ItemisedProductsId == null).Any())
						{
							items = model.ItemisedProducts.Where(x => x.ItemisedProductsId == null).ToList();
						}
						return new ReturnModel() { result = true, products = items };
					}
					else
					{
						return new ReturnModel() { result = false, returnStr = "Merchant not found, we are investigating this merchant and will update our system. We will inform you once this has been corrected. We will create this transaction for you" };
					}
				}
			}
			return new ReturnModel() { result = false, returnStr = "Oops something went wrong, please try a different pricture" };

		}
		public CFType Weighting(ExpenseModel expense)
		{
			List<ProductClassModel> classModel = expense.ItemisedProducts.Where(x=>x.ItemisedProductsId != null).Select(y => y.ProductsModel.ProductClassModel).Distinct().ToList();
			List<ExpenseWeighting> weightings = new List<ExpenseWeighting>();
			CFType type = new CFType();
			List<CFType> types = type.GetCFList();
			foreach(ItemisedProducts item in expense.ItemisedProducts.Where(x=>x.ItemisedProductsId !=null))
			{
				weightings.Add(new ExpenseWeighting()
				{
					Amount = item.Amount,
					CFType = types.Where(x => x.Id == item.ProductsModel.ProductClassModel.CFTypeId).FirstOrDefault()
			});
			}
			weightings = weightings
				.GroupBy(x => x.CFType)
				.Select(y => new ExpenseWeighting
				{
					CFType = y.Key,
					Amount = y.Sum(z => z.Amount)
				}).ToList();
			return weightings.OrderBy(x => x.Amount).Select(x => x.CFType).First();

		}
		public async Task<ExpenseModel> CreatePartial(string url)
		{
			RunReader reader = new RunReader();
			reader = await reader.GetRunReader(url);
			if (reader == null)
			{
				return new ExpenseModel() { BlobLink = "Error" };
			}
			bool checker = false;
			foreach (string item in reader.Results)
			{
				switch (item)
				{
					case "DUE VAT INCL":
						return PicknPayPartial(reader, url);
				}
			}
			if (!checker)
			{
				EmailFunction email = new EmailFunction();
				email.EmailError("Merchant not found", url);
			}
			return null;
		}
		private ExpenseModel PicknPayPartial(RunReader reader, string url)
		{
			ExpenseModel model = new ExpenseModel();
			int i = 0;
			while (reader.Results[i] != "DUE VAT INCL")
			{
				i++;
			}
			model.BlobLink = url;
			model.Date = DateTime.Now;
			model.Total = Convert.ToDouble(reader.Results[i + 1]);
			RetailBranches retailBranch = new RetailBranches();
			model.RetailBranches = retailBranch.Get("Pickn Pay", "General");
			model.ExpenseModelId = Guid.NewGuid().ToString();
			return model;
		}
		public async Task<ExpenseModel> Create(string url)
		{
			RunReader reader = new RunReader();
			reader = await reader.GetRunReader(url);
			if(reader == null)
			{
				return new ExpenseModel() { BlobLink = "Error" };
			}
			bool checker = false;
			foreach(string item in reader.Results)
			{
				switch(item.ToUpper())
				{
					case "Checkers":
						return Checkers(reader, url);
					case "Pickn Pay":
						return PicknPay(reader, url);
					case "SUPERSPAR":
						return SuperSpar(reader, url);
				}
			}
			if(!checker)
			{
				EmailFunction email = new EmailFunction();
				email.EmailError("Merchant not found", url);
			}
			return null;
		}
		private ExpenseModel SuperSpar(RunReader reader,string url)
		{
			ExpenseModel model = new ExpenseModel();
			RetailBranches retailBranch = new RetailBranches();
			int i = 1;
			while (reader.Results[i].ToLower() == "ins" || reader.Results[i].ToLower() == "del")
			{
				i++;
			}
			string merchant = reader.Results[i];
			i++;
			string branch = reader.Results[i];
			model.RetailBranches = retailBranch.Get(merchant, branch);
			while(reader.Results[i].ToLower() != "r")
			{
				i++;
			}
			i++;
			while(reader.Results[i].ToLower() != "total")
			{
				double temp;
				double amount = 0;
				double number = 1;
				double savings = 0;
				string productName = "";
				if((double.TryParse(reader.Results[i+1], out temp) || double.TryParse(reader.Results[i + 1].Substring(0,reader.Results[i+1].Length-2), out temp)) && reader.Results[i + 1].Substring(reader.Results[i + 1].Length - 2,1).ToLower() == "n" )
				{

				}
				else if(reader.Results[i+2].Length > 1)
				{
					if(double.TryParse(reader.Results[i + 2], out temp) || double.TryParse(reader.Results[i + 2].Substring(0, reader.Results[i + 2].Length - 2), out temp))
					{

					}
				}

			}
			return model;
		}
		private RunReader SuperSparCleaner(RunReader reader)
		{
			RunReader copy = new RunReader()
			{ Results = new List<string>() };
			foreach (string item in reader.Results)
			{
				copy.Results.Add(new string(item));
			}
			RunReader output = new RunReader();
			output.Results = new List<string>();
			int i = 0;
			while (reader.Results[i].ToLower() != "r")
			{
				output.Results.Add(reader.Results[i]);
				i++;
			}
			i++;
			output.Results.Add(reader.Results[i]);
			//converting prices
			foreach (string item in copy.Results)
			{
				double temp;
				if ((item.Substring(item.Length - 2,1).ToLower() == "n" || item.Substring(item.Length - 2, 1).ToLower() == "a") && double.TryParse(item.Substring(0,item.Length - 2),out temp))
				{
					int index = reader.Results.IndexOf(item); 					
					reader.Results[index] = item.Substring(0, item.Length - 2);	  			
				}
			}
			// combining lines
			while (reader.Results[i].ToLower() != "total")
			{
				double temp;
				double amount = 0;
				double number = 1;
				string prodcutName = "";
				string measure = "";
				if (reader.Results[i].Length > 1)// && isProduct(reader.Results[i]))
				{
					int j = i;
					while(amount == 0 && prodcutName == "")
					{
						if(isMeasure(reader.Results[j]))
						{
							measure = reader.Results[j];
						}
					}
				}
			}
			return output;
		}
		private ExpenseModel PicknPay(RunReader reader, string url)
		{
			reader = PicknPayCleaner(reader);
			ExpenseModel model = new ExpenseModel();
			RetailBranches retailBranch = new RetailBranches();
			int i = 0;
			while (reader.Results[i].ToLower() == "ins" || reader.Results[i].ToLower() == "del")
			{
				i++;
			}
			string merchant = reader.Results[i];
			i++;
			string branch = reader.Results[i];
			model.RetailBranches = retailBranch.Get(merchant, branch);
			while(reader.Results[i].Substring(reader.Results[i].Length-4) != model.RetailBranches.PhoneNumber.Substring(model.RetailBranches.PhoneNumber.Length - 4))
			{
				i++;
			}
			i++;
			while (reader.Results[i].ToUpper() != "DUE VAT INCL")
			{
				double temp;
				double amount = 0;
				double number = 1;
				double savings = 0;
				string productName = "";
				if(double.TryParse(reader.Results[i+2], out temp))
				{
					productName = reader.Results[i];
					i++;
					number = Convert.ToDouble(reader.Results[i]); 
					i++;
					amount = Convert.ToDouble(reader.Results[i]);
					i++;
					if(reader.Results[i + 1].Contains("cash-off") || reader.Results[i + 1].Contains("Smart Shopper"))
					{
						i++;
						i++;
						savings	= Convert.ToDouble(reader.Results[i])*-1;
					}
				}
				else if (reader.Results[i+2].Contains("cash-off") || reader.Results[i + 2].Contains("Smart Shopper"))
				{
					productName = reader.Results[i];
					i++;
					amount = Convert.ToDouble(reader.Results[i]);
					i++;
					i++;
					savings = Convert.ToDouble(reader.Results[i]) * -1;
				}
				else
				{
					productName = reader.Results[i];
					i++;
					amount = Convert.ToDouble(reader.Results[i]);
				}
				i++;
			}
			return model;
		}
		private RunReader PicknPayCleaner(RunReader reader)
		{
			RunReader copy = new RunReader()
			{ Results = new List<string>() };
			foreach(string item in reader.Results)
			{
				copy.Results.Add(new string(item));
			}
			RunReader output = new RunReader();
			output.Results = new List<string>();
			foreach (string item in copy.Results)
			{
				if(item.Substring(item.Length-1).ToLower() == "v")
				{
					int index = reader.Results.IndexOf(item);
					if (item.Substring(item.Length - 2).ToLower() == "#v")
					{
						reader.Results[index] = item.Substring(0, item.Length - 2);
					}
					else
					{
						reader.Results[index] = item.Substring(0, item.Length - 1);
					}					
				}
			} 			
			int i = 0;
			while (reader.Results[i].ToLower() == "ins" || reader.Results[i].ToLower() == "del")
			{
				output.Results.Add(reader.Results[i]);
				i++;
			}
			output.Results.Add(reader.Results[i]);
			i++;
			output.Results.Add(reader.Results[i]);
			i++;
			output.Results.Add(reader.Results[i]);
			i++;
			output.Results.Add(reader.Results[i]);
			i++;
			//output.Results.Add(reader.Results[i]);
			//i = i + 3;
			while (reader.Results[i].ToUpper() != "DUE VAT INCL")
			{
				double temp;
				if (reader.Results[i + 1].Length > 1)
				{
					if(double.TryParse(reader.Results[i + 1], out temp))
					{
						output.Results.Add(reader.Results[i]);
					}
					else if (double.TryParse(reader.Results[i + 1].Substring(0, reader.Results[i + 1].Length - 2), out temp))
					{
						output.Results.Add(reader.Results[i] + " " + reader.Results[i + 1]);
						i++;
					}
					else
					{
						output.Results.Add(reader.Results[i]);
					}
				}
				else
				{
					output.Results.Add(reader.Results[i]);
					i++;
					output.Results.Add(reader.Results[i]);
					i++;
					output.Results.Add(reader.Results[i]);
				}
				i++;
				output.Results.Add(reader.Results[i]);
				i++;

			}
			output.Results.Add(reader.Results[i]);
			output.Results.Add(reader.Results[i + 1]);
			return output;
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
					string productName = "";
					double number = 1;
					if (reader.Results[i].Substring(reader.Results[i].Length - 1,1) != "@")
					{
						productName = reader.Results[i];
						savings = CheckersSavings(i +2, i -1 , reader, amount);
						if (savings != 0)
						{
							amount = CheckersSavingsType(reader, amount, i - 1);
						}
					}
					else
					{
						productName = reader.Results[i - 1];
						savings = CheckersSavings(i + 3, i + 1, reader, amount);
						if(savings != 0)
						{
							amount = CheckersSavingsType(reader, amount, i + 1);
						}
						int j = 0;
						while(j < reader.Results[i].Length)
						{
							if(Char.IsWhiteSpace(reader.Results[i],j))
							{
								break;
							}
							j++;
						}
						number = Convert.ToInt32(reader.Results[i].Substring(0, j));
						i++;
					}
					model.ItemisedProducts.Add(new ItemisedProducts(productName, amount, savings, number));
				}
				i++;
			}
			i++;
			model.Total = Convert.ToDouble(reader.Results[i].Substring(1));
			return model;
		}
		/// <summary>
		/// Checks if a savings has been applied to the sale for Checkers
		/// </summary>
		/// <param name="xtra">XTRASAVE test location relative to product name</param>
		/// <param name="discount">Original Price location relative to product name</param>
		/// <param name="reader">Text Reader output</param>
		/// <param name="amount">The sale amount captured</param>
		/// <returns>Savings as a double on the single item</returns>
		private double CheckersSavings(int xtra,int discount,RunReader reader,double amount)
		{
			double savings = 0;
			if (reader.Results[xtra].Length > 8)
			{
				if (reader.Results[xtra].Substring(0, 8).ToUpper() == "XTRASAVE")
				{
					savings = Convert.ToDouble(reader.Results[xtra + 1].Substring(2));
				}
			}
			if (reader.Results[discount].Length > 15)
			{
				if (reader.Results[discount].Substring(0, 14).ToUpper() == "ORIGINAL PRICE")
				{
					savings = Convert.ToDouble(reader.Results[discount].Substring(15).ToLower()) - amount;
				}
			}
			return savings;
		}
		/// <summary>
		/// Checks if the savings was produced by Price Reductions, if so the amount is corrected for this adjustemnt while still holding the savings
		/// </summary>
		/// <param name="reader">Text Reader output</param>
		/// <param name="amount">The sale amount captured</param>
		/// <param name="discount">Original Price location relative to product name</param>
		/// <returns>Corrected amount variable</returns>
		private double CheckersSavingsType(RunReader reader, double amount, int discount)
		{
			if (reader.Results[discount].Length > 15)
			{
				if (reader.Results[discount].Substring(0, 14).ToUpper() == "ORIGINAL PRICE")
				{
					amount = Convert.ToDouble(reader.Results[discount].Substring(15).ToLower());
				}
			}
			return amount;
		}
		//if bool isProduct(string item)
		//{

		//}
		private bool isMeasure(string item)
		{
			double temp;
			if (item.Length > 2)
			{
				return !double.TryParse(item, out temp) && double.TryParse(item.Substring(0, item.Length - 2), out temp) && isMeasureMetric(item);
			}
			else
			{
				return false;
			}
		}
		private bool isMeasureMetric(string item)
		{
			
				switch (item.Substring(item.Length - 2))
				{
					case "ML":
						return true;
					case "GR":
						return true;
					case "KG":
						return true;
					case "'S":
						return true;
					default:
						return false;
				}
			
		}
		private void Save()
		{
			this.RetailBranches = null;
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				_context.Add(this);
				//_context.SaveChanges();
			}
		}
		
	}
	public static class ExtensionMethods
	{
		// Deep clone
		public static T DeepClone<T>(this T a)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, a);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}
	}
	public class ExpenseWeighting
	{
		public CFType CFType { get; set; }
		public double Amount { get; set; }
	}
	public class ReturnModel
	{
		public bool result { get; set; }
		public string returnStr { get; set; }
		public List<ItemisedProducts> products { get; set; }
	}
}