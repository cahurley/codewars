using System;
using System.Collections.Generic;
using System.Drawing;

namespace WindwardopolisLibrary.map
{
	/// <summary>
	/// Movement within a tile.
	/// </summary>
	public class TileMovement
	{
		/// <summary>
		/// The number of position units within a tile. For a value of 24 units per tile, 0,0 - 23,23 are all on tile 0,0.
		/// </summary>
		public const int UNITS_PER_TILE = 24;

		/// <summary>
		/// Max speed in a straight-away
		/// </summary>
		public const int MAX_STRAIGHT_SPEED = 6;

		/// <summary>
		/// The number of map units required for a car at a stop to cross and clear an intersection. Actual numbers are 5 * 6
		/// for an inner curve, 7 * 6 for a straight away, and 8 * 6 for an outer curve. We do the 7 * 6 and for an outer
		/// curve a car coming along will have to slow down. 
		/// </summary>
		public const int NUM_UNITS_CROSS_ROAD = 42;

		/// <summary>
		/// The maximum number of steps through any tile.
		/// </summary>
		public const int MAX_TILE_STEPS = 48;

		/// <summary>
		/// Max speed in a curve.
		/// </summary>
		public const int MAX_CURVE_SPEED = 3;

		public int MaxSpeed { get; private set; }

		public Move[] Moves { get; private set; }

		private static readonly TileMovement North,
		                                     East,
		                                     South,
		                                     West,
		                                     TurnNE,
		                                     TurnES,
		                                     TurnSW,
		                                     TurnWN,
		                                     TurnNW,
		                                     TurnWS,
		                                     TurnSE,
		                                     TurnEN,
		                                     Unorth,
		                                     Ueast,
		                                     Usouth,
		                                     Uwest;

		static TileMovement()
		{
			// straight
			North = CtorStraight(0, -1, 0);
			East = CtorStraight(1, 0, 90);
			South = CtorStraight(0, 1, 180);
			West = CtorStraight(-1, 0, 270);

			// inner curves - angles are the quadrant for the +/- 1 we want for direction.
			TurnNE = CtorInnerCurve(UNITS_PER_TILE / 4, 180, 270, 1);
			TurnES = CtorInnerCurve(UNITS_PER_TILE / 4, 270, 360, 1);
			TurnSW = CtorInnerCurve(UNITS_PER_TILE / 4, 0, 90, 1);
			TurnWN = CtorInnerCurve(UNITS_PER_TILE / 4, 90, 180, 1);

			// outer curves - angles are the quadrant for the +/- 1 we want for direction.
			TurnNW = CtorOuterCurve(UNITS_PER_TILE / 2, 360, 270, -1);
			TurnWS = CtorOuterCurve(UNITS_PER_TILE / 2, 270, 180, -1);
			TurnSE = CtorOuterCurve(UNITS_PER_TILE / 2, 180, 90, -1);
			TurnEN = CtorOuterCurve(UNITS_PER_TILE / 2, 90, 0, -1);

			// U turns
			Unorth = CtorUturn(0, -1, 360, 180);
			Ueast = CtorUturn(1, 0, 90, -90);
			Usouth = CtorUturn(0, 1, 180, 0);
			Uwest = CtorUturn(-1, 0, 270, 90);
		}

		private static TileMovement CtorStraight(int dx, int dy, int angle)
		{
			List<Move> moves = new List<Move>();
			for (int offset = 0; offset < UNITS_PER_TILE; offset++)
				moves.Add(new Move(new Point(dx, dy), angle));
			return new TileMovement(MAX_STRAIGHT_SPEED, moves.ToArray());
		}

		// movement is both the radius and the total movement in the X and Y direction.
		private static TileMovement CtorOuterCurve(int movement, int angleStart, int angleEnd, int rotate)
		{

			List<Move> moves = new List<Move>();

			// into the curve
			double radians = (angleStart * Math.PI) / 180d;
			int dx = (int) Math.Sin(radians);
			int dy = (int) - Math.Cos(radians);
			for (int offset = 0; offset < UNITS_PER_TILE / 4; offset++)
				moves.Add(new Move(new Point(dx, dy), NormalizeAngle(angleStart)));

			int xPrev = (int)(movement * Math.Cos(radians));
			int yPrev = (int)(movement * Math.Sin(radians));

			// rotate 1 degree at a time calculating x, y, & angle
			for (int angle = angleStart; rotate > 0 ? angle <= angleEnd : angle >= angleEnd; angle += rotate)
			{
				radians = (angle * Math.PI) / 180d;
				int x = (int) (movement*Math.Cos(radians));
				int y = (int) (movement*Math.Sin(radians));
				if ((x == xPrev) && (y == yPrev))
					continue;

				moves.Add(new Move(new Point(x - xPrev, y - yPrev), NormalizeAngle(angle + (rotate == 1 ? 180 : 0))));
				xPrev = x;
				yPrev = y;
			}

			// out of the curve
			radians = (angleEnd * Math.PI) / 180d;
			dx = (int) Math.Sin(radians);
			dy = (int) - Math.Cos(radians);
			for (int offset = 0; offset < UNITS_PER_TILE / 4; offset++)
				moves.Add(new Move(new Point(dx, dy), NormalizeAngle(angleEnd)));

			return new TileMovement(MAX_CURVE_SPEED, moves.ToArray());
		}

		// movement is both the radius and the total movement in the X and Y direction.
		private static TileMovement CtorInnerCurve(int movement, int angleStart, int angleEnd, int rotate)
		{

			List<Move> moves = new List<Move>();
			double radians = (angleStart * Math.PI) / 180d;
			int xPrev = (int)(movement * Math.Cos(radians));
			int yPrev = (int)(movement * Math.Sin(radians));

			// rotate 1 degree at a time calculating x, y, & angle
			for (int angle = angleStart; rotate > 0 ? angle <= angleEnd : angle >= angleEnd; angle += rotate)
			{
				radians = (angle * Math.PI) / 180d;
				int x = (int)(movement * Math.Cos(radians));
				int y = (int)(movement * Math.Sin(radians));
				if ((x == xPrev) && (y == yPrev))
					continue;

				moves.Add(new Move(new Point(x - xPrev, y - yPrev), NormalizeAngle(angle + (rotate == 1 ? 180 : 0))));
				xPrev = x;
				yPrev = y;
			}

			return new TileMovement(MAX_CURVE_SPEED, moves.ToArray());
		}

		private static TileMovement CtorUturn(int dx, int dy, int angleStart, int angleEnd)
		{
			List<Move> moves = new List<Move>();
			// into the U
			for (int offset = 0; offset < UNITS_PER_TILE / 2; offset++)
				moves.Add(new Move(new Point(dx, dy), NormalizeAngle(angleStart)));

			// semi-circle
			const double radius = UNITS_PER_TILE / 4.0;
			double radians = (angleStart * Math.PI) / 180d;
			int xPrev = (int)(radius * Math.Cos(radians));
			int yPrev = (int)(radius * Math.Sin(radians));

			// rotate 1 degree at a time calculating x, y, & angle
			for (int angle = angleStart; angle >= angleEnd; angle--)
			{
				radians = (angle * Math.PI) / 180d;
				int x = (int)(radius * Math.Cos(radians));
				int y = (int)(radius * Math.Sin(radians));
				if ((x == xPrev) && (y == yPrev))
					continue;

				moves.Add(new Move(new Point(x - xPrev, y - yPrev), NormalizeAngle(angle)));
				xPrev = x;
				yPrev = y;
			}

			// out of the U
			for (int offset = 0; offset < UNITS_PER_TILE / 2; offset++)
				moves.Add(new Move(new Point(-dx, -dy), NormalizeAngle(angleEnd)));
			return new TileMovement(MAX_CURVE_SPEED, moves.ToArray());
		}

		/// <summary>
		/// Return an angle in the range 0..359
		/// </summary>
		/// <param name="angle">The angle to normalize.</param>
		/// <returns>The normalized angle.</returns>
		public static int NormalizeAngle(int angle)
		{
			while (angle >= 360)
				angle -= 360;
			while (angle < 0)
				angle += 360;
			return angle;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public TileMovement(int maxSpeed, Move[] moves)
		{
			MaxSpeed = maxSpeed;
			Moves = moves;
		}

		public static TileMovement GetMove(MapSquare.COMPASS_DIRECTION start, MapSquare.COMPASS_DIRECTION end)
		{
			switch (start)
			{
				case MapSquare.COMPASS_DIRECTION.NORTH:
					switch (end)
					{
						case MapSquare.COMPASS_DIRECTION.NORTH:
							return North;
						case MapSquare.COMPASS_DIRECTION.EAST:
							return TurnNE;
						case MapSquare.COMPASS_DIRECTION.SOUTH:
							return Unorth;
						case MapSquare.COMPASS_DIRECTION.WEST:
							return TurnNW;
					}
					break;
				case MapSquare.COMPASS_DIRECTION.EAST:
					switch (end)
					{
						case MapSquare.COMPASS_DIRECTION.NORTH:
							return TurnEN;
						case MapSquare.COMPASS_DIRECTION.EAST:
							return East;
						case MapSquare.COMPASS_DIRECTION.SOUTH:
							return TurnES;
						case MapSquare.COMPASS_DIRECTION.WEST:
							return Ueast;
					}
					break;
				case MapSquare.COMPASS_DIRECTION.SOUTH:
					switch (end)
					{
						case MapSquare.COMPASS_DIRECTION.NORTH:
							return Usouth;
						case MapSquare.COMPASS_DIRECTION.EAST:
							return TurnSE;
						case MapSquare.COMPASS_DIRECTION.SOUTH:
							return South;
						case MapSquare.COMPASS_DIRECTION.WEST:
							return TurnSW;
					}
					break;
				case MapSquare.COMPASS_DIRECTION.WEST:
					switch (end)
					{
						case MapSquare.COMPASS_DIRECTION.NORTH:
							return TurnWN;
						case MapSquare.COMPASS_DIRECTION.EAST:
							return Uwest;
						case MapSquare.COMPASS_DIRECTION.SOUTH:
							return TurnWS;
						case MapSquare.COMPASS_DIRECTION.WEST:
							return West;
					}
					break;
			}
			throw new ApplicationException("unknown direction(s)");
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			Point offset = new Point();
			foreach (Move move in Moves)
				offset.Offset(move.Position);
			return string.Format("speed:{0}, numMoves:{1}, totalOffset:{2}", MaxSpeed, Moves.Length, offset);
		}

		public class Move
		{
			public Point Position { get; private set; }
			public int Angle { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:System.Object"/> class.
			/// </summary>
			public Move(Point position, int angle)
			{
				Position = position;
				Angle = angle;
			}

			/// <summary>
			/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
			/// </returns>
			public override string ToString()
			{
				return string.Format("{0},{1} ; {2}", Position.X, Position.Y, Angle);
			}
		}
	}
}
