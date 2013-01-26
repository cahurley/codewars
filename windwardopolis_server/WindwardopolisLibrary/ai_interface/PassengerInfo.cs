
using System.Collections.Generic;
using WindwardopolisLibrary.units;

namespace WindwardopolisLibrary.ai_interface
{
	public class PassengerInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public PassengerInfo(Passenger src)
		{
			Trap.trap();
			Name = src.Name;
			BusStop = new CompanyInfo(src.BusStop);
		}

		/// <summary>
		/// The name of this passenger.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The limo the passenger is presently in. null if not in a limo.
		/// </summary>
		public LimoInfo Car { get; private set; }

		/// <summary>
		/// The bus stop the passenger is presently waiting in. null if in a limo or has arrived at final destination.
		/// </summary>
		public CompanyInfo BusStop { get; private set; }

		/// <summary>
		/// The company the passenger wishes to go to. This is valid both at a bus stop and in a car. It is null if
		/// they have been delivered to their final destination.
		/// </summary>
		public CompanyInfo Destination { get; private set; }

		/// <summary>
		/// The remaining companies the passenger wishes to go to after destination, in order. This does not include
		/// the Destination company.
		/// </summary>
		public IList<CompanyInfo> Companies { get; private set; }
	}
}
