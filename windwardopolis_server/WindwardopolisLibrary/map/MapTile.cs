using System;
using System.Drawing;

namespace WindwardopolisLibrary.map
{
	/// <summary>
	/// This represents a specific tile type such as a road that curves NE. There is only one object
	/// of this class for a given tile type. And then MapSquare objects point to the appropiate object
	/// of this type.
	/// </summary>
	public class MapTile
	{
		#region enums

		/// <summary>
		/// The direction of the road. Do not change these numbers, they are used as an index into an array.
		/// </summary>
		public enum DIRECTION
		{
			/// <summary>
			/// Road running north/south.
			/// </summary>
			NORTH_SOUTH = 0,

			/// <summary>
			/// Road running east/west.
			/// </summary>
			EAST_WEST = 1,

			/// <summary>
			/// A 4-way intersection.
			/// </summary>
			INTERSECTION = 2,

			/// <summary>
			/// A north/south road ended on the north side.
			/// </summary>
			NORTH_UTURN = 3,

			/// <summary>
			/// An east/west road ended on the east side.
			/// </summary>
			EAST_UTURN = 4,

			/// <summary>
			/// A north/south road ended on the south side.
			/// </summary>
			SOUTH_UTURN = 5,

			/// <summary>
			/// An east/west road ended on the west side.
			/// </summary>
			WEST_UTURN = 6,

			/// <summary>
			/// A T junction where the | of the T is entering from the north.
			/// </summary>
			T_NORTH = 7,

			/// <summary>
			/// A T junction where the | of the T is entering from the east.
			/// </summary>
			T_EAST = 8,

			/// <summary>
			/// A T junction where the | of the T is entering from the south.
			/// </summary>
			T_SOUTH = 9,

			/// <summary>
			/// A T junction where the | of the T is entering from the west.
			/// </summary>
			T_WEST = 10,

			/// <summary>
			/// A curve entered northward and exited eastward (or vice-versa).
			/// </summary>
			CURVE_NE = 11,

			/// <summary>
			/// A curve entered northward and exited westward (or vice-versa).
			/// </summary>
			CURVE_NW = 12,

			/// <summary>
			/// A curve entered southward and exited eastward (or vice-versa).
			/// </summary>
			CURVE_SE = 13,

			/// <summary>
			/// A curve entered southward and exited westward (or vice-versa).
			/// </summary>
			CURVE_SW = 14,
		};

		/// <summary>
		/// What type of square it is.
		/// </summary>
		public enum TYPE
		{
			/// <summary>
			/// Park. Nothing on this, does nothing, cannot be driven on.
			/// </summary>
			PARK,

			/// <summary>
			/// A road. The road DIRECTION determines which way cars can travel on the road.
			/// </summary>
			ROAD,

			/// <summary>
			/// A company's bus stop. This is where passengers are loaded and unloaded.
			/// </summary>
			BUS_STOP,

			/// <summary>
			/// Company building. Nothing on this, does nothing, cannot be driven on.
			/// </summary>
			COMPANY,
		}

		#endregion

		/// <summary>
		/// The type of square.
		/// </summary>
		public TYPE Type { get; private set; }

		/// <summary>
		/// True if the square can be driven on (ROAD or BUS_STOP).
		/// </summary>
		public bool IsDriveable
		{
			get { return Type == TYPE.ROAD || Type == TYPE.BUS_STOP; }
		}

		/// <summary>
		/// The direction of the road. This is only used for ROAD and BUS_STOP tiles.
		/// </summary>
		public DIRECTION Direction { get; private set; }

		/// <summary>
		/// The bitmap for this tile.
		/// </summary>
		public Image SpriteBitmap { get; private set; }

		private static readonly Random rand = new Random();

		/// <summary>
		/// Create the object. Private because the factory returns singletons.
		/// </summary>
		/// <param name="type">The type of square.</param>
		/// <param name="direction">The direction of the road. This is only used for ROAD and BUS_STOP tiles.</param>
		private MapTile(TYPE type, DIRECTION direction)
		{
			Type = type;
			Direction = direction;
			switch (type)
			{
				case TYPE.PARK:
					SpriteBitmap = Sprites.park;
					break;
				case TYPE.ROAD:
					switch (direction)
					{
						case DIRECTION.NORTH_SOUTH:
							SpriteBitmap = Sprites.road_NS;
							break;
						case DIRECTION.EAST_WEST:
							SpriteBitmap = new Bitmap(Sprites.road_NS);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
							break;
						case DIRECTION.INTERSECTION:
							SpriteBitmap = Sprites.road_intersection;
							break;

						case DIRECTION.NORTH_UTURN:
							SpriteBitmap = Sprites.road_u_turn;
							break;
						case DIRECTION.EAST_UTURN:
							SpriteBitmap = new Bitmap(Sprites.road_u_turn);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
							break;
						case DIRECTION.SOUTH_UTURN:
							SpriteBitmap = new Bitmap(Sprites.road_u_turn);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
							break;
						case DIRECTION.WEST_UTURN:
							SpriteBitmap = new Bitmap(Sprites.road_u_turn);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
							break;

						case DIRECTION.T_NORTH:
							SpriteBitmap = Sprites.road_t;
							break;
						case DIRECTION.T_EAST:
							SpriteBitmap = new Bitmap(Sprites.road_t);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
							break;
						case DIRECTION.T_SOUTH:
							SpriteBitmap = new Bitmap(Sprites.road_t);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
							break;
						case DIRECTION.T_WEST:
							SpriteBitmap = new Bitmap(Sprites.road_t);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
							break;

						case DIRECTION.CURVE_NE:
							SpriteBitmap = Sprites.road_curve;
							break;
						case DIRECTION.CURVE_NW:
							SpriteBitmap = new Bitmap(Sprites.road_curve);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
							break;
						case DIRECTION.CURVE_SE:
							SpriteBitmap = new Bitmap(Sprites.road_curve);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
							break;
						case DIRECTION.CURVE_SW:
							SpriteBitmap = new Bitmap(Sprites.road_curve);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
							break;
					}
					break;
				case TYPE.COMPANY:
					SpriteBitmap = Sprites.Building_01;
					break;
				case TYPE.BUS_STOP:
					switch (direction)
					{
						case DIRECTION.NORTH_UTURN:
							SpriteBitmap = Sprites.road_bus_stop;
							break;
						case DIRECTION.EAST_UTURN:
							SpriteBitmap = new Bitmap(Sprites.road_bus_stop);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
							break;
						case DIRECTION.SOUTH_UTURN:
							SpriteBitmap = new Bitmap(Sprites.road_bus_stop);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
							break;
						case DIRECTION.WEST_UTURN:
							SpriteBitmap = new Bitmap(Sprites.road_bus_stop);
							SpriteBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
							break;
						default:
							SpriteBitmap = Sprites.road_bus_stop;
							break;
					}
					break;
				default:
					Trap.trap();
					SpriteBitmap = Sprites.park;
					break;
			}
		}

		/// <summary>
		/// Return a direction to a random tile adjoining this one that's legal. U-turn only
		/// if no alternatives.
		/// </summary>
		/// <param name="direction">The vehicle direction (used to avoid u-turns)</param>
		/// <returns>The direction to the adjoining tile.</returns>
		public MapSquare.COMPASS_DIRECTION GetRandomNext(MapSquare.COMPASS_DIRECTION direction)
		{

			// for T and 4-way
			MapSquare.COMPASS_DIRECTION[] choices;

			switch (Direction)
			{
				case DIRECTION.NORTH_SOUTH:
					return direction;
				case DIRECTION.EAST_WEST:
					return direction;
				case DIRECTION.INTERSECTION:
					choices = new[] { MapSquare.COMPASS_DIRECTION.NORTH, MapSquare.COMPASS_DIRECTION.EAST,
										MapSquare.COMPASS_DIRECTION.SOUTH, MapSquare.COMPASS_DIRECTION.WEST};
					break;

				case DIRECTION.NORTH_UTURN:
					return MapSquare.COMPASS_DIRECTION.SOUTH;
				case DIRECTION.EAST_UTURN:
					return MapSquare.COMPASS_DIRECTION.WEST;
				case DIRECTION.SOUTH_UTURN:
					return MapSquare.COMPASS_DIRECTION.NORTH;
				case DIRECTION.WEST_UTURN:
					return MapSquare.COMPASS_DIRECTION.EAST;

				case DIRECTION.T_NORTH:
					choices = new[] { MapSquare.COMPASS_DIRECTION.NORTH, MapSquare.COMPASS_DIRECTION.EAST, MapSquare.COMPASS_DIRECTION.WEST};
					break;
				case DIRECTION.T_EAST:
					choices = new[] { MapSquare.COMPASS_DIRECTION.NORTH, MapSquare.COMPASS_DIRECTION.EAST, MapSquare.COMPASS_DIRECTION.SOUTH};
					break;
				case DIRECTION.T_SOUTH:
					choices = new[] { MapSquare.COMPASS_DIRECTION.EAST, MapSquare.COMPASS_DIRECTION.SOUTH, MapSquare.COMPASS_DIRECTION.WEST};
					break;
				case DIRECTION.T_WEST:
					choices = new[] { MapSquare.COMPASS_DIRECTION.NORTH, MapSquare.COMPASS_DIRECTION.SOUTH, MapSquare.COMPASS_DIRECTION.WEST};
					break;

				case DIRECTION.CURVE_NE:
					return direction == MapSquare.COMPASS_DIRECTION.NORTH ? MapSquare.COMPASS_DIRECTION.EAST : MapSquare.COMPASS_DIRECTION.SOUTH;
				case DIRECTION.CURVE_NW:
					return direction == MapSquare.COMPASS_DIRECTION.NORTH ? MapSquare.COMPASS_DIRECTION.WEST : MapSquare.COMPASS_DIRECTION.SOUTH;
				case DIRECTION.CURVE_SE:
					return direction == MapSquare.COMPASS_DIRECTION.SOUTH ? MapSquare.COMPASS_DIRECTION.EAST : MapSquare.COMPASS_DIRECTION.NORTH;
				case DIRECTION.CURVE_SW:
					return direction == MapSquare.COMPASS_DIRECTION.SOUTH ? MapSquare.COMPASS_DIRECTION.WEST : MapSquare.COMPASS_DIRECTION.NORTH;
				default:
					throw new ApplicationException("illegal direction");
			}

			// have to choose from 3 or 4 - but not the direction came from
			int index = rand.Next(choices.Length - 1);
			MapSquare.COMPASS_DIRECTION ptUturn = MapSquare.UTurn(direction);
			foreach (MapSquare.COMPASS_DIRECTION choice in choices)
			{
				if (choice == ptUturn)
					continue;
				if (index == 0)
					return choice;
				index--;
			}
			throw new ApplicationException("bad random number");
		}

		/// <summary>
		/// Return a direction to the adjoining tile that is the only or straight direction. Returns NONE if there
		/// are multiple choices and both are a turn (ie entering the base of a T).
		/// </summary>
		/// <param name="direction">The vehicle direction (used to avoid u-turns)</param>
		/// <returns>The direction to the adjoining tile.</returns>
		public MapSquare.COMPASS_DIRECTION GetStraightNext(MapSquare.COMPASS_DIRECTION direction)
		{

			switch (Direction)
			{
				case DIRECTION.NORTH_SOUTH:
					return direction;
				case DIRECTION.EAST_WEST:
					return direction;
				case DIRECTION.INTERSECTION:
					return direction;

				case DIRECTION.NORTH_UTURN:
					return MapSquare.COMPASS_DIRECTION.SOUTH;
				case DIRECTION.EAST_UTURN:
					return MapSquare.COMPASS_DIRECTION.WEST;
				case DIRECTION.SOUTH_UTURN:
					return MapSquare.COMPASS_DIRECTION.NORTH;
				case DIRECTION.WEST_UTURN:
					return MapSquare.COMPASS_DIRECTION.EAST;

				case DIRECTION.T_NORTH:
					return direction == MapSquare.COMPASS_DIRECTION.SOUTH ? MapSquare.COMPASS_DIRECTION.NONE : direction;
				case DIRECTION.T_EAST:
					return direction == MapSquare.COMPASS_DIRECTION.WEST ? MapSquare.COMPASS_DIRECTION.NONE : direction;
				case DIRECTION.T_SOUTH:
					return direction == MapSquare.COMPASS_DIRECTION.NORTH ? MapSquare.COMPASS_DIRECTION.NONE : direction;
				case DIRECTION.T_WEST:
					return direction == MapSquare.COMPASS_DIRECTION.EAST ? MapSquare.COMPASS_DIRECTION.NONE : direction;

				case DIRECTION.CURVE_NE:
					return direction == MapSquare.COMPASS_DIRECTION.NORTH
					       	? MapSquare.COMPASS_DIRECTION.EAST
					       	: MapSquare.COMPASS_DIRECTION.SOUTH;
				case DIRECTION.CURVE_NW:
					return direction == MapSquare.COMPASS_DIRECTION.NORTH
					       	? MapSquare.COMPASS_DIRECTION.WEST
					       	: MapSquare.COMPASS_DIRECTION.SOUTH;
				case DIRECTION.CURVE_SE:
					return direction == MapSquare.COMPASS_DIRECTION.SOUTH
					       	? MapSquare.COMPASS_DIRECTION.EAST
					       	: MapSquare.COMPASS_DIRECTION.NORTH;
				case DIRECTION.CURVE_SW:
					return direction == MapSquare.COMPASS_DIRECTION.SOUTH
					       	? MapSquare.COMPASS_DIRECTION.WEST
					       	: MapSquare.COMPASS_DIRECTION.NORTH;
				default:
					throw new ApplicationException("illegal direction");
			}
		}

		/// <summary>
		/// Return a singleton object for the passed in type and direction.
		/// </summary>
		/// <param name="type">The type of square.</param>
		/// <param name="direction">The direction of the road. This is only used for ROAD and BUS_STOP tiles.</param>
		/// <returns></returns>
		public static MapTile Factory(TYPE type, DIRECTION direction)
		{
			switch (type)
			{
				case TYPE.PARK:
					if (rand.Next(0, 4) != 0)
						return parkMapTiles[0];
					if (rand.Next(0, 4) != 0)
						return parkMapTiles[4];
					return parkMapTiles[rand.Next(0, parkMapTiles.Length)];
				case TYPE.COMPANY:
					return officeMapTiles[rand.Next(0, officeMapTiles.Length)];
				case TYPE.BUS_STOP:
					return busStopMapTiles[(int) direction];
				case TYPE.ROAD:
					return mapTiles[(int) direction];
				default:
					return parkMapTiles[0];
			}
		}

		private static readonly MapTile []parkMapTiles;
		private static readonly MapTile []officeMapTiles;
		private static readonly MapTile [] busStopMapTiles;
		private static readonly MapTile[] mapTiles;

		private static readonly Bitmap[] parks = { Sprites.park, Sprites.park1, Sprites.park2, Sprites.park3, Sprites.park4 };
		private static readonly Bitmap[] offices = { Sprites.Building_01, Sprites.Building_02, Sprites.Building_03, Sprites.Building_04, Sprites.Building_05, Sprites.Building_06, Sprites.Building_07 };

		static MapTile()
		{
			parkMapTiles = new MapTile[parks.Length];
			for (int ind = 0; ind < parks.Length; ind++)
				parkMapTiles[ind] = new MapTile(TYPE.PARK, DIRECTION.INTERSECTION) {SpriteBitmap = parks[ind]};

			officeMapTiles = new MapTile[offices.Length];
			for (int ind = 0; ind < offices.Length; ind++)
				officeMapTiles[ind] = new MapTile(TYPE.COMPANY, DIRECTION.INTERSECTION) { SpriteBitmap = offices[ind] };

			busStopMapTiles = new MapTile[15];
			for (int ind = 0; ind < 15; ind++ )
				busStopMapTiles[ind] = new MapTile(TYPE.BUS_STOP, (DIRECTION)ind);

			mapTiles = new MapTile[15];
			for (int ind = 0; ind < 15; ind++)
				mapTiles[ind] = new MapTile(TYPE.ROAD, (DIRECTION)ind);
		}

	}
}