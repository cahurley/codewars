// No comments about how this is the world's worst A* implementation. It is purposely simplistic to leave the teams
// the opportunity to improve greatly upon this. (I was yelled at last year for making the sample A.I.'s too good.)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WindwardopolisLibrary;
using WindwardopolisLibrary.map;

namespace Windwardopolis.game_ai
{
	/// <summary>
	/// The Pathfinder (maybe I should name it Frémont).
	/// Good intro at http://www.policyalmanac.org/games/aStarTutorial.htm
	/// </summary>
	public class SimpleAStar
	{
		private static readonly Point[] offsets = {new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };

		private const int DEAD_END = 10000;

		private static readonly Point ptOffMap = new Point(-1, -1);

		public static List<Point> CalculatePath(GameMap map, Point start, Point end)
		{

			// should never happen but just to be sure
			if (start == end)
				return new List<Point> {start};

			// nodes are points we have walked to
			Dictionary<Point, TrailPoint> nodes = new Dictionary<Point, TrailPoint>();
			// points we have in a TrailPoint, but not yet evaluated.
			List<TrailPoint> notEvaluated = new List<TrailPoint>();

			TrailPoint tpOn = new TrailPoint(start, end, 0);
			while (true)
			{
				nodes.Add(tpOn.MapTile, tpOn);

				// get the neighbors
				TrailPoint tpClosest = null;
				foreach (Point ptOffset in offsets)
				{
					Point pt = new Point(tpOn.MapTile.X + ptOffset.X, tpOn.MapTile.Y + ptOffset.Y);
					MapSquare square = map.SquareOrDefault(pt);
					// off the map or not a road/bus stop
					if ((square == null) || (!square.Tile.IsDriveable))
						continue;

					// already evaluated - add it in
					if (nodes.ContainsKey(pt))
					{
						TrailPoint tpAlreadyEvaluated = nodes[pt];
						tpAlreadyEvaluated.Cost = Math.Min(tpAlreadyEvaluated.Cost, tpOn.Cost + 1);
						tpOn.Neighbors.Add(tpAlreadyEvaluated);
						continue;
					}

					// add this one in
					TrailPoint tpNeighbor = new TrailPoint(pt, end, tpOn.Cost + 1);
					tpOn.Neighbors.Add(tpNeighbor);
					// may already be in notEvaluated. If so remove it as this is a more recent cost estimate
					int indTp = notEvaluated.FindIndex(tp => tp.MapTile == tpNeighbor.MapTile);
					if (indTp != -1)
						notEvaluated.RemoveAt(indTp);

					// we only assign to tpClosest if it is closer to the destination. If it's further away, then we
					// use notEvaluated below to find the one closest to the dest that we have not walked yet.
					if (tpClosest == null)
					{
						if (tpNeighbor.Distance < tpOn.Distance)
							// new neighbor is closer - work from this next. 
							tpClosest = tpNeighbor;
						else
							// this is further away - put in the list to try if a better route is not found
							notEvaluated.Add(tpNeighbor);
					}
					else
						if (tpClosest.Distance <= tpNeighbor.Distance)
							// this is further away - put in the list to try if a better route is not found
							notEvaluated.Add(tpNeighbor);
						else
						{
							// this is closer than tpOn and another neighbor - use it next.
							notEvaluated.Add(tpClosest);
							tpClosest = tpNeighbor;
						}
				}

				// re-calc based on neighbors
				tpOn.RecalculateDistance(ptOffMap, map.Width);

				// if no closest, then get from notEvaluated. This is where it guarantees that we are getting the shortest
				// route - we go in here if the above did not move a step closer. This may not either as the best choice
				// may be the neighbor we didn't go with above - but we drop into this to find the closest based on what we know.
				if (tpClosest == null)
				{
					if (notEvaluated.Count == 0)
					{
						Trap.trap();
						break;
					}
					// We need the closest one as that's how we find the shortest path.
					tpClosest = notEvaluated[0];
					int index = 0;
					for (int ind = 1; ind < notEvaluated.Count; ind++ )
					{
						TrailPoint tpNotEval = notEvaluated[ind];
						if (tpNotEval.Distance >= tpClosest.Distance) 
							continue;
						tpClosest = tpNotEval;
						index = ind;
					}
					notEvaluated.RemoveAt(index);
				}

				// if we're at end - we're done!
				if (tpClosest.MapTile == end)
				{
					tpClosest.Neighbors.Add(tpOn);
					nodes.Add(tpClosest.MapTile, tpClosest);
					break;
				}

				// try this one
				tpOn = tpClosest;
			}


			List<Point> path = new List<Point>();
			if (! nodes.ContainsKey(end))
			{
				Trap.trap();
				return path;
			}

			// Create the return path - from end back to beginning.
			tpOn = nodes[end];
			path.Add(tpOn.MapTile);
			while (tpOn.MapTile != start)
			{
				List<TrailPoint> neighbors = tpOn.Neighbors;
				int cost = tpOn.Cost;

				tpOn = tpOn.Neighbors[0];
				for (int ind = 1; ind < neighbors.Count; ind++)
					if (neighbors[ind].Cost < tpOn.Cost)
						tpOn = neighbors[ind];

				// we didn't get to the start.
				if (tpOn.Cost >= cost)
				{
					Trap.trap();
					return path;
				}
				path.Insert(0, tpOn.MapTile);
			}

			return path;
		}

		class TrailPoint
		{
			/// <summary>
			/// The Map tile for this point in the trail.
			/// </summary>
			public Point MapTile { get; private set; }

			/// <summary>
			/// The neighboring tiles (up to 4). If 0 then this point has been added as a neighbor but is in the
			/// notEvaluated List because it has not yet been tried.
			/// </summary>
			public List<TrailPoint> Neighbors { get; private set; }

			/// <summary>
			/// Estimate of the distance to the end. Direct line if have no neighbors. Best neighbor.Distance + 1
			/// if have neighbors. This value is bad if it's along a trail that failed.
			/// </summary>
			public int Distance { get; private set; }

			/// <summary>
			/// The number of steps from the start to this tile.
			/// </summary>
			public int Cost { get; set; }

			public TrailPoint(Point pt, Point end, int cost)
			{
				MapTile = pt;
				Neighbors = new List<TrailPoint>();
				Distance = Math.Abs(MapTile.X - end.X) + Math.Abs(MapTile.Y - end.Y);
				Cost = cost;
			}

			public void RecalculateDistance(Point mapTileCaller, int remainingSteps)
			{

				Trap.trap(Distance == 0);
				// if no neighbors then this is in notEvaluated and so can't recalculate.
				if (Neighbors.Count == 0)
					return;

				int shortestDistance;
				// if just 1 neighbor, then it's a dead end
				if (Neighbors.Count == 1)
					shortestDistance = DEAD_END;
				else
				{
					shortestDistance = Neighbors.Select(neighborOn => neighborOn.Distance).Min();
					// it's 1+ lowest neighbor value (unless a dead end)
					if (shortestDistance != DEAD_END)
						shortestDistance++;
				}

				// no change, no need to recalc neighbors
				if (shortestDistance == Distance)
					return;

				// new value (could be longer or shorter)
				Distance = shortestDistance;

				// if gone too far, no more recalculate
				if (remainingSteps-- < 0)
					return;

				//  Need to tell our neighbors - except the one that called us
				foreach (TrailPoint neighborOn in Neighbors.Where(neighborOn => neighborOn.MapTile != mapTileCaller))
					neighborOn.RecalculateDistance(MapTile, remainingSteps);

				// and we re-calc again because that could have changed our neighbor's values
				shortestDistance = Neighbors.Select(neighborOn => neighborOn.Distance).Min();
				// it's 1+ lowest neighbor value (unless a dead end)
				if (shortestDistance != DEAD_END)
					shortestDistance++;
				Distance = shortestDistance;
			}

			public override string ToString()
			{
				return string.Format("Map={0}, Cost={1}, Distance={2}, Neighbors={3}", MapTile, Cost, Distance, Neighbors.Count);
			}
		}
	}
}
