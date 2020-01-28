using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using DailyForecaster.Models;
namespace DailyForecaster
{
	/// <summary>
	/// Sub class to the actual Cash flow Item
	/// </summary>
	public class CashFlowItem
	{
		[Key]
		[Required]
		public string ID;
		[Required]
		public CFType CFType;
		[Required]
		public CFClassification CFClassification;
	}
}
