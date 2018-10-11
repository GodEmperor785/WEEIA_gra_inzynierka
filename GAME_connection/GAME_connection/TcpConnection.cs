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
	/// </summary>
	public class TcpConnection {
		private TcpClient tcpClient;
		private NetworkStream netStream;
		private IPEndPoint ipData;
		private readonly IFormatter serializer;
		private readonly object sendLock = new object();
		private readonly object receiveLock = new object();

		public TcpConnection(TcpClient client) {
			this.TcpClient = client;
			this.NetStream = client.GetStream();
			this.IpData = client.Client.RemoteEndPoint as IPEndPoint;    //property Address and Port
			this.serializer = new BinaryFormatter();
		}

		/// <summary>
		/// Used to send GamePacket to client specified in constructor (TcpClient)
		/// </summary>
		/// <param name="packet">instance of GamePacket object to send</param>
		public void Send(GamePacket packet) {
			lock (sendLock) {
				serializer.Serialize(netStream, packet);
			}
		}

		/// <summary>
		/// Used to send GamePacket to client specified in constructor (TcpClient), with timeout
		/// </summary>
		/// <param name="packet">instance of GamePacket object to send</param>
		/// <param name="timeout">timeout for send operation</param>
		public void SendWithTimeout(GamePacket packet, long timeout) {
			lock (sendLock) {
				Console.WriteLine("Send z timeoutem");
				Send(packet);
			}
		}

		/// <summary>
		/// Used to receive incoming GamePacket
		/// </summary>
		/// <returns>received (deserialized) GamePacket</returns>
		public GamePacket Receive() {
			lock (receiveLock) {
				return (GamePacket)serializer.Deserialize(netStream);
			}
		}

		/// <summary>
		/// Used to receive incoming GamePacket, with timeout
		/// </summary>
		/// <param name="timeout">timeout for receive operation</param>
		/// <returns>received (deserialized) GamePacke</returns>
		public GamePacket ReceiveWithTimeout(long timeout) {
			lock (receiveLock) {
				Console.WriteLine("Receive z timeoutem");
				return Receive();
			}
		}

		public TcpClient TcpClient { get => tcpClient; set => tcpClient = value; }
		public NetworkStream NetStream { get => netStream; set => netStream = value; }
		public IPEndPoint IpData { get => ipData; set => ipData = value; }

		public string RemoteIpAddress { 
			get { return IpData.Address.ToString(); }
		}

		public int RemoteIpPort {
			get { return IpData.Port; }
		}
	}

}
