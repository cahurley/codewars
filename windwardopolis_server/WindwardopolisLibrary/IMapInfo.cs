// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.

using System.Collections.Generic;
using WindwardopolisLibrary.map;
using WindwardopolisLibrary.units;

namespace WindwardopolisLibrary
{
	public interface IMapInfo
	{

		/// <summary>
		/// The game map.
		/// </summary>
		GameMap Map { get; }

		/// <summary>
		/// The Player's Limos to display on the map.
		/// </summary>
		List<Player> Players { get; }

		/// <summary>
		/// The number of pixels per tile. This will range from 48 to 6.
		/// </summary>
		int PixelsPerTile { get; }
	}
}
