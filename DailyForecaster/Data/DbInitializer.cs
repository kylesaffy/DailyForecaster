using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyForecaster.Models;

namespace DailyForecaster.Data
{
	public class DbInitializer
	{
		public static void Initialize(FinPlannerContext context)
		{
			context.Database.EnsureCreated();
			if (context.CFTypes.Any())
			{
				return;
			}
			var cfTypes = new CFType[]
			{
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Salary"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Rent"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Groceries"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Electricty"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Loans"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Car Loan"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Insurance"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Bank Charges"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Domestic"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Cellphone"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Gym"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Internet"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Medical Aid"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Entertainment"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Takeout"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "School Fees"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Transportation"},
				new CFType{Id = Guid.NewGuid().ToString(),Custom = false,Name = "Medication"}
			};
			foreach(CFType item in cfTypes)
			{
				context.CFTypes.Add(item);
			}
			context.SaveChanges();
		}
	}
}
