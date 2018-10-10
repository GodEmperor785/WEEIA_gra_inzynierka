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
	/// class used to send and receive GamePackets between server and client
	/// </summary>
	public class TcpConnection {
		private TcpClient tcpClient;
		private NetworkStream netStream;
		private IPEndPoint ipData;
		private readonly IFormatter serializer;

		public TcpConnection(TcpClient client) {
			this.TcpClient = client;
			this.NetStream = client.GetStream();
			IpData = client.Client.RemoteEndPoint as IPEndPoint;    //property Address i Port
			serializer = new BinaryFormatter();
		}

		/// <summary>
		/// Used to send GamePacket to client specified in constructor (TcpClient)
		/// </summary>
		/// <param name="packet">instance of GamePacket object to send</param>
		public void Send(GamePacket packet) {
			serializer.Serialize(netStream, packet);
		}

		/// <summary>
		/// Used to receive incoming GamePacket
		/// </summary>
		/// <returns>received (deserialized) GamePacket</returns>
		public GamePacket Receive() {
			return (GamePacket)serializer.Deserialize(netStream);
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
