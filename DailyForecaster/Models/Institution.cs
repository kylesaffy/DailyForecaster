﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class Institution
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string ContactNumber { get; set; }
		[Required]
		public string WebLink { get; set; }
		public string Email { get; set; }
		public string BlobString { get; set; }
		public bool isDummy { get; set; }
		/// <summary>
		/// Retirives a list of institutions from the database
		/// </summary>
		/// <param name="dummy">bool on whether dummy institutions are to be included</param>
		/// <returns>A generic list of instituions that are either of dummy type or not</returns>
		public List<Institution> GetInstitutions(bool dummy = false)
		{
			using(FinPlannerContext _context = new FinPlannerContext())
			{
				return _context
					.Institution
					.Where(x => x.isDummy == dummy)
					.ToList();
			}
		}
	}
}
