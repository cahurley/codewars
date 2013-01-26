package net.windward.Windwardopolis.AI;

import net.windward.Windwardopolis.api.Passenger;

import java.awt.*;

public class PlayerAIBase
{
	public static interface PlayerOrdersEvent
	{
		void invoke(String order, java.util.ArrayList<Point> path, java.util.ArrayList<Passenger> pickUp);
	}

	public enum STATUS
	{
		/** 
		 Called ever N ticks to update the AI with the game status.
		*/
		UPDATE,
		/** 
		 The car has no path.
		*/
		NO_PATH,
		/** 
		 The passenger was abandoned, no passenger was picked up.
		*/
		PASSENGER_ABANDONED,
		/** 
		 The passenger was delivered, no passenger was picked up.
		*/
		PASSENGER_DELIVERED,
		/** 
		 The passenger was delivered or abandoned, a new passenger was picked up.
		*/
		PASSENGER_DELIVERED_AND_PICKED_UP,
		/** 
		 The passenger refused to exit at the bus stop because an enemy was there.
		*/
		PASSENGER_REFUSED,
		/** 
		 A passenger was picked up. There was no passenger to deliver.
		*/
		PASSENGER_PICKED_UP,
		/** 
		 At a bus stop, nothing happened (no drop off, no pick up).
		*/
		PASSENGER_NO_ACTION;

		public int getValue()
		{
			return this.ordinal();
		}

		public static STATUS forValue(int value)
		{
			return values()[value];
		}
	}
}