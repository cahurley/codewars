package net.windward.Windwardopolis.api;

import java.awt.*;

public class Limo
{
	/** 
	 Create the object.
	 @param mapPosition The location in map units of the center of the vehicle.
	 @param angle The Angle this unit is facing.
	*/
	public Limo(Point mapPosition, int angle)
	{
		setMapPosition(mapPosition);
		setAngle(angle);
		setPath(new java.util.ArrayList<Point>());
	}

	/** 
	 The location in map units of the center of the vehicle.
	*/
	private Point privateMapPosition;
	public final Point getMapPosition()
	{
		return privateMapPosition;
	}
	public final void setMapPosition(Point value)
	{
		privateMapPosition = value;
	}

	/** 
	 0 .. 359 The Angle this unit is facing. 0 is North and 90 is East.
	*/
	private int privateAngle;
	public final int getAngle()
	{
		return privateAngle;
	}
	public final void setAngle(int value)
	{
		privateAngle = value;
	}

	/** 
	 The passenger in this limo. null if no passenger.
	*/
	private Passenger privatePassenger;
	public final Passenger getPassenger()
	{
		return privatePassenger;
	}
	public final void setPassenger(Passenger value)
	{
		privatePassenger = value;
	}

	/** 
	 Only set for the AI's own Limo - the number of tiles remaining in the Limo's path.
	 This may be wrong after movement as all we get is a count. This is updated with the
	 most recent list sent to the server.
	*/
	private java.util.ArrayList<Point> privatePath;
	public final java.util.ArrayList<Point> getPath()
	{
		return privatePath;
	}
	private void setPath(java.util.ArrayList<Point> value)
	{
		privatePath = value;
	}

	@Override
	public String toString()
	{
		if (getPassenger() != null)
		{
			return String.format("%1$s:%2$s; Passenger:%3$s; Dest:%4$s; PathLength:%5$s", getMapPosition(), getAngle(), getPassenger() == null ? "{none}" : getPassenger().getName(), getPassenger().getDestination(), getPath().size());
		}
		return String.format("%1$s:%2$s; Passenger:%3$s", getMapPosition(), getAngle(), getPassenger() == null ? "{none}" : getPassenger().getName());
	}
}