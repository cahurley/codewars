// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.

using System;
using System.Drawing;
using System.Xml.Linq;
using WindwardopolisLibrary.units;

namespace WindwardopolisLibrary.map
{
	/// <summary>
	/// A single map square.
	/// </summary>
	public class MapSquare : ISprite
	{
		/// <summary>
		/// Stop signs and signals for an intersection square.
		/// </summary>
		[Flags]
		public enum STOP_SIGNS
		{
			/// <summary>
			/// No stop signs or signals.
			/// </summary>
			NONE = 0,
			/// <summary>
			/// A stop entering from the North side.
			/// </summary>
			STOP_NORTH = 0x01,
			/// <summary>
			/// A stop entering from the East side.
			/// </summary>
			STOP_EAST = 0x02,
			/// <summary>
			/// A stop entering from the South side.
			/// </summary>
			STOP_SOUTH = 0x04,
			/// <summary>
			/// A stop entering from the West side.
			/// </summary>
			STOP_WEST = 0x08
		}

		/// <summary>
		/// Specifies movement directions from tile to tile.
		/// </summary>
		public enum COMPASS_DIRECTION
		{
			/// <summary>
			/// Used for multiple or unknown direction.
			/// </summary>
			NONE = 0,
			/// <summary>
			/// North
			/// </summary>
			NORTH = 1,
			/// <summary>
			/// East
			/// </summary>
			EAST = 2,
			/// <summary>
			/// South
			/// </summary>
			SOUTH = 3,
			/// <summary>
			/// West
			/// </summary>
			WEST = 4
		}

		public static COMPASS_DIRECTION PointsDirection (Point start, Point end)
		{
			if (start.Y == end.Y)
			{
				if (start.X == end.X)
					throw new ApplicationException("no movement");
				return start.X < end.X ? COMPASS_DIRECTION.EAST : COMPASS_DIRECTION.WEST;
			}
			if (start.X != end.X)
				throw new ApplicationException("diaganol movement");
			return start.Y < end.Y ? COMPASS_DIRECTION.SOUTH : COMPASS_DIRECTION.NORTH;
		}

		/// <summary>
		/// Rounds an angle to a direction. At exactly 45/135/225/315 no gaurantees.
		/// </summary>
		/// <param name="angle">The angle in degrees 0 .. 359.</param>
		/// <returns>The rounded direction.</returns>
		public static COMPASS_DIRECTION AngleToDirection(int angle)
		{
			if ((45 <= angle) && (angle <= 135))
				return COMPASS_DIRECTION.EAST;
			if ((135 <= angle) && (angle <= 225))
				return COMPASS_DIRECTION.SOUTH;
			if ((225 <= angle) && (angle <= 315))
				return COMPASS_DIRECTION.WEST;
			return COMPASS_DIRECTION.NORTH;
		}

		public static COMPASS_DIRECTION UTurn(COMPASS_DIRECTION dir)
		{
			switch (dir)
			{
				case COMPASS_DIRECTION.NORTH:
					return COMPASS_DIRECTION.SOUTH;
				case COMPASS_DIRECTION.EAST:
					return COMPASS_DIRECTION.WEST;
				case COMPASS_DIRECTION.SOUTH:
					return COMPASS_DIRECTION.NORTH;
				case COMPASS_DIRECTION.WEST:
					return COMPASS_DIRECTION.EAST;
			}
			throw new ApplicationException("illegal compass direction");
		}

		/// <summary>
		/// The status of a traffic signal. One direction is green or yellow and the other by definition is red.
		/// </summary>
		public enum SIGNAL_DIRECTION
		{
			NONE,
			NORTH_SOUTH_GREEN,
			NORTH_SOUTH_YELLOW,
			EAST_WEST_GREEN,
			EAST_WEST_YELLOW,
		}

		/// <summary>
		/// The map tile for this square.
		/// </summary>
		public MapTile Tile { get; private set; }

		/// <summary>
		/// Settings for stop signs in this square. NONE for none.
		/// </summary>
		public STOP_SIGNS StopSigns { get; set; }

		/// <summary>
		/// The type of square.
		/// </summary>
		public SIGNAL_DIRECTION SignalDirection { get; set; }

		/// <summary>
		/// Number of seconds the light has been green in the given direction.
		/// </summary>
		public int TimeSignalGreen { get; set; }

		/// <summary>
		/// -1 if not a start position. Otherwise the angle for the starting vehicle.
		/// </summary>
		public COMPASS_DIRECTION StartPosition { get; set; }

		/// <summary>
		/// The company for this tile. Only set if a BUS_STOP.
		/// </summary>
		public Company Company { get; set; }

		/// <summary>
		/// The bitmap for this sprite. This will change on ticks for animated sprites.
		/// </summary>
		public Image SpriteBitmap
		{
			get { return Tile.SpriteBitmap; }
		}

		/// <summary>
		/// Change this square's direction.
		/// </summary>
		public MapTile.DIRECTION Direction
		{
			set { Tile = MapTile.Factory(Tile.Type, value); }
		}

		/// <summary>
		/// Change this square's type.
		/// </summary>
		public MapTile.TYPE Type
		{
			set { Tile = MapTile.Factory(value, Tile.Direction); }
		}

		/// <summary>
		/// Create a map square set to PARK.
		/// </summary>
		public MapSquare()
		{
			Tile = MapTile.Factory(MapTile.TYPE.PARK, MapTile.DIRECTION.INTERSECTION);
		}

		public MapSquare(XElement element)
		{
			MapTile.TYPE type = (MapTile.TYPE)Enum.Parse(typeof(MapTile.TYPE), element.Attribute("type").Value);
			MapTile.DIRECTION direction = (MapTile.DIRECTION)Enum.Parse(typeof(MapTile.DIRECTION), element.Attribute("direction").Value);
			Tile = MapTile.Factory(type, direction);

			XAttribute attr = element.Attribute("signals");
			if (attr != null)
				StopSigns = (STOP_SIGNS) Enum.Parse(typeof (STOP_SIGNS), attr.Value);
			attr = element.Attribute("signal-direction");
			if (attr != null)
				SignalDirection = (SIGNAL_DIRECTION)Enum.Parse(typeof(SIGNAL_DIRECTION), attr.Value);
			attr = element.Attribute("start-position");
			if (attr != null)
				StartPosition = (COMPASS_DIRECTION)Enum.Parse(typeof(COMPASS_DIRECTION), element.Attribute("start-position").Value);
		}

		/// <summary>
		/// Return true if entering the square in this direction requires a stop.
		/// </summary>
		/// <param name="direction">The direction the vehicle is travelling</param>
		/// <returns></returns>
		public bool IsStop(COMPASS_DIRECTION direction)
		{
			if (SignalDirection != SIGNAL_DIRECTION.NONE)
			{
				// must be green to go through. Yellow means it wants to flip and someone is ALREADY in it.
				if ((direction == COMPASS_DIRECTION.NORTH) || (direction == COMPASS_DIRECTION.SOUTH))
					return SignalDirection != SIGNAL_DIRECTION.NORTH_SOUTH_GREEN;
				return SignalDirection != SIGNAL_DIRECTION.EAST_WEST_GREEN;
			}

			if (StopSigns == STOP_SIGNS.NONE)
				return false;

			// check each direction for a stop. The angle tells us what side they are trying to enter from
			if (direction == COMPASS_DIRECTION.EAST)
				return (StopSigns & STOP_SIGNS.STOP_WEST) != 0;
			if (direction == COMPASS_DIRECTION.SOUTH)
				return (StopSigns & STOP_SIGNS.STOP_NORTH) != 0;
			if (direction == COMPASS_DIRECTION.WEST)
				return (StopSigns & STOP_SIGNS.STOP_EAST) != 0;
			return (StopSigns & STOP_SIGNS.STOP_SOUTH) != 0;
		}

		/// <summary>
		/// Return true if entering the square in this direction requires a stop. Assumes all lights are green.
		/// </summary>
		/// <param name="direction">The direction the vehicle is travelling</param>
		/// <returns></returns>
		public bool IsStopAllGreen(COMPASS_DIRECTION direction)
		{
			if (StopSigns == STOP_SIGNS.NONE)
				return false;

			// check each direction for a stop. The angle tells us what side they are trying to enter from
			if (direction == COMPASS_DIRECTION.EAST)
				return (StopSigns & STOP_SIGNS.STOP_WEST) != 0;
			if (direction == COMPASS_DIRECTION.SOUTH)
				return (StopSigns & STOP_SIGNS.STOP_NORTH) != 0;
			if (direction == COMPASS_DIRECTION.WEST)
				return (StopSigns & STOP_SIGNS.STOP_EAST) != 0;
			return (StopSigns & STOP_SIGNS.STOP_SOUTH) != 0;
		}

		public XElement GetXML(int x, int y)
		{
			XElement xmlSquare = new XElement("square", new XAttribute("x", x), new XAttribute("y", y),
											new XAttribute("type", Tile.Type), new XAttribute("direction", Tile.Direction));
			if (StopSigns != STOP_SIGNS.NONE)
				xmlSquare.Add(new XAttribute("signals", StopSigns));
			if (SignalDirection != SIGNAL_DIRECTION.NONE)
				xmlSquare.Add(new XAttribute("signal-direction", SignalDirection));
			if (StartPosition != COMPASS_DIRECTION.NONE)
				xmlSquare.Add(new XAttribute("start-position", StartPosition));
			return xmlSquare;
		}

		/// <summary>
		/// Create a map square of the requested type. no road
		/// </summary>
		/// <param name="type">The square type.</param>
		public MapSquare(MapTile.TYPE type)
		{
			Tile = MapTile.Factory(type, MapTile.DIRECTION.INTERSECTION);
		}

		/// <summary>
		/// Called once each tick (used for animation).
		/// </summary>
		/// <returns>true to kill this sprite, false to keep it alive.</returns>
		public bool IncreaseTick()
		{
			return false;
		}
	}
}
