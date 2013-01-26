using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Linq;
using WindwardopolisLibrary;
using WindwardopolisLibrary.ai_interface;
using WindwardopolisLibrary.map;
using WindwardopolisLibrary.units;

namespace Windwardopolis
{
	/// <summary>
	/// Local end of communicattion link with remote AI.
	/// </summary>
	public class RemoteAI : IPlayerAI
	{
		private readonly Framework framework;
		private PlayerAIBase.PlayerOrdersEvent sendOrders;

	    public RemoteAI(Framework framework, string guid)
	    {
	    	this.framework = framework;
	        TcpGuid = guid;
	    }

        /// <summary>
        /// The GUID for this player's connection. This will change if the connection has to be re-established. It is
        /// null for the local AIs.
        /// </summary>
        public string TcpGuid { get; set; }

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
		}

	    /// <summary>
		/// Called when the game starts, providing all info.
		/// </summary>
		public void Setup(GameMap map, Player me, List<Player> players, List<Company> companies, List<Passenger> passengers, PlayerAIBase.PlayerOrdersEvent ordersEvent)
		{
			sendOrders = ordersEvent; 
			
			XDocument doc = new XDocument();
			XElement elemRoot = new XElement("setup", new XAttribute("game-start", true), new XAttribute("my-guid", me.Guid));
			doc.Add(elemRoot);

			// the map
			XElement elemMap = new XElement("map", new XAttribute("width", map.Width), new XAttribute("height", map.Height), new XAttribute("units-tile", TileMovement.UNITS_PER_TILE));
			elemRoot.Add(elemMap);
			for (int x = 0; x < map.Width; x++ )
				for (int y = 0; y < map.Height; y++)
				{
					MapSquare square = map.Squares[x][y];
					XElement elemRow = new XElement("tile", new XAttribute("x", x), new XAttribute("y", y),
					                                new XAttribute("type", square.Tile.Type));
					if (square.Tile.IsDriveable)
					{
						elemRow.Add(new XAttribute("direction", square.Tile.Direction));
						if (square.StopSigns != MapSquare.STOP_SIGNS.NONE)
							elemRow.Add(new XAttribute("stop-sign", square.StopSigns));
						if (square.SignalDirection != MapSquare.SIGNAL_DIRECTION.NONE)
							elemRow.Add(new XAttribute("signal", true));
					}
					elemMap.Add(elemRow);
				}

			// all players (including me)
			XElement elemPlayers = new XElement("players");
			elemRoot.Add(elemPlayers);
			foreach (Player plyrOn in players)
				elemPlayers.Add(new XElement("player", new XAttribute("guid", plyrOn.Guid),
												   new XAttribute("name", plyrOn.Name),
												   new XAttribute("limo-x", plyrOn.Limo.Location.TilePosition.X),
												   new XAttribute("limo-y", plyrOn.Limo.Location.TilePosition.Y),
												   new XAttribute("limo-angle", plyrOn.Limo.Location.Angle)));

			// all companies
			XElement elemCompanies = new XElement("companies");
			elemRoot.Add(elemCompanies);
			foreach (Company cmpnyOn in companies)
				elemCompanies.Add(new XElement("company", new XAttribute("name", cmpnyOn.Name),
							new XAttribute("bus-stop-x", cmpnyOn.BusStop.X), new XAttribute("bus-stop-y", cmpnyOn.BusStop.Y)));

			// all passengers
			XElement elemPassengers = new XElement("passengers");
			elemRoot.Add(elemPassengers);
			foreach (Passenger psngrOn in passengers)
			{
				XElement elemPassenger = new XElement("passenger", new XAttribute("name", psngrOn.Name), new XAttribute("points-delivered", psngrOn.PointsDelivered));
				// if due to a re-start, these can be null
				if (psngrOn.Lobby != null)
					elemPassenger.Add(new XAttribute("lobby", psngrOn.Lobby.Name));
				if (psngrOn.Destination != null)
					elemPassenger.Add(new XAttribute("destination", psngrOn.Destination.Name));
				foreach (Company route in psngrOn.Companies)
					elemPassenger.Add(new XElement("route", route.Name));
				foreach (Passenger enemy in psngrOn.Enemies)
					elemPassenger.Add(new XElement("enemy", enemy.Name));
				elemPassengers.Add(elemPassenger);
			}

			framework.tcpServer.SendMessage(TcpGuid, doc.ToString()); 
		}

		/// <summary>
		/// Call the AI with a status message.
		/// </summary>
		/// <param name="xmlMessage">Can be null. The remote AI XML for a message without the status or my player values set.</param>
		/// <param name="about">The player this status is about. Will be set to receiving user for status not specific to a player.</param>
		/// <param name="status">The status for this message.</param>
		/// <param name="players">All the players.</param>
		/// <param name="passengers">All the passengers.</param>
		public void GameStatus(XDocument xmlMessage, Player about, PlayerAIBase.STATUS status, List<Player> players, List<Passenger> passengers)
		{

			if (TcpGuid == null)
				return;

			if (xmlMessage == null)
				xmlMessage = BuildMessageXml(players, passengers);
			XAttribute attr = xmlMessage.Root.Attribute("status");
			if (attr == null)
				xmlMessage.Root.Add(new XAttribute("status", status));
			else
				attr.Value = status.ToString();

			attr = xmlMessage.Root.Attribute("player-guid");
			if (attr == null)
				xmlMessage.Root.Add(new XAttribute("player-guid", about.Guid));
			else
				attr.Value = about.Guid;

			StringBuilder buf = new StringBuilder();
			Player player = framework.GameEngine.Players.Find(pl => pl.TcpGuid == TcpGuid);
			if (player != null)
				foreach (Point ptOn in player.Limo.PathTiles)
					buf.Append(Convert.ToString(ptOn.X) + ',' + Convert.ToString(ptOn.Y) + ';');

			XElement elem = xmlMessage.Root.Element("path");
			if (elem == null)
			{
				elem = new XElement("path");
				xmlMessage.Root.Add(elem);
				elem.Value = buf.ToString();
			}
			else
				elem.Value = buf.ToString();

			if (player != null)
			{
				buf.Clear();
				foreach (Passenger psngr in player.PickUp)
					buf.Append(psngr.Name + ';');
			}
			elem = xmlMessage.Root.Element("pick-up");
			if (elem == null)
			{
				elem = new XElement("pick-up");
				xmlMessage.Root.Add(elem);
				elem.Value = buf.ToString();
			}
			else
				elem.Value = buf.ToString();
			
			framework.tcpServer.SendMessage(TcpGuid, xmlMessage.ToString());
		}

		public static XDocument BuildMessageXml(List<Player> players, List<Passenger> passengers)
		{

			XDocument doc = new XDocument();
			XElement elemRoot = new XElement("status");
			doc.Add(elemRoot);

			// all players (including me)
			XElement elemPlayers = new XElement("players");
			elemRoot.Add(elemPlayers);
			foreach (Player plyrOn in players)
			{
				XElement elemPlayer = new XElement("player", new XAttribute("guid", plyrOn.Guid),
												   new XAttribute("score", plyrOn.Score),
												   new XAttribute("limo-x", plyrOn.Limo.Location.TilePosition.X),
												   new XAttribute("limo-y", plyrOn.Limo.Location.TilePosition.Y),
												   new XAttribute("limo-angle", plyrOn.Limo.Location.Angle));
				if (plyrOn.Passenger != null)
					elemPlayer.Add(new XAttribute("passenger", plyrOn.Passenger.Name));
				if (plyrOn.PassengersDelivered.Count > 0)
					elemPlayer.Add(new XAttribute("last-delivered", plyrOn.PassengersDelivered[plyrOn.PassengersDelivered.Count - 1].Name));
				elemPlayers.Add(elemPlayer);
			}

			// all passengers
			XElement elemPassengers = new XElement("passengers");
			elemRoot.Add(elemPassengers);
			foreach (Passenger psngrOn in passengers)
			{
				XElement elemPassenger = new XElement("passenger", new XAttribute("name", psngrOn.Name));
				if (psngrOn.Destination != null)
					elemPassenger.Add(new XAttribute("destination", psngrOn.Destination.Name));
				if (psngrOn.Car != null)
					elemPassenger.Add(new XAttribute("status", "travelling"));
				else if (psngrOn.Destination != null)
				{
					elemPassenger.Add(new XAttribute("lobby", psngrOn.Lobby.Name));
					elemPassenger.Add(new XAttribute("status", "lobby"));
				}
				else
				{
					Trap.trap();
					elemPassenger.Add(new XAttribute("status", "done"));
				}

				elemPassengers.Add(elemPassenger);
			}

			return doc;
		}

		/// <summary>
		/// Post an order to the engine. This can be called from a thread other than the UI thread.
		/// </summary>
		/// <param name="myPlayerGuid">The player this order is for.</param>
		/// <param name="path">The new path for this player's limo. Count == 0 for no path change.</param>
		/// <param name="pickUp">The new pick-up list for this player's limo. Count == 0 for no pick-up change.</param>
		public void PostOrder(string myPlayerGuid, List<Point> path, List<Passenger> pickUp)
		{
			sendOrders(myPlayerGuid, path, pickUp);
		}

	}
}
