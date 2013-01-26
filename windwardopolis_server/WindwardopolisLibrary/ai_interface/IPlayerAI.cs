using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using WindwardopolisLibrary.map;
using WindwardopolisLibrary.units;

namespace WindwardopolisLibrary.ai_interface
{
	public interface IPlayerAI : IDisposable
	{
        /// <summary>
        /// The GUID for this player's connection. This will change if the connection has to be re-established. It is
        /// null for the local AIs.
        /// </summary>
        string TcpGuid { get; set; }

		/// <summary>
		/// Called when the game starts, providing all info.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup..</param>
		/// <param name="players">All the players.</param>
		/// <param name="companies">All companies on the board.</param>
		/// <param name="passengers">All the passengers.</param>
		/// <param name="ordersEvent">Callback to pass orders to the engine.</param>
		void Setup(GameMap map, Player me, List<Player> players, List<Company> companies, List<Passenger> passengers, 
							PlayerAIBase.PlayerOrdersEvent ordersEvent);

		/// <summary>
		/// Call the AI with a status message.
		/// </summary>
		/// <param name="xmlMessage">Can be null. The remote AI XML for a message without the status or my player values set.</param>
		/// <param name="about">The player this status is about. Will be set to receiving user for status not specific to a player.</param>
		/// <param name="status">The status for this message.</param>
		/// <param name="players">All the players.</param>
		/// <param name="passengers">All the passengers.</param>
		void GameStatus(XDocument xmlMessage, Player about, PlayerAIBase.STATUS status, List<Player> players, List<Passenger> passengers);

		/// <summary>
		/// Post an order to the engine. This can be called from a thread other than the UI thread.
		/// </summary>
		/// <param name="myPlayerGuid">The player this order is for.</param>
		/// <param name="path">The new path for this player's limo. Count == 0 for no path change.</param>
		/// <param name="pickUp">The new pick-up list for this player's limo. Count == 0 for no pick-up change.</param>
		void PostOrder(string myPlayerGuid, List<Point> path, List<Passenger> pickUp);
	}

	public class PlayerAIBase
	{
		public delegate void PlayerOrdersEvent(string playerGuid, List<Point> path, List<Passenger> pickUp);

		public enum STATUS
		{
			/// <summary>
			/// Called ever N ticks to update the AI with the game status.
			/// </summary>
			UPDATE,
			/// <summary>
			/// The car has no path.
			/// </summary>
			NO_PATH,
			/// <summary>
			/// The passenger was abandoned, no passenger was picked up.
			/// </summary>
			PASSENGER_ABANDONED,
			/// <summary>
			/// The passenger was delivered, no passenger was picked up.
			/// </summary>
			PASSENGER_DELIVERED,
			/// <summary>
			/// The passenger was delivered or abandoned, a new passenger was picked up.
			/// </summary>
			PASSENGER_DELIVERED_AND_PICKED_UP,
			/// <summary>
			/// The passenger refused to exit at the bus stop because an enemy was there.
			/// </summary>
			PASSENGER_REFUSED,
			/// <summary>
			/// A passenger was picked up. There was no passenger to deliver.
			/// </summary>
			PASSENGER_PICKED_UP,
			/// <summary>
			/// At a bus stop, nothing happened (no drop off, no pick up).
			/// </summary>
			PASSENGER_NO_ACTION
		}
	}
}
