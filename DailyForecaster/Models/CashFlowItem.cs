﻿using Microsoft.EntityFrameworkCore;
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
		[Required]
		public string Id;
		
	}
}
