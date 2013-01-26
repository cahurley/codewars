// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using WindwardopolisLibrary.ai_interface;
using WindwardopolisLibrary.map;

namespace WindwardopolisLibrary.units
{
	/// <summary>
	/// Adds engine items to the player object.
	/// </summary>
	public class Player : IDisposable
	{
        /// <summary>
        /// How many passengers must be delivered to win.
        /// </summary>
        public const int NUM_PASSENGERS_TO_WIN = 8;

		private readonly IPlayerAI ai;

		private int passengerDeliveredPoints;

		/// <summary>
		/// What the communication with the remote AI player mode is.
		/// </summary>
		public enum COMM_MODE
		{
			/// <summary>
			/// Waiting for initial start move.
			/// </summary>
			WAITING_FOR_START,
			/// <summary>
			/// Got the start move.
			/// </summary>
			RECEIVED_START,
		}

		/// <summary>
		/// Create a player object. This is used during setup.
		/// </summary>
		/// <param name="guid">The unique identifier for this player.</param>
		/// <param name="name">The name of the player.</param>
		/// <param name="school">The school this player is from.</param>
		/// <param name="language">The computer language this A.I. was written in.</param>
		/// <param name="avatar">The avatar of the player.</param>
		/// <param name="limo">The limo for this player.</param>
		/// <param name="spriteColor">The color of this player's sprite.</param>
		/// <param name="ai">The AI for this player.</param>
		public Player(string guid, string name, string school, string language, Image avatar, Limo limo, Color spriteColor, IPlayerAI ai)
		{
			Guid = guid;
			Name = name;
			School = school.Length <= 11 ? school : school.Substring(0, 11);
			Language = language;
			Avatar = avatar;
			Limo = limo;
			passengerDeliveredPoints = 0;
			SpriteColor = spriteColor;
			TransparentSpriteColor = Color.FromArgb(96, spriteColor.R, spriteColor.G, spriteColor.B);
			this.ai = ai;
			IsConnected = true;

			PassengersDelivered = new List<Passenger>();
			PickUp = new List<Passenger>();
			Scoreboard = new List<float>();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			ai.Dispose();
		}

		public void Reset()
		{
            // limos back to start, passengers reset (location and destinations)?
			passengerDeliveredPoints = 0;
			PassengersDelivered.Clear();
			PickUp.Clear();
		    Limo.Reset();
		    Passenger = null;
			WaitingForReply = COMM_MODE.WAITING_FOR_START;
		}

        /// <summary>
        /// The GUID for this player's connection. This will change if the connection has to be re-established. It is
        /// null for the local AIs.
        /// </summary>
        public string TcpGuid
        {
			get { return ai.TcpGuid; }
        	set
        	{
        		IsConnected = value != null;
        		ai.TcpGuid = value;
        	}
        }

		/// <summary>
		/// true if connected.
		/// </summary>
		public bool IsConnected { get; private set; }

		public void Setup(GameMap map, Player me, List<Player> players, List<Company> companies, List<Passenger> passengers,
				   PlayerAIBase.PlayerOrdersEvent ordersEvent)
		{
			ai.Setup(map, me, players, companies, passengers, ordersEvent);
		}

		/// <summary>
		/// Call the AI with a status message.
		/// </summary>
		/// <param name="xmlMessage">Can be null. The remote AI XML for a message without the status or my player values set.</param>
		/// <param name="about">The player this is about.</param>
		/// <param name="status">The status for this message.</param>
		/// <param name="players">All the players.</param>
		/// <param name="passengers">All the passengers.</param>
		public void GameStatus(XDocument xmlMessage, Player about, PlayerAIBase.STATUS status, List<Player> players, List<Passenger> passengers)
		{
			ai.GameStatus(xmlMessage, about, status, players, passengers);
		}

		/// <summary>
		/// Post an order to the engine. This can be called from a thread other than the UI thread.
		/// </summary>
		/// <param name="path">The new path for this player's limo. Count == 0 for no path change.</param>
		/// <param name="pickUp">The new pick-up list for this player's limo. Count == 0 for no pick-up change.</param>
		public void SendOrder(List<Point> path, List<Passenger> pickUp)
		{
			ai.PostOrder(Guid, path, pickUp);
		}

		#region properties

		/// <summary>
		/// The unique identifier for this player. This will remain constant for the length of the game (while the Player objects passed will
		/// change on every call).
		/// </summary>
		public string Guid { get; private set; }

		/// <summary>
		/// The name of the player.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The avatar of the player.
		/// </summary>
		public Image Avatar { get; private set; }

		/// <summary>
		/// The player's limo.
		/// </summary>
		public Limo Limo { get; private set; }

		/// <summary>
		/// The school this player is from.
		/// </summary>
		public string School { get; private set; }

		/// <summary>
		/// The computer language this A.I. was written in.
		/// </summary>
		public string Language { get; private set; }

		/// <summary>
		/// We are waiting for a reply from this player.
		/// </summary>
		public COMM_MODE WaitingForReply { get; set; }

		/// <summary>
		/// The score for this player - this game
		/// </summary>
		public float Score
	    {
            get
            {
                if (PassengersDelivered.Count >= NUM_PASSENGERS_TO_WIN)
					return passengerDeliveredPoints + 2;
                if (Passenger == null)
					return passengerDeliveredPoints;
                int distTotal = Math.Abs(Passenger.Destination.BusStop.X - Passenger.Start.BusStop.X) +
                                Math.Abs(Passenger.Destination.BusStop.Y - Passenger.Start.BusStop.Y);
                int distRemaining = Math.Abs(Passenger.Destination.BusStop.X - Limo.Location.TilePosition.X) +
                            Math.Abs(Passenger.Destination.BusStop.Y - Limo.Location.TilePosition.Y);
                if (distRemaining > distTotal)
					return passengerDeliveredPoints;
                // all the way is 1/2 point
				return passengerDeliveredPoints + 0.5f * (float)(distTotal - distRemaining) / distTotal;
            }
	    }

	    /// <summary>
		/// The passengers delivered - this game
		/// </summary>
		public List<Passenger> PassengersDelivered { get; private set; }

		/// <summary>
		/// The score for this player - previous games.
		/// </summary>
		public List<float> Scoreboard { get; private set; }

		/// <summary>
		/// The color for this player on the status window.
		/// </summary>
		public Color SpriteColor { get; private set; }

		/// <summary>
		/// The color for this player on the status window.
		/// </summary>
		public Color TransparentSpriteColor { get; private set; }

		/// <summary>
		/// Passenger in limo. null if none.
		/// </summary>
		public Passenger Passenger { get; set; }

		/// <summary>
		/// The next bus stop in this player's path. null if none.
		/// </summary>
		public Company NextBusStop { get; set; }

		/// <summary>
		/// Who to pick up at the next bus stop. Can be empty and can also only list people not there.
		/// </summary>
		public List<Passenger> PickUp { get; private set; }

		#endregion

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return String.Format("{0}, Score: {1}", Name, Score);
		}

		public void Delivered(Passenger passenger)
		{
			passengerDeliveredPoints += passenger.PointsDelivered;
			PassengersDelivered.Add(passenger);
		}
	}
}
