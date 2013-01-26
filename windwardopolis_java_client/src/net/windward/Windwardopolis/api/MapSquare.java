package net.windward.Windwardopolis.api;

import org.dom4j.Element;
import org.dom4j.Attribute;

import java.util.StringTokenizer;

public class MapSquare
{
	/** 
	 The direction of the road. Do not change these numbers, they are used as an index into an array.
	*/
	public enum DIRECTION
	{
		/** 
		 Road running north/south.
		*/
		NORTH_SOUTH(0),

		/** 
		 Road running east/west.
		*/
		EAST_WEST(1),

		/** 
		 A 4-way intersection.
		*/
		INTERSECTION(2),

		/** 
		 A north/south road ended on the north side.
		*/
		NORTH_UTURN(3),

		/** 
		 An east/west road ended on the east side.
		*/
		EAST_UTURN(4),

		/** 
		 A north/south road ended on the south side.
		*/
		SOUTH_UTURN(5),

		/** 
		 An east/west road ended on the west side.
		*/
		WEST_UTURN(6),

		/** 
		 A T junction where the | of the T is entering from the north.
		*/
		T_NORTH(7),

		/** 
		 A T junction where the | of the T is entering from the east.
		*/
		T_EAST(8),

		/** 
		 A T junction where the | of the T is entering from the south.
		*/
		T_SOUTH(9),

		/** 
		 A T junction where the | of the T is entering from the west.
		*/
		T_WEST(10),

		/** 
		 A curve entered northward and exited eastward (or vice-versa).
		*/
		CURVE_NE(11),

		/** 
		 A curve entered northward and exited westward (or vice-versa).
		*/
		CURVE_NW(12),

		/** 
		 A curve entered southward and exited eastward (or vice-versa).
		*/
		CURVE_SE(13),

		/** 
		 A curve entered southward and exited westward (or vice-versa).
		*/
		CURVE_SW(14);

		private int intValue;
		private static java.util.HashMap<Integer, DIRECTION> mappings;
		private static java.util.HashMap<Integer, DIRECTION> getMappings()
		{
			if (mappings == null)
			{
				synchronized (DIRECTION.class)
				{
					if (mappings == null)
					{
						mappings = new java.util.HashMap<Integer, DIRECTION>();
					}
				}
			}
			return mappings;
		}

		private DIRECTION(int value)
		{
			intValue = value;
			getMappings().put(value, this);
		}

		public int getValue()
		{
			return intValue;
		}

		public static DIRECTION forValue(int value)
		{
			return getMappings().get(value);
		}
	}

	/** 
	 What type of square it is.
	*/
	public enum TYPE
	{
		/** 
		 Park. Nothing on this, does nothing, cannot be driven on.
		*/
		PARK,

		/** 
		 A road. The road DIRECTION determines which way cars can travel on the road.
		*/
		ROAD,

		/** 
		 A company's bus stop. This is where passengers are loaded and unloaded.
		*/
		BUS_STOP,

		/** 
		 Company building. Nothing on this, does nothing, cannot be driven on.
		*/
		COMPANY;

		public int getValue()
		{
			return this.ordinal();
		}

		public static TYPE forValue(int value)
		{
			return values()[value];
		}
	}


    public enum  STOP_SIGNS
    {
        /// <summary>
        /// No stop signs or signals.
        /// </summary>
        NONE,
        /// <summary>
        /// A stop entering from the North side.
        /// </summary>
        STOP_NORTH,
        /// <summary>
        /// A stop entering from the East side.
        /// </summary>
        STOP_EAST,
        /// <summary>
        /// A stop entering from the South side.
        /// </summary>
        STOP_SOUTH,
        /// <summary>
        /// A stop entering from the West side.
        /// </summary>
        STOP_WEST;

        public int getValue()
      		{
      			return this.ordinal();
      		}

      		public static STOP_SIGNS forValue(int value)
      		{
      			return values()[value];
      		}
    }

    /**
   	 Which side(s) of the square have a wall. A square can have multiple walls. Note, couldn't figure out how to handle
   	 to/from string for bitmask enums.
   	*/
   	public static final int NONE = 0;
   	public static final int STOP_NORTH = 0x01;
   	public static final int STOP_EAST = 0x02;
   	public static final int STOP_SOUTH = 0x04;
   	public static final int STOP_WEST = 0x08;
    private static final String [] STOPNames = {"STOP_NORTH", "STOP_EAST", "STOP_SOUTH", "STOP_WEST"};

   	public static int parseSTOPs(String STOP) {

   		int rtn = 0;
   		StringTokenizer tok = new StringTokenizer(STOP, ",");
   		while (tok.hasMoreTokens()) {
   			String item = tok.nextToken().trim();
   			int bitMask = 0x01;
   			for (int ind=0; ind<STOPNames.length; ind++, bitMask <<= 1)
   				if (STOPNames[ind].equals(item)) {
   					rtn |= bitMask;
   					break;
   				}
   		}
   		return rtn;
   	}

	public MapSquare(Element elemTile)
	{
		setType(TYPE.valueOf(elemTile.attributeValue("type")));
		if (getIsDriveable())
		{
			setDirection(DIRECTION.valueOf(elemTile.attributeValue("direction")));
			Attribute attr = elemTile.attribute("stop-sign");
            String test = elemTile.attributeValue("stop-sign");
            if(test!=null)
			    setStopSigns(parseSTOPs(test));
			attr = elemTile.attribute("signal");
			setSignal(attr != null && attr.getValue().toLowerCase().equals("true"));
		}
	}

	public final void ctor(Company company)
	{
		setCompany(company);
	}

	/** 
	 Settings for stop signs in this square. NONE for none.
	*/
	private int privateStopSigns = NONE;
	public final int getStopSigns()
	{
		return privateStopSigns;
	}
	private void setStopSigns(int value)
	{
		privateStopSigns = value;
	}

	/** 
	 The type of square.
	*/
	private boolean privateSignal;
	public final boolean getSignal()
	{
		return privateSignal;
	}
	private void setSignal(boolean value)
	{
		privateSignal = value;
	}

	/** 
	 The type of square.
	*/
	private TYPE privateType = TYPE.values()[0];
	public final TYPE getType()
	{
		return privateType;
	}
	private void setType(TYPE value)
	{
		privateType = value;
	}

	/** 
	 The company for this tile. Only set if a BUS_STOP.
	*/
	private Company privateCompany;
	public final Company getCompany()
	{
		return privateCompany;
	}
	private void setCompany(Company value)
	{
		privateCompany = value;
	}

	/** 
	 True if the square can be driven on (ROAD or BUS_STOP).
	*/
	public final boolean getIsDriveable()
	{
		return getType() == TYPE.ROAD || getType() == TYPE.BUS_STOP;
	}

	/** 
	 The direction of the road. This is only used for ROAD and BUS_STOP tiles.
	*/
	private DIRECTION privateDirection = DIRECTION.values()[0];
	public final DIRECTION getDirection()
	{
		return privateDirection;
	}
	private void setDirection(DIRECTION value)
	{
		privateDirection = value;
	}

}