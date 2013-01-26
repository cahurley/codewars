using System;
using System.Collections.Generic;
using System.Drawing;
using WindwardopolisLibrary.map;

namespace WindwardopolisLibrary.units
{
	public class Limo
	{
		/// <summary>
		/// This goes back to the rear bumper of the car. When the car is straight that's 23/4 or 5-3/4. When at 45 degrees, it's
		/// 6.7 units so we round up to 7.
		/// </summary>
		public const int NUM_TAIL = 7;

		/// <summary>
		/// How many map units (steps) forward the Future list will be populated. This is required because a limo starting at
		/// speed 1 needs 6-1/2 (rounded up to 7) turns to clear an intersection (42).
		/// And also the number of steps to pull a U-turn in an intersection when an oncoming car is at full speed (90).
		/// </summary>
		public const int NUM_FUTURE = 90;

		/// <summary>
		/// If a limo is stopped this many turns, it will be jumped to the end of it's tile path.
		/// </summary>
		public const int MAX_TURNS_STOPPED = 20;

		#region variables including auto properties

		/// <summary>
		/// The location on the map for this unit.
		/// </summary>
		public BoardLocation Location { get; set; }

		/// <summary>
		/// The path of tiles (not map units) for this limo to take.
		/// </summary>
		public List<Point> PathTiles { get; private set; }

		/// <summary>
		/// The bitmap for this limo facing North. The Location has no impact on this bitmap.
		/// </summary>
		public Bitmap VehicleBitmap { get; private set; }

		/// <summary>
		/// The speed of the limo. This is the number of Location units it will move in 1 tick and has a 
		/// value of 0 (stopped) to TileMovement.MaxSpeed.
		/// </summary>
		public float Speed { get; set; }

		/// <summary>
		/// The number of steps to move this vehicle this turn. The whole part of this is assigned to StepsRemaining
		/// and the fraction is carried over to the next tick.
		/// </summary>
		public float AccruedSteps { get; set; }

		/// <summary>
		/// The steps to take this turn (usually 0 or 1 but larger if stuck).
		/// </summary>
		public int StepsRemaining { get; set; }

		/// <summary>
		/// true when the car is stopped at a stop sign, stop light, or waiting for a car to get out of the way.
		/// </summary>
		public bool IsStopped { get; set; }

		/// <summary>
		/// The number of turns this limo has been stopped.
		/// </summary>
		public int NumTurnsStopped { get; set; }

		/// <summary>
		/// If true this vehicle does not look for other vehicles and is moved AccruedSteps. Used to handle a stuck vehicle.
		/// </summary>
		public bool IsForceMove { get; set; }

		/// <summary>
		/// The previous center points of this vehicle. This is the back half of the car based on where the center point previously was.
		/// This data is held (and updated) as the vehicle moves. Points are map position.
		/// </summary>
		private Queue<Point> TailMap { get; set; }

		/// <summary>
		/// The previous center points of this vehicle. This is the back half of the car based on where the center point previously was.
		/// This data is held (and updated) as the vehicle moves. Points are quarter tile position.
		/// </summary>
		public List<Point> TailQuarter { get; private set; }

		/// <summary>
		/// The future center points of this vehicle. This is from the center point of the vehicle on. This data is calculated
		/// anew for each turn. Points are quarter tile positions.
		/// </summary>
		public List<QuarterSteps> Future { get; private set; }

		private readonly Dictionary<int, Bitmap> vehicleRotated;

		#endregion

		#region constructors

		public Limo(BoardLocation location, Bitmap limoBitmap)
		{
			Location = new BoardLocation(location);
			VehicleBitmap = limoBitmap;
			PathTiles = new List<Point>();
			TailMap = new Queue<Point>();
			TailQuarter = new List<Point>();
			Future = new List<QuarterSteps>();
			vehicleRotated = new Dictionary<int, Bitmap>();
			vehicleRotated[0] = limoBitmap;
		}

		#endregion

		/// <summary>
		/// Reset vehicle to restart a game. No path, tail, etc.
		/// </summary>
        public void Reset()
        {
            PathTiles.Clear();
			TailMap.Clear();
			TailQuarter.Clear();
            Future.Clear();
			Speed = 1;
			NumTurnsStopped = 0;
			AccruedSteps = 0;
			StepsRemaining = 1;
			IsStopped = false;
			StepsRemaining = 0;
			IsForceMove = false;
        }

		/// <summary>
		/// Stop the car.
		/// </summary>
		public void Stop ()
		{
			Speed = 0;
			StepsRemaining = 0;
			AccruedSteps = 0;
			IsStopped = true;
			NumTurnsStopped++;
		}

		/// <summary>
		/// Start up from a stop.
		/// </summary>
		public void Go()
		{
			Speed = 1;
			StepsRemaining = 1;
			AccruedSteps = 0;
			IsStopped = false;
			NumTurnsStopped = 0;
		}

		/// <summary>
		/// Accelerate the car one tick
		/// </summary>
		public void Accelerate()
		{
			Speed = Math.Min(Speed + 0.05f, Location.TileMoves.MaxSpeed);
		}

		/// <summary>
		/// Decelerate the car one tick. This is double the accelerate as subsequent to this, on the 
		/// next tick, it will be accelerated.
		/// </summary>
		public void Decelerate()
		{
			Speed = Math.Max(Speed - 0.1f, 0);
		}

		/// <summary>
		/// The vehicle bitmap at the Location.Angle.
		/// </summary>
		public Bitmap BitmapAtAngle
		{
			get
			{
				if (vehicleRotated.ContainsKey(Location.Angle))
					return vehicleRotated[Location.Angle];
				Bitmap bmp = Utilities.RotateImage(VehicleBitmap, Location.Angle);
				vehicleRotated[Location.Angle] = bmp;
				return bmp;
			}
		}

		#region movement

		/// <summary>
		/// Move forward one step.
		/// </summary>
		public void Move()
		{

			// add center point to end of Tail list & queue
			Point ptQtr = BoardLocation.MapToQuarterTile(Location.MapPosition);
			if ((TailQuarter.Count == 0) || (TailQuarter[TailQuarter.Count - 1] != ptQtr))
				TailQuarter.Add(ptQtr);
			TailMap.Enqueue(Location.MapPosition);

			// remove from end of tail
			while (TailMap.Count > NUM_TAIL)
			{
				TailMap.Dequeue();
				if ((TailQuarter.Count > 0) && (TailQuarter[0] != BoardLocation.MapToQuarterTile(TailMap.Peek())))
					TailQuarter.RemoveAt(0);
			}

			// the move means we're one forward into the future - remove it
			if (Future.Count > 0)
			{
				if (Future[0].Steps <= 1)
					Future.RemoveAt(0);
				else
					Future[0].Steps--;
			}

			Location.Move();
			NumTurnsStopped = 0;
		}

		#endregion

		#region calculate position

		/// <summary>
		/// Calculate the new location of the center of the car moving it N steps. Estimates if steps lead out of a tile.
		/// </summary>
		/// <param name="numMoves">The number of moves forward from Location to return this for.</param>
		public Point CalculateNextPosition(int numMoves)
		{

			TileMovement.Move posOn = Location.MoveOn;
			TileMovement.Move posNumMoves;
			int x = Location.MapPosition.X;
			int y = Location.MapPosition.Y;
			if (numMoves == 0)
				posNumMoves = posOn;
			else
			{
				posNumMoves = Location.MoveOn;
				for (int ind = Location.OffsetTileMoves; ind < Math.Min(Location.OffsetTileMoves + numMoves, Location.TileMoves.Moves.Length); ind++)
				{
					posNumMoves = Location.TileMoves.Moves[ind];
					x += posNumMoves.Position.X;
					y += posNumMoves.Position.Y;
					numMoves--;
				}
			}
			if (numMoves > 0)
			{
				float sineAngle = (float)Math.Sin((Math.PI * posNumMoves.Angle) / 180f);
				// cosine is negative because angle 0 is move up and thats y--
				float cosineAngle = -(float)Math.Cos((Math.PI * posNumMoves.Angle) / 180f);
				// basically multiples numMoves by -1, 0, or 1 depending on the angle.
				x += numMoves * (int)Math.Round(sineAngle);
				y += numMoves * (int)Math.Round(cosineAngle);
			}

			return new Point(x, y);
		}

		/// <summary>
		/// The location of the front of the car in map units.
		/// </summary>
		public Point Front
		{
			get
			{
				int x = Location.MapPosition.X;
				int y = Location.MapPosition.Y;
				TileMovement.Move posNumMoves = Location.MoveOn;
				x += posNumMoves.Position.X;
				y += posNumMoves.Position.Y;

				float sineAngle = (float)Math.Sin((Math.PI * posNumMoves.Angle) / 180f);
				// cosine is negative because angle 0 is move up and thats y--
				float cosineAngle = -(float)Math.Cos((Math.PI * posNumMoves.Angle) / 180f);

				// adds in the vehicle length to the center point.
				x += (int)((VehicleBitmap.Height / 4f) * sineAngle);
				y += (int)((VehicleBitmap.Height / 4f) * cosineAngle);
				return new Point(x, y);
			}
		}

		/// <summary>
		/// The location of the front of the car after it moves one step forward in map units.
		/// </summary>
		public Point FrontNextStep
		{
			get
			{
				int x = Location.MapPosition.X;
				int y = Location.MapPosition.Y;
				TileMovement.Move posNumMoves = Location.MoveOn;
				x += posNumMoves.Position.X;
				y += posNumMoves.Position.Y;

				float sineAngle = (float) Math.Sin((Math.PI*posNumMoves.Angle)/180f);
				// cosine is negative because angle 0 is move up and thats y--
				float cosineAngle = -(float) Math.Cos((Math.PI*posNumMoves.Angle)/180f);

				// adds in the vehicle length to the center point.
				x += (int) ((VehicleBitmap.Height / 4f) * sineAngle);
				y += (int) ((VehicleBitmap.Height / 4f) * cosineAngle);

				// basically multiples by -1, 0, or 1 depending on the angle for the 1 step forward.
				x += (int) Math.Round(sineAngle);
				y += (int) Math.Round(cosineAngle);
				return new Point(x, y);
			}
		}

		#endregion

		/// <summary>
		/// Add a point to the Future list. Passed in in map units and will either add a step if this quarter
		/// point exists or add a new one if it does not.
		/// </summary>
		/// <param name="ptMap">The point to add in map units.</param>
		public void AddFuture (Point ptMap)
		{
			Point ptQtr = BoardLocation.MapToQuarterTile(ptMap);
			QuarterSteps stepOn = Future.Find(qs => qs.QuarterLocation == ptQtr);
			if (stepOn != null)
				stepOn.Steps++;
			else
				Future.Add(new QuarterSteps(ptQtr));
		}

		private const int addMiddleNearLane = TileMovement.UNITS_PER_TILE / 4;
		private const int addMiddleTile = TileMovement.UNITS_PER_TILE / 2;
		private const int addMiddleFarLane = (3 * TileMovement.UNITS_PER_TILE) / 4;

		public void SetTileLocation(MapSquare square, Point ptTile, MapSquare.COMPASS_DIRECTION direction)
		{
			switch (square.Tile.Direction)
			{
				case MapTile.DIRECTION.NORTH_SOUTH:
				case MapTile.DIRECTION.EAST_WEST:
				case MapTile.DIRECTION.INTERSECTION:
				case MapTile.DIRECTION.T_NORTH:
				case MapTile.DIRECTION.T_EAST:
				case MapTile.DIRECTION.T_SOUTH:
				case MapTile.DIRECTION.T_WEST:
					switch (direction)
					{
						case MapSquare.COMPASS_DIRECTION.NORTH:
							Location =
								new BoardLocation(new Point(ptTile.X*TileMovement.UNITS_PER_TILE + addMiddleFarLane,
								                            ptTile.Y*TileMovement.UNITS_PER_TILE + addMiddleTile),
								                  TileMovement.GetMove(MapSquare.COMPASS_DIRECTION.NORTH, MapSquare.COMPASS_DIRECTION.NORTH));
							return;
						case MapSquare.COMPASS_DIRECTION.EAST:
							Location =
								new BoardLocation(new Point(ptTile.X*TileMovement.UNITS_PER_TILE + addMiddleTile,
								                            ptTile.Y*TileMovement.UNITS_PER_TILE + addMiddleFarLane),
								                  TileMovement.GetMove(MapSquare.COMPASS_DIRECTION.EAST, MapSquare.COMPASS_DIRECTION.EAST));
							return;
					case MapSquare.COMPASS_DIRECTION.SOUTH:
							Location = new BoardLocation(new Point(ptTile.X*TileMovement.UNITS_PER_TILE + addMiddleNearLane,
							                                       ptTile.Y*TileMovement.UNITS_PER_TILE + addMiddleTile),
							                             TileMovement.GetMove(MapSquare.COMPASS_DIRECTION.SOUTH,
							                                                  MapSquare.COMPASS_DIRECTION.SOUTH));
							return;
					case MapSquare.COMPASS_DIRECTION.WEST:
							Location = new BoardLocation(new Point(ptTile.X * TileMovement.UNITS_PER_TILE + addMiddleTile,
													   (ptTile.Y * TileMovement.UNITS_PER_TILE + addMiddleNearLane)),
											 TileMovement.GetMove(MapSquare.COMPASS_DIRECTION.WEST,
																  MapSquare.COMPASS_DIRECTION.WEST));
							return;
					}
					return;
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format("Speed: {0:0.00}, Path count: {1}", Speed, PathTiles.Count);
		}

		/// <summary>
		/// Track points in the Future list.
		/// </summary>
		public class QuarterSteps
		{
			/// <summary>
			/// The quarter point for these steps.
			/// </summary>
			public Point QuarterLocation { get; private set; }

			/// <summary>
			/// The number of steps in this tile quarter.
			/// </summary>
			public int Steps { get; set; }

			/// <summary>
			/// Create the object. Steps is set to 1.
			/// </summary>
			/// <param name="quarterLocation">The quarter point for these steps.</param>
			public QuarterSteps(Point quarterLocation)
			{
				QuarterLocation = quarterLocation;
				Steps = 1;
			}

			/// <summary>
			/// Display the data as {x:y}:steps.
			/// </summary>
			/// <returns>Display the data as {x:y}:steps.</returns>
			public override string ToString()
			{
				return string.Format("{0}:{1}", QuarterLocation, Steps);
			}
		}
	}
}
