
using System.Drawing;

namespace WindwardopolisLibrary.ai_interface
{
	public class LimoInfo
	{
		/// <summary>
		/// The tile in the map the center of this unit is on.
		/// </summary>
		public Point TilePosition { get; set; }

		/// <summary>
		/// The tile in the map the center of this unit will move into next. This can be changed if a path change 
		/// message is received by the engine before the vehicle passes into this tile.
		/// </summary>
		public Point NextTilePosition { get; set; }

		/// <summary>
		/// The passenger in this limo. null if no passenger.
		/// </summary>
		public PassengerInfo Passenger { get; set; }

		/// <summary>
		/// Only set for the AI's own Limo - the next bus stop in the Limo's path.
		/// </summary>
		public Point Destination { get; set; }

		/// <summary>
		/// Only set for the AI's own Limo - the number of tiles remaining in the Limo's path.
		/// </summary>
		public int PathLength { get; set; }
	}
}
