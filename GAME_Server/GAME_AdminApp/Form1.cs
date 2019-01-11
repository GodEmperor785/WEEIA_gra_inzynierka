using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GAME_connection;

namespace GAME_AdminApp {
	public partial class LoginForm : Form {

		public LoginForm() {
			InitializeComponent();
			ipTextBox.Text = AdminApp.DefaultIP;
			portBox.Value = AdminApp.DefaultPort;
			LoginButton.Enabled = false;
		}

		private void button1_Click(object sender, EventArgs e) {}

		private void LoginButton_Click(object sender, EventArgs e) {
			string login = textBox1.Text;
			string password = textBox2.Text;
			GamePacket packet = new GamePacket(OperationType.LOGIN, new Player(login, password));
			AdminApp.Connection.Send(packet);
			packet = AdminApp.Connection.GetReceivedPacket();
			if(packet.OperationType == OperationType.SUCCESS) {
				SetupShowAppForm();
			}
			else {
				MessageBox.Show((string)packet.Packet, "Login failed!");
			}
		}

		private void connectButton_Click(object sender, EventArgs e) {
			string ip = ipTextBox.Text;
			int port = Convert.ToInt32(portBox.Value);
			bool connectionResult = AdminApp.ConnectToServer(ip, port);
			if (connectionResult) {
				infoLabel.Text = "Connected to: " + ip + ":" + port;
				LoginButton.Enabled = true;
				this.AcceptButton = LoginButton;
			}
		}

		private void SetupShowAppForm() {
			GamePacket packet = AdminApp.Connection.GetReceivedPacket();
			AdminApp.GameData = (AdminDataPacket)packet.Packet;

			//initialize dropdown lists after receiving data from server
			AdminApp.AppForm.InitializeShipDropDownLists();
			AdminApp.AppForm.InitializeWeaponsDropDownLists();
			AdminApp.AppForm.InitializeDefencesDropDownLists();
			AdminApp.AppForm.InitializeBaseModifiers();
			AdminApp.AppForm.InitializeUsers();
			AdminApp.AppForm.InitializeLootboxes();
			AdminApp.AppForm.Show();

			this.Hide();
		}

		private void LoginForm_FormClosed(object sender, FormClosedEventArgs e) {
			AdminApp.ExitApp();
		}
	}
}
