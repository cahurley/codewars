using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace WindwardopolisLibrary.map
{
	/// <summary>
	/// A location and Angle on the board. This location is in map units. There are UNITS_PER_TILE map units per map tile.
	/// </summary>
	public class BoardLocation
	{

		#region variables including auto-props

		public TileMovement TileMoves { get; private set; }

		public int OffsetTileMoves { get; private set; }

		private Point mapPosition;

		#endregion

		#region ctor's

		/// <summary>
		/// Create the object
		/// </summary>
		/// <param name="mapPosition">The board square located on.</param>
		/// <param name="movement">The movement for the tile placed on</param>
		public BoardLocation(Point mapPosition, TileMovement movement)
		{
			this.mapPosition = new Point(mapPosition.X, mapPosition.Y);
			TileMoves = movement;
			OffsetTileMoves = movement.Moves.Length / 2;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="src">Initialize with the values in this struct.</param>
		public BoardLocation(BoardLocation src)
		{
			mapPosition = new Point(src.MapPosition.X, src.MapPosition.Y);
			TileMoves = src.TileMoves;
			OffsetTileMoves = src.OffsetTileMoves;
		}

		#endregion

		#region properties

		/// <summary>
		/// The position of the center of the unit. The unit measure has UNITS_PER_TILE per tile.
		/// </summary>
		public Point MapPosition
		{
			get { return mapPosition; }
		}

		/// <summary>
		/// The direction of the car at present, rounded to the nearest 90 degrees. Generally only useful when entering or exiting
		/// a tile.
		/// </summary>
		public MapSquare.COMPASS_DIRECTION Direction
		{
			get { return MapSquare.AngleToDirection(Angle); }
		}

		/// <summary>
		/// The move presently on.
		/// </summary>
		public TileMovement.Move MoveOn
		{
			get
			{
				Trap.trap(OffsetTileMoves >= TileMoves.Moves.Length);
				return TileMoves.Moves[Math.Min(OffsetTileMoves, TileMoves.Moves.Length - 1)];
			}
		}

		/// <summary>
		/// 0 .. 359 The Angle this unit is facing.
		/// </summary>
		public int Angle
		{
			get { return MoveOn.Angle; }
		}

		/// <summary>
		/// true if a move remains inside the tile. The last inter-tile move moves into the next tile and will return false here.
		/// true means that OffsetTileMoves can increase 1 and still is in this tile, not just that the present value is. 
		/// </summary>
		/// <returns>true if move occurs within tile.</returns>
		public bool IsMoveInsideTile
		{
			get { return OffsetTileMoves < TileMoves.Moves.Length - 1; }
		}

		/// <summary>
		/// The position of the map tile the center of the unit is on. This is tile units, not map units.
		/// </summary>
		public Point TilePosition
		{
			get { return MapToTile(MapPosition); }
		}

		/// <summary>
		/// When the unit moves out of the tile it is presently in, it will move into this tile.
		/// </summary>
		public Point NextTilePosition
		{
			get
			{
				Point ptOnTile = TilePosition;
				Point ptNextMap = new Point(MapPosition.X, MapPosition.Y);
				for (int index = OffsetTileMoves; index < TileMoves.Moves.Length; index++)
				{
					ptNextMap.Offset(TileMoves.Moves[index].Position);
					Point ptNextTile = MapToTile(ptNextMap);
					if (ptNextTile != ptOnTile)
						return ptNextTile;
				}

				// didn't move out
				switch (MapSquare.AngleToDirection(TileMoves.Moves[TileMoves.Moves.Length - 1].Angle))
				{
					case MapSquare.COMPASS_DIRECTION.NORTH:
						ptNextMap.Y--;
						break;
					case MapSquare.COMPASS_DIRECTION.EAST:
						ptNextMap.X++;
						break;
					case MapSquare.COMPASS_DIRECTION.SOUTH:
						ptNextMap.Y++;
						break;
					case MapSquare.COMPASS_DIRECTION.WEST:
						ptNextMap.X--;
						break;
				}
				return MapToTile(ptNextMap);
			}
		}

		/// <summary>
		/// Calculate the new location of the center of the car moving it forward 1 step. Moves straight into next tile if at end of
		/// moves within a tile (which is correct).
		/// </summary>
		public Point NextPosition
		{
			get
			{
				int x = MapPosition.X;
				int y = MapPosition.Y;
				if (OffsetTileMoves < TileMoves.Moves.Length)
				{
					TileMovement.Move posNumMoves = MoveOn;
					x += posNumMoves.Position.X;
					y += posNumMoves.Position.Y;
				}
				else
				{
					TileMovement.Move posNumMoves = TileMoves.Moves[TileMoves.Moves.Length - 1];
					double radians = (Math.PI * posNumMoves.Angle) / 180f;
					float sineAngle = (float)Math.Sin(radians);
					// cosine is negative because angle 0 is move up and thats y--
					float cosineAngle = -(float)Math.Cos(radians);
					// basically multiples numMoves by -1, 0, or 1 depending on the angle.
					x += (int) Math.Round(sineAngle);
					y += (int) Math.Round(cosineAngle);
				}

				return new Point(x, y);
			}
		}

		#endregion

		/// <summary>
		/// Move forward one step. This handles moving through a curve. Do not call this to move out of a tile!
		/// </summary>
		public void Move()
		{
			mapPosition.X += MoveOn.Position.X;
			mapPosition.Y += MoveOn.Position.Y;
			OffsetTileMoves++;
		}

		/// <summary>
		/// Set the location to be entering a new tile
		/// </summary>
		/// <param name="ptMap">The point on the map where entering the tile (not the tile point).</param>
		/// <param name="movement">The movement for travelling through the tile.</param>
		public void SetMapPosition(Point ptMap, TileMovement movement)
		{
#if DEBUG
			int xOff = ptMap.X%TileMovement.UNITS_PER_TILE;
			Trap.trap((xOff != 0) && (xOff != 6) && (xOff != 18));
			int yOff = ptMap.Y%TileMovement.UNITS_PER_TILE;
			Trap.trap((yOff != 0) && (yOff != 6) && (yOff != 18));
#endif

			mapPosition = ptMap;
			TileMoves = movement;
			OffsetTileMoves = 0;
		}

		/// <summary>
		/// Converts a map point to the tile it is in.
		/// </summary>
		/// <param name="pt">A point on the map.</param>
		/// <returns>The tile it is in.</returns>
		public static Point MapToTile (Point pt)
		{
			return new Point(pt.X / TileMovement.UNITS_PER_TILE, pt.Y / TileMovement.UNITS_PER_TILE);
		}

		/// <summary>
		/// Converts a map point to the quarter tile it is in.
		/// </summary>
		/// <param name="pt">A point on the map.</param>
		/// <returns>The quarter tile it is in.</returns>
		public static Point MapToQuarterTile(Point pt)
		{
			return new Point(pt.X / (TileMovement.UNITS_PER_TILE / 2), pt.Y / (TileMovement.UNITS_PER_TILE / 2));
		}

		/// <summary>
		/// Converts a quarter tile to the tile it is in.
		/// </summary>
		/// <param name="pt">A quarter tile.</param>
		/// <returns>The tile it is in.</returns>
		public static Point QuarterTileToTile(Point pt)
		{
			return new Point(pt.X / 2, pt.Y / 2);
		}

		/// <summary>
		/// Equality operator.
		/// </summary>
		/// <param name="obj">The BoardLocation to compare to.</param>
		/// <returns>true if both objects are at the same position in the same Angle.</returns>
		public override bool Equals(object obj)
		{
			BoardLocation loc = (BoardLocation) obj;
			return Angle == loc.Angle && MapPosition == loc.MapPosition;
		}

		/// <summary>
		/// The hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return Angle.GetHashCode() ^ MapPosition.GetHashCode();
		}

		/// <summary>
		/// Displays the value of the object.
		/// </summary>
		/// <returns>The value of the object.</returns>
		public override string ToString()
		{
			string tilePos = string.Format("{0},{1}", (float)MapPosition.X / TileMovement.UNITS_PER_TILE, (float)MapPosition.Y / TileMovement.UNITS_PER_TILE);
			if (OffsetTileMoves < TileMoves.Moves.Length)
				return string.Format("pos:{0} ({4}), angle:{1}, moveOffset:{2}, moveOn:{3}", MapPosition, Angle, OffsetTileMoves, TileMoves, tilePos);
			return string.Format("pos:{0} ({2}), moves:{1}", MapPosition, TileMoves, tilePos);
		}
	}
}
