using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class SimulatioManagementVM
	{
		
	}
	public class CollectionAssignment
	{
		public Collections Collections { get; set; }
		public List<Simulation> Simulations { get; set; }
		public CollectionAssignment() { }
		public CollectionAssignment(string collectionsId)
		{
			Collections = new Collections(collectionsId);
			Simulation simulation = new Simulation();
			//Simulations = simulation.get
		}
	}
}
