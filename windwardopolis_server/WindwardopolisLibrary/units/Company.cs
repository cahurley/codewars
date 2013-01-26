using System.Collections.Generic;
using System.Drawing;

namespace WindwardopolisLibrary.units
{
	/// <summary>
	/// A Company that a Passenger is transported to/from in a Limo.
	/// </summary>
	public class Company
	{
		public const int NUM_COMPANIES = 12;

		/// <summary>
		/// The name of the company.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The map tile with the company's bus stop.
		/// </summary>
		public Point BusStop { get; set; }

		/// <summary>
		/// The company logo.
		/// </summary>
		public Bitmap Logo { get; private set; }

		/// <summary>
		/// The Passengers waiting at this company's bus stop for a ride.
		/// </summary>
		public IList<Passenger> Passengers { get; private set; }

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="name">The name of the company.</param>
		/// <param name="logo">The company logo.</param>
		public Company(string name, Bitmap logo)
		{
			Name = name;
			Logo = logo;
			Passengers = new List<Passenger> ();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// All of the companies and passengers for the game. Each call creates a new list so the returned list can be changed.
		/// </summary>
		public static void GenerateCompaniesAndPassengers(out List<Company> allCompanies, out List<Passenger> allPassengers)
		{

			allCompanies = new List<Company>();
			allPassengers = new List<Passenger>();

			allCompanies.Add(new Company("Microsoft", Sprites.microsoft));
			allCompanies.Add(new Company("Hewlett-Packard", Sprites.hp)); // Tesla, DropBox, LinkedIn
			allCompanies.Add(new Company("IBM", Sprites.ibm));
			allCompanies.Add(new Company("Oracle", Sprites.oracle));
			allCompanies.Add(new Company("Facebook", Sprites.facebook));
			allCompanies.Add(new Company("JetBrains", Sprites.jetbrains));
			allCompanies.Add(new Company("Windward", Sprites.windward));
			allCompanies.Add(new Company("Apple", Sprites.apple));
			allCompanies.Add(new Company("LinkedIn", Sprites.linkedin));
			allCompanies.Add(new Company("Google", Sprites.google));
			allCompanies.Add(new Company("Amazon", Sprites.amazon));
			allCompanies.Add(new Company("Twitter", Sprites.twitter));

			allPassengers.Add(new Passenger("Steve Ballmer", allCompanies[0], allCompanies, Sprites.SteveBallmer, 2));
			allPassengers.Add(new Passenger("Meg Whitman", allCompanies[1], allCompanies, Sprites.MegWhitman, 2));
			allPassengers.Add(new Passenger("Ginni Rometty", allCompanies[2], allCompanies, Sprites.GinniRometty, 1));
			allPassengers.Add(new Passenger("Larry Ellison", allCompanies[3], allCompanies, Sprites.LarryEllison, 1));
			allPassengers.Add(new Passenger("Mark Zuckerberg", allCompanies[4], allCompanies, Sprites.MarkZuckerberg, 1));
			allPassengers.Add(new Passenger("Oleg Stepanov", allCompanies[5], allCompanies, Sprites.OlegStepanov, 2));
			allPassengers.Add(new Passenger("Shirley Clawson", allCompanies[6], allCompanies, Sprites.ShirleyClawson, 3));
			allPassengers.Add(new Passenger("Tim Cook", allCompanies[7], allCompanies, Sprites.TimCook, 1));
			allPassengers.Add(new Passenger("Jeff Weiner", allCompanies[8], allCompanies, Sprites.JeffWeiner, 1));
			allPassengers.Add(new Passenger("Larry Page", allCompanies[9], allCompanies, Sprites.LarryPage, 1));
			allPassengers.Add(new Passenger("Jeff Bezos", allCompanies[10], allCompanies, Sprites.JeffBezos, 1));
			allPassengers.Add(new Passenger("Dick Costolo", allCompanies[11], allCompanies, Sprites.DickCostolo, 1));

			// how we handle them pointing to each other
			allCompanies[0].Passengers.Add(allPassengers[0]);
			allCompanies[1].Passengers.Add(allPassengers[1]);
			allCompanies[2].Passengers.Add(allPassengers[2]);
			allCompanies[3].Passengers.Add(allPassengers[3]);
			allCompanies[4].Passengers.Add(allPassengers[4]);
			allCompanies[5].Passengers.Add(allPassengers[5]);
			allCompanies[6].Passengers.Add(allPassengers[6]);
			allCompanies[7].Passengers.Add(allPassengers[7]);
			allCompanies[8].Passengers.Add(allPassengers[8]);
			allCompanies[9].Passengers.Add(allPassengers[9]);
			allCompanies[10].Passengers.Add(allPassengers[10]);
			allCompanies[11].Passengers.Add(allPassengers[11]);

		    Passenger.ctor(allPassengers);

			allPassengers.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));
			allCompanies.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
		}
	}
}
