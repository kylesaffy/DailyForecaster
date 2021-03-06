﻿using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	[Serializable]
	public class ItemisedProducts
	{
		public string ItemisedProductsId { get; set; }
		public string ProductsModelId { get; set; }
		[ForeignKey("ProductsModelId")]
		public ProductsModel ProductsModel { get; set; }
		public double NumberOfProducts { get; set; }
		public double Amount { get; set; }
		public double Savings { get; set; }
		public double SubTotal { get; set; }
		public string ExpenseModelId { get; set; }
		[ForeignKey("ExpenseModelId")]
		public ExpenseModel ExpenseModel { get; set; }
		public ItemisedProducts() { }
		/// <summary>
		/// Initalises a new Itemised Product
		/// </summary>
		/// <param name="productStr">Invoice title for item purchased</param>
		/// <param name="amount">The Rand amount for the product being purchased</param>
		/// <param name="savings">Any savings that is incurred on the line item</param>
		/// <param name="number">Number of items purchased</param>
		public ItemisedProducts(string productStr, double amount,double savings, double number)
		{
			ProductsModel productsModel = new ProductsModel();
			try
			{
				ProductsModel = productsModel.Get(productStr);
				ItemisedProductsId = Guid.NewGuid().ToString();
				ProductsModelId = productStr;
				NumberOfProducts = number;
				Amount = Math.Round(amount,2);
				Savings = Math.Round(savings,2);
				SubTotal = Math.Round((amount * number) - savings, 2);
			}
			catch
			{
				ProductClassModel productClass = new ProductClassModel();
				ProductsModel product = new ProductsModel();
				product.ProductsModelId = productStr;
				product.ProductClassModel = productClass;
				ProductsModel = product;
				EmailFunction email = new EmailFunction();
				ProductsModelId = productStr;
				NumberOfProducts = number;
				Amount = Math.Round(amount, 2);
				Savings = Math.Round(savings, 2);
				SubTotal = Math.Round((amount * number) - savings, 2);
				email.EmailError("Product not found", null, productStr);
			}
		}
		public void Save()
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				this.ProductsModel = null;
				_context.Add(this);
				_context.SaveChanges();
			}
		}
	}
}
