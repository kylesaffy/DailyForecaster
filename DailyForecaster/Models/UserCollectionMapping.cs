﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class UserCollectionMapping
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public string CollectionId { get; set; }
		[Required]
		public string UserId { get; set; }
	}
}
