using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using GAME_connection;
using GAME_Validator;

namespace GAME_AdminApp {
	public static class AdminApp {
		//https://www.sourcecodester.com/book/7127/multiple-forms-c.html
		//https://stackoverflow.com/questions/15535214/removing-a-specific-row-in-tablelayoutpanel

		private static bool alreadyClosed;
		private static object alreadyClosedLock = new object();
		public static TcpConnection Connection { get; set; }
		public static LoginForm LoginForm { get; set; }
		public static AdminForm AppForm { get; set; }
		public static bool AlreadyClosed {
			get {
				bool local;
				lock (alreadyClosedLock) {
					local = alreadyClosed;
				}
				return local;
			}
			set {
				lock (alreadyClosedLock) {
					alreadyClosed = value;
				}
			}
		}
		public static AdminDataPacket GameData { get; set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			LoginForm = new LoginForm();
			AppForm = new AdminForm();
			AlreadyClosed = false;
			Application.Run(LoginForm);
		}

		public static bool ConnectToServer(string ip, int port) {
			try {
				TcpClient client = new TcpClient(ip, port);
				Connection = new TcpConnection(client, true, Log);
				//new TcpConnection(client, true, Log, true, true);		//dla ssl
				return true;
			} catch(Exception ex) {
				MessageBox.Show("Connection to " + ip + ":" + port + " failed!" + Environment.NewLine + ex.Message, "Connection failed!");
				return false;
			}
		}

		public static void ExitApp() {
			if (!AlreadyClosed) {
				AlreadyClosed = true;
				if(Connection != null) Connection.SendDisconnect();
				LoginForm.Close();
				AppForm.Close();
				Application.Exit();
				//Environment.Exit(0);
			}
		}

		public static void Log(string msg) {
			Console.WriteLine(msg);
		}
	}
}
