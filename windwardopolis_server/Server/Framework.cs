// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Forms;
using Windwardopolis.game_ai;
using Windwardopolis.game_engine;
using WindwardopolisLibrary;
using WindwardopolisLibrary.ai_interface;
using WindwardopolisLibrary.map;
using WindwardopolisLibrary.units;
using Timer = System.Windows.Forms.Timer;

namespace Windwardopolis
{
	/// <summary>
	/// This is the layer between the XML messages and the engine. It also handles the basic game timer.
	/// </summary>
	public class Framework : IEngineCallback
	{

		/// <summary>
		/// The state the game is presently in for communications with the players.
		/// </summary>
		public enum COMM_STATE
		{
			/// <summary>
			/// Could not load the map
			/// </summary>
			NO_MAP,

			/// <summary>
			/// Accepting players joining the game.
			/// </summary>
			ACCEPTING_JOINS,

			/// <summary>
			/// Waiting for players to send setup info.
			/// </summary>
			ACCEPTING_SETUP,

			/// <summary>
			/// Players all setup, ready to start the game.
			/// </summary>
			READY_TO_START,

			/// <summary>
			/// The game is paused.
			/// </summary>
			PAUSED,

			/// <summary>
			/// Executing a step.
			/// </summary>
			STEP,

			/// <summary>
			/// The game is running.
			/// </summary>
			RUNNING,

			/// <summary>
			/// Game is over.
			/// </summary>
			GAME_OVER,
		}

		/// <summary>
		/// The number of players in the game.
		/// </summary>
		public const int NUM_PLAYERS = 10;

		/// <summary>
		/// It posts a status to the AI players every POST_TO_PLAYER_INTERVAL ticks
		/// </summary>
		private const int POST_TO_PLAYER_INTERVAL = 24 * 4;

	    // how many times we update sprites per second (does not change for different ticksPerSecond).
		private const int FRAMES_PER_SECOND = 24;

		private bool fullSpeed;
		private int _prevMovesPerSecond = 96;

		/// <summary>
		/// If this is true, then we do 2K moves/second and no map update. Player status updated once every 5 seconds 
		/// </summary>
		public bool FullSpeed
		{
			get { return fullSpeed; }
			set
			{
				if (value)
				{
					_prevMovesPerSecond = MovesPerSecond;
					fullSpeed = true;
					MovesPerSecond = 2000;
				}
				else
				{
					MovesPerSecond = _prevMovesPerSecond;
					fullSpeed = false;
				}
			}
		}

		// how often we broadcast the game status (does not change for different ticksPerSecond).
		private const int SECONDS_WAIT_READY = 2;

		internal int ticksSinceLastUpdate = POST_TO_PLAYER_INTERVAL;

		/// <summary>
		/// The number of game ticks per second. The default is 60. It takes 24 ticks to move across a tile.
		/// </summary>
		public int MovesPerSecond { get; set; }

		/// <summary>
		/// the number of game ticks from the start of the game. This works off ticksPerSecond and can be changed
		/// to speed up/slow down the game.
		/// </summary>
		public int GameTicks { get; private set; }

		/// <summary>
		/// If true, then start the game as soon as 1 remote AI joins.
		/// </summary>
		public bool DebugStartMode { get; set; }

		/// <summary>
		/// If this user is set, starts, connects to this user, then exits with a return code of 0.
		/// </summary>
		public string TestUser { get; private set; }

		/// <summary>
		/// The number of games to run for the AutoRun. 0 if not an auto-run.
		/// </summary>
		public int AutoRunNumGames { get; private set; }

		/// <summary>
		/// The filename to write the auto run to.
		/// </summary>
		public string AutoRunFilename { get; private set; }

		/// <summary>
		/// If an autorun - the users to allow (and wait for)
		/// </summary>
		public List<string> AutoRunUsers { get; private set; }

		/// <summary>
		/// true if play the traffic noises.
		/// </summary>
		public bool PlaySounds { get; set; }

		// the traffic WAV file.
		public readonly SoundPlayer trafficPlayer;

		public readonly SoundPlayer winPlayer;

		private COMM_STATE commState = COMM_STATE.ACCEPTING_JOINS;

		internal readonly TcpServer tcpServer = new TcpServer();

		// new connections
		private readonly List<string> pendingGuids = new List<string>();

		// the game engine
		private readonly Engine engine;

		internal readonly List<string> mapFilenames = new List<string>(); 

		// the main window.
		internal readonly IUserDisplay mainWindow;

		// The game timer. Fires on every tick.
		private Timer timerWorker;

		// The communication timer. Fires 1 second after requesting setup.
		private Timer timerClientWait;

		private static readonly Random rand = new Random();

		// true when we'll accept messages
		bool acceptMessages = true;

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Framework)); 

		/// <summary>
		/// Create the engine.
		/// </summary>
		/// <param name="mainWindow">The main window.</param>
		public Framework(IUserDisplay mainWindow)
		{
			PlaySounds = true;
			MovesPerSecond = TileMovement.UNITS_PER_TILE * 4;
			this.mainWindow = mainWindow;

			string[] args = Environment.GetCommandLineArgs();
			if (args.Length >= 3 && args[1] == "/t")
				TestUser = args[2].Trim();

			if (args.Length > 4 && (args[1] == "/a"))
			{
				AutoRunNumGames = int.Parse(args[2]);
				AutoRunFilename = Path.GetFullPath(args[3]);
				AutoRunUsers = new List<string>();
				for (int ind=4; ind<args.Length; ind++)
					AutoRunUsers.Add(args[ind]);
			}

			// get directory with maps
			string path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) ?? "";
			if (path.ToLower().EndsWith("bin\\debug"))
				path = path.Substring(0, path.Length - 9);
			else
				if (path.ToLower().EndsWith("bin\\release"))
					path = path.Substring(0, path.Length - 12);

			// get maps
			ConfigurationManager.RefreshSection("Windwardopolis");
			NameValueCollection coll = (NameValueCollection)ConfigurationManager.GetSection("Windwardopolis");
			if (string.IsNullOrEmpty(coll["maps"]))
			{
				commState = COMM_STATE.NO_MAP;
				MessageBox.Show("There is no maps setting in the config file\nThe game will exit.", "Windwardopolis", MessageBoxButtons.OK);
				Environment.Exit(1);
			}
			string[] maps = coll["maps"].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string filename in maps.Select(mapOn => Path.Combine(path, mapOn)).Where(File.Exists))
				mapFilenames.Add(filename);

			if (mapFilenames.Count == 0)
			{
				commState = COMM_STATE.NO_MAP;
				MessageBox.Show("The specified maps do not exist\nThe game will exit.", "Windwardopolis", MessageBoxButtons.OK);
				Environment.Exit(1);
			}

			// traffic noises
			using (Stream wavFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("Windwardopolis.Resources.Town_Traffic_01.wav"))
			{
				trafficPlayer = new SoundPlayer(wavFile);
				trafficPlayer.Load();
			}
			using (Stream wavFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("Windwardopolis.Resources.Crowding Cheering Charge-SoundBible.com-284606164.wav"))
			{
				winPlayer = new SoundPlayer(wavFile);
				winPlayer.Load();
			}

			engine = new Engine(this, mapFilenames[rand.Next(mapFilenames.Count())]);
			mainWindow.NewMap();
			mainWindow.UpdateMap();

			tcpServer.Start(this);
		}

		public Engine GameEngine
		{
			get { return engine; }
		}

		private delegate void StatusMessageDelegate(string text);
		private delegate void IncomingMessageDelegate(string guid, string message);

		public void StatusMessage(string message)
		{
			mainWindow.CtrlForInvoke.BeginInvoke(new StatusMessageDelegate(mainWindow.StatusMessage), new object[] { message });
		}

		public void ConnectionEstablished(string guid)
		{
			pendingGuids.Add(guid);
		}

		public void IncomingMessage(string guid, string message)
		{
			mainWindow.CtrlForInvoke.BeginInvoke(new IncomingMessageDelegate(_IncomingMessage), new object[] { guid, message });
		}

		public void ConnectionLost(string guid)
		{
			mainWindow.CtrlForInvoke.BeginInvoke(new StatusMessageDelegate(_ConnectionLost), new object[] { guid });
		}

		private void _ConnectionLost(string guid)
		{
			Player player = engine.Players.FirstOrDefault(pl => pl.TcpGuid == guid);
			if (player == null)
			{
				log.Warn(string.Format("unknown TCP GUID {0} dropped", guid));
				return;
			}
			player.TcpGuid = null;

			string msg = string.Format("Player {0} lost connection", player.Name);
			log.Info(msg);
			mainWindow.StatusMessage(msg);
		}

		/// <summary>
		/// The game play mode.
		/// </summary>
		public COMM_STATE Mode
		{
			get { return commState; }
		}

		private void _IncomingMessage(string guid, string message)
		{

			if (!acceptMessages)
			{
				Trap.trap();
				return;
			}

			try
			{
				// get the xml
				XDocument xml;
				try
				{
					xml = XDocument.Parse(message);
				}
				catch (Exception ex)
				{
					log.Error(string.Format("Bad message (XML) from connection {0}, exception: {1}", guid, ex));
					// if an existing player we'll just ignore it. Otherwise we close the connection
					if (engine.Players.Any(pl => pl.TcpGuid == guid))
						return;
					Trap.trap();
					tcpServer.CloseConnection(guid);
					return;
				}

				Player player = engine.Players.FirstOrDefault(pl => pl.TcpGuid == guid);
				XElement root = xml.Root;
				if (root == null)
				{
					Trap.trap();
					log.Error(string.Format("Bad message (XML) from connection {0} - no root node", guid));
					return;
				}

				// if not an existing player, it must be <join>
				if ((player == null) && (root.Name.LocalName != "join"))
				{
					Trap.trap();
					log.Error(string.Format("New player from connection {0} - not a join", guid));
					tcpServer.CloseConnection(guid);
					return;
				}

				switch (root.Name.LocalName)
				{
					case "join":
						MsgPlayerJoining(player, guid, root);
						return;
					case "ready":
						MsgPlayerReady(player, root);
						return;
					case "move":
						MsgPlayerOrders(player, root);
						return;
					default:
						Trap.trap();
						log.Error(string.Format("Bad message (XML) from server - root node {0}", root.Name.LocalName));
						break;
				}
			}
			catch (Exception ex)
			{
				mainWindow.StatusMessage(string.Format("Error on incoming message. Exception: {0}", ex));
			}
		}

		private void CommTimerStart()
		{
			lock (this)
			{
				CommTimerClose();
				timerClientWait = new Timer { Interval = SECONDS_WAIT_READY * 1000 };
				timerClientWait.Tick += CommTimeout;
				timerClientWait.Start();
			}
		}

		private void CommTimerClose()
		{
			lock (this)
			{
				if (timerClientWait == null)
					return;
				timerClientWait.Stop();
				timerClientWait.Dispose();
				timerClientWait = null;
			}
		}

		private void CommTimeout(object sender, EventArgs e)
		{
			CommTimerClose();
			commState = COMM_STATE.READY_TO_START;
			mainWindow.CtrlForInvoke.BeginInvoke((MethodInvoker) (() => mainWindow.SetupGame()));
		}

		/// <summary>
		/// Player is (re)joining the game.
		/// </summary>
		/// <param name="player">The player. null if a join, non-null (maybe) if a re-join.</param>
		/// <param name="guid">The tcp guid of the message.</param>
		/// <param name="root">The XML message.</param>
		private void MsgPlayerJoining(Player player, string guid, XElement root)
		{

			// if the player exists do nothing because we're already set up (client sent it twice).
			if (player != null)
			{
				Trap.trap();
				log.Error(string.Format("Join on existing connected player. connection: {0}", guid));
				return;
			}

			// must have a name
			string name = AttributeOrNull(root, "name");
			if (string.IsNullOrEmpty(name))
			{
				Trap.trap();
				log.Error(string.Format("Join with no name refused. Guid: {0}", guid));
				tcpServer.CloseConnection(guid);
				return;
			}

			// if auto-run, must match
			if (AutoRunNumGames > 0)
			{
				if (! AutoRunUsers.Contains(name))
				{
					log.Error(string.Format("Join from non auto-run player {0} refused. Guid: {1}", name, guid));
					tcpServer.CloseConnection(guid);
					return;
				}
			}

			// check for new connection on existing player
			player = engine.Players.FirstOrDefault(pl => pl.Name == name);
			if (player != null && player.TcpGuid == null)
			{
				player.TcpGuid = guid;
				// resend the setup (they may have re-started)
				if (commState != COMM_STATE.ACCEPTING_JOINS && commState != COMM_STATE.GAME_OVER)
				{
					Trap.trap();
					player.WaitingForReply = Player.COMM_MODE.WAITING_FOR_START;
					GameEngine.RestartPlayer(player);
				}
				if (log.IsInfoEnabled)
					log.Info(string.Format("Player {0} reconnected", name));
				mainWindow.StatusMessage(string.Format("Player {0} reconnected", name));
				return;
			}

			// unique name?
			player = engine.Players.FirstOrDefault(pl => pl.Name == name);
			if (player != null)
			{
				log.Error(string.Format("Player {0} name already exists, duplicate refused.", name));
				mainWindow.StatusMessage(string.Format("Player {0} name already exists, duplicate refused.", name));
				tcpServer.CloseConnection(guid);
				return;
			}

			int ind = engine.Players.Count;
			// do we have room?
			if (ind >= NUM_PLAYERS)
			{
				log.Error(string.Format("Can't add a {0}th player. Name: {1}", NUM_PLAYERS + 1, name));
				tcpServer.CloseConnection(guid);
				return;
			}

			// we're in progress - no new players
			if (commState != COMM_STATE.ACCEPTING_JOINS)
			{
				Trap.trap();
				log.Error(string.Format("Game in progress - new players not allowed. Name: {0}", name));
				mainWindow.StatusMessage(string.Format("game in progress - new players not allowed. Name: {0}", name));
				tcpServer.CloseConnection(guid);
				return;
			}

			// ok, we're all good. Create the player
			XElement elemAvatar = root.Element("avatar");
			Image avatar = null;
			if (elemAvatar != null)
			{
				try
				{
					byte[] data = Convert.FromBase64String(elemAvatar.Value);
					avatar = new Bitmap(new MemoryStream(data));
                    // bugbug - scale if not 32x32
					if ((avatar.Width != 32) || (avatar.Height != 32))
						mainWindow.StatusMessage(string.Format("Avatar for player {0} not 32x32", name));
				}
				catch (Exception ex)
				{
					mainWindow.StatusMessage(string.Format("Avatar for player {0} had error {1}", name, ex.Message));
				}
			}

			string school = AttributeOrNull(root, "school");
			string language = AttributeOrNull(root, "language");
			player = new Player(Guid.NewGuid().ToString(), name, school, language, avatar, new Limo(engine.StartLocations[ind], robotBitmaps[ind]), playerColors[ind], new RemoteAI(this, guid));
			engine.Players.Add(player);

			// if we have 10, we're ready to go
			int maxPlayers = AutoRunNumGames == 0 ? NUM_PLAYERS : AutoRunUsers.Count;
			if (DebugStartMode || engine.Players.Count >= maxPlayers || (!string.IsNullOrEmpty(TestUser)))
				CloseJoins();

			string msg = string.Format("Player {0} joined from {1}", name, tcpServer.GetIpAddress(guid));
			log.Info(msg);
			mainWindow.StatusMessage(msg);

			mainWindow.NewPlayerAdded();
			mainWindow.UpdateMenu();
		}

		/// <summary>
		/// Initialize the game, ask players for start positions.
		/// </summary>
		public void CloseJoins()
		{

			// add AI players as needed
			var starts = engine.StartLocations;
			int indName = 0;
            for (int ind = engine.Players.Count; ind < NUM_PLAYERS; ind++)
				engine.Players.Add(new Player(Guid.NewGuid().ToString(), simpleAiNames[indName++], "Windward", "C#", null,
												new Limo(starts[ind], robotBitmaps[ind]), playerColors[ind], new PlayerAI()));
			InitializeGame();
		}

		/// <summary>
		/// Player is ready to start.
		/// </summary>
		/// <param name="player">The player. null if a join, non-null (maybe) if a re-join.</param>
		/// <param name="root">The XML message.</param>
		private void MsgPlayerReady(Player player, XElement root)
		{

			List<Point> path;
			List<Passenger> pickup;
			ReadMessage(player, root, out path, out pickup);
			GameEngine.RemoteMessages.Enqueue(new Engine.AiMessage(player, path, pickup));

			player.WaitingForReply = Player.COMM_MODE.RECEIVED_START;
			// if all ready (or all AI), we start
			if (engine.Players.All(pl => pl.WaitingForReply == Player.COMM_MODE.RECEIVED_START || pl.TcpGuid == null))
			{
				if (commState == COMM_STATE.ACCEPTING_JOINS || commState == COMM_STATE.ACCEPTING_SETUP)
					commState = COMM_STATE.READY_TO_START;
				CommTimerClose();
				mainWindow.SetupGame();

				// test mode? If so we're good and so exit with code 0.
				if ((!string.IsNullOrEmpty(TestUser)) && player.Name == TestUser)
				{
					tcpServer.SendMessage(player.TcpGuid, "<exit/>");
					Thread.Sleep(100);
					Environment.Exit(0);
				}

				if (DebugStartMode || AutoRunNumGames != 0)
					Play();
			}
		}

		private void ReadMessage(Player player, XElement root, out List<Point> path, out List<Passenger> pickup)
		{

			// get the requested path. Does not check that it is on the map or continuous - the engine checks that.
			path = new List<Point>();
			XElement element = root.Element("path");
			if (element != null)
			{
				string trail = element.Value;
				if (!string.IsNullOrEmpty(trail))
				{
					string[] steps = trail.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string stepOn in steps)
					{
						string[] coords = stepOn.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						int x, y;
						if ((coords.Length != 2) || (!int.TryParse(coords[0], out x)) || (!int.TryParse(coords[1], out y)))
						{
							Trap.trap();
							StatusMessage(string.Format("Player {0} provided an invalid path element {1}", player.Name, stepOn));
							break;
						}
						path.Add(new Point(x, y));
					}
				}
			}

			// get the requested pick-up list. Does not check for already delivered - the engine checks that.
			pickup = new List<Passenger>();
			element = root.Element("pick-up");
			if (element != null)
			{
				string allNames = element.Value;
				if (!string.IsNullOrEmpty(allNames))
				{
					string[] names = allNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string nameOn in names)
					{
						Passenger psngr = GameEngine.Passengers.FirstOrDefault(ps => ps.Name == nameOn);
						if (psngr != null)
							pickup.Add(psngr);
						else
						{
							Trap.trap();
							StatusMessage(string.Format("Player {0} provided an invalid pick-up name {1}", player.Name, nameOn));
						}
					}
				}
			}
		}

		/// <summary>
		/// Player is ready to start.
		/// </summary>
		/// <param name="player">The player. null if a join, non-null (maybe) if a re-join.</param>
		/// <param name="root">The XML message.</param>
		private void MsgPlayerOrders(Player player, XElement root)
		{
			List<Point> path;
			List<Passenger> pickup;
			ReadMessage(player, root, out path, out pickup);
			GameEngine.RemoteMessages.Enqueue(new Engine.AiMessage(player, path, pickup));
		}

		private void InitializeGame()
		{

			commState = COMM_STATE.ACCEPTING_SETUP;

			// reset engine & players
			engine.Initialize();

			// if all ready (or all AI), we start
			if (engine.Players.All(pl => pl.WaitingForReply == Player.COMM_MODE.RECEIVED_START || pl.TcpGuid == null))
				commState = COMM_STATE.READY_TO_START;
			else
				CommTimerStart();

			mainWindow.SetupGame();
		}

		public void RestartJoins()
		{
			acceptMessages = false;

			foreach (Player player in engine.Players.Where(player => player.TcpGuid != null))
				tcpServer.SendMessage(player.TcpGuid, "<exit/>");
			Thread.Sleep(100);

			foreach (Player plyr in engine.Players)
				tcpServer.CloseConnection(plyr.TcpGuid);
			foreach (Player plyr in engine.Players)
				plyr.Dispose();
			engine.Players.Clear();

			commState = COMM_STATE.ACCEPTING_JOINS;
			acceptMessages = true;

			const string msg = "Clear players, re-open for joins";
			log.Info(msg);
			mainWindow.StatusMessage(msg);

			mainWindow.ResetPlayers();
			mainWindow.UpdateMenu();
		}

		/// <summary>
		/// Start or continue (from pause) the game.
		/// </summary>
		public void Play()
		{

			if (AutoRunNumGames != 0)
				FullSpeed = true;

			if ((commState == COMM_STATE.PAUSED) && (timerWorker != null))
			{
				commState = COMM_STATE.RUNNING;
				timerWorker.Start();
				if (PlaySounds)
					trafficPlayer.PlayLooping();
			}
			else
				_Play();
		}

		private void _Play()
		{

			// in case a reset
			if (timerWorker == null)
			{
				if (engine.GameOn != 0)
					InitializeGame();
				engine.GameOn++;

				GameTicks = 0;

				timerWorker = new Timer { Interval = 1000 / FRAMES_PER_SECOND, Tag = rand.Next() };
				timerWorker.Tick += FrameTick;
			}

			// we're running
			CommTimerClose();
			commState = COMM_STATE.RUNNING;

			mainWindow.UpdateMap();
			mainWindow.UpdatePlayers();
			mainWindow.UpdateMenu();

			timerWorker.Start();

			if (PlaySounds)
				trafficPlayer.PlayLooping();
		}

		public void Step()
		{
			// stop the ticker
			if (timerWorker != null)
				timerWorker.Stop();
			else
			{
				// new game - we create the timer as that IDs if we're starting a new game. But we don't start it
				if (engine.GameOn != 0)
					InitializeGame();
				engine.GameOn++;

				GameTicks = 0;

				timerWorker = new Timer { Interval = 1000 / FRAMES_PER_SECOND, Tag = rand.Next() };
				timerWorker.Tick += FrameTick;

				CommTimerClose();
				commState = COMM_STATE.PAUSED;
			}

			// run one tick
			commState = COMM_STATE.STEP;
			FrameTick(null, null);
			commState = COMM_STATE.PAUSED;
			
			// update windows
			mainWindow.UpdateMap();
			mainWindow.UpdateMenu();
		}

		public void PauseAtEndOfTurn()
		{
			commState = COMM_STATE.PAUSED;
			timerWorker.Stop();
			if (PlaySounds)
				trafficPlayer.Stop();

			// update windows
			mainWindow.UpdateMap();
			mainWindow.UpdateMenu();
		}

		/// <summary>
		/// End the game.
		/// </summary>
		public void Stop()
		{
			FullSpeed = false;
			commState = COMM_STATE.GAME_OVER;
			CommTimerClose();
			if (timerWorker != null)
			{
				timerWorker.Stop();
				timerWorker.Dispose();
				timerWorker = null;
			}
			if (PlaySounds)
				trafficPlayer.Stop();
			commState = COMM_STATE.GAME_OVER;

			// update windows
			mainWindow.UpdateMap();
			mainWindow.UpdateMenu();
		}

		private DateTime last;

		/// <summary>
		/// The main timer tick handler. Calls all game logic from here on each tick. This is called once every FRAMES_PER_SECOND.
		/// </summary>
		private void FrameTick(object sender, EventArgs e)
		{

			int numTicks = commState == COMM_STATE.STEP ? 1 : MovesPerSecond / FRAMES_PER_SECOND;
			numTicks = Math.Max(numTicks, 1);
			numTicks = Math.Min(numTicks, FullSpeed ? 500 : 42);

			while (numTicks-- > 0)
			{
				// find winner in case game over
				Player plyrWin = engine.Players.OrderByDescending(pl => pl.PassengersDelivered.Count).First();
				if (plyrWin.PassengersDelivered.Count >= Player.NUM_PASSENGERS_TO_WIN)
				{
					mainWindow.StatusMessage(string.Format("Game over, winner {0} with {1} points", plyrWin.Name, plyrWin.Score));
					GameOver();
					return;
				}

				// see if time to post a status update
				if (ticksSinceLastUpdate++ >= POST_TO_PLAYER_INTERVAL)
				{
					XDocument xmlMessage = RemoteAI.BuildMessageXml(engine.Players, engine.Passengers);
					foreach (Player plyrOn in engine.Players)
						plyrOn.GameStatus(xmlMessage, plyrOn, plyrOn.Limo.PathTiles.Count == 0
						                                      	? PlayerAIBase.STATUS.NO_PATH
						                                      	: PlayerAIBase.STATUS.UPDATE, engine.Players, engine.Passengers);
					ticksSinceLastUpdate = 0;
				}

				GameTicks++;
				engine.Tick();
			}

			// render the map & player status
			if (!FullSpeed)
			{
				List<Point> limoLocations = engine.Players.Select(plyrOn => plyrOn.Limo.Location.MapPosition).ToList();
				mainWindow.RenderMapChanges(limoLocations);
				mainWindow.UpdatePlayers();
			}
			else
				if (playerUpdateInterval-- < 0)
				{
					mainWindow.UpdatePlayers();
					playerUpdateInterval = 24;
				}
		}

		private int playerUpdateInterval = 0;

		private void GameOver()
		{
			Stop();
			if (PlaySounds)
				winPlayer.Play();

			// add scores to playes
			foreach (Player plyrOn in engine.Players)
				plyrOn.Scoreboard.Add(plyrOn.Score);

			// update all windows
			mainWindow.UpdateDebug();
			mainWindow.UpdatePlayers();
			mainWindow.UpdateMenu();
			mainWindow.UpdateMap();

			if (AutoRunNumGames <= 0)
				return;
			if (engine.GameOn < AutoRunNumGames)
			{
				Play();
				return;
			}

			// write out the results
			XDocument xml = new XDocument();
			XElement root = new XElement("players");
			xml.Add(root);
			foreach (Player player in engine.Players)
			{
				XElement elem = new XElement("player", new XAttribute("name", player.Name)) { Value = string.Join(";", player.Scoreboard) };
				root.Add(elem);
			}
			xml.Save(AutoRunFilename);

			foreach (Player player in engine.Players.Where(player => player.TcpGuid != null))
				tcpServer.SendMessage(player.TcpGuid, "<exit/>");

			Program.exitCode = 0;
			mainWindow.Exit();
		}

		private static string AttributeOrNull(XElement element, string name)
		{
			XAttribute attr = element.Attribute(name);
			return attr == null ? null : attr.Value;
		}

		// names for SimpleAI
		private static readonly string[] simpleAiNames = {
		                                          	"Sheldon Cooper", "Leonard Hofstadter", "Rajesh Koothrappali", "Howard Wolowitz", "Penny", 
													"Bernadette Rostenkowski", "Amy Farrah Fowler", "Leslie Winkle", "Barry Kripke", "Stuart Bloom"
		                                          };

		private static readonly Bitmap[] robotBitmaps = {
		                                                	Sprites.Car_01, Sprites.Car_02, Sprites.Car_03, Sprites.Car_04, Sprites.Car_05, 
															Sprites.Car_06, Sprites.Car_07, Sprites.Car_08, Sprites.Car_09, Sprites.Car_10
		                                                };

		private static readonly Color[] playerColors = {
		                                               	Color.FromArgb(255, 255, 3, 3), Color.FromArgb( 255, 0, 66, 255), Color.FromArgb(255, 28, 230, 185), Color.FromArgb(255, 83, 0, 127), Color.FromArgb(255, 255, 252, 1),
		                                               	Color.FromArgb(255, 254, 138, 14), Color.FromArgb(255, 32, 192, 0), Color.FromArgb(255, 229, 91, 176), Color.FromArgb(255, 126, 191, 241), Color.FromArgb(255, 16, 98, 70)
		                                               };

	}
}
