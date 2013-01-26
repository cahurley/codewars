using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using Windwardopolis.game_ai;
using WindwardopolisLibrary;
using WindwardopolisLibrary.map;
using WindwardopolisLibrary.units;

namespace Windwardopolis
{
	/// <summary>
	/// The main window.
	/// </summary>
	public partial class MainWindow : Form, IUserDisplay, IMapInfo
	{
		private PlayerScore dlgScoreboard;
		private StatusMessages dlgStatusList;
		private DebugWindow dlgDebugWindow;

		/// <summary>
		/// Create the window.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			mapDisplay.MouseClickedEvent += mapDisplay_MouseClickedEvent;
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			// initialize engine
			GameFramework = new Framework(this);

			// restore windows
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Windward Studios\Windwardopolis\windows"))
				if (key != null)
				{
					// only restore if matches
					int cx = (int) key.GetValue("desktop-cx");
					int cy = (int) key.GetValue("desktop-cy");
					if ((cx == Screen.PrimaryScreen.WorkingArea.Width) && (cy == Screen.PrimaryScreen.WorkingArea.Height))
					{
						RestoreWindow(this, "main");
						FormWindowState state;
						Point location;
						Size size;
						RestoreWindow(key, "scoreboard", out state, out location, out size);
						if (state != FormWindowState.Minimized)
							CreateOrActivateScoreboard();
						RestoreWindow(key, "status", out state, out location, out size);
						if (state != FormWindowState.Minimized)
							CreateOrActivateStatus();
					}

					GameFramework.MovesPerSecond = (int) key.GetValue("game-speed", GameFramework.MovesPerSecond);

					toolStripMenuItemCoordinates.Checked = Convert.ToBoolean(key.GetValue("display-coordinates", "false"));
					toolStripMenuItemLimoShadow.Checked = Convert.ToBoolean(key.GetValue("display-shadow", "false"));
					toolStripMenuItemLimoShadowAll.Checked = Convert.ToBoolean(key.GetValue("display-shadow-all", "false"));

					GameFramework.DebugStartMode = Convert.ToBoolean(key.GetValue("run-debug", "false"));
					GameFramework.PlaySounds = Convert.ToBoolean(key.GetValue("play-sounds", "true"));
				}

			DebugCheckEnable();
			MuteBitmap();

			mapDisplay.DisplayCoordinates = toolStripMenuItemCoordinates.Checked;
			mapDisplay.DisplayShadows = toolStripMenuItemLimoShadow.Checked;
			mapDisplay.DisplayShadowsAll = toolStripMenuItemLimoShadowAll.Checked;

			PixelsPerTile = 24;
			toolStripMenuItemZoom100.Checked = true;
			mapDisplay.ZoomChanged();

			UpdateMap();
			UpdatePlayers();
			UpdateMenu();
		}

		public void Exit()
		{
			Close();
		}

		public static void RestoreWindow(Form ctrl, string name)
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Windward Studios\Windwardopolis\windows"))
				if (key != null)
				{
					// only restore if matches
					int cx = (int) key.GetValue("desktop-cx");
					int cy = (int) key.GetValue("desktop-cy");
					if ((cx == Screen.PrimaryScreen.WorkingArea.Width) && (cy == Screen.PrimaryScreen.WorkingArea.Height))
					{
						FormWindowState state;
						Point location;
						Size size;
						RestoreWindow(key, name, out state, out location, out size);
						switch (state)
						{
							case FormWindowState.Maximized:
								ctrl.WindowState = state;
								break;
							case FormWindowState.Normal:
								ctrl.Location = location;
								ctrl.Size = size;
								break;
						}
					}
				}
		}

		/// <summary>
		/// The engine that operates the game.
		/// </summary>
		internal Framework GameFramework { get; private set; }

		/// <summary>
		/// Used for Invoke when we get TCP callbacks.
		/// </summary>
		public Control CtrlForInvoke
		{
			get { return this; }
		}

		private void toolStripButtonJoin_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Join");
			if (
				MessageBox.Show(this, @"Do you want to end this series and drop all players?", @"Windwardopolis",
				                MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			GameFramework.RestartJoins();
			UpdateMenu();
		}

		private void toolStripButtonClosed_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Locked");
			if (GameFramework.GameEngine.Players.Count < Framework.NUM_PLAYERS)
				if (
					MessageBox.Show(this, @"Have all players joined for this series?", @"Windwardopolis", MessageBoxButtons.YesNo) !=
					DialogResult.Yes)
					return;
			GameFramework.CloseJoins();
			UpdateMenu();
		}

		/// <summary>
		/// The play button was clicked.
		/// </summary>
		private void Play_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Play");
			GameFramework.Play();
			UpdateMenu();
		}

		private void toolStripButtonStep_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Step");
			GameFramework.Step();
			UpdateMenu();
		}

		/// <summary>
		/// The pause button was clicked.
		/// </summary>
		private void Pause_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Pause");
			GameFramework.FullSpeed = false;
			GameFramework.PauseAtEndOfTurn();
			UpdateMenu();
		}

		/// <summary>
		/// The stop button was clicked.
		/// </summary>
		private void Stop_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Stop");
			if (MessageBox.Show(this, @"Do you want to end this run?", @"Windwardopolis", MessageBoxButtons.YesNo) !=
			    DialogResult.Yes)
				return;
			GameFramework.FullSpeed = false;
			GameFramework.Stop();
			UpdateMenu();
		}

		private void toolStripButtonSpeed_Click(object sender, EventArgs e)
		{

			Trace.WriteLine("menu: Speed");
			using (GameSpeed dlg = new GameSpeed(GameFramework.MovesPerSecond))
			{
				if (dlg.ShowDialog() == DialogResult.OK)
					GameFramework.MovesPerSecond = dlg.MovesPerSecond;
			}
			UpdateMenu();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			GameFramework.PlaySounds = ! GameFramework.PlaySounds;
			MuteBitmap();

			if (GameFramework.Mode == Framework.COMM_STATE.RUNNING)
			{
				if (!GameFramework.PlaySounds)
					GameFramework.trafficPlayer.Stop();
				else
					GameFramework.trafficPlayer.PlayLooping();
			}
		}

		private void MuteBitmap()
		{
			toolStripMuteSound.Image = GameFramework.PlaySounds ? Status.loudspeaker : Status.loudspeaker_preferences;
		}

		private void toolStripMenuItem2_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 48;
			toolStripMenuItemZoom200.Checked = true;
			toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom75.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
		}

		private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 24;
			toolStripMenuItemZoom100.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom75.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
		}

		private void toolStripMenuItem4_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 18;
			toolStripMenuItemZoom75.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
		}

		private void toolStripMenuItem5_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 12;
			toolStripMenuItemZoom50.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom75.Checked = false;
			mapDisplay.Invalidate();
		}

		private void toolStripMenuItemCoordinates_Click(object sender, EventArgs e)
		{
			toolStripMenuItemCoordinates.Checked = !toolStripMenuItemCoordinates.Checked;
			mapDisplay.DisplayCoordinates = toolStripMenuItemCoordinates.Checked;
			mapDisplay.Invalidate();
		}

		private void ToolStripMenuItemLimoShadow_Click(object sender, EventArgs e)
		{
			toolStripMenuItemLimoShadow.Checked = !toolStripMenuItemLimoShadow.Checked;
			if (toolStripMenuItemLimoShadow.Checked)
				toolStripMenuItemLimoShadowAll.Checked = false;
			mapDisplay.DisplayShadows = toolStripMenuItemLimoShadow.Checked;
			mapDisplay.DisplayShadowsAll = toolStripMenuItemLimoShadowAll.Checked;
			mapDisplay.Invalidate();
		}

		private void toolStripMenuItemLimoShadowAll_Click(object sender, EventArgs e)
		{
			toolStripMenuItemLimoShadowAll.Checked = !toolStripMenuItemLimoShadowAll.Checked;
			if (toolStripMenuItemLimoShadowAll.Checked)
				toolStripMenuItemLimoShadow.Checked = false;
			mapDisplay.DisplayShadows = toolStripMenuItemLimoShadow.Checked;
			mapDisplay.DisplayShadowsAll = toolStripMenuItemLimoShadowAll.Checked;
			mapDisplay.Invalidate();
		}

		private void toolStripDebugReset_Click(object sender, EventArgs e)
		{
			GameFramework.Stop();
			GameFramework.RestartJoins();
		}

		#region scoreboard

		private void toolStripButtonScoreboard_Click(object sender, EventArgs e)
		{
			CreateOrActivateScoreboard();
		}

		private void CreateOrActivateScoreboard()
		{
			if (dlgScoreboard != null)
			{
				dlgScoreboard.WindowState = FormWindowState.Normal;
				dlgScoreboard.Activate();
				return;
			}

			dlgScoreboard = new PlayerScore(GameFramework);
			dlgScoreboard.SetupGame();
			dlgScoreboard.Closing += dlgScoreboard_Closing;
			dlgScoreboard.Show(this);
		}

		private void dlgScoreboard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			dlgScoreboard = null;
		}

		#endregion

		#region debug window

		private void toolStripButtonDebugWindow_Click(object sender, EventArgs e)
		{
			CreateOrActivateDebugWindow();
		}

		private void CreateOrActivateDebugWindow()
		{
			if (dlgDebugWindow != null)
			{
				dlgDebugWindow.WindowState = FormWindowState.Normal;
				dlgDebugWindow.Activate();
				return;
			}

			dlgDebugWindow = new DebugWindow(GameFramework.GameEngine.Players, GameFramework.GameEngine.companies,
			                                 GameFramework.GameEngine.Passengers);
			dlgDebugWindow.Closing += dlgDebugWindow_Closing;
			dlgDebugWindow.Show();
		}

		private void dlgDebugWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			dlgDebugWindow = null;
		}

		#endregion

		#region Debug move vehicles

		private Player playerMouseMove;
		private Player playerMouseSetDest;

		private void mapDisplay_MouseClickedEvent(Point mapUnits, MouseButtons buttons)
		{

			Cursor = Cursors.Default;
			if (GameFramework.Mode != Framework.COMM_STATE.PAUSED && GameFramework.Mode != Framework.COMM_STATE.READY_TO_START)
			{
				playerMouseMove = playerMouseSetDest = null;
				return;
			}

			if ((buttons & MouseButtons.Right) != 0)
			{
				foreach (Player plyrOn in GameFramework.GameEngine.Players)
				{
					int xDiff = Math.Abs(plyrOn.Limo.Location.MapPosition.X - mapUnits.X);
					int yDiff = Math.Abs(plyrOn.Limo.Location.MapPosition.Y - mapUnits.Y);
					if ((xDiff >= PixelsPerTile/4) || (yDiff >= PixelsPerTile/4))
						continue;
					contextMenuRMB.Tag = plyrOn;
					foreach (ToolStripMenuItem menu in toolStripMenuItemSetPassenger.DropDownItems)
						menu.Enabled = !plyrOn.PassengersDelivered.Contains((Passenger) menu.Tag);
					contextMenuRMB.Show(MousePosition);
					return;
				}
				Console.Beep();
				return;
			}

			if (((buttons & MouseButtons.Left) == 0) || (playerMouseMove == null && playerMouseSetDest == null))
				return;

			// get the quarter tile location (needed for direction) on car move.
			Point ptQuarter = new Point(mapUnits.X/(TileMovement.UNITS_PER_TILE/2), mapUnits.Y/(TileMovement.UNITS_PER_TILE/2));
			Point ptTile = new Point(ptQuarter.X/2, ptQuarter.Y/2);

			// can only set the car or dest in a road or bus-stop
			MapSquare square = GameFramework.GameEngine.MainMap.SquareOrDefault(ptTile);
			if (square == null)
			{
				Console.Beep();
				Trap.trap();
				return;
			}

			if (playerMouseSetDest != null)
			{
				if (square.Tile.Type != MapTile.TYPE.BUS_STOP)
				{
					Console.Beep();
					playerMouseMove = playerMouseSetDest = null;
					return;
				}

				List<Point> path = SimpleAStar.CalculatePath(GameFramework.GameEngine.MainMap,
				                                             playerMouseSetDest.Limo.Location.TilePosition, ptTile);
				if (path.Count > 1)
					path.Add(path[path.Count - 2]);
				GameFramework.GameEngine.ProcessAllMoveMessages();
				GameFramework.GameEngine.PlayerOrders(playerMouseSetDest, path, new List<Passenger>());
				playerMouseMove = playerMouseSetDest = null;
			}

			if (playerMouseMove != null)
			{
				if (square.Tile.Type != MapTile.TYPE.ROAD && square.Tile.Type != MapTile.TYPE.BUS_STOP)
				{
					Console.Beep();
					playerMouseMove = playerMouseSetDest = null;
					return;
				}

				// process before we change the location.
				GameFramework.GameEngine.ProcessAllMoveMessages();

				switch (square.Tile.Direction)
				{
					case MapTile.DIRECTION.NORTH_SOUTH:
						playerMouseMove.Limo.SetTileLocation(square, ptTile,
						                                     (ptQuarter.X & 1) == 1
						                                     	? MapSquare.COMPASS_DIRECTION.NORTH
						                                     	: MapSquare.COMPASS_DIRECTION.SOUTH);
						break;
					case MapTile.DIRECTION.EAST_WEST:
						playerMouseMove.Limo.SetTileLocation(square, ptTile,
						                                     (ptQuarter.Y & 1) == 1
						                                     	? MapSquare.COMPASS_DIRECTION.EAST
						                                     	: MapSquare.COMPASS_DIRECTION.WEST);
						break;

					case MapTile.DIRECTION.INTERSECTION:
						if ((ptQuarter.X & 1) == 1 && (ptQuarter.Y & 1) == 1)
							playerMouseMove.Limo.SetTileLocation(square, ptTile, MapSquare.COMPASS_DIRECTION.NORTH);
						else if ((ptQuarter.X & 1) == 0 && (ptQuarter.Y & 1) == 1)
							playerMouseMove.Limo.SetTileLocation(square, ptTile, MapSquare.COMPASS_DIRECTION.EAST);
						else if ((ptQuarter.X & 1) == 0 && (ptQuarter.Y & 1) == 0)
							playerMouseMove.Limo.SetTileLocation(square, ptTile, MapSquare.COMPASS_DIRECTION.SOUTH);
						else
							playerMouseMove.Limo.SetTileLocation(square, ptTile, MapSquare.COMPASS_DIRECTION.WEST);
						break;

					default:
						Console.Beep();
						playerMouseMove = playerMouseSetDest = null;
						return;
				}

				Point ptDest = playerMouseMove.NextBusStop != null
				               	? playerMouseMove.NextBusStop.BusStop
				               	: (playerMouseMove.Limo.PathTiles.Count > 1
				               	   	? playerMouseMove.Limo.PathTiles[playerMouseMove.Limo.PathTiles.Count - 2]
				               	   	: Point.Empty);
				List<Point> path;
				if (ptDest != Point.Empty)
				{
					path = SimpleAStar.CalculatePath(GameFramework.GameEngine.MainMap, playerMouseMove.Limo.Location.TilePosition,
					                                 ptDest);
					if (path.Count > 1)
						path.Add(path[path.Count - 2]);
				}
				else
					path = new List<Point>();
				GameFramework.GameEngine.PlayerOrders(playerMouseMove, path, new List<Passenger>());

				playerMouseMove = playerMouseSetDest = null;
				mapDisplay.Invalidate();
				UpdatePlayers();
				UpdateDebug();
			}
		}

		private void toolStripMenuItemMoveCar_Click(object sender, EventArgs e)
		{
			playerMouseMove = contextMenuRMB.Tag as Player;
			Cursor = Cursors.Cross;
		}

		private void toolStripMenuItemSetDest_Click(object sender, EventArgs e)
		{
			playerMouseSetDest = contextMenuRMB.Tag as Player;
			Cursor = Cursors.Cross;
		}

		private void selectCompany_Click(object sender, EventArgs e)
		{
			Player plyr = contextMenuRMB.Tag as Player;
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			if (plyr == null || menuItem == null)
			{
				Trap.trap();
				Console.Beep();
				return;
			}

			// process before we change the passenger.
			GameFramework.GameEngine.ProcessAllMoveMessages();

			Passenger passenger = (Passenger) menuItem.Tag;
			Company companyAt = passenger.Lobby;
			Limo travelingIn = passenger.Car;

			passenger.DebugAssign(plyr.Limo);
			plyr.Passenger = passenger;
			plyr.PickUp.Remove(passenger);

			if (companyAt != null)
				companyAt.Passengers.Remove(passenger);
			else if (travelingIn != null)
				Players.Find(pl => pl.Limo == travelingIn).Passenger = null;
			UpdatePlayers();
			UpdateDebug();
		}

		#endregion

		#region status form

		private void toolStripButtonStatusList_Click(object sender, EventArgs e)
		{
			CreateOrActivateStatus();
		}

		private void CreateOrActivateStatus()
		{
			if (dlgStatusList != null)
			{
				dlgStatusList.WindowState = FormWindowState.Normal;
				dlgStatusList.Activate();
				return;
			}

			dlgStatusList = new StatusMessages();
			dlgStatusList.Closing += dlgStatusList_Closing;
			dlgStatusList.Show();
			foreach (string msg in messages)
				dlgStatusList.AddMessage(msg);
		}

		private void dlgStatusList_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			dlgStatusList = null;
		}

		private readonly IList<string> messages = new List<string>();

		/// <summary>
		/// Adds a message to the status window.
		/// </summary>
		/// <param name="message">The message to add.</param>
		public void StatusMessage(string message)
		{
			messages.Add(message);
			if (dlgStatusList != null)
				dlgStatusList.AddMessage(message);
		}

		#endregion

		/// <summary>
		/// Update the main window menu.
		/// </summary>
		public void UpdateMenu()
		{
			toolStripButtonJoinOpened.Enabled = GameFramework.Mode == Framework.COMM_STATE.GAME_OVER;
			toolStripButtonJoinClosed.Enabled = GameFramework.Mode == Framework.COMM_STATE.ACCEPTING_JOINS; // bugbug && GameFramework.GameEngine.Players.Count > 0;
			toolStripButtonPlay.Enabled = GameFramework.Mode == Framework.COMM_STATE.READY_TO_START ||
			                              GameFramework.Mode == Framework.COMM_STATE.PAUSED ||
			                              GameFramework.Mode == Framework.COMM_STATE.GAME_OVER;
			toolStripButtonStep.Enabled = GameFramework.Mode == Framework.COMM_STATE.READY_TO_START ||
			                              GameFramework.Mode == Framework.COMM_STATE.PAUSED ||
			                              GameFramework.Mode == Framework.COMM_STATE.GAME_OVER;
			toolStripButtonPause.Enabled = GameFramework.Mode == Framework.COMM_STATE.RUNNING;
			toolStripButtonStop.Enabled = GameFramework.Mode == Framework.COMM_STATE.RUNNING ||
			                              GameFramework.Mode == Framework.COMM_STATE.PAUSED;

			toolStripButtonDebugWindow.Enabled = GameFramework.GameEngine.Players != null &&
			                                     GameFramework.GameEngine.Players.Count > 0 &&
			                                     GameFramework.GameEngine.companies != null &&
			                                     GameFramework.GameEngine.companies.Count > 0 &&
			                                     GameFramework.GameEngine.Passengers != null &&
			                                     GameFramework.GameEngine.Passengers.Count > 0;
			toolStripDebugReset.Enabled = GameFramework.DebugStartMode &&
			                              (GameFramework.Mode == Framework.COMM_STATE.RUNNING ||
			                               GameFramework.Mode == Framework.COMM_STATE.STEP
			                               || GameFramework.Mode == Framework.COMM_STATE.PAUSED ||
			                               GameFramework.Mode == Framework.COMM_STATE.GAME_OVER);
			toolStripFullSpeed.Enabled = GameFramework.Mode == Framework.COMM_STATE.READY_TO_START ||
										  GameFramework.Mode == Framework.COMM_STATE.PAUSED ||
										  GameFramework.Mode == Framework.COMM_STATE.GAME_OVER;
		}

		/// <summary>
		/// Got a close window event. Ask for confirmation if a game is in process.
		/// </summary>
		private void GameDisplay_FormClosing(object sender, FormClosingEventArgs fcea)
		{
			if ((GameFramework.Mode != Framework.COMM_STATE.GAME_OVER) &&
			    (MessageBox.Show(this, @"Do you want to close the game?", @"Windwardopolis", MessageBoxButtons.YesNo) !=
			     DialogResult.Yes))
				fcea.Cancel = true;

			if (fcea.Cancel)
				return;

			SaveWindowLocations();

			if (dlgScoreboard != null)
				dlgScoreboard.Close();
			if (dlgStatusList != null)
				dlgStatusList.Close();
			if (dlgDebugWindow != null)
				dlgDebugWindow.Close();
			GameFramework.tcpServer.CloseAllConnections();
		}

		private void SaveWindowLocations()
		{
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Windward Studios\Windwardopolis\windows"))
				if (key != null)
				{
					// save desktop size - only restore if matches
					key.SetValue("desktop-cx", Screen.PrimaryScreen.WorkingArea.Width);
					key.SetValue("desktop-cy", Screen.PrimaryScreen.WorkingArea.Height);

					SaveWindow(key, this, "main");
					if (dlgScoreboard != null)
						SaveWindow(key, dlgScoreboard, "scoreboard");
					else
						key.SetValue("scoreboard-mode", FormWindowState.Minimized);
					if (dlgStatusList != null)
						SaveWindow(key, dlgStatusList, "status");
					else
						key.SetValue("status-mode", FormWindowState.Minimized);

					key.SetValue("game-speed", GameFramework.MovesPerSecond);

					key.SetValue("display-coordinates", Convert.ToString(toolStripMenuItemCoordinates.Checked));
					key.SetValue("display-shadow", Convert.ToString(toolStripMenuItemLimoShadow.Checked));
					key.SetValue("display-shadow-all", Convert.ToString(toolStripMenuItemLimoShadowAll.Checked));

					key.SetValue("run-debug", Convert.ToString(GameFramework.DebugStartMode));
					key.SetValue("play-sounds", Convert.ToString(GameFramework.PlaySounds));
				}
		}

		private static void SaveWindow(RegistryKey key, Form ctrl, string name)
		{
			key.SetValue(name + "-mode", ctrl.WindowState);
			key.SetValue(name + "-x", ctrl.Location.X);
			key.SetValue(name + "-y", ctrl.Location.Y);
			key.SetValue(name + "-cx", ctrl.Size.Width);
			key.SetValue(name + "-cy", ctrl.Size.Height);
		}

		private static void RestoreWindow(RegistryKey key, string name, out FormWindowState state, out Point location,
		                                  out Size size)
		{
			string str = (string) key.GetValue(name + "-mode");
			state = string.IsNullOrEmpty(str)
			        	? FormWindowState.Minimized
			        	: (FormWindowState) Enum.Parse(typeof (FormWindowState), str);
			int x = (int) key.GetValue(name + "-x", -1);
			int y = (int) key.GetValue(name + "-y", -1);
			location = x == -1 || y == -1 ? Point.Empty : new Point(x, y);
			x = (int) key.GetValue(name + "-cx", -1);
			y = (int) key.GetValue(name + "-cy", -1);
			size = x == -1 || y == -1 ? Size.Empty : new Size(x, y);
		}

		/// <summary>
		/// The app is exiting - close all connections.
		/// </summary>
		private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			GameFramework.tcpServer.CloseAllConnections();
		}

		/// <summary>
		/// Called when starting a game.
		/// </summary>
		public void SetupGame()
		{
			if (dlgScoreboard != null)
				dlgScoreboard.SetupGame();
			if (dlgDebugWindow != null)
				dlgDebugWindow.Setup(GameFramework.GameEngine.Players, GameFramework.GameEngine.companies,
				                     GameFramework.GameEngine.Passengers);

			// put all companies in the pop-up
			toolStripMenuItemSetPassenger.DropDownItems.Clear();
			foreach (Passenger psngr in GameFramework.GameEngine.Passengers)
			{
				ToolStripMenuItem menu = new ToolStripMenuItem(psngr.Name) {Tag = psngr};
				menu.Click += selectCompany_Click;
				toolStripMenuItemSetPassenger.DropDownItems.Add(menu);
			}

			UpdateMap();
			UpdatePlayers();
			UpdateMenu();
			UpdateDebug();
		}

		/// <summary>
		/// New player added to the game
		/// </summary>
		public void NewPlayerAdded()
		{
			if (dlgScoreboard != null)
				dlgScoreboard.NewPlayerAdded();
			Trap.trap(dlgDebugWindow != null);
			if (dlgDebugWindow != null)
				dlgDebugWindow.Setup(GameFramework.GameEngine.Players, GameFramework.GameEngine.companies,
				                     GameFramework.GameEngine.Passengers);
		}

		/// <summary>
		/// Update the map window.
		/// </summary>
		public void UpdateMap()
		{
			mapDisplay.Invalidate();
			mapDisplay.Update();
		}

		/// <summary>
		/// Called to re-render all locations with vehicles.
		/// </summary>
		/// <param name="limoLocations">Where the limos are presently located.</param>
		public void RenderMapChanges(List<Point> limoLocations)
		{
			mapDisplay.InvalidateLimos(limoLocations);
			mapDisplay.Update();
		}

		/// <summary>
		/// Called to update (re-draw) the player status windows.
		/// </summary>
		public void UpdatePlayers()
		{
			if (dlgScoreboard != null)
				dlgScoreboard.UpdatePlayers();
		}

		public void UpdateDebug()
		{
			if (dlgDebugWindow != null)
				dlgDebugWindow.Update(GameFramework.GameEngine.Players, GameFramework.GameEngine.companies,
				                      GameFramework.GameEngine.Passengers);
		}

		/// <summary>
		/// Called to delete all player status windows the player status windows.
		/// </summary>
		public void ResetPlayers()
		{
			Trap.trap(dlgScoreboard == null);
			if (dlgScoreboard != null)
				dlgScoreboard.ResetPlayers();
			Trap.trap(dlgDebugWindow != null);
			if (dlgDebugWindow != null)
				dlgDebugWindow.Setup(GameFramework.GameEngine.Players, GameFramework.GameEngine.companies,
				                     GameFramework.GameEngine.Passengers);
		}

		/// <summary>
		/// Set the turn or phase number increases.
		/// </summary>
		/// <param name="turn">The turn number.</param>
		public void TurnNumber(int turn)
		{
			Text = string.Format("Windwardopolis - Game: {0}", turn);
		}

		/// <summary>
		/// The game map.
		/// </summary>
		public GameMap Map
		{
			get { return GameFramework == null ? null : (GameFramework.GameEngine == null ? null : GameFramework.GameEngine.MainMap); }
		}

		/// <summary>
		/// The Limos to display on the map.
		/// </summary>
		public List<Player> Players
		{
			get { return GameFramework.GameEngine.Players; }
		}

		private int pixelsPerTile;

		/// <summary>
		/// The pixels per tile (due to zoom). Will be 48, 24, 12, or 6
		/// </summary>
		public int PixelsPerTile
		{
			get { return pixelsPerTile; }
			private set
			{
				pixelsPerTile = value;
				mapDisplay.ZoomChanged();
			}
		}

		public void NewMap()
		{
			mapDisplay.ZoomChanged();
		}

		private void toolStripDebugRun_Click(object sender, EventArgs e)
		{
			GameFramework.DebugStartMode = !toolStripDebugRun.Checked;
			DebugCheckEnable();
		}

		private void DebugCheckEnable()
		{
			toolStripDebugRun.Checked = GameFramework.DebugStartMode;
			toolStripDebugRun.Image = toolStripDebugRun.Checked ? Status.debug_run : Status.bug_yellow;
		}

		private void toolStripFullSpeed_Click(object sender, EventArgs e)
		{
			GameFramework.FullSpeed = true;
			GameFramework.Play();
			UpdateMenu();
		}
	}
}
