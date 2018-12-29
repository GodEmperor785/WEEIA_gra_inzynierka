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
		public static List<AdminAppPlayer> AllUsers { get; set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(GlobalExceptionHandler);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ThreadExceptionHandler);
			LoginForm = new LoginForm();
			AppForm = new AdminForm();
			AlreadyClosed = false;
			Application.Run(LoginForm);
		}

		private static void DisplayConnectionLost() {
			if (!AlreadyClosed) {
				MessageBox.Show("Connection Lost", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				ExitApp();
			}
		}

		private static void DisplayUnhandledException(Exception exc) {
			MessageBox.Show("Unhandled error - " + exc.Message + Environment.NewLine + exc.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			if (!AlreadyClosed) ExitApp();
		}

		private static void GlobalExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e) {
			DisplayUnhandledException(e.Exception);
		}

		private static void ThreadExceptionHandler(object sender, UnhandledExceptionEventArgs e) {
			var exc = e.ExceptionObject as Exception;
			DisplayUnhandledException(exc);
		}

		private static void Connection_ConnectionEnded(object sender, GameEventArgs e) {
			DisplayConnectionLost();
		}

		public static bool ConnectToServer(string ip, int port) {
			try {
				TcpClient client = new TcpClient(ip, port);
				Connection = new TcpConnection(client, true, Log);
				//new TcpConnection(client, true, Log, true, true);		//dla ssl
				Connection.ConnectionEnded += Connection_ConnectionEnded;
				return true;
			}
			catch (Exception ex) {
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
