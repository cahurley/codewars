// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.

namespace Windwardopolis
{
	public interface IEngineCallback
	{

		/// <summary>
		/// Adds a message to the status window.
		/// </summary>
		/// <param name="message">The message to add.</param>
		void StatusMessage(string message);

		void ConnectionEstablished(string guid);

		void IncomingMessage(string guid, string message);

		void ConnectionLost(string guid);

	}
}
