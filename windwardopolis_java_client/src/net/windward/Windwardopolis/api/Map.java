package net.windward.Windwardopolis.api;


import java.awt.*;
import org.dom4j.Element;

public class Map
{
	public Map(Element elemMap, java.util.ArrayList<Company> companies)
	{
		int width = Integer.parseInt(elemMap.attributeValue("width"));
		int height = Integer.parseInt(elemMap.attributeValue("height"));
		setUnitsPerTile(Integer.parseInt(elemMap.attributeValue("units-tile")));

		setSquares(new MapSquare[width][]);
		for (int x = 0; x < width; x++)
		{
			getSquares()[x] = new MapSquare[height];
		}

		for (Object elemTile : elemMap.selectNodes("tile"))
		{
            Element elemSq = (Element) elemTile;
			int x = Integer.parseInt(elemSq.attributeValue("x"));
			int y = Integer.parseInt(elemSq.attributeValue("y"));
			getSquares()[x][y] = new MapSquare(elemSq);
		}

		for (Company cmpyOn : companies)
		{
			getSquares()[cmpyOn.getBusStop().x][cmpyOn.getBusStop().y].ctor(cmpyOn);
		}
	}

	/** 
	 The map squares. This is in the format [x][y].
	*/
	private MapSquare[][] privateSquares;
	public final MapSquare[][] getSquares()
	{
		return privateSquares;
	}
	protected final void setSquares(MapSquare[][] value)
	{
		privateSquares = value;
	}

	/** 
	 The number of map units in a tile. Some points are in map units and
	 some are in tile units.
	*/
	private int privateUnitsPerTile;
	public final int getUnitsPerTile()
	{
		return privateUnitsPerTile;
	}
	private void setUnitsPerTile(int value)
	{
		privateUnitsPerTile = value;
	}

	/** 
	 Convert from map units to tile units.
	 
	 @param ptMap Point in map units.
	 @return Point in tile units.
	*/
	public final Point MapToTile(Point ptMap)
	{
		return new Point(ptMap.x / getUnitsPerTile(), ptMap.y / getUnitsPerTile());
	}

	/** 
	 The width of the map. Units are squares.
	*/
	public final int getWidth()
	{
		return getSquares().length;
	}

	/** 
	 The height of the map. Units are squares.
	*/
	public final int getHeight()
	{
		return getSquares()[0].length;
	}

	/** 
	 Returns the requested point or null if off the map.
	 
	 @param pt
	 @return 
	*/
	public final MapSquare SquareOrDefault(Point pt)
	{
		if ((pt.x < 0) || (pt.y < 0) || (pt.x >= getWidth()) || (pt.y >= getHeight()))
		{
			return null;
		}
		return getSquares()[pt.x][pt.y];
	}
}