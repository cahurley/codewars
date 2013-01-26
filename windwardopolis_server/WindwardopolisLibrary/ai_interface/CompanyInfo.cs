
using System.Collections.Generic;
using System.Drawing;
using WindwardopolisLibrary.units;

namespace WindwardopolisLibrary.ai_interface
{
	public class CompanyInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public CompanyInfo(Company src)
		{
			Trap.trap();
			Name = src.Name;
			BusStop = src.BusStop;
		}

		/// <summary>
		/// The name of the company.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The map tile with the company's bus stop.
		/// </summary>
		public Point BusStop { get; private set; }

		/// <summary>
		/// The name of the passengers waiting at this company's bus stop for a ride.
		/// </summary>
		public IList<PassengerInfo> Passengers { get; private set; }
	}
}
