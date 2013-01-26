using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WindwardopolisLibrary.units
{
	/// <summary>
	/// A passenger who is transported from Company to Company in a Limo.
	/// </summary>
	public class Passenger
	{
		private static readonly Random rand = new Random();

		/// <summary>
		/// The name of this passenger.
		/// </summary>
		public string Name { get; private set; }

        /// <summary>
        /// The passenger logo.
        /// </summary>
        public Bitmap Logo { get; private set; }

		/// <summary>
		/// The number of points a player gets for delivering this passenger.
		/// </summary>
		public int PointsDelivered { get; private set; }

		/// <summary>
		/// The limo the passenger is presently in. null if not in a limo.
		/// </summary>
		public Limo Car { get; private set; }

		/// <summary>
		/// The bus stop the passenger is presently waiting in. null if in a limo or has arrived at final destination.
		/// </summary>
		public Company Lobby { get; private set; }

        /// <summary>
        /// The company the passenger started from. This is valid only in a car. If the passenger was abandoned and then 
        /// subsequently picked up, this is the company they were picked up from.
        /// </summary>
        public Company Start { get; private set; }

		/// <summary>
		/// The company the passenger wishes to go to. This is valid both at a bus stop and in a car. It is null if
		/// they have been delivered to their final destination.
		/// </summary>
		public Company Destination { get; private set; }

		/// <summary>
		/// The remaining companies the passenger wishes to go to after destination, in order. This does not include
		/// the Destination company.
		/// </summary>
		public IList<Company> Companies { get; private set; }

		/// <summary>
		/// The passengers this passenger will not share a bus stop with.
		/// </summary>
		public List<Passenger> Enemies { get; private set; }

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="name">The name of the passenger.</param>
		/// <param name="busStop">The company the passenger starts at.</param>
		/// <param name="companies">The list of all companies. This list, EXCEPT for the busStop company, will
		/// be placed in the Companies property in random order. The first will then be removed and set in Destination.</param>
		/// <param name="logo">Headshot of the passenger.</param>
		/// <param name="points">The number of points a player gets for delivering this passenger.</param>
		public Passenger(string name, Company busStop, IEnumerable<Company> companies, Bitmap logo, int points)
		{
			Name = name;
			Lobby = busStop;
			Companies = new List<Company>(companies.Where(cp => cp != busStop).OrderBy(item => rand.Next()));
			Destination = Companies[0]; 
			Companies.RemoveAt(0);
			Enemies = new List<Passenger>();
			Logo = logo;
			PointsDelivered = points;
		}

        internal static void ctor (List<Passenger> passengers)
        {

            // assign enemies
			List<Passenger> [] enemies = new List<Passenger>[6];
        	for (int ind=0; ind<enemies.Length; ind++)
				enemies[ind] = new List<Passenger>(passengers.OrderBy(psngr => rand.Next()));

            foreach (Passenger psngrOn in passengers)
            {
				int numEnemies = rand.Next(psngrOn.PointsDelivered, psngrOn.PointsDelivered + 3);
				for (int ind=0; ind<enemies.Length && numEnemies>0; ind++)
				{
					Passenger enemy = enemies[ind].FirstOrDefault(psngr => psngr != psngrOn && !psngrOn.Enemies.Contains(psngr));
					if (enemy != null)
					{
						psngrOn.Enemies.Add(enemy);
						enemies[ind].Remove(enemy);
						numEnemies--;
					}
				}
				psngrOn.Enemies.Sort((a, b) => a.Name.CompareTo(b.Name));
             }
        }

		public void EnterCar(Limo limo)
		{
            Start = Lobby;
            Lobby = null;
			Car = limo;
		}

		public void Arrived(Company cmpny)
		{
			Destination = Companies.Count == 0 ? null : Companies[0];
			Companies.Remove(Destination);
			Lobby = Companies.Count == 0 ? null : cmpny;
		    Start = null;
			Car = null;
		}

		public void Abandoned(Company cmpny)
		{
			Lobby = cmpny;
            Start = null;
            Car = null;
		}

		public void DebugAssign(Limo limo)
		{
			if (Start == null)
				Start = Lobby;
			Lobby = null;
			Car = limo;
		}

	    public override string ToString()
	    {
	        return string.Format("{0}, Lobby:{1}, Limo:{2}, Dest:{3}", Name, Lobby, Car, Destination);
	    }
	}
}
