using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	class Program {
		private static int port = TcpConnection.DEFAULT_PORT;
		private static string ip = "127.0.0.1";

		static void Main(string[] args) {
			List<Thread> threads = new List<Thread>();

			IPAddress ipAddress = IPAddress.Parse(ip);
			TcpListener listener = new TcpListener(ipAddress, port);
			listener.Start();
			Console.WriteLine("Server listening on: " + ip + ":" + port);

			while (true) {
				Console.WriteLine("Server is waiting for client...");
				TcpClient client = listener.AcceptTcpClient();
				TcpConnection gameClient = new TcpConnection(client, false);
				Console.WriteLine("Client connected - ip: " + gameClient.RemoteIpAddress + " port: " + gameClient.RemotePortNumber);

				Thread t = new Thread(new ParameterizedThreadStart(Test));
				threads.Add(t);
				t.Start(gameClient);
			}
		}

		private static void Test(object clientObj) {
			try {
				TcpConnection client = (TcpConnection)clientObj;

				Console.WriteLine("Trying to receive...");
				GamePacket packet = client.GetReceivedPacket();
				Console.WriteLine("Received packet: " + packet.OperationType);

				Console.WriteLine("Sleeping some time...");
				Thread.Sleep(3000);

				Console.WriteLine("Trying to send...");
				string msg = "server test msg";
				client.Send(new GamePacket(OperationType.LOGIN, msg));

				/*Console.WriteLine("Trying to receive with timeout...");
				try {
					client.GetReceivedPacket(2000, 1);
				} catch (ReceiveTimeoutException e) {
					Console.WriteLine("Failed to receive with timeout. Disconnected player number: " + e.PlayerNumber);
				}*/

				Console.WriteLine("Waiting for client to send DISCONNECT...");
				packet = client.GetReceivedPacket();
				if (packet.OperationType == OperationType.DISCONNECT) {
					Console.WriteLine("received disconnect");
					client.Disconnect();
				}
				else Console.WriteLine("not ok");

				Console.WriteLine("All OK! Closing...");
				//Thread.Sleep(1000);
				//client.Dispose();
			} catch(ConnectionEndedException ex) {
				Console.WriteLine("Exception: " + ex.Message);
			}
			Console.WriteLine("Test end");
		}

	}

}
