using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace GAME_connection {
	/// <summary>
	/// Class used to send and receive GamePackets between server and client.
	/// <para>Should be used instead of TcpClient and its NetworkStream, if you need to have same connection in multiple places PASS instance of this object, DON'T create new object.</para>
	/// <para>All send and receive operations are synchronized separately (one lock for send and one for receive)</para>
	/// <para>Receive operations are preformed on separate thread to allow constant connection testing</para>
	/// </summary>
	public class TcpConnection : IDisposable {
		public static readonly int DEFAULT_PORT = 100001;

		private static readonly int pollingIntervalMilis = 100;
		private static readonly int connectionTestIntervalMilis = 2000;

		private TcpClient tcpClient;
		private NetworkStream netStream;
		private IFormatter serializer;
		private string remoteIpAddress;
		private int remotePortNumber;

		private readonly object sendLock = new object();
		private readonly object receiveLock = new object();
		private readonly object queueLock = new object();
		private readonly object keepReceivingLock = new object();
		private readonly object keepTestingConnectionLock = new object();

		private Queue<GamePacket> receivedPackets = new Queue<GamePacket>();
		private Thread receiver;
		private Thread connectionTester;	//thread to periodically test connection - should be used on client
		private bool keepReceiving;
		private bool keepTestingConnection;

		public TcpConnection(TcpClient client, bool isClient) {
			this.TcpClient = client;
			this.NetStream = client.GetStream();

			IPEndPoint ipData = client.Client.RemoteEndPoint as IPEndPoint;
			this.RemoteIpAddress = ipData.Address.ToString();
			this.RemotePortNumber = ipData.Port;
			this.serializer = new BinaryFormatter();

			keepReceiving = true;
			receiver = new Thread(new ThreadStart(DoReceiving));
			receiver.Start();

			if(isClient) {
				keepTestingConnection = true;
				connectionTester = new Thread(new ThreadStart(DoTestConnection));
				connectionTester.Start();
			}
		}

		#region Send/Write
		/// <summary>
		/// Used to send <see cref="GamePacket"/> to client specified in constructor (TcpClient)
		/// </summary>
		/// <param name="packet">instance of <see cref="GamePacket"/> object to send</param>
		public void Send(GamePacket packet) {
			lock (sendLock) {
				serializer.Serialize(netStream, packet);
			}
		}

		/// <summary>
		/// Used to send <see cref="GamePacket"/> to client specified in constructor (TcpClient), with timeout in miliseconds
		/// </summary>
		/// <param name="packet">instance of <see cref="GamePacket"/> object to send</param>
		/// <param name="timeout">timeout in miliseconds for send operation</param>
		public void SendWithTimeout(GamePacket packet, long timeout) {
			lock (sendLock) {
				Console.WriteLine("Send z timeoutem");
				Send(packet);
			}
		}
		#endregion

		#region Receive/Read
		/// <summary>
		/// Gets oldest unprocessed received packet
		/// </summary>
		/// <returns></returns>
		public GamePacket GetReceivedPacket() {
			while(true) {
				if (QueueCount > 0) return receivedPackets.Dequeue();
				Thread.Sleep(pollingIntervalMilis);
			}
		}

		/// <summary>
		/// Gets oldest unprocessed received packet with timeout in miliseconds, on timeout throws exception
		/// </summary>
		/// <param name="timeoutMilis">timeout in miliseconds for receive operation</param>
		/// <returns></returns>
		public GamePacket GetReceivedPacket(int timeoutMilis) {
			int elapsedTime = 0;
			while(elapsedTime < timeoutMilis) {
				if (QueueCount > 0) return receivedPackets.Dequeue();
				Thread.Sleep(pollingIntervalMilis);
				elapsedTime += pollingIntervalMilis;
			}
			throw new Exception("Timeout of " + timeoutMilis + " ms happened while receiving");
		}

		private int QueueCount {
			get {
				int queueCount;
				lock (queueLock) {
					queueCount = receivedPackets.Count;
				}
				return queueCount;
			}
		}
		#endregion

		#region Receiver
		/// <summary>
		/// Main receiving thread, receives <see cref="GamePacket"/>s and puts them on the queue, ignores <see cref="OperationType.CONNECTION_TEST"/> packets
		/// </summary>
		private void DoReceiving() {
			while(KeepReceiving) {
				GamePacket receivedPacket = Receive();
				if (receivedPacket.OperationType == OperationType.CONNECTION_TEST) Console.WriteLine("CONNECTION_TEST received");
				else {
					lock(queueLock) {
						receivedPackets.Enqueue(receivedPacket);
					}
				}
				//than block on another read operation
			}
		}

		/// <summary>
		/// Used to receive <see cref="GamePacket"/>s
		/// </summary>
		/// <returns>received (deserialized) GamePacket</returns>
		private GamePacket Receive() {
			lock (receiveLock) {
				return (GamePacket)serializer.Deserialize(netStream);
			}
		}

		private bool KeepReceiving {
			get {
				bool localKeepReceiving;
				lock (keepReceivingLock) {
					localKeepReceiving = keepReceiving;
				}
				return localKeepReceiving;
			}
			set {
				lock (keepReceivingLock) {
					keepReceiving = value;
				}
			}
		}
		#endregion

		#region Connection Tester
		/// <summary>
		/// Periodically tests connection, used on client
		/// </summary>
		private void DoTestConnection() {
			while(KeepTestingConnection) {
				Send(GamePacket.CreateConnectionTestPacket());
				Thread.Sleep(connectionTestIntervalMilis);
			}
		}

		private bool KeepTestingConnection {
			get {
				bool localKeepTestingConnection;
				lock (keepTestingConnectionLock) {
					localKeepTestingConnection = keepTestingConnection;
				}
				return localKeepTestingConnection;
			}
			set {
				lock (keepTestingConnectionLock) {
					keepTestingConnection = value;
				}
			}
		}
		#endregion

		public TcpClient TcpClient { get => tcpClient; set => tcpClient = value; }
		public NetworkStream NetStream { get => netStream; set => netStream = value; }
		public string RemoteIpAddress { get => remoteIpAddress; set => remoteIpAddress = value; }
		public int RemotePortNumber { get => remotePortNumber; set => remotePortNumber = value; }

		#region IDisposable
		public void Dispose() {
			KeepTestingConnection = false;
			connectionTester.Join();

			KeepReceiving = false;
			receiver.Join();

			TcpClient.Dispose();
		}
		#endregion

	}

}
