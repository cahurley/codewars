using System.Collections.Generic;
using System.Windows.Forms;
using WindwardopolisLibrary;
using WindwardopolisLibrary.units;

namespace Windwardopolis
{
	public partial class DebugWindow : Form
	{
		private TreeNode nodeCompanies;
		private TreeNode nodePassengers;

		public DebugWindow(List<Player> players, List<Company> companies, List<Passenger> passengers)
		{
			InitializeComponent();

			Setup(players, companies, passengers);
		}

		public void Setup(List<Player> players, List<Company> companies, List<Passenger> passengers)
		{

			// may be a new game
			treeView.Nodes.Clear();

			// set up companies
			nodeCompanies = new TreeNode("Companies") {Tag = companies};
			treeView.Nodes.Add(nodeCompanies);
			foreach (Company company in companies)
				nodeCompanies.Nodes.Add(new TreeNode(string.Format("{0} - {1}", company.Name, company.BusStop)) {Tag = company});

			// set up passengers
			nodePassengers = new TreeNode("Passengers") {Tag = companies};
			treeView.Nodes.Add(nodePassengers);
			foreach (Passenger passenger in passengers)
			{
				TreeNode nodePassenger = new TreeNode(passenger.Name) {Tag = passenger};
				nodePassengers.Nodes.Add(nodePassenger);
				nodePassenger.Nodes.Add("lobby");
				nodePassenger.Nodes.Add("destination");

				// pre-build route (we just delete from this)
				TreeNode nodeRoute = new TreeNode("Route");
				nodePassenger.Nodes.Add(nodeRoute);
				foreach (Company route in passenger.Companies)
					nodeRoute.Nodes.Add(new TreeNode(route.Name) {Tag = route});

				TreeNode nodeEnemies = new TreeNode("Enemies");
				nodePassenger.Nodes.Add(nodeEnemies);
				foreach (Passenger enemy in passenger.Enemies)
					nodeEnemies.Nodes.Add(new TreeNode(enemy.Name) {Tag = enemy});
			}

			Update(players, companies, passengers);
		}

		public void Update(List<Player> players, List<Company> companies, List<Passenger> passengers)
		{
			
			// update companies
			foreach (TreeNode nodeCompany in nodeCompanies.Nodes)
			{
				Company company = companies.Find(cpy => cpy.Name == ((Company)nodeCompany.Tag).Name);
				List<Passenger> cpyPassengers = new List<Passenger>(company.Passengers);
				cpyPassengers.Sort((a, b) => a.Name.CompareTo(b.Name));
				for (int index = 0; index < nodeCompany.Nodes.Count; )
				{
					Passenger psngr = cpyPassengers.Find(ps => ps.Name == ((Passenger)nodeCompany.Nodes[index].Tag).Name);
					if (psngr == null)
					{
						nodeCompany.Nodes.RemoveAt(index);
						continue;
					}
					cpyPassengers.Remove(psngr);
					index++;
				}
				// insert the new ones in sorted order
				foreach (Passenger passenger in cpyPassengers)
				{
					bool isInserted = false;
					for (int index = 0; index < nodeCompany.Nodes.Count; index++)
						if (System.String.Compare(passenger.Name, nodeCompany.Nodes[index].Text) < 0)
						{
							Trap.trap(index > 0);
							nodeCompany.Nodes.Insert(index, new TreeNode(passenger.Name) { Tag = passenger });
							isInserted = true;
							break;
						}
					if (!isInserted)
						nodeCompany.Nodes.Add(new TreeNode(passenger.Name) { Tag = passenger });
				}
			}

			// update companies. Nodes (if exist) are in the order Car/Lobby, Destination, Route (pre-set, just delete), Enemies (do nothing - already set)
			foreach (TreeNode nodePassenger in nodePassengers.Nodes)
			{
				Passenger passenger = passengers.Find(psngr => psngr.Name == ((Passenger) nodePassenger.Tag).Name);
				// may be done
				if (passenger.Destination == null)
				{
					Trap.trap();
					nodePassenger.Nodes[0].Text = nodePassenger.Nodes[1].Text = "{all done}";
					nodePassenger.Nodes[0].Tag = nodePassenger.Nodes[1].Tag = null;
					nodePassenger.Nodes[2].Nodes.Clear();
					continue;
				}
				else
				{
					if (passenger.Car != null)
					{
						Player plyr = players.Find(pl => pl.Limo == passenger.Car);
						nodePassenger.Nodes[0].Text = string.Format("Limo: {0} - {1}", plyr != null ? plyr.Name : "Limo", passenger.Car.Location.TilePosition);
						nodePassenger.Nodes[0].Tag = passenger.Car;
					}
					else
					{
						nodePassenger.Nodes[0].Text = string.Format("Lobby: {0} - {1}", passenger.Lobby.Name, passenger.Lobby.BusStop);
						nodePassenger.Nodes[0].Tag = passenger.Lobby;
					}
					nodePassenger.Nodes[1].Text = string.Format("Destination: {0} - {1}", passenger.Destination.Name, passenger.Destination.BusStop);
					nodePassenger.Nodes[1].Tag = passenger.Destination;
				}

				// for the route we just delete from the top if the total is different
				while (nodePassenger.Nodes[2].Nodes.Count > passenger.Companies.Count)
					nodePassenger.Nodes[2].Nodes.RemoveAt(0);
			}
		}
	}
}
