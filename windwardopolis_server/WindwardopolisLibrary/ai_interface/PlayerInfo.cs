using System.Collections.Generic;
using WindwardopolisLibrary.units;

namespace WindwardopolisLibrary.ai_interface
{
	/// <summary>
	/// The interface for players. Implemented by both the internal AIs and the communicator with the remote AIs.
	/// </summary>
	public class PlayerInfo
	{

		public PlayerInfo(Player src)
		{
			Trap.trap();
			Guid = src.Guid;
		}

		/// <summary>
		/// The unique identifier for this player. This will remain constant for the length of the game (while the Player objects passed will
		/// change on every call).
		/// </summary>
		public string Guid { get; private set; }

		/// <summary>
		/// Who to pick up at the next bus stop. Can be empty and can also only list people not there.
		/// </summary>
		public List<PassengerInfo> PickUp { get; private set; }

		/// <summary>
		/// The passengers delivered - this game.
		/// </summary>
		public List<PassengerInfo> PassengersDelivered { get; private set; }

		/// <summary>
		/// The player's limo.
		/// </summary>
		public LimoInfo Limo { get; private set; }
	}
}
