using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Win32;
using WindwardopolisLibrary;
using WindwardopolisLibrary.map;
using WindwardopolisLibrary.units;

namespace MapBuilder
{
	public partial class FormMap : Form, IMapInfo
	{
		private enum MOUSE_MODE
		{
			TILE,
			STOP_HORIZ,
			STOP_VERT,
			SIGNAL,
			START_NORTH,
			START_EAST,
			START_SOUTH,
			START_WEST,
			COMPANY,
			CLEAR
		}

		private string caption;
		private string captionNoFilename;
		private bool mapIsDirty;
		private string filename;
		private string initialDir;
		private MapTile.TYPE mapType = MapTile.TYPE.ROAD;
		private bool mouseDown;

		private MOUSE_MODE mouseMode = MOUSE_MODE.TILE;

		private readonly List<Company> companies;
		private readonly List<Passenger> passengers;


		/// <summary>
		/// The game map.
		/// </summary>
		public GameMap Map { get; private set; }

		/// <summary>
		/// The Limos to display on the map.
		/// </summary>
		public List<Player> Players
		{
			get { return new List<Player>(); }
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

		public FormMap()
		{
			InitializeComponent();
			InitModes();
			PixelsPerTile = 24;
			toolStripMenuItemZoom100.Checked = true;
			mapDisplay.DisplayCoordinates = true;

			initialDir = Path.GetDirectoryName(new Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath) ?? "";
			if (initialDir.ToLower().EndsWith("bin\\debug"))
				initialDir = initialDir.Substring(0, initialDir.Length - 10);
			else
				if (initialDir.ToLower().EndsWith("bin\\release"))
					initialDir = initialDir.Substring(0, initialDir.Length - 11);
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Windward Studios\Windwardopolis\map-editor"))
				if (key != null)
					initialDir = key.GetValue("map-folder", initialDir) as string;

			Company.GenerateCompaniesAndPassengers(out companies, out passengers);
			DetermineEnables();
		}

		private void InitModes()
		{
			mapType = MapTile.TYPE.ROAD;
			Cursor = Cursors.Arrow;
			mouseMode = MOUSE_MODE.TILE;
			DetermineEnables();
		}

		private void FormMap_Load(object sender, EventArgs e)
		{
			caption = Text;
			captionNoFilename = caption.Substring(0, caption.LastIndexOf('-')).Trim();
			Text = captionNoFilename;
		}

		#region file menu

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!CheckValidMap())
				return;

			if (CheckSaveDirty()) 
				return;

			using (NewMap dlg = new NewMap())
			{
				if (dlg.ShowDialog(this) != DialogResult.OK)
					return;
				Map = null; // empty prev memory
				Map = new GameMap(dlg.MapWidth, dlg.MapHeight);
				mapDisplay.NewMap();
			}
			filename = null;

			InitModes();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!CheckValidMap())
				return;

			if (CheckSaveDirty())
				return;

			using (OpenFileDialog dlg = new OpenFileDialog())
			{
				dlg.CheckFileExists = true;
				dlg.DefaultExt = "xml";
				if (! string.IsNullOrEmpty(initialDir))
					dlg.InitialDirectory = initialDir;
				dlg.Filter = "Map file (*.xml)|*.xml";
				if (dlg.ShowDialog(this) != DialogResult.OK)
					return;
				filename = dlg.FileName;
				initialDir = Path.GetDirectoryName(filename);
			}
			XDocument xml = XDocument.Load(filename);
			Text = string.Format(caption, Path.GetFileName(filename));
			Map = new GameMap(xml, companies);
			mapDisplay.NewMap();

			InitModes();
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{

			if (CheckCloseDirty())
				return;

			Map = null;
			mapDisplay.NewMap();
			Text = captionNoFilename;
			filename = null;

			InitModes();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{

			if (Map == null)
				return;
			Map.CalculateAllSquares();

			bool readOnly = false;
			if ((!string.IsNullOrEmpty(filename)) && File.Exists(filename))
			{
				readOnly = new FileInfo(filename).IsReadOnly;
				if (readOnly)
					MessageBox.Show(this, string.Format("The file {0} is read-only. Please save with another name", filename),
					                "MapBuilder", MessageBoxButtons.OK);
			}

			if (readOnly || string.IsNullOrEmpty(filename) || (!File.Exists(filename)))
			{
				saveAsToolStripMenuItem_Click(sender, e);
				return;
			}

			XDocument xml = new XDocument();
			xml.Add(Map.XML);
			xml.Save(filename);
			mapIsDirty = false;
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Map == null)
				return;
			Map.CalculateAllSquares();

			using (SaveFileDialog dlg = new SaveFileDialog())
			{
				dlg.DefaultExt = "xml";
				if (! string.IsNullOrEmpty(initialDir))
					dlg.InitialDirectory = initialDir;
				dlg.Filter = "Map file (*.xml)|*.xml";
				if (dlg.ShowDialog(this) != DialogResult.OK)
					return;
				if (File.Exists(dlg.FileName) && (new FileInfo(dlg.FileName).IsReadOnly))
					MessageBox.Show(this, string.Format("The file {0} is read-only. Please save with another name", dlg.FileName),
									"MapBuilder", MessageBoxButtons.OK);

				filename = dlg.FileName;
				initialDir = Path.GetDirectoryName(filename);
				Text = string.Format(caption, Path.GetFileName(filename));
			}
			XDocument xml = new XDocument();
			xml.Add(Map.XML);
			xml.Save(filename);
			mapIsDirty = false;
		}

		private void DetermineEnables()
		{
			closeToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = Map != null;
			objectToolStripMenuItem.Enabled = viewToolStripMenuItem.Enabled = mapToolStripMenuItem.Enabled = Map != null;
		}

		private bool CheckValidMap()
		{
			if (Map == null || Map.Width == 0)
				return true;

			string errMsg = Map.ValidateMap(companies);
			if (errMsg == null)
				return true;

			string msg = "The map has the following errors. Do you want to save it?\n" + errMsg;
			return MessageBox.Show(this, msg, "Map Builder", MessageBoxButtons.YesNo) != DialogResult.No;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (CheckCloseDirty()) 
				return;
			Close();
		}

		private void FormMap_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (CheckCloseDirty())
				e.Cancel = true;
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Windward Studios\Windwardopolis\map-editor"))
				if (key != null)
					key.SetValue("map-folder", initialDir);
		}

		private bool CheckSaveDirty()
		{
			return CheckDirty("Do you want to overwrite this map?");
		}

		private bool CheckCloseDirty()
		{
			return CheckDirty("Do you want to discard this map?");
		}

		private bool CheckDirty(string msg)
		{
			if (mapIsDirty)
				if (MessageBox.Show(this, msg, "Map Builder", MessageBoxButtons.YesNo) == DialogResult.No)
					return true;
			return false;
		}

		#endregion

		#region object menu

		private void roadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mapType = MapTile.TYPE.ROAD;
			Cursor = Cursors.Arrow;
			mouseMode = MOUSE_MODE.TILE;
		}

		private void busStopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mapType = MapTile.TYPE.BUS_STOP;
			Cursor = Cursors.Arrow;
			mouseMode = MOUSE_MODE.TILE;
		}

		private void companyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mapType = MapTile.TYPE.COMPANY;
			Cursor = Cursors.Arrow;
			mouseMode = MOUSE_MODE.TILE;
		}

		private void parkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mapType = MapTile.TYPE.PARK;
			Cursor = Cursors.Arrow;
			mouseMode = MOUSE_MODE.TILE;
		}

		private void horizontalStopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.SizeWE;
			mouseMode = MOUSE_MODE.STOP_HORIZ;
		}

		private void verticalStopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.SizeNS;
			mouseMode = MOUSE_MODE.STOP_VERT;
		}

		private void trafficLightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.SizeAll;
			mouseMode = MOUSE_MODE.SIGNAL;
		}

		private void northStartToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.PanNorth;
			mouseMode = MOUSE_MODE.START_NORTH;
		}

		private void eastStartToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.PanEast;
			mouseMode = MOUSE_MODE.START_EAST;
		}

		private void southStartToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.PanSouth;
			mouseMode = MOUSE_MODE.START_SOUTH;
		}

		private void westStartToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.PanWest;
			mouseMode = MOUSE_MODE.START_WEST;
		}

		private void companyNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.Cross;
			mouseMode = MOUSE_MODE.COMPANY;
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.No;
			mouseMode = MOUSE_MODE.CLEAR;
		}

		private void toolStripMenuItemZoom200_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 48;
			toolStripMenuItemZoom200.Checked = true;
			toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom75.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
		}

		private void toolStripMenuItemZoom100_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 24;
			toolStripMenuItemZoom100.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom75.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
		}

		private void toolStripMenuItemZoom50_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 18;
			toolStripMenuItemZoom75.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
		}

		private void toolStripMenuItemZoom25_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 12;
			toolStripMenuItemZoom50.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom75.Checked = false;
			mapDisplay.Invalidate();
		}

		#endregion

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Map == null)
			{
				Invalidate(true);
				return;
			}
			Map.CalculateAllSquares();
			Invalidate(true);
		}

		#region mouse

		private void FormMap_MouseDown(object sender, MouseEventArgs mea)
		{
			if (mouseMode != MOUSE_MODE.TILE)
				return;

			mouseDown = true;
			MouseSetTileType(mea);
		}

		private void FormMap_MouseMove(object sender, MouseEventArgs mea)
		{
			if (mouseDown)
				MouseSetTileType(mea);
		}

		private void FormMap_MouseUp(object sender, MouseEventArgs mea)
		{
			if (mouseDown)
				MouseSetTileType(mea);
			mouseDown = false;
		}

		private void FormMap_MouseLeave(object sender, EventArgs e)
		{
			mouseDown = false;
		}

		private void MouseSetTileType(MouseEventArgs mea)
		{
			if ((mea.Location.X < 0) || (mea.Location.Y < 0) || (Map == null))
				return;

			int x = (mea.Location.X - mapDisplay.AutoScrollPosition.X) / PixelsPerTile;
			int y = (mea.Location.Y - mapDisplay.AutoScrollPosition.Y) / PixelsPerTile;
			if ((x >= Map.Width) || (y >= Map.Height))
				return;

			// make it this type
			Map.Squares[x][y].Type = mapType;

			// figure out direction - including it's neighbors as this can change them.
			if (y > 0)
				Map.CalculateSquare(x, y - 1);
			if (x > 0)
				Map.CalculateSquare(x - 1, y);
			Map.CalculateSquare(x, y);
			if (x < Map.Width - 1)
				Map.CalculateSquare(x + 1, y);
			if (y < Map.Height - 1)
				Map.CalculateSquare(x, y + 1);

			// redraw the 3x3 tiles
			mapDisplay.Invalidate(new Rectangle(Math.Max(0, mea.Location.X - PixelsPerTile * 2),
												Math.Max(0, mea.Location.Y - PixelsPerTile * 2),
												PixelsPerTile * 4, PixelsPerTile * 4));
			mapIsDirty = true;
		}

		private void mapDisplay_MouseClick(object sender, MouseEventArgs mea)
		{
			if ((mea.Location.X < 0) || (mea.Location.Y < 0) || (Map == null))
				return;

			int x = (mea.Location.X - mapDisplay.AutoScrollPosition.X) / PixelsPerTile;
			int y = (mea.Location.Y - mapDisplay.AutoScrollPosition.Y) / PixelsPerTile;
			if ((x >= Map.Width) || (y >= Map.Height))
				return;

			MapSquare square = Map.Squares[x][y];
			switch (mouseMode)
			{
				case MOUSE_MODE.TILE:
					// nada
					return;
				case MOUSE_MODE.STOP_HORIZ:
					square.StopSigns = MapSquare.STOP_SIGNS.STOP_EAST | MapSquare.STOP_SIGNS.STOP_WEST;
					square.SignalDirection = MapSquare.SIGNAL_DIRECTION.NONE;
					Map.CalculateSquare(x, y);
					break;
				case MOUSE_MODE.STOP_VERT:
					square.StopSigns = MapSquare.STOP_SIGNS.STOP_NORTH | MapSquare.STOP_SIGNS.STOP_SOUTH;
					square.SignalDirection = MapSquare.SIGNAL_DIRECTION.NONE;
					Map.CalculateSquare(x, y);
					break;
				case MOUSE_MODE.SIGNAL:
					square.SignalDirection = MapSquare.SIGNAL_DIRECTION.NORTH_SOUTH_GREEN;
					square.StopSigns = MapSquare.STOP_SIGNS.NONE;
					Map.CalculateSquare(x, y);
					break;
				case MOUSE_MODE.START_NORTH:
					square.StartPosition = MapSquare.COMPASS_DIRECTION.NORTH;
					break;
				case MOUSE_MODE.START_EAST:
					square.StartPosition = MapSquare.COMPASS_DIRECTION.EAST;
					break;
				case MOUSE_MODE.START_SOUTH:
					square.StartPosition = MapSquare.COMPASS_DIRECTION.SOUTH;
					break;
				case MOUSE_MODE.START_WEST:
					square.StartPosition = MapSquare.COMPASS_DIRECTION.WEST;
					break;
				case MOUSE_MODE.CLEAR :
					square.SignalDirection = MapSquare.SIGNAL_DIRECTION.NONE;
					square.StopSigns = MapSquare.STOP_SIGNS.NONE;
					square.StartPosition = MapSquare.COMPASS_DIRECTION.NONE;
					square.Company = null;
					Map.CalculateSquare(x, y);
					break;

				case MOUSE_MODE.COMPANY:
					if (square.Tile.Type != MapTile.TYPE.BUS_STOP)
						return;
					break;
			}

			// redraw the 3x3 tiles
			mapDisplay.Invalidate(new Rectangle(Math.Max(0, mea.Location.X - PixelsPerTile * 2),
												Math.Max(0, mea.Location.Y - PixelsPerTile * 2),
												PixelsPerTile * 4, PixelsPerTile * 4));
			mapIsDirty = true;
		}

		#endregion

		private void trimToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Map.Trim();
			mapIsDirty = true;
			Map.CalculateAllSquares();
			Invalidate(true);
		}

		private void addRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Map.AddRows(1);
			mapIsDirty = true;
			Map.CalculateAllSquares();
			Invalidate(true);
		}

		private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Map.AddColumns(1);
			mapIsDirty = true;
			Map.CalculateAllSquares();
			Invalidate(true);
		}

		private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Map.Rotate90();
			mapIsDirty = true;
			Map.CalculateAllSquares();
			mapDisplay.ZoomChanged();
			Invalidate(true);
		}
	}
}
