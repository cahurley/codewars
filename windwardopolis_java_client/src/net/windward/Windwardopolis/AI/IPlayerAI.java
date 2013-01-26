package net.windward.Windwardopolis.AI;


import net.windward.Windwardopolis.api.Company;
import net.windward.Windwardopolis.api.Map;
import net.windward.Windwardopolis.api.Passenger;
import net.windward.Windwardopolis.api.Player;

public interface IPlayerAI
{

	/** 
	 Called when your robot must be placed on the board. This is called at the start of the game.
	 
	 @param map The game map.
	 @param me My Player object. This is also in the players list.
	 @param players All players (including you).
	 @param companies The companies on the map.
	 @param passengers The passengers that need a lift.
	 @param ordersEvent Method to call to send orders to the server.
	*/
	void Setup(Map map, Player me, java.util.ArrayList<Player> players, java.util.ArrayList<Company> companies, java.util.ArrayList<Passenger> passengers, PlayerAIBase.PlayerOrdersEvent ordersEvent);

	/** 
	 Called to send an update message to this A.I. We do NOT have to reply to it.
	 
	 @param status The status message.
	 @param plyrStatus The status of my player.
	 @param players The status of all players.
	 @param passengers The status of all passengers.
	*/
	void GameStatus(PlayerAIBase.STATUS status, Player plyrStatus, java.util.ArrayList<Player> players, java.util.ArrayList<Passenger> passengers);
}