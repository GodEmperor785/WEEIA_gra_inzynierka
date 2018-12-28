namespace GAME_AdminApp {
	partial class ShipWeaponsForm {
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
			this.shipDescriptionLabel = new System.Windows.Forms.Label();
			this.saveButton = new System.Windows.Forms.Button();
			this.backButton = new System.Windows.Forms.Button();
			this.weaponPanel = new System.Windows.Forms.Panel();
			this.weaponTable = new System.Windows.Forms.TableLayoutPanel();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.weaponPanel.SuspendLayout();
			this.weaponTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// shipDescriptionLabel
			// 
			this.shipDescriptionLabel.AutoSize = true;
			this.shipDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.shipDescriptionLabel.Location = new System.Drawing.Point(13, 13);
			this.shipDescriptionLabel.Name = "shipDescriptionLabel";
			this.shipDescriptionLabel.Size = new System.Drawing.Size(53, 13);
			this.shipDescriptionLabel.TabIndex = 0;
			this.shipDescriptionLabel.Text = "Ship ID:";
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(16, 798);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 23);
			this.saveButton.TabIndex = 2;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// backButton
			// 
			this.backButton.Location = new System.Drawing.Point(98, 798);
			this.backButton.Name = "backButton";
			this.backButton.Size = new System.Drawing.Size(75, 23);
			this.backButton.TabIndex = 3;
			this.backButton.Text = "Back";
			this.backButton.UseVisualStyleBackColor = true;
			this.backButton.Click += new System.EventHandler(this.backButton_Click);
			// 
			// weaponPanel
			// 
			this.weaponPanel.AutoScroll = true;
			this.weaponPanel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.weaponPanel.Controls.Add(this.weaponTable);
			this.weaponPanel.Location = new System.Drawing.Point(16, 30);
			this.weaponPanel.Name = "weaponPanel";
			this.weaponPanel.Size = new System.Drawing.Size(1610, 762);
			this.weaponPanel.TabIndex = 4;
			// 
			// weaponTable
			// 
			this.weaponTable.AutoSize = true;
			this.weaponTable.ColumnCount = 10;
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 327F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 209F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 143F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 131F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 159F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 106F));
			this.weaponTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 198F));
			this.weaponTable.Controls.Add(this.label10, 9, 0);
			this.weaponTable.Controls.Add(this.label9, 8, 0);
			this.weaponTable.Controls.Add(this.label8, 7, 0);
			this.weaponTable.Controls.Add(this.label6, 5, 0);
			this.weaponTable.Controls.Add(this.label5, 4, 0);
			this.weaponTable.Controls.Add(this.label4, 3, 0);
			this.weaponTable.Controls.Add(this.label3, 2, 0);
			this.weaponTable.Controls.Add(this.label2, 1, 0);
			this.weaponTable.Controls.Add(this.label1, 0, 0);
			this.weaponTable.Controls.Add(this.label7, 6, 0);
			this.weaponTable.Location = new System.Drawing.Point(4, 4);
			this.weaponTable.Name = "weaponTable";
			this.weaponTable.RowCount = 1;
			this.weaponTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.weaponTable.Size = new System.Drawing.Size(1603, 33);
			this.weaponTable.TabIndex = 0;
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label10.Location = new System.Drawing.Point(1408, 10);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(121, 13);
			this.label10.TabIndex = 9;
			this.label10.Text = "Choose this weapon";
			// 
			// label9
			// 
			this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label9.Location = new System.Drawing.Point(1302, 10);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(83, 13);
			this.label9.TabIndex = 8;
			this.label9.Text = "Chance to hit";
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label8.Location = new System.Drawing.Point(1143, 10);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(98, 13);
			this.label8.TabIndex = 7;
			this.label8.Text = "Range multiplier";
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label6.Location = new System.Drawing.Point(862, 10);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(82, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "Weapon type";
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label5.Location = new System.Drawing.Point(719, 10);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(127, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Number of projectiles";
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label4.Location = new System.Drawing.Point(585, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Damage";
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label3.Location = new System.Drawing.Point(376, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Faction";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label2.Location = new System.Drawing.Point(49, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Name";
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label1.Location = new System.Drawing.Point(3, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(20, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "ID";
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label7.Location = new System.Drawing.Point(1012, 10);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(83, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "AP Efficiency";
			// 
			// ShipWeaponsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1638, 847);
			this.Controls.Add(this.weaponPanel);
			this.Controls.Add(this.backButton);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.shipDescriptionLabel);
			this.Name = "ShipWeaponsForm";
			this.Text = "Ship weapons";
			this.weaponPanel.ResumeLayout(false);
			this.weaponPanel.PerformLayout();
			this.weaponTable.ResumeLayout(false);
			this.weaponTable.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label shipDescriptionLabel;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Panel weaponPanel;
		private System.Windows.Forms.TableLayoutPanel weaponTable;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label10;
	}
}