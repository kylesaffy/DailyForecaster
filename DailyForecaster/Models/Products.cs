using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class ProductsModel
	{
		public string ProductsModelId { get; set; }
		public string ProductClassModelId { get; set; }
		[ForeignKey("ProductClassModelId")]
		public ProductClassModel ProductClassModel { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// Returns a ProductsModel by ID
		/// </summary>
		/// <param name="id">ID of the ProductsModel required</param>
		/// <returns>a complete ProductsModel</returns>
		public ProductsModel Get(string id)
		{
			ProductClassModel productClass = new ProductClassModel();
			ProductsModel product = new ProductsModel();
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				product = _context.ProductsModel.Find(id);
			}
			product.ProductClassModel = productClass.Get(product.ProductClassModelId);
			return product;
		}
	}
}
