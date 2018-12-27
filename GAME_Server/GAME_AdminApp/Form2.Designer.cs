namespace GAME_AdminApp {
	partial class AdminForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.playerTab = new System.Windows.Forms.TabControl();
			this.shipTab = new System.Windows.Forms.TabPage();
			this.shipSyncButton = new System.Windows.Forms.Button();
			this.addShipButton = new System.Windows.Forms.Button();
			this.submitTableLayout = new System.Windows.Forms.TableLayoutPanel();
			this.label17 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.label26 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.submitButton = new System.Windows.Forms.Button();
			this.shipNameBox = new System.Windows.Forms.TextBox();
			this.shipFactionBox = new System.Windows.Forms.ComboBox();
			this.shipCostBox = new System.Windows.Forms.NumericUpDown();
			this.shipEvasionBox = new System.Windows.Forms.NumericUpDown();
			this.shipHpBox = new System.Windows.Forms.NumericUpDown();
			this.shipWeaponsModifyButton = new System.Windows.Forms.Button();
			this.shipDefencesModifyButton = new System.Windows.Forms.Button();
			this.shipSizeBox = new System.Windows.Forms.NumericUpDown();
			this.shipArmorBox = new System.Windows.Forms.NumericUpDown();
			this.shipExpUnlockBox = new System.Windows.Forms.NumericUpDown();
			this.shipRarityBox = new System.Windows.Forms.ComboBox();
			this.shipDescriptionLabel = new System.Windows.Forms.Label();
			this.shipPanel = new System.Windows.Forms.Panel();
			this.shipTable = new System.Windows.Forms.TableLayoutPanel();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.shipIdSearchBox = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.shipNameSearchBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.shipFaction = new System.Windows.Forms.ComboBox();
			this.shipSearchButton = new System.Windows.Forms.Button();
			this.weaponTab = new System.Windows.Forms.TabPage();
			this.defenceTab = new System.Windows.Forms.TabPage();
			this.modifierTab = new System.Windows.Forms.TabPage();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.exitButton = new System.Windows.Forms.Button();
			this.playerTab.SuspendLayout();
			this.shipTab.SuspendLayout();
			this.submitTableLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.shipCostBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.shipEvasionBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.shipHpBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.shipSizeBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.shipArmorBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.shipExpUnlockBox)).BeginInit();
			this.shipPanel.SuspendLayout();
			this.shipTable.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.shipIdSearchBox)).BeginInit();
			this.SuspendLayout();
			// 
			// playerTab
			// 
			this.playerTab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.playerTab.Controls.Add(this.shipTab);
			this.playerTab.Controls.Add(this.weaponTab);
			this.playerTab.Controls.Add(this.defenceTab);
			this.playerTab.Controls.Add(this.modifierTab);
			this.playerTab.Controls.Add(this.tabPage1);
			this.playerTab.Location = new System.Drawing.Point(13, 13);
			this.playerTab.Name = "playerTab";
			this.playerTab.SelectedIndex = 0;
			this.playerTab.Size = new System.Drawing.Size(1653, 874);
			this.playerTab.TabIndex = 0;
			// 
			// shipTab
			// 
			this.shipTab.AutoScroll = true;
			this.shipTab.Controls.Add(this.shipSyncButton);
			this.shipTab.Controls.Add(this.addShipButton);
			this.shipTab.Controls.Add(this.submitTableLayout);
			this.shipTab.Controls.Add(this.shipDescriptionLabel);
			this.shipTab.Controls.Add(this.shipPanel);
			this.shipTab.Controls.Add(this.tableLayoutPanel1);
			this.shipTab.Location = new System.Drawing.Point(4, 22);
			this.shipTab.Name = "shipTab";
			this.shipTab.Padding = new System.Windows.Forms.Padding(3);
			this.shipTab.Size = new System.Drawing.Size(1645, 848);
			this.shipTab.TabIndex = 0;
			this.shipTab.Text = "Ship Templates";
			this.shipTab.UseVisualStyleBackColor = true;
			// 
			// shipSyncButton
			// 
			this.shipSyncButton.AutoSize = true;
			this.shipSyncButton.Location = new System.Drawing.Point(1323, 613);
			this.shipSyncButton.Name = "shipSyncButton";
			this.shipSyncButton.Size = new System.Drawing.Size(179, 23);
			this.shipSyncButton.TabIndex = 5;
			this.shipSyncButton.Text = "Synchronize templates with Server";
			this.shipSyncButton.UseVisualStyleBackColor = true;
			this.shipSyncButton.Click += new System.EventHandler(this.shipSyncButton_Click);
			// 
			// addShipButton
			// 
			this.addShipButton.AutoSize = true;
			this.addShipButton.Location = new System.Drawing.Point(1508, 613);
			this.addShipButton.Name = "addShipButton";
			this.addShipButton.Size = new System.Drawing.Size(124, 23);
			this.addShipButton.TabIndex = 4;
			this.addShipButton.Text = "Add new ship template";
			this.addShipButton.UseVisualStyleBackColor = true;
			this.addShipButton.Click += new System.EventHandler(this.addShipButton_Click);
			// 
			// submitTableLayout
			// 
			this.submitTableLayout.ColumnCount = 4;
			this.submitTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.31579F));
			this.submitTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.68421F));
			this.submitTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
			this.submitTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 743F));
			this.submitTableLayout.Controls.Add(this.label17, 0, 0);
			this.submitTableLayout.Controls.Add(this.label18, 0, 1);
			this.submitTableLayout.Controls.Add(this.label19, 0, 2);
			this.submitTableLayout.Controls.Add(this.label20, 0, 3);
			this.submitTableLayout.Controls.Add(this.label21, 0, 4);
			this.submitTableLayout.Controls.Add(this.label22, 0, 5);
			this.submitTableLayout.Controls.Add(this.label23, 2, 0);
			this.submitTableLayout.Controls.Add(this.label24, 2, 1);
			this.submitTableLayout.Controls.Add(this.label25, 2, 2);
			this.submitTableLayout.Controls.Add(this.label26, 2, 3);
			this.submitTableLayout.Controls.Add(this.label27, 2, 4);
			this.submitTableLayout.Controls.Add(this.submitButton, 3, 5);
			this.submitTableLayout.Controls.Add(this.shipNameBox, 1, 0);
			this.submitTableLayout.Controls.Add(this.shipFactionBox, 1, 1);
			this.submitTableLayout.Controls.Add(this.shipCostBox, 1, 2);
			this.submitTableLayout.Controls.Add(this.shipEvasionBox, 1, 3);
			this.submitTableLayout.Controls.Add(this.shipHpBox, 1, 4);
			this.submitTableLayout.Controls.Add(this.shipWeaponsModifyButton, 1, 5);
			this.submitTableLayout.Controls.Add(this.shipDefencesModifyButton, 3, 0);
			this.submitTableLayout.Controls.Add(this.shipSizeBox, 3, 1);
			this.submitTableLayout.Controls.Add(this.shipArmorBox, 3, 2);
			this.submitTableLayout.Controls.Add(this.shipExpUnlockBox, 3, 3);
			this.submitTableLayout.Controls.Add(this.shipRarityBox, 3, 4);
			this.submitTableLayout.Location = new System.Drawing.Point(7, 640);
			this.submitTableLayout.Name = "submitTableLayout";
			this.submitTableLayout.RowCount = 6;
			this.submitTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.12281F));
			this.submitTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.87719F));
			this.submitTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.submitTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
			this.submitTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.submitTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.submitTableLayout.Size = new System.Drawing.Size(1625, 176);
			this.submitTableLayout.TabIndex = 3;
			// 
			// label17
			// 
			this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(3, 8);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(38, 13);
			this.label17.TabIndex = 0;
			this.label17.Text = "Name:";
			// 
			// label18
			// 
			this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(3, 37);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(45, 13);
			this.label18.TabIndex = 1;
			this.label18.Text = "Faction:";
			// 
			// label19
			// 
			this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(3, 67);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(31, 13);
			this.label19.TabIndex = 2;
			this.label19.Text = "Cost:";
			// 
			// label20
			// 
			this.label20.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(3, 95);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(107, 13);
			this.label20.TabIndex = 3;
			this.label20.Text = "Evasion (from 0 to 1):";
			// 
			// label21
			// 
			this.label21.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(3, 124);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(25, 13);
			this.label21.TabIndex = 4;
			this.label21.Text = "HP:";
			// 
			// label22
			// 
			this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(3, 154);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(56, 13);
			this.label22.TabIndex = 5;
			this.label22.Text = "Weapons:";
			// 
			// label23
			// 
			this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(763, 8);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(56, 13);
			this.label23.TabIndex = 6;
			this.label23.Text = "Defences:";
			// 
			// label24
			// 
			this.label24.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label24.AutoSize = true;
			this.label24.Location = new System.Drawing.Point(763, 37);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(30, 13);
			this.label24.TabIndex = 7;
			this.label24.Text = "Size:";
			// 
			// label25
			// 
			this.label25.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label25.AutoSize = true;
			this.label25.Location = new System.Drawing.Point(763, 67);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(37, 13);
			this.label25.TabIndex = 8;
			this.label25.Text = "Armor:";
			// 
			// label26
			// 
			this.label26.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label26.AutoSize = true;
			this.label26.Location = new System.Drawing.Point(763, 95);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(75, 13);
			this.label26.TabIndex = 9;
			this.label26.Text = "Exp to unlock:";
			// 
			// label27
			// 
			this.label27.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label27.AutoSize = true;
			this.label27.Location = new System.Drawing.Point(763, 124);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(37, 13);
			this.label27.TabIndex = 10;
			this.label27.Text = "Rarity:";
			// 
			// submitButton
			// 
			this.submitButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.submitButton.Enabled = false;
			this.submitButton.Location = new System.Drawing.Point(1547, 149);
			this.submitButton.Name = "submitButton";
			this.submitButton.Size = new System.Drawing.Size(75, 23);
			this.submitButton.TabIndex = 11;
			this.submitButton.Text = "Submit";
			this.submitButton.UseVisualStyleBackColor = true;
			this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
			// 
			// shipNameBox
			// 
			this.shipNameBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipNameBox.Enabled = false;
			this.shipNameBox.Location = new System.Drawing.Point(127, 4);
			this.shipNameBox.MaxLength = 256;
			this.shipNameBox.Name = "shipNameBox";
			this.shipNameBox.Size = new System.Drawing.Size(279, 20);
			this.shipNameBox.TabIndex = 12;
			// 
			// shipFactionBox
			// 
			this.shipFactionBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipFactionBox.Enabled = false;
			this.shipFactionBox.FormattingEnabled = true;
			this.shipFactionBox.Location = new System.Drawing.Point(127, 33);
			this.shipFactionBox.Name = "shipFactionBox";
			this.shipFactionBox.Size = new System.Drawing.Size(174, 21);
			this.shipFactionBox.TabIndex = 13;
			// 
			// shipCostBox
			// 
			this.shipCostBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipCostBox.Enabled = false;
			this.shipCostBox.Location = new System.Drawing.Point(127, 63);
			this.shipCostBox.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
			this.shipCostBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.shipCostBox.Name = "shipCostBox";
			this.shipCostBox.Size = new System.Drawing.Size(120, 20);
			this.shipCostBox.TabIndex = 14;
			this.shipCostBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// shipEvasionBox
			// 
			this.shipEvasionBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipEvasionBox.DecimalPlaces = 3;
			this.shipEvasionBox.Enabled = false;
			this.shipEvasionBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
			this.shipEvasionBox.Location = new System.Drawing.Point(127, 92);
			this.shipEvasionBox.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.shipEvasionBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
			this.shipEvasionBox.Name = "shipEvasionBox";
			this.shipEvasionBox.Size = new System.Drawing.Size(120, 20);
			this.shipEvasionBox.TabIndex = 15;
			this.shipEvasionBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
			// 
			// shipHpBox
			// 
			this.shipHpBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipHpBox.Enabled = false;
			this.shipHpBox.Location = new System.Drawing.Point(127, 121);
			this.shipHpBox.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.shipHpBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.shipHpBox.Name = "shipHpBox";
			this.shipHpBox.Size = new System.Drawing.Size(120, 20);
			this.shipHpBox.TabIndex = 16;
			this.shipHpBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// shipWeaponsModifyButton
			// 
			this.shipWeaponsModifyButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipWeaponsModifyButton.AutoSize = true;
			this.shipWeaponsModifyButton.Enabled = false;
			this.shipWeaponsModifyButton.Location = new System.Drawing.Point(127, 149);
			this.shipWeaponsModifyButton.Name = "shipWeaponsModifyButton";
			this.shipWeaponsModifyButton.Size = new System.Drawing.Size(94, 23);
			this.shipWeaponsModifyButton.TabIndex = 17;
			this.shipWeaponsModifyButton.Text = "Modify weapons";
			this.shipWeaponsModifyButton.UseVisualStyleBackColor = true;
			this.shipWeaponsModifyButton.Click += new System.EventHandler(this.shipWeaponsModifyButton_Click);
			// 
			// shipDefencesModifyButton
			// 
			this.shipDefencesModifyButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipDefencesModifyButton.AutoSize = true;
			this.shipDefencesModifyButton.Enabled = false;
			this.shipDefencesModifyButton.Location = new System.Drawing.Point(885, 3);
			this.shipDefencesModifyButton.Name = "shipDefencesModifyButton";
			this.shipDefencesModifyButton.Size = new System.Drawing.Size(95, 23);
			this.shipDefencesModifyButton.TabIndex = 18;
			this.shipDefencesModifyButton.Text = "Modify defences";
			this.shipDefencesModifyButton.UseVisualStyleBackColor = true;
			this.shipDefencesModifyButton.Click += new System.EventHandler(this.shipDefencesModifyButton_Click);
			// 
			// shipSizeBox
			// 
			this.shipSizeBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipSizeBox.DecimalPlaces = 2;
			this.shipSizeBox.Enabled = false;
			this.shipSizeBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.shipSizeBox.Location = new System.Drawing.Point(885, 34);
			this.shipSizeBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.shipSizeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.shipSizeBox.Name = "shipSizeBox";
			this.shipSizeBox.Size = new System.Drawing.Size(120, 20);
			this.shipSizeBox.TabIndex = 19;
			this.shipSizeBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// shipArmorBox
			// 
			this.shipArmorBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipArmorBox.Enabled = false;
			this.shipArmorBox.Location = new System.Drawing.Point(885, 63);
			this.shipArmorBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.shipArmorBox.Name = "shipArmorBox";
			this.shipArmorBox.Size = new System.Drawing.Size(120, 20);
			this.shipArmorBox.TabIndex = 20;
			// 
			// shipExpUnlockBox
			// 
			this.shipExpUnlockBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipExpUnlockBox.Enabled = false;
			this.shipExpUnlockBox.Location = new System.Drawing.Point(885, 92);
			this.shipExpUnlockBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.shipExpUnlockBox.Name = "shipExpUnlockBox";
			this.shipExpUnlockBox.Size = new System.Drawing.Size(120, 20);
			this.shipExpUnlockBox.TabIndex = 21;
			// 
			// shipRarityBox
			// 
			this.shipRarityBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipRarityBox.Enabled = false;
			this.shipRarityBox.FormattingEnabled = true;
			this.shipRarityBox.Location = new System.Drawing.Point(885, 120);
			this.shipRarityBox.Name = "shipRarityBox";
			this.shipRarityBox.Size = new System.Drawing.Size(170, 21);
			this.shipRarityBox.TabIndex = 22;
			// 
			// shipDescriptionLabel
			// 
			this.shipDescriptionLabel.AutoSize = true;
			this.shipDescriptionLabel.Location = new System.Drawing.Point(4, 623);
			this.shipDescriptionLabel.Name = "shipDescriptionLabel";
			this.shipDescriptionLabel.Size = new System.Drawing.Size(45, 13);
			this.shipDescriptionLabel.TabIndex = 2;
			this.shipDescriptionLabel.Text = "Ship ID:";
			// 
			// shipPanel
			// 
			this.shipPanel.AutoScroll = true;
			this.shipPanel.BackColor = System.Drawing.Color.WhiteSmoke;
			this.shipPanel.Controls.Add(this.shipTable);
			this.shipPanel.Location = new System.Drawing.Point(7, 45);
			this.shipPanel.Name = "shipPanel";
			this.shipPanel.Size = new System.Drawing.Size(1628, 562);
			this.shipPanel.TabIndex = 1;
			// 
			// shipTable
			// 
			this.shipTable.AutoSize = true;
			this.shipTable.ColumnCount = 13;
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.88889F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.11111F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 163F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 89F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 119F));
			this.shipTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 151F));
			this.shipTable.Controls.Add(this.label13, 9, 0);
			this.shipTable.Controls.Add(this.label12, 8, 0);
			this.shipTable.Controls.Add(this.label11, 7, 0);
			this.shipTable.Controls.Add(this.label10, 6, 0);
			this.shipTable.Controls.Add(this.label9, 5, 0);
			this.shipTable.Controls.Add(this.label8, 4, 0);
			this.shipTable.Controls.Add(this.label7, 3, 0);
			this.shipTable.Controls.Add(this.label5, 1, 0);
			this.shipTable.Controls.Add(this.label4, 0, 0);
			this.shipTable.Controls.Add(this.label6, 2, 0);
			this.shipTable.Controls.Add(this.label14, 10, 0);
			this.shipTable.Controls.Add(this.label15, 11, 0);
			this.shipTable.Controls.Add(this.label16, 12, 0);
			this.shipTable.Location = new System.Drawing.Point(6, 3);
			this.shipTable.Name = "shipTable";
			this.shipTable.RowCount = 1;
			this.shipTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.shipTable.Size = new System.Drawing.Size(1619, 25);
			this.shipTable.TabIndex = 0;
			// 
			// label13
			// 
			this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label13.Location = new System.Drawing.Point(1130, 6);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(39, 13);
			this.label13.TabIndex = 9;
			this.label13.Text = "Armor";
			// 
			// label12
			// 
			this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label12.Location = new System.Drawing.Point(1056, 6);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(31, 13);
			this.label12.TabIndex = 8;
			this.label12.Text = "Size";
			// 
			// label11
			// 
			this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label11.Location = new System.Drawing.Point(963, 6);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(61, 13);
			this.label11.TabIndex = 7;
			this.label11.Text = "Defences";
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label10.Location = new System.Drawing.Point(872, 6);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(60, 13);
			this.label10.TabIndex = 6;
			this.label10.Text = "Weapons";
			// 
			// label9
			// 
			this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label9.Location = new System.Drawing.Point(783, 6);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(24, 13);
			this.label9.TabIndex = 5;
			this.label9.Text = "HP";
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label8.Location = new System.Drawing.Point(716, 6);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(52, 13);
			this.label8.TabIndex = 4;
			this.label8.Text = "Evasion";
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label7.Location = new System.Drawing.Point(636, 6);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(32, 13);
			this.label7.TabIndex = 3;
			this.label7.Text = "Cost";
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label5.Location = new System.Drawing.Point(115, 6);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(39, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Name";
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label4.Location = new System.Drawing.Point(3, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(20, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "ID";
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label6.Location = new System.Drawing.Point(473, 6);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(49, 13);
			this.label6.TabIndex = 2;
			this.label6.Text = "Faction";
			// 
			// label14
			// 
			this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label14.Location = new System.Drawing.Point(1243, 6);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(85, 13);
			this.label14.TabIndex = 10;
			this.label14.Text = "Exp to unlock";
			// 
			// label15
			// 
			this.label15.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label15.Location = new System.Drawing.Point(1351, 6);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(40, 13);
			this.label15.TabIndex = 11;
			this.label15.Text = "Rarity";
			// 
			// label16
			// 
			this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label16.AutoSize = true;
			this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label16.Location = new System.Drawing.Point(1470, 6);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(120, 13);
			this.label16.TabIndex = 12;
			this.label16.Text = "Modify this template";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 7;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.51351F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86.48649F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 446F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 491F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 146F));
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.shipIdSearchBox, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.shipNameSearchBox, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this.shipFaction, 5, 0);
			this.tableLayoutPanel1.Controls.Add(this.shipSearchButton, 6, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1628, 31);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(21, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "ID:";
			// 
			// shipIdSearchBox
			// 
			this.shipIdSearchBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipIdSearchBox.Location = new System.Drawing.Point(53, 5);
			this.shipIdSearchBox.Name = "shipIdSearchBox";
			this.shipIdSearchBox.Size = new System.Drawing.Size(118, 20);
			this.shipIdSearchBox.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(374, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Ship template name:";
			// 
			// shipNameSearchBox
			// 
			this.shipNameSearchBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipNameSearchBox.Location = new System.Drawing.Point(487, 5);
			this.shipNameSearchBox.MaxLength = 256;
			this.shipNameSearchBox.Name = "shipNameSearchBox";
			this.shipNameSearchBox.Size = new System.Drawing.Size(289, 20);
			this.shipNameSearchBox.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(933, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Faction:";
			// 
			// shipFaction
			// 
			this.shipFaction.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipFaction.FormattingEnabled = true;
			this.shipFaction.Location = new System.Drawing.Point(993, 5);
			this.shipFaction.Name = "shipFaction";
			this.shipFaction.Size = new System.Drawing.Size(171, 21);
			this.shipFaction.TabIndex = 5;
			// 
			// shipSearchButton
			// 
			this.shipSearchButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.shipSearchButton.Location = new System.Drawing.Point(1484, 4);
			this.shipSearchButton.Name = "shipSearchButton";
			this.shipSearchButton.Size = new System.Drawing.Size(75, 23);
			this.shipSearchButton.TabIndex = 6;
			this.shipSearchButton.Text = "Search";
			this.shipSearchButton.UseVisualStyleBackColor = true;
			this.shipSearchButton.Click += new System.EventHandler(this.shipSearchButton_Click);
			// 
			// weaponTab
			// 
			this.weaponTab.Location = new System.Drawing.Point(4, 22);
			this.weaponTab.Name = "weaponTab";
			this.weaponTab.Padding = new System.Windows.Forms.Padding(3);
			this.weaponTab.Size = new System.Drawing.Size(1645, 848);
			this.weaponTab.TabIndex = 1;
			this.weaponTab.Text = "Weapons";
			this.weaponTab.UseVisualStyleBackColor = true;
			// 
			// defenceTab
			// 
			this.defenceTab.Location = new System.Drawing.Point(4, 22);
			this.defenceTab.Name = "defenceTab";
			this.defenceTab.Padding = new System.Windows.Forms.Padding(3);
			this.defenceTab.Size = new System.Drawing.Size(1645, 848);
			this.defenceTab.TabIndex = 2;
			this.defenceTab.Text = "Defence Systems";
			this.defenceTab.UseVisualStyleBackColor = true;
			// 
			// modifierTab
			// 
			this.modifierTab.Location = new System.Drawing.Point(4, 22);
			this.modifierTab.Name = "modifierTab";
			this.modifierTab.Padding = new System.Windows.Forms.Padding(3);
			this.modifierTab.Size = new System.Drawing.Size(1645, 848);
			this.modifierTab.TabIndex = 3;
			this.modifierTab.Text = "Base Modifiers";
			this.modifierTab.UseVisualStyleBackColor = true;
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(1645, 848);
			this.tabPage1.TabIndex = 4;
			this.tabPage1.Text = "Users";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// exitButton
			// 
			this.exitButton.Location = new System.Drawing.Point(1583, 903);
			this.exitButton.Name = "exitButton";
			this.exitButton.Size = new System.Drawing.Size(75, 23);
			this.exitButton.TabIndex = 1;
			this.exitButton.Text = "Exit";
			this.exitButton.UseVisualStyleBackColor = true;
			this.exitButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// AdminForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1677, 940);
			this.Controls.Add(this.exitButton);
			this.Controls.Add(this.playerTab);
			this.Name = "AdminForm";
			this.Text = "GAME Admin Application";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AdminForm_FormClosed);
			this.playerTab.ResumeLayout(false);
			this.shipTab.ResumeLayout(false);
			this.shipTab.PerformLayout();
			this.submitTableLayout.ResumeLayout(false);
			this.submitTableLayout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.shipCostBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.shipEvasionBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.shipHpBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.shipSizeBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.shipArmorBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.shipExpUnlockBox)).EndInit();
			this.shipPanel.ResumeLayout(false);
			this.shipPanel.PerformLayout();
			this.shipTable.ResumeLayout(false);
			this.shipTable.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.shipIdSearchBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl playerTab;
		private System.Windows.Forms.TabPage shipTab;
		private System.Windows.Forms.TabPage weaponTab;
		private System.Windows.Forms.TabPage defenceTab;
		private System.Windows.Forms.TabPage modifierTab;
		private System.Windows.Forms.Button exitButton;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown shipIdSearchBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox shipNameSearchBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox shipFaction;
		private System.Windows.Forms.Button shipSearchButton;
		private System.Windows.Forms.Panel shipPanel;
		private System.Windows.Forms.TableLayoutPanel shipTable;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label shipDescriptionLabel;
		private System.Windows.Forms.Button addShipButton;
		private System.Windows.Forms.TableLayoutPanel submitTableLayout;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Button submitButton;
		private System.Windows.Forms.TextBox shipNameBox;
		private System.Windows.Forms.ComboBox shipFactionBox;
		private System.Windows.Forms.NumericUpDown shipCostBox;
		private System.Windows.Forms.NumericUpDown shipEvasionBox;
		private System.Windows.Forms.NumericUpDown shipHpBox;
		private System.Windows.Forms.Button shipWeaponsModifyButton;
		private System.Windows.Forms.Button shipDefencesModifyButton;
		private System.Windows.Forms.NumericUpDown shipSizeBox;
		private System.Windows.Forms.NumericUpDown shipArmorBox;
		private System.Windows.Forms.NumericUpDown shipExpUnlockBox;
		private System.Windows.Forms.ComboBox shipRarityBox;
		private System.Windows.Forms.Button shipSyncButton;
	}
}