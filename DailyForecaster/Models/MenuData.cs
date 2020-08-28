using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class MenuData
	{
		public bool Category { get; set; } 
		public string Title { get; set; }
		public string Key { get; set; }
		public string Icon { get; set; }
		public int Count { get; set; }
		public string Url { get; set; }
		public List<MenuData> Children { get; set; }
	}
}
