package net.windward.Windwardopolis.AI;

import java.awt.*;
import java.util.ArrayList;
import java.util.HashMap;

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

    public static int last_cost;

    private static HashMap<Pair<Point, Point>, Pair<ArrayList<Point>, Integer> > paths = new HashMap<Pair<Point, Point>, Pair<ArrayList<Point>, Integer>>();

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
        java.util.HashMap<Point, TrailPoint> seen_nodes = new java.util.HashMap<Point, TrailPoint>();


        TrailPoint tpOn = new TrailPoint(start, end, 0);

        java.util.ArrayList<TrailPoint> queue = new ArrayList<TrailPoint>();

        queue.add(tpOn);
        seen_nodes.put(start, tpOn);

        java.util.HashMap<TrailPoint, TrailPoint> previous = new HashMap<TrailPoint, TrailPoint>();

        if(paths.containsKey(new Pair<Point, Point>(start, end))){
            last_cost = paths.get(new Pair<Point, Point>(start, end)).getRight();
            return paths.get(new Pair<Point, Point>(start, end)).getLeft();
        }

        while(true) {

            //otherwise calculate it
            TrailPoint trail_temp = queue.remove(0);

            for (Point ptOffset : offsets) {
                Point pt = new Point(trail_temp.getMapTile().x + ptOffset.x, trail_temp.getMapTile().y + ptOffset.y);
                MapSquare square = map.SquareOrDefault(pt);
                // off the map or not a road/bus stop
                if ((square == null) || (!square.getIsDriveable())) {
                    continue;
                }

                if (!seen_nodes.containsKey(pt)) {

                    TrailPoint tp= new TrailPoint(pt, end, trail_temp.getCost()+1);
                    queue.add(tp);
                    seen_nodes.put(pt, tp);
                    //continue;
                    previous.put(tp, trail_temp);
                    if (pt.equals(end)) {
                        last_cost = tp.getCost();
                        //construct path here
                        ArrayList<Point> path = new ArrayList<Point>();
                        TrailPoint prev = previous.get(tp);
                        path.add(tp.getMapTile());
                        int cost = 0;

                        do {
                            cost++;
                            path.add(0, prev.getMapTile());
                            paths.put(new Pair<Point, Point>(prev.getMapTile(),end), new Pair((ArrayList<Point>)path.clone(), cost));
                            //System.out.println(seen_nodes.size());
                            prev = previous.get(prev);
                        } while ((!prev.getMapTile().equals(start)));
                        last_cost = cost;
                        return path;
                    }
                }

            }

        }
    }

    private static class Pair<L,R> {

        private final L left;
        private final R right;

        public Pair(L left, R right) {
            this.left = left;
            this.right = right;
        }

        public L getLeft() { return left; }
        public R getRight() { return right; }

        @Override
        public int hashCode() { return left.hashCode() ^ right.hashCode(); }

        @Override
        public boolean equals(Object o) {
            if (o == null) return false;
            if (!(o instanceof Pair)) return false;
            Pair pairo = (Pair) o;
            return this.left.equals(pairo.getLeft()) &&
                    this.right.equals(pairo.getRight());
        }

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