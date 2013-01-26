using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using WindwardopolisLibrary;
using WindwardopolisLibrary.ai_interface;
using WindwardopolisLibrary.map;
using WindwardopolisLibrary.units;

namespace Windwardopolis.game_ai
{
	/// <summary>
	/// A very simplistic implementation of the AI. This AI is used for additional players if we don't have 10 remote players.
	/// </summary>
	public class PlayerAI : IPlayerAI
	{
		private Thread aiThread;
		private AiWorker aiWorker;

		private static readonly Random rand = new Random();

        /// <summary>
        /// The GUID for this player's connection. This will change if the connection has to be re-established. It is
        /// null for the local AIs.
        /// </summary>
        public string TcpGuid
	    {
            get { return null; }
	        set { /* nada */ }
	    }

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			if (aiThread == null)
				return;
			aiWorker.ExitThread = true;
			aiWorker.EventThread.Set();
			aiThread.Join(100);
			if (aiThread.IsAlive)
			{
				Trap.trap();
				aiThread.Abort();
			}
			aiThread = null;
		}

		/// <summary>
		/// Called when the game starts, providing all info.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup..</param>
		/// <param name="players">All the players.</param>
		/// <param name="companies">All companies on the board.</param>
		/// <param name="passengers">All the passengers.</param>
		/// <param name="ordersEvent">Callback to pass orders to the engine.</param>
		public void Setup(GameMap map, Player me, List<Player> players, List<Company> companies, List<Passenger> passengers, PlayerAIBase.PlayerOrdersEvent ordersEvent)
		{

			// start the thread
			aiWorker = new AiWorker(map, me.Guid, companies, ordersEvent);
	    	aiThread = new Thread(aiWorker.MainLoop) {Name = me.Name.Replace(' ', '_'), IsBackground = true};
	    	aiThread.Start();

			// cause it to pick a passenger to pick up and calculate the path to that company.
			aiWorker.AddMessage(PlayerAIBase.STATUS.NO_PATH, me, AllPickups(me, passengers));
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

			// only care if for me, and an action (ie not status)
			if ((status == PlayerAIBase.STATUS.UPDATE) || ((about != null) && (about.Guid != aiWorker.MyPlayerGuid)))
				return;

			Player player = players.Find(pl => pl.Guid == aiWorker.MyPlayerGuid);
			Trap.trap(about == null);
			if (about == null)
				about = player;
			aiWorker.AddMessage(status, about, AllPickups(player, passengers));
		}

		/// <summary>
		/// Post an order to the engine. This can be called from a thread other than the UI thread.
		/// </summary>
		/// <param name="myPlayerGuid">The player this order is for.</param>
		/// <param name="path">The new path for this player's limo. Count == 0 for no path change.</param>
		/// <param name="pickUp">The new pick-up list for this player's limo. Count == 0 for no pick-up change.</param>
		public void PostOrder(string myPlayerGuid, List<Point> path, List<Passenger> pickUp)
		{
			Trap.trap();
			// we don't use this - remote AIs only
		}

		/// <summary>
		/// List of all passengers we can pick up. Does not include ones already delivered and if presently carrying one,
		/// does not include that one. Returned in random order, different random order each time called.
		/// </summary>
		private static List<Passenger> AllPickups(Player me, IEnumerable<Passenger> passengers)
		{
			List<Passenger> pickup = new List<Passenger>();
			pickup.AddRange(passengers.Where(
					psngr =>
					(!me.PassengersDelivered.Contains(psngr)) && (psngr != me.Passenger) && (psngr.Car == null) &&
					(psngr.Destination != null)).OrderBy(psngr => rand.Next()));
			return pickup;
		}

		#region the AI worker thread

		private class StatusMessage
		{
			public PlayerAIBase.STATUS Status { get; private set; }
			public Point LimoTileLocation { get; private set; }
			public Point PassengerDestBusStop { get; private set; }
			public List<Passenger> Pickup { get; private set; }

			public StatusMessage(PlayerAIBase.STATUS status, Player player, List<Passenger> pickup)
			{
				Status = status;
				LimoTileLocation = player.Limo.Location.TilePosition;
				PassengerDestBusStop = player.Passenger == null ? Point.Empty : player.Passenger.Destination.BusStop;
				Pickup = pickup;
			}
		}

		private class AiWorker
		{
			private readonly GameMap gameMap;

			public string MyPlayerGuid { get; private set; }
			private readonly List<Company> Companies;
			private readonly PlayerAIBase.PlayerOrdersEvent sendOrders;

			private readonly Queue<StatusMessage> messages;

			/// <summary>
			/// The event handle to bounce when a message is added or the thread is ended.
			/// </summary>
			public EventWaitHandle EventThread { get; private set; }

			/// <summary>
			/// Set to true when ending this thread. Need to bounce EventThread after setting this.
			/// </summary>
			public bool ExitThread { private get; set; }

			public AiWorker(GameMap gameMap, string myPlayerGuid, List<Company> companies, PlayerAIBase.PlayerOrdersEvent sendOrders)
			{
				this.gameMap = gameMap;
				MyPlayerGuid = myPlayerGuid;
				Companies = companies;
				this.sendOrders = sendOrders;
				messages = new Queue<StatusMessage>();

				// get it ready to handle events.
				EventThread = new AutoResetEvent(false);
				ExitThread = false;
			}

			public void AddMessage(PlayerAIBase.STATUS status, Player about, List<Passenger> pickup)
			{
				StatusMessage msg = new StatusMessage(status, about, pickup);
				lock (messages)
					messages.Enqueue(msg);
				EventThread.Set();
			}

			public void MainLoop()
			{
				while (EventThread.WaitOne())
				{
					if (ExitThread)
						return;
					try
					{
						Point ptLimoLocation = Point.Empty;
						Point ptDest = Point.Empty;
						List<Passenger> pickup = null;
						// build up a single operation from all messages
						lock (messages)
						{
							Trap.trap(messages.Count > 1);
							while (messages.Count > 0)
							{
								StatusMessage msg = messages.Dequeue();
								if (msg == null)
								{
									Trap.trap();
									continue;
								}
								ptLimoLocation = msg.LimoTileLocation;

								switch (msg.Status)
								{
									case PlayerAIBase.STATUS.NO_PATH:
									case PlayerAIBase.STATUS.PASSENGER_NO_ACTION:
										if (msg.PassengerDestBusStop == Point.Empty)
										{
											pickup = msg.Pickup;
											if (pickup.Count == 0)
												break;
											ptDest = pickup[0].Lobby.BusStop;
										}
										else
											ptDest = msg.PassengerDestBusStop;
										break;
									case PlayerAIBase.STATUS.PASSENGER_DELIVERED:
									case PlayerAIBase.STATUS.PASSENGER_ABANDONED:
										pickup = msg.Pickup;
										if (pickup.Count == 0)
											break;
										ptDest = pickup[0].Lobby.BusStop;
										break;
									case PlayerAIBase.STATUS.PASSENGER_REFUSED:
										ptDest = Companies.Where(cpy => cpy.BusStop != msg.PassengerDestBusStop).OrderBy(cpy => rand.Next()).First().BusStop;
										break;
									case PlayerAIBase.STATUS.PASSENGER_DELIVERED_AND_PICKED_UP:
									case PlayerAIBase.STATUS.PASSENGER_PICKED_UP:
										pickup = msg.Pickup;
										if (pickup.Count == 0)
											break;
										ptDest = msg.PassengerDestBusStop;
										break;
								}
							}
						}

						if (ptDest == Point.Empty)
							continue;
						if (pickup == null)
							pickup = new List<Passenger>();

						// get the path from where we are to the dest.
						List<Point> path = CalculatePathPlus1(ptLimoLocation, ptDest);

						sendOrders(MyPlayerGuid, path, pickup);
					}
					catch (Exception)
					{
						Trap.trap();
						// next message
					}

				}
				Trap.trap();
			}

			private List<Point> CalculatePathPlus1(Point ptLimo, Point ptDest)
			{
				List<Point> path = SimpleAStar.CalculatePath(gameMap, ptLimo, ptDest);
				// add in leaving the bus stop so it has orders while we get the message saying it got there and are deciding what to do next.
				if (path.Count > 1)
					path.Add(path[path.Count - 2]);
				return path;
			}

		}

		#endregion
	}
}
