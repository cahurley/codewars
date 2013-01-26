// code primarily from http://forum.codecall.net/classes-code-snippets/26089-c-example-high-performance-tcp-server-client.html
// Edited by Windward Studios, Inc. (www.windward.net). No copyright claimed by Windward on changes.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WindwardopolisLibrary;

namespace Windwardopolis
{
	public class TcpServer
	{
		private const int BUFFER_SIZE = 65536 * 4;
		private const int port = 1707;
		private Socket serverSocket;
		private IEngineCallback engineCallback;
		private readonly List<ConnectionInfo> connections = new List<ConnectionInfo>();

		private class ConnectionInfo
		{
			public ConnectionInfo()
			{
				Guid = System.Guid.NewGuid().ToString();
				messageBuffer = new FifoByteBuffer(BUFFER_SIZE);
			}

			public string Guid { get; private set; }
			public Socket Socket { get; set; }
			public byte[] SocketReadBuffer { get; set; }

			// we store up a message here.
			// length: 0  => have less than 4 bytes in (including 0)
			//         >0 => have part/all of a message - length bytes have been removed from the buffer
			private readonly FifoByteBuffer messageBuffer;
			private int messageLength;

			public void ReceivedData(int bytesRead)
			{
				messageBuffer.Write(SocketReadBuffer, 0, bytesRead);
			}

			public bool HasMessage
			{
				get
				{
					// we may need to extract the length
					if (messageLength == 0 && messageBuffer.Count >= 4)
					{
						messageLength = BitConverter.ToInt32(messageBuffer.Read(4), 0);
//						messageBuffer.Grow(messageLength);
					}
					return messageLength > 0 && messageLength <= messageBuffer.Count;
				}
			}

			public string Message
			{
				get
				{
					string rtn = Encoding.UTF8.GetString(messageBuffer.Read(messageLength));
					messageLength = 0;
					return rtn;
				}
			}
		}

		private void SetupServerSocket()
		{
			IPEndPoint myEndpoint = new IPEndPoint(IPAddress.Any, port);

			// Create the socket, bind it, and start listening
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
			               	{Blocking = false, ReceiveBufferSize = BUFFER_SIZE, SendBufferSize = BUFFER_SIZE};

			serverSocket.Bind(myEndpoint);
			serverSocket.Listen((int) SocketOptionName.MaxConnections);
		}

		public void Start(IEngineCallback callback)
		{

			engineCallback = callback;

			try
			{
				SetupServerSocket();
				for (int i = 0; i < 10; i++)
					serverSocket.BeginAccept(AcceptCallback, serverSocket);
				engineCallback.StatusMessage("TCP server started");
			}
			catch (Exception e)
			{
				engineCallback.StatusMessage(string.Format("Failed. Exception: {0}", e));
			}
		}

		private void AcceptCallback(IAsyncResult result)
		{

			ConnectionInfo connection = new ConnectionInfo();
			try
			{
				// Finish Accept
				Socket s = (Socket) result.AsyncState;
				connection.Socket = s.EndAccept(result);
				connection.Socket.Blocking = false;
				connection.SocketReadBuffer = new byte[BUFFER_SIZE];
				lock (connections) 
					connections.Add(connection);

				// the below can throw an exception if there's a network issue or the join comes in wrong during the call.
				try
				{
					// Start Receive
					connection.Socket.BeginReceive(connection.SocketReadBuffer, 0, connection.SocketReadBuffer.Length, SocketFlags.None, ReceiveCallback, connection);

					Trace.WriteLine(string.Format("New connection from {0}", connection.Socket.RemoteEndPoint));
					engineCallback.ConnectionEstablished(connection.Guid);
				}
				catch (Exception ex)
				{
					Trap.trap();
					// this connection is dead, tell the engine.
					Trace.WriteLine(string.Format("ERROR: AcceptCallback - Exception: {0}", ex));
					engineCallback.StatusMessage(string.Format("AcceptCallback - Exception: {0}", ex));
					string guid = connection.Guid;
					CloseConnection(connection);
					engineCallback.ConnectionLost(guid);
					// continue down so we still start accepting again.
				}

				// Start new Accept
				serverSocket.BeginAccept(AcceptCallback, result.AsyncState);
			}
			catch (SocketException exc)
			{
				CloseConnection(connection);
				Trace.WriteLine(string.Format("ERROR: AcceptCallback - Socket exception:{0} ", exc.SocketErrorCode));
				engineCallback.StatusMessage(string.Format("AcceptCallback - Socket exception:{0} ", exc.SocketErrorCode));
			}
			catch (Exception exc)
			{
				CloseConnection(connection);
				Trace.WriteLine(string.Format("ERROR: AcceptCallback - Exception: {0}", exc));
				engineCallback.StatusMessage(string.Format("AcceptCallback - Exception: {0}", exc));
			}
		}

		private void ReceiveCallback(IAsyncResult result)
		{

			ConnectionInfo connection = (ConnectionInfo) result.AsyncState;
			// might be closed
			if (! connection.Socket.Connected)
			{
				string guid = connection.Guid;
				CloseConnection(connection);
				Trace.WriteLine(string.Format("Connection to {0} closed", guid));
				engineCallback.StatusMessage(string.Format("Connection to {0} closed", guid));
				engineCallback.ConnectionLost(guid);
				return;
			}

			try
			{
				int bytesRead = connection.Socket.EndReceive(result);
				if (0 == bytesRead)
				{
					string guid = connection.Guid;
					CloseConnection(connection);
					engineCallback.ConnectionLost(guid);
					return;
				}

				lock (connection)
					connection.ReceivedData(bytesRead);

				// only way we have multiple messages is an error on the client side - but that could happen.
				while (true)
				{
					string message;
					lock (connection)
					{
						if (!connection.HasMessage)
							break;
						message = connection.Message;
					}
					engineCallback.IncomingMessage(connection.Guid, message);
				}

				connection.Socket.BeginReceive(connection.SocketReadBuffer, 0, connection.SocketReadBuffer.Length, SocketFlags.None, ReceiveCallback, connection);
			}
			catch (SocketException exc)
			{
				string guid = connection.Guid;
				string ipAddr = GetIpAddress(guid);
				CloseConnection(connection);
				string msg = (exc.SocketErrorCode == SocketError.ConnectionReset
												? string.Format("Connection {0} closed", ipAddr)
				                             	: string.Format("AcceptCallback - Socket exception:{0} ", exc.SocketErrorCode));
				Trace.WriteLine("ERROR: " + msg);
				engineCallback.StatusMessage(msg);
				engineCallback.ConnectionLost(guid);
			}
			catch (Exception exc)
			{
				string guid = connection.Guid;
				CloseConnection(connection);
				Trace.WriteLine(string.Format("ERROR: AcceptCallback - Exception: {0}", exc));
				engineCallback.StatusMessage(string.Format("AcceptCallback - Exception: {0}", exc));
				engineCallback.ConnectionLost(guid);
			}
		}

		public string GetIpAddress(string guid)
		{
			ConnectionInfo conn = connections.Find(cn => cn.Guid == guid);
			return conn == null ? null : conn.Socket.RemoteEndPoint.ToString();
		}

		public void SendMessage(string guid, string msg)
		{
			ConnectionInfo conn = connections.Find(cn => cn.Guid == guid);
			if (conn == null)
			{
				engineCallback.ConnectionLost(guid);
				return;
			}

			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(msg);
				byte[] length = BitConverter.GetBytes(bytes.Length);
				conn.Socket.Send(length, length.Length, SocketFlags.None);
				for (int offset = 0; offset < bytes.Length; offset += BUFFER_SIZE)
					conn.Socket.Send(bytes, offset, Math.Min(bytes.Length - offset, BUFFER_SIZE), SocketFlags.None);
			}
			catch (Exception exc)
			{
				// we close the connection as that's the safest way to have it restart and get sync'ed on messages.
				CloseConnection(conn);
				Trace.WriteLine(string.Format("ERROR: SendMessage error, reseting connection - Exception: {0}", exc));
				engineCallback.StatusMessage(string.Format("SendMessage error, reseting connection - Exception: {0}", exc));
				engineCallback.ConnectionLost(guid);
			}
		}

		public void CloseConnection(string guid)
		{
			ConnectionInfo conn = connections.Find(cn => cn.Guid == guid);
			if (conn != null)
				CloseConnection(conn);
		}

		private void CloseConnection(ConnectionInfo ci)
		{
			if (ci.Socket.Connected)
				ci.Socket.Close();
			lock (connections)
				connections.Remove(ci);
		}

		public void CloseAllConnections()
		{
			lock (connections)
			{
				for (int i = connections.Count - 1; i >= 0; i--)
					CloseConnection(connections[i]);
			}
			Thread.Sleep(500);
		}
	}
}