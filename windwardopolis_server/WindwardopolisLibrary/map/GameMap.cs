using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Linq;
using WindwardopolisLibrary.units;

namespace WindwardopolisLibrary.map
{
	/// <summary>
	/// The game map.
	/// </summary>
	public class GameMap
	{
		/// <summary>
		/// The map squares. This is in the format [x][y].
		/// </summary>
		public MapSquare[][] Squares { get; protected set; }

		/// <summary>
		/// Create an empty map
		/// </summary>
		public GameMap(int width, int height)
		{
			Squares = new MapSquare[width][];
			for (int x = 0; x < width; x++)
			{
				Squares[x] = new MapSquare[height];
				for (int y = 0; y < height; y++)
					Squares[x][y] = new MapSquare();
			}
		}

		public GameMap(XDocument xml, List<Company> companies)
		{
			int width = int.Parse(xml.Root.Attribute("width").Value);
			int height = int.Parse(xml.Root.Attribute("height").Value);

			Squares = new MapSquare[width][];
			for (int x = 0; x < width; x++)
				Squares[x] = new MapSquare[height];

			foreach (XElement elementOn in xml.Root.Elements("square"))
			{
				int x = int.Parse(elementOn.Attribute("x").Value);
				int y = int.Parse(elementOn.Attribute("y").Value);
				MapSquare square = new MapSquare(elementOn);
				Squares[x][y] = square;
			}
		}

		public static GameMap CreateMap()
		{
			return new GameMap(60, 60);
		}

		public static GameMap OpenMap(string filename, List<Company> companies)
		{
			XDocument xml = XDocument.Load(filename);
			return new GameMap(xml, companies);
		}

		/// <summary>
		/// The width of the map. Units are squares.
		/// </summary>
		public int Width
		{
			get { return Squares.Length; }
		}

		/// <summary>
		/// The height of the map. Units are squares.
		/// </summary>
		public int Height
		{
			get { return Squares[0].Length; }
		}

		/// <summary>
		/// Returns the requested point or null if off the map.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public MapSquare SquareOrDefault(Point pt)
		{
			if ((pt.X < 0) || (pt.Y < 0) || (pt.X >= Width) || (pt.Y >= Height))
				return null;
			return Squares[pt.X][pt.Y];
		}

		public string ValidateMap(List<Company> companies)
		{

			int numStarts = 0;
			int numBusStops = 0;

			StringBuilder buf = new StringBuilder();
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
				{
					MapSquare square = Squares[x][y];
					if (square.Tile.Type != MapTile.TYPE.ROAD)
					{
						if (square.StartPosition != MapSquare.COMPASS_DIRECTION.NONE)
							buf.Append(string.Format("non-road {0} has a start position {1}\n", new Point(x, y), square.StartPosition));
						if (square.SignalDirection != MapSquare.SIGNAL_DIRECTION.NONE)
							buf.Append(string.Format("non-road {0} has a signal {1}\n", new Point(x, y), square.SignalDirection));
						if (square.StopSigns != MapSquare.STOP_SIGNS.NONE)
							buf.Append(string.Format("non-road {0} has a stop sign {1}\n", new Point(x, y), square.StopSigns));
					}

					if (square.Tile.Type == MapTile.TYPE.BUS_STOP)
						numBusStops++;

					if (square.Tile.Type != MapTile.TYPE.ROAD)
						continue;

					if (square.StartPosition != MapSquare.COMPASS_DIRECTION.NONE)
						numStarts++;
					switch (square.StartPosition)
					{
						case MapSquare.COMPASS_DIRECTION.NORTH:
						case MapSquare.COMPASS_DIRECTION.SOUTH:
							if (square.Tile.Direction != MapTile.DIRECTION.NORTH_SOUTH)
								buf.Append(string.Format("tile {0} has a start position {1} (needs to be North/South)\n", new Point(x, y), square.StartPosition));
							break;
						case MapSquare.COMPASS_DIRECTION.EAST:
						case MapSquare.COMPASS_DIRECTION.WEST:
							if (square.Tile.Direction != MapTile.DIRECTION.EAST_WEST)
								buf.Append(string.Format("tile {0} has a start position {1} (needs to be East/West)\n", new Point(x, y), square.StartPosition));
							break;
					}

					if (square.SignalDirection != MapSquare.SIGNAL_DIRECTION.NONE)
						switch (square.Tile.Direction)
						{
							case MapTile.DIRECTION.T_NORTH:
							case MapTile.DIRECTION.T_EAST:
							case MapTile.DIRECTION.T_SOUTH:
							case MapTile.DIRECTION.T_WEST:
							case MapTile.DIRECTION.INTERSECTION:
								break;
							default:
								buf.Append(string.Format("tile {0} has a signal {1} (intersections only)\n", new Point(x, y), square.SignalDirection));
								break;
						}

					if (square.StopSigns != MapSquare.STOP_SIGNS.NONE)
						switch (square.Tile.Direction)
						{
							case MapTile.DIRECTION.T_NORTH:
							case MapTile.DIRECTION.T_EAST:
							case MapTile.DIRECTION.T_SOUTH:
							case MapTile.DIRECTION.T_WEST:
							case MapTile.DIRECTION.INTERSECTION:
								break;
							default:
								buf.Append(string.Format("tile {0} has a stop sign {1} (intersections only)\n", new Point(x, y), square.StopSigns));
								break;
						}
				}

			if (numStarts < 12)
				buf.Append(string.Format("Only has {0} start positions\n", numStarts));
			if (numBusStops < Company.NUM_COMPANIES)
				buf.Append(string.Format("Only has {0} company positions\n", numBusStops));
			string str = buf.ToString().Trim();
			return str.Length == 0 ? null : str;
		}

		/// <summary>
		/// Figure out the road direction for all map squares set to a road.
		/// </summary>
		public void CalculateAllSquares()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					CalculateSquare(x, y);
		}

		public void CalculateSquare(int x, int y)
		{
			MapSquare square = Squares[x][y];
			if (!square.Tile.IsDriveable)
			{
				square.StartPosition = MapSquare.COMPASS_DIRECTION.NONE;
				square.SignalDirection = MapSquare.SIGNAL_DIRECTION.NONE;
				square.StopSigns = MapSquare.STOP_SIGNS.NONE;
				return;
			}
			int mode = 0;
			mode |= y > 0 && Squares[x][y - 1].Tile.IsDriveable ? 0x01 : 0; // above
			mode |= y + 1 < Height && Squares[x][y + 1].Tile.IsDriveable ? 0x02 : 0; // below
			mode |= x > 0 && Squares[x - 1][y].Tile.IsDriveable ? 0x04 : 0; // left
			mode |= x + 1 < Width && Squares[x + 1][y].Tile.IsDriveable ? 0x08 : 0; // right

			// which road direction
			switch (mode)
			{
				case 0: // orphan
					square.Direction = MapTile.DIRECTION.INTERSECTION;
					break;
				case 1: // end of road above
					square.Direction = MapTile.DIRECTION.SOUTH_UTURN;
					break;
				case 2: // end of road below
					square.Direction = MapTile.DIRECTION.NORTH_UTURN;
					break;
				case 3: // vertical road
					square.Direction = MapTile.DIRECTION.NORTH_SOUTH;
					break;
				case 4: // end of road to left
					square.Direction = MapTile.DIRECTION.EAST_UTURN;
					break;
				case 5: // corner
					square.Direction = MapTile.DIRECTION.CURVE_SW;
					break;
				case 6: // corner
					square.Direction = MapTile.DIRECTION.CURVE_NW;
					break;
				case 7: // T to the left
					square.Direction = MapTile.DIRECTION.T_WEST;
					break;
				case 8: // end of road to right
					square.Direction = MapTile.DIRECTION.WEST_UTURN;
					break;
				case 9: // corner
					square.Direction = MapTile.DIRECTION.CURVE_SE;
					break;
				case 0x0A: // corner
					square.Direction = MapTile.DIRECTION.CURVE_NE;
					break;
				case 0x0B: // T to the right
					square.Direction = MapTile.DIRECTION.T_EAST;
					break;
				case 0x0C: // horizontal road
					square.Direction = MapTile.DIRECTION.EAST_WEST;
					break;
				case 0x0D: // T above
					square.Direction = MapTile.DIRECTION.T_NORTH;
					break;
				case 0x0E: // T below
					square.Direction = MapTile.DIRECTION.T_SOUTH;
					break;
				case 0x0F: // intersection
					square.Direction = MapTile.DIRECTION.INTERSECTION;
					break;
			}

			// no signal or stop signs unless 3+ road neighbors
			switch (mode)
			{
				case 0:

				case 0x01:
				case 0x02:
				case 0x04:
				case 0x08:

				case 0x03:
				case 0x05:
				case 0x09:
				case 0x06:
				case 0x0A:
				case 0x0C:

					square.StopSigns = MapSquare.STOP_SIGNS.NONE;
					square.SignalDirection = MapSquare.SIGNAL_DIRECTION.NONE;
					break;
			}

			// adjust stop signs if no road on that side
			if (((square.StopSigns & MapSquare.STOP_SIGNS.STOP_NORTH) != 0) && ((mode & 0x01) == 0))
				square.StopSigns &= ~ MapSquare.STOP_SIGNS.STOP_NORTH;
			if (((square.StopSigns & MapSquare.STOP_SIGNS.STOP_EAST) != 0) && ((mode & 0x08) == 0))
				square.StopSigns &= ~MapSquare.STOP_SIGNS.STOP_EAST;
			if (((square.StopSigns & MapSquare.STOP_SIGNS.STOP_SOUTH) != 0) && ((mode & 0x02) == 0))
				square.StopSigns &= ~MapSquare.STOP_SIGNS.STOP_SOUTH;
			if (((square.StopSigns & MapSquare.STOP_SIGNS.STOP_WEST) != 0) && ((mode & 0x04) == 0))
				square.StopSigns &= ~MapSquare.STOP_SIGNS.STOP_WEST;
		}

		/// <summary>
		/// Create the XML of the map that we send to players.
		/// </summary>
		public XElement XML
		{
			get
			{
				XElement xmlMap = new XElement("map", new XAttribute("width", Width), new XAttribute("height", Height));

				for (int x = 0; x < Width; x++)
					for (int y = 0; y < Height; y++)
						xmlMap.Add(Squares[x][y].GetXML(x, y));

				return xmlMap;
			}
		}

		/// <summary>
		/// Trim the map so only have 2 tiles of park outside of road limits.
		/// </summary>
		public void Trim()
		{

			// reduce columns first (map is column major)
			int startX = 0;
			for (int xOn=0; xOn<Width; xOn++)
			{
				bool nonPark = false;
				for (int yOn=0; yOn<Height; yOn++)
					if (Squares[xOn][yOn].Tile.Type != MapTile.TYPE.PARK)
					{
						nonPark = true;
						break;
					}
				if (nonPark)
					break;
				startX = xOn;
			}
			int endX = Width - 1;
			for (int xOn=Width-1; xOn>startX; xOn--)
			{
				bool nonPark = false;
				for (int yOn = 0; yOn < Height; yOn++)
					if (Squares[xOn][yOn].Tile.Type != MapTile.TYPE.PARK)
					{
						nonPark = true;
						break;
					}
				if (nonPark)
					break;
				endX = xOn;
			}
			startX = Math.Max(0, startX - 1);
			endX = Math.Min(Width - 1, endX + 1);
			if (startX > 0 || endX < Width-1)
			{
				MapSquare[][] squares = new MapSquare[endX - startX + 1][];
				for (int index = 0; index < squares.Length; index++)
					squares[index] = Squares[startX + index];
				Squares = squares;
			}

			// now the rows.
			int startY = 0;
			for (int yOn = 0; yOn < Height; yOn++)
			{
				bool nonPark = false;
				for (int xOn = 0; xOn < Width; xOn++)
					if (Squares[xOn][yOn].Tile.Type != MapTile.TYPE.PARK)
					{
						nonPark = true;
						break;
					}
				if (nonPark)
					break;
				startY = yOn;
			}
			int endY = Height - 1;
			for (int yOn = Height - 1; yOn > startY; yOn--)
			{
				bool nonPark = false;
				for (int xOn = 0; xOn < Width; xOn++)
					if (Squares[xOn][yOn].Tile.Type != MapTile.TYPE.PARK)
					{
						nonPark = true;
						break;
					}
				if (nonPark)
					break;
				endY = yOn;
			}
			startY = Math.Max(0, startY - 1);
			endY = Math.Min(Height - 1, endY + 1);
			if (startY > 0 || endY < Height - 1)
			{
				for (int xOn = 0; xOn < Width; xOn++)
				{
					MapSquare[] column = new MapSquare[endY - startY + 1];
					for (int yOn = 0; yOn < column.Length; yOn++)
						column[yOn] = Squares[xOn][startY + yOn];
					Squares[xOn] = column;
				}
			}
		}

		/// <summary>
		/// Add rows to the bottom of the map.
		/// </summary>
		/// <param name="num">Number of rows to add.</param>
		public void AddRows(int num)
		{
			int height = Height;
			for (int x = 0; x < Width; x++)
			{
				MapSquare[] column = new MapSquare[height + num];
				for (int y = 0; y < height; y++)
					column[y] = Squares[x][y];
				for (int y = height; y < height + num; y++)
					column[y] = new MapSquare();
				Squares[x] = column;
			}

		}

		/// <summary>
		/// Add columns to the right of the map.
		/// </summary>
		/// <param name="num">Number of columns to add.</param>
		public void AddColumns(int num)
		{
			MapSquare[][] squares = new MapSquare[Width + num][];
			for (int x = 0; x < Width; x++)
				squares[x] = Squares[x];
			for (int x = Width; x < Width + num; x++)
			{
				squares[x] = new MapSquare[Height];
				for (int y = 0; y < Height; y++)
					squares[x][y] = new MapSquare();
			}
			Squares = squares;
		}

		/// <summary>
		/// Rotate the map 90 degrees.
		/// </summary>
		public void Rotate90()
		{
			int width = Height;
			int height = Width;
			MapSquare[][] squares = new MapSquare[width][];
			for (int x = 0; x < width; x++)
			{
				squares[x] = new MapSquare[height];
				for (int y = 0; y < height; y++)
					squares[x][y] = Squares[y][width - x - 1];
			}
			Squares = squares;
		}
	}
}
