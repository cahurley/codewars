namespace Windwardopolis
{
	partial class PlayerStatus
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerStatus));
			this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
			this.labelScore = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.pictureBoxRobot = new System.Windows.Forms.PictureBox();
			this.pictPassenger = new System.Windows.Forms.PictureBox();
			this.pictDestination = new System.Windows.Forms.PictureBox();
			this.labelPassenger = new System.Windows.Forms.Label();
			this.labelDestination = new System.Windows.Forms.Label();
			this.pictNoConnection = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRobot)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictPassenger)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictDestination)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictNoConnection)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxAvatar
			// 
			this.pictureBoxAvatar.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxAvatar.Image")));
			this.pictureBoxAvatar.Location = new System.Drawing.Point(4, 4);
			this.pictureBoxAvatar.Name = "pictureBoxAvatar";
			this.pictureBoxAvatar.Size = new System.Drawing.Size(32, 32);
			this.pictureBoxAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxAvatar.TabIndex = 0;
			this.pictureBoxAvatar.TabStop = false;
			// 
			// labelScore
			// 
			this.labelScore.AutoSize = true;
			this.labelScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelScore.Location = new System.Drawing.Point(65, 4);
			this.labelScore.Name = "labelScore";
			this.labelScore.Size = new System.Drawing.Size(48, 26);
			this.labelScore.TabIndex = 1;
			this.labelScore.Text = "000";
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelName.AutoEllipsis = true;
			this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelName.Location = new System.Drawing.Point(4, 63);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(255, 17);
			this.labelName.TabIndex = 3;
			this.labelName.Text = "David Thielen";
			// 
			// pictureBoxRobot
			// 
			this.pictureBoxRobot.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxRobot.Location = new System.Drawing.Point(40, 4);
			this.pictureBoxRobot.Name = "pictureBoxRobot";
			this.pictureBoxRobot.Size = new System.Drawing.Size(23, 32);
			this.pictureBoxRobot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxRobot.TabIndex = 4;
			this.pictureBoxRobot.TabStop = false;
			// 
			// pictPassenger
			// 
			this.pictPassenger.Location = new System.Drawing.Point(130, 4);
			this.pictPassenger.Name = "pictPassenger";
			this.pictPassenger.Size = new System.Drawing.Size(24, 24);
			this.pictPassenger.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictPassenger.TabIndex = 5;
			this.pictPassenger.TabStop = false;
			// 
			// pictDestination
			// 
			this.pictDestination.Location = new System.Drawing.Point(130, 33);
			this.pictDestination.Name = "pictDestination";
			this.pictDestination.Size = new System.Drawing.Size(24, 24);
			this.pictDestination.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictDestination.TabIndex = 6;
			this.pictDestination.TabStop = false;
			// 
			// labelPassenger
			// 
			this.labelPassenger.AutoEllipsis = true;
			this.labelPassenger.Location = new System.Drawing.Point(160, 9);
			this.labelPassenger.Name = "labelPassenger";
			this.labelPassenger.Size = new System.Drawing.Size(99, 14);
			this.labelPassenger.TabIndex = 7;
			this.labelPassenger.Text = "{none}";
			// 
			// labelDestination
			// 
			this.labelDestination.AutoEllipsis = true;
			this.labelDestination.Location = new System.Drawing.Point(160, 38);
			this.labelDestination.Name = "labelDestination";
			this.labelDestination.Size = new System.Drawing.Size(102, 14);
			this.labelDestination.TabIndex = 8;
			this.labelDestination.Text = "{none}";
			// 
			// pictNoConnection
			// 
			this.pictNoConnection.Image = global::Windwardopolis.Status.flash;
			this.pictNoConnection.Location = new System.Drawing.Point(65, 33);
			this.pictNoConnection.Name = "pictNoConnection";
			this.pictNoConnection.Size = new System.Drawing.Size(24, 24);
			this.pictNoConnection.TabIndex = 9;
			this.pictNoConnection.TabStop = false;
			this.pictNoConnection.Visible = false;
			// 
			// PlayerStatus
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.pictNoConnection);
			this.Controls.Add(this.labelDestination);
			this.Controls.Add(this.labelPassenger);
			this.Controls.Add(this.pictDestination);
			this.Controls.Add(this.pictPassenger);
			this.Controls.Add(this.pictureBoxRobot);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.labelScore);
			this.Controls.Add(this.pictureBoxAvatar);
			this.DoubleBuffered = true;
			this.Name = "PlayerStatus";
			this.Size = new System.Drawing.Size(258, 108);
			this.Load += new System.EventHandler(this.PlayerStatus_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PlayerStatus_Paint);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRobot)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictPassenger)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictDestination)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictNoConnection)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxAvatar;
		private System.Windows.Forms.Label labelScore;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.PictureBox pictureBoxRobot;
        private System.Windows.Forms.PictureBox pictPassenger;
        private System.Windows.Forms.PictureBox pictDestination;
        private System.Windows.Forms.Label labelPassenger;
        private System.Windows.Forms.Label labelDestination;
		private System.Windows.Forms.PictureBox pictNoConnection;
	}
}
