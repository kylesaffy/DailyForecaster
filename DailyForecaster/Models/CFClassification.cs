using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace DailyForecaster.Models
{
	/// <summary>
	/// Idetifies whether or not this is a Income or Expense
	/// </summary>
	public class CFClassification
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int Sign { get; set; }
	}
}
