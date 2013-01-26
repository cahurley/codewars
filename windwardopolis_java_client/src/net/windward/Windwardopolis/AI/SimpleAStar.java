package net.windward.Windwardopolis.AI;

import java.awt.*;

import net.windward.Windwardopolis.api.Map;
import net.windward.Windwardopolis.api.MapSquare;
import net.windward.Windwardopolis.TRAP;
// No comments about how this is the world's worst A* implementation. It is purposely simplistic to leave the teams
// the opportunity to improve greatly upon this. (I was yelled at last year for making the sample A.I.'s too good.)
//
// Also there is (at least) one very subtle bug in the below code. It is very rarely hit.


/**
 * The Pathfinder (maybe I should name it Fr�mont).
 * Good intro at http://www.policyalmanac.org/games/aStarTutorial.htm
 */
public class SimpleAStar {
    private static final Point[] offsets = {
            new Point(-1, 0),
            new Point(1, 0),
            new Point(0, -1),
            new Point(0, 1)
    };

    private static final int DEAD_END = 10000;

    private static final Point ptOffMap = new Point(-1, -1);

    /**
     * Calculate a path from start to end. No comments about how this is the world's worst A* implementation. It is purposely
     * simplistic to leave the teams the opportunity to improve greatly upon this. (I was yelled at last year for making the
     * sample A.I.'s too good.)
     *
     * @param map   The game map.
     * @param start The tile units of the start point (inclusive).
     * @param end   The tile units of the end point (inclusive).
     * @return The path from start to end.
     */
    public static java.util.ArrayList<Point> CalculatePath(Map map, Point start, Point end) {

        // should never happen but just to be sure
        if (start == end) {
            return new java.util.ArrayList<Point>(java.util.Arrays.asList(new Point[]{start}));
        }

        // nodes are points we have walked to
        java.util.HashMap<Point, TrailPoint> nodes = new java.util.HashMap<Point, TrailPoint>();
        // points we have in a TrailPoint, but not yet evaluated.
        java.util.ArrayList<TrailPoint> notEvaluated = new java.util.ArrayList<TrailPoint>();

        TrailPoint tpOn = new TrailPoint(start, end, 0);
        while (true) {
            nodes.put(tpOn.getMapTile(), tpOn);

            // get the neighbors
            TrailPoint tpClosest = null;
            for (Point ptOffset : offsets) {
                Point pt = new Point(tpOn.getMapTile().x + ptOffset.x, tpOn.getMapTile().y + ptOffset.y);
                MapSquare square = map.SquareOrDefault(pt);
                // off the map or not a road/bus stop
                if ((square == null) || (!square.getIsDriveable())) {
                    continue;
                }

                // already evaluated - add it in
                if (nodes.containsKey(pt)) {
                    TrailPoint tpAlreadyEvaluated = nodes.get(pt);
                    tpAlreadyEvaluated.setCost(Math.min(tpAlreadyEvaluated.getCost(), tpOn.getCost() + 1));
                    tpOn.getNeighbors().add(tpAlreadyEvaluated);
                    continue;
                }

                // add this one in
                TrailPoint tpNeighbor = new TrailPoint(pt, end, tpOn.getCost() + 1);
                tpOn.getNeighbors().add(tpNeighbor);

                // may already be in notEvaluated. If so remove it as this is a more recent cost estimate

                int indTp = -1;

                for (int i = 0; i < notEvaluated.size(); i++) {
                    TrailPoint tp = notEvaluated.get(i);
                    if (tp.getMapTile() == tpNeighbor.getMapTile()) {
                        indTp = i;
                        break;
                    }

                }

                if (indTp != -1)
                    notEvaluated.remove(indTp);


                // we only assign to tpClosest if it is closer to the destination. If it's further away, then we
                // use notEvaluated below to find the one closest to the dest that we have not walked yet.
                if (tpClosest == null) {
                    if (tpNeighbor.getDistance() < tpOn.getDistance())
                    // new neighbor is closer - work from this next.
                    {
                        tpClosest = tpNeighbor;
                    } else
                    // this is further away - put in the list to try if a better route is not found
                    {
                        notEvaluated.add(tpNeighbor);
                    }
                } else {
                    if (tpClosest.getDistance() <= tpNeighbor.getDistance())
                    // this is further away - put in the list to try if a better route is not found
                    {
                        notEvaluated.add(tpNeighbor);
                    } else {
                        // this is closer than tpOn and another neighbor - use it next.
                        notEvaluated.add(tpClosest);
                        tpClosest = tpNeighbor;
                    }
                }
            }

            // re-calc based on neighbors
            tpOn.RecalculateDistance(ptOffMap, map.getWidth());

            // if no closest, then get from notEvaluated. This is where it guarantees that we are getting the shortest
            // route - we go in here if the above did not move a step closer. This may not either as the best choice
            // may be the neighbor we didn't go with above - but we drop into this to find the closest based on what we know.
            if (tpClosest == null) {
                if (notEvaluated.isEmpty()) {
                    TRAP.trap();
                    break;
                }
                // We need the closest one as that's how we find the shortest path.
                tpClosest = notEvaluated.get(0);
                int index = 0;
                for (int ind = 1; ind < notEvaluated.size(); ind++) {
                    TrailPoint tpNotEval = notEvaluated.get(ind);
                    if (tpNotEval.getDistance() >= tpClosest.getDistance()) {
                        continue;
                    }
                    tpClosest = tpNotEval;
                    index = ind;
                }
                notEvaluated.remove(index);
            }

            // if we're at end - we're done!
            if (tpClosest.getMapTile() == end) {
                tpClosest.getNeighbors().add(tpOn);
                nodes.put(tpClosest.getMapTile(), tpClosest);
                break;
            }

            // try this one
            tpOn = tpClosest;
        }

        // Create the return path - from end back to beginning.
        java.util.ArrayList<Point> path = new java.util.ArrayList<Point>();
        tpOn = nodes.get(end);
        path.add(tpOn.getMapTile());
        while (tpOn.getMapTile() != start) {
            java.util.ArrayList<TrailPoint> neighbors = tpOn.getNeighbors();
            int cost = tpOn.getCost();

            tpOn = tpOn.getNeighbors().get(0);
            for (int ind = 1; ind < neighbors.size(); ind++) {
                if (neighbors.get(ind).getCost() < tpOn.getCost()) {
                    tpOn = neighbors.get(ind);
                }
            }

            // we didn't get to the start.
            if (tpOn.getCost() >= cost) {
                TRAP.trap();
                return path;
            }
            path.add(0, tpOn.getMapTile());
        }

        return path;
    }

    private static class TrailPoint {
        /**
         * The Map tile for this point in the trail.
         */
        private Point privateMapTile;

        public final Point getMapTile() {
            return privateMapTile;
        }

        private void setMapTile(Point value) {
            privateMapTile = value;
        }

        /**
         * The neighboring tiles (up to 4). If 0 then this point has been added as a neighbor but is in the
         * notEvaluated List because it has not yet been tried.
         */
        private java.util.ArrayList<TrailPoint> privateNeighbors;

        public final java.util.ArrayList<TrailPoint> getNeighbors() {
            return privateNeighbors;
        }

        private void setNeighbors(java.util.ArrayList<TrailPoint> value) {
            privateNeighbors = value;
        }

        /**
         * Estimate of the distance to the end. Direct line if have no neighbors. Best neighbor.Distance + 1
         * if have neighbors. This value is bad if it's along a trail that failed.
         */
        private int privateDistance;

        public final int getDistance() {
            return privateDistance;
        }

        private void setDistance(int value) {
            privateDistance = value;
        }

        /**
         * The number of steps from the start to this tile.
         */
        private int privateCost;

        public final int getCost() {
            return privateCost;
        }

        public final void setCost(int value) {
            privateCost = value;
        }

        public TrailPoint(Point pt, Point end, int cost) {
            setMapTile(pt);
            setNeighbors(new java.util.ArrayList<TrailPoint>());
            setDistance(Math.abs(getMapTile().x - end.x) + Math.abs(getMapTile().y - end.y));
            setCost(cost);
        }

        public final void RecalculateDistance(Point mapTileCaller, int remainingSteps) {

            TRAP.trap(getDistance() == 0);
            // if no neighbors then this is in notEvaluated and so can't recalculate.
            if (getNeighbors().isEmpty()) {
                return;
            }

            int shortestDistance = 666;
            // if just 1 neighbor, then it's a dead end
            if (getNeighbors().size() == 1) {
                shortestDistance = DEAD_END;
            } else {
                for (TrailPoint neighborOn : getNeighbors()) {
                    if (shortestDistance > neighborOn.getDistance())
                        shortestDistance = neighborOn.getDistance();
                }
                // it's 1+ lowest neighbor value (unless a dead end)
                if (shortestDistance != DEAD_END) {
                    shortestDistance++;
                }
            }

            // no change, no need to recalc neighbors
            if (shortestDistance == getDistance()) {
                return;
            }

            // new value (could be longer or shorter)
            setDistance(shortestDistance);

            // if gone to far, no more recalculate
            if (remainingSteps-- < 0) {
                return;
            }

            //  Need to tell our neighbors - except the one that called us
//C# TO JAVA CONVERTER TODO TASK: There is no Java equivalent to LINQ queries:


            for (TrailPoint neighborOn : getNeighbors()) {
                if (neighborOn.getMapTile() != mapTileCaller)
                    neighborOn.RecalculateDistance(getMapTile(), remainingSteps);
            }

            // and we re-calc again because that could have changed our neighbor's values
            for (TrailPoint neighborOn : getNeighbors()) {
                if (shortestDistance > neighborOn.getDistance())
                    shortestDistance = neighborOn.getDistance();
            }
            // it's 1+ lowest neighbor value (unless a dead end)
            if (shortestDistance != DEAD_END) {
                shortestDistance++;
            }
            setDistance(shortestDistance);
        }

        @Override
        public String toString() {
            return String.format("Map=%1$s, Cost=%2$s, Distance=%3$s, Neighbors=%4$s", getMapTile(), getCost(), getDistance(), getNeighbors().size());
        }
    }
}