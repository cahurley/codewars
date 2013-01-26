
namespace Windwardopolis
{
	partial class MainWindow
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.toolStripMainMenu = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonJoinOpened = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonJoinClosed = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonPlay = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStep = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPause = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonSpeed = new System.Windows.Forms.ToolStripButton();
			this.toolStripDropDownZoom = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripMenuItemZoom200 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemZoom100 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemZoom75 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemZoom50 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemCoordinates = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemLimoShadow = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemLimoShadowAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonScoreboard = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStatusList = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDebugWindow = new System.Windows.Forms.ToolStripButton();
			this.toolStripDebugRun = new System.Windows.Forms.ToolStripButton();
			this.toolStripDebugReset = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.contextMenuRMB = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemMoveCar = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSetDest = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSetPassenger = new System.Windows.Forms.ToolStripMenuItem();
			this.mapDisplay = new WindwardopolisLibrary.map.MapDisplay();
			this.toolStripMuteSound = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripFullSpeed = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMainMenu.SuspendLayout();
			this.contextMenuRMB.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripMainMenu
			// 
			this.toolStripMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonJoinOpened,
            this.toolStripButtonJoinClosed,
            this.toolStripSeparator2,
            this.toolStripButtonPlay,
            this.toolStripButtonStep,
            this.toolStripButtonPause,
            this.toolStripButtonStop,
            this.toolStripSeparator1,
            this.toolStripButtonSpeed,
            this.toolStripDropDownZoom,
			this.toolStripMuteSound,
            this.toolStripSeparator3,
            this.toolStripButtonScoreboard,
            this.toolStripButtonStatusList,
            this.toolStripButtonDebugWindow,
            this.toolStripSeparator7,
            this.toolStripDebugRun,
            this.toolStripDebugReset,
            this.toolStripFullSpeed});
			this.toolStripMainMenu.Location = new System.Drawing.Point(0, 0);
			this.toolStripMainMenu.Name = "toolStripMainMenu";
			this.toolStripMainMenu.Size = new System.Drawing.Size(1050, 25);
			this.toolStripMainMenu.TabIndex = 1;
			this.toolStripMainMenu.Text = "toolStrip1";
			// 
			// toolStripButtonJoinOpened
			// 
			this.toolStripButtonJoinOpened.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonJoinOpened.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonJoinOpened.Image")));
			this.toolStripButtonJoinOpened.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonJoinOpened.Name = "toolStripButtonJoinOpened";
			this.toolStripButtonJoinOpened.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonJoinOpened.Text = "join";
			this.toolStripButtonJoinOpened.Click += new System.EventHandler(this.toolStripButtonJoin_Click);
			// 
			// toolStripButtonJoinClosed
			// 
			this.toolStripButtonJoinClosed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonJoinClosed.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonJoinClosed.Image")));
			this.toolStripButtonJoinClosed.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonJoinClosed.Name = "toolStripButtonJoinClosed";
			this.toolStripButtonJoinClosed.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonJoinClosed.Text = "lock";
			this.toolStripButtonJoinClosed.Click += new System.EventHandler(this.toolStripButtonClosed_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonPlay
			// 
			this.toolStripButtonPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPlay.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPlay.Image")));
			this.toolStripButtonPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonPlay.Name = "toolStripButtonPlay";
			this.toolStripButtonPlay.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonPlay.Text = "Play";
			this.toolStripButtonPlay.Click += new System.EventHandler(this.Play_Click);
			// 
			// toolStripButtonStep
			// 
			this.toolStripButtonStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStep.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStep.Image")));
			this.toolStripButtonStep.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonStep.Name = "toolStripButtonStep";
			this.toolStripButtonStep.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonStep.Text = "step";
			this.toolStripButtonStep.Click += new System.EventHandler(this.toolStripButtonStep_Click);
			// 
			// toolStripButtonPause
			// 
			this.toolStripButtonPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPause.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPause.Image")));
			this.toolStripButtonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonPause.Name = "toolStripButtonPause";
			this.toolStripButtonPause.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonPause.Text = "Pause";
			this.toolStripButtonPause.Click += new System.EventHandler(this.Pause_Click);
			// 
			// toolStripButtonStop
			// 
			this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStop.Image")));
			this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonStop.Name = "toolStripButtonStop";
			this.toolStripButtonStop.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonStop.Text = "Stop";
			this.toolStripButtonStop.Click += new System.EventHandler(this.Stop_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonSpeed
			// 
			this.toolStripButtonSpeed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSpeed.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSpeed.Image")));
			this.toolStripButtonSpeed.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonSpeed.Name = "toolStripButtonSpeed";
			this.toolStripButtonSpeed.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonSpeed.Text = "speed";
			this.toolStripButtonSpeed.Click += new System.EventHandler(this.toolStripButtonSpeed_Click);
			// 
			// toolStripDropDownZoom
			// 
			this.toolStripDropDownZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripDropDownZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemZoom200,
            this.toolStripMenuItemZoom100,
            this.toolStripMenuItemZoom75,
            this.toolStripMenuItemZoom50,
            this.toolStripSeparator4,
            this.toolStripMenuItemCoordinates,
            this.toolStripMenuItemLimoShadow,
            this.toolStripMenuItemLimoShadowAll});
			this.toolStripDropDownZoom.Image = global::Windwardopolis.Status.view;
			this.toolStripDropDownZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownZoom.Name = "toolStripDropDownZoom";
			this.toolStripDropDownZoom.Size = new System.Drawing.Size(29, 22);
			this.toolStripDropDownZoom.Text = "toolStripDropDownZoom";
			// 
			// toolStripMenuItemZoom200
			// 
			this.toolStripMenuItemZoom200.Name = "toolStripMenuItemZoom200";
			this.toolStripMenuItemZoom200.Size = new System.Drawing.Size(163, 22);
			this.toolStripMenuItemZoom200.Text = "200%";
			this.toolStripMenuItemZoom200.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
			// 
			// toolStripMenuItemZoom100
			// 
			this.toolStripMenuItemZoom100.Name = "toolStripMenuItemZoom100";
			this.toolStripMenuItemZoom100.Size = new System.Drawing.Size(163, 22);
			this.toolStripMenuItemZoom100.Text = "100%";
			this.toolStripMenuItemZoom100.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
			// 
			// toolStripMenuItemZoom75
			// 
			this.toolStripMenuItemZoom75.Name = "toolStripMenuItemZoom75";
			this.toolStripMenuItemZoom75.Size = new System.Drawing.Size(163, 22);
			this.toolStripMenuItemZoom75.Text = "75%";
			this.toolStripMenuItemZoom75.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
			// 
			// toolStripMenuItemZoom50
			// 
			this.toolStripMenuItemZoom50.Name = "toolStripMenuItemZoom50";
			this.toolStripMenuItemZoom50.Size = new System.Drawing.Size(163, 22);
			this.toolStripMenuItemZoom50.Text = "50%";
			this.toolStripMenuItemZoom50.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(160, 6);
			// 
			// toolStripMenuItemCoordinates
			// 
			this.toolStripMenuItemCoordinates.Name = "toolStripMenuItemCoordinates";
			this.toolStripMenuItemCoordinates.Size = new System.Drawing.Size(163, 22);
			this.toolStripMenuItemCoordinates.Text = "Coordinates";
			this.toolStripMenuItemCoordinates.Click += new System.EventHandler(this.toolStripMenuItemCoordinates_Click);
			// 
			// toolStripMenuItemLimoShadow
			// 
			this.toolStripMenuItemLimoShadow.Name = "toolStripMenuItemLimoShadow";
			this.toolStripMenuItemLimoShadow.Size = new System.Drawing.Size(163, 22);
			this.toolStripMenuItemLimoShadow.Text = "Limo Shadow";
			this.toolStripMenuItemLimoShadow.Click += new System.EventHandler(this.ToolStripMenuItemLimoShadow_Click);
			// 
			// toolStripMenuItemLimoShadowAll
			// 
			this.toolStripMenuItemLimoShadowAll.Name = "toolStripMenuItemLimoShadowAll";
			this.toolStripMenuItemLimoShadowAll.Size = new System.Drawing.Size(163, 22);
			this.toolStripMenuItemLimoShadowAll.Text = "Limo Shadow All";
			this.toolStripMenuItemLimoShadowAll.Click += new System.EventHandler(this.toolStripMenuItemLimoShadowAll_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonScoreboard
			// 
			this.toolStripButtonScoreboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonScoreboard.Image = global::Windwardopolis.Status.window_application;
			this.toolStripButtonScoreboard.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonScoreboard.Name = "toolStripButtonScoreboard";
			this.toolStripButtonScoreboard.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonScoreboard.Text = "Scoreboard";
			this.toolStripButtonScoreboard.Click += new System.EventHandler(this.toolStripButtonScoreboard_Click);
			// 
			// toolStripButtonStatusList
			// 
			this.toolStripButtonStatusList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStatusList.Image = global::Windwardopolis.Status.table_sql;
			this.toolStripButtonStatusList.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonStatusList.Name = "toolStripButtonStatusList";
			this.toolStripButtonStatusList.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonStatusList.Text = "Status";
			this.toolStripButtonStatusList.Click += new System.EventHandler(this.toolStripButtonStatusList_Click);
			// 
			// toolStripButtonDebugWindow
			// 
			this.toolStripButtonDebugWindow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDebugWindow.Image = global::Windwardopolis.Status.debug_view;
			this.toolStripButtonDebugWindow.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDebugWindow.Name = "toolStripButtonDebugWindow";
			this.toolStripButtonDebugWindow.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonDebugWindow.Text = "Debug Window";
			this.toolStripButtonDebugWindow.Click += new System.EventHandler(this.toolStripButtonDebugWindow_Click);
			// 
			// toolStripDebugRun
			// 
			this.toolStripDebugRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripDebugRun.Image = global::Windwardopolis.Status.bug_yellow;
			this.toolStripDebugRun.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDebugRun.Name = "toolStripDebugRun";
			this.toolStripDebugRun.Size = new System.Drawing.Size(23, 22);
			this.toolStripDebugRun.Text = "Debug Start";
			this.toolStripDebugRun.Click += new System.EventHandler(this.toolStripDebugRun_Click);
			// 
			// toolStripDebugReset
			// 
			this.toolStripDebugReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripDebugReset.Image = global::Windwardopolis.Status.debug_reset;
			this.toolStripDebugReset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDebugReset.Name = "toolStripDebugReset";
			this.toolStripDebugReset.Size = new System.Drawing.Size(23, 22);
			this.toolStripDebugReset.Text = "Debug Reset";
			this.toolStripDebugReset.Click += new System.EventHandler(this.toolStripDebugReset_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
			// contextMenuRMB
			// 
			this.contextMenuRMB.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMoveCar,
            this.toolStripMenuItemSetDest,
            this.toolStripMenuItemSetPassenger});
			this.contextMenuRMB.Name = "contextMenuRMB";
			this.contextMenuRMB.Size = new System.Drawing.Size(154, 70);
			// 
			// toolStripMenuItemMoveCar
			// 
			this.toolStripMenuItemMoveCar.Name = "toolStripMenuItemMoveCar";
			this.toolStripMenuItemMoveCar.Size = new System.Drawing.Size(153, 22);
			this.toolStripMenuItemMoveCar.Text = "Move Car";
			this.toolStripMenuItemMoveCar.Click += new System.EventHandler(this.toolStripMenuItemMoveCar_Click);
			// 
			// toolStripMenuItemSetDest
			// 
			this.toolStripMenuItemSetDest.Name = "toolStripMenuItemSetDest";
			this.toolStripMenuItemSetDest.Size = new System.Drawing.Size(153, 22);
			this.toolStripMenuItemSetDest.Text = "Set Destination";
			this.toolStripMenuItemSetDest.Click += new System.EventHandler(this.toolStripMenuItemSetDest_Click);
			// 
			// toolStripMenuItemSetPassenger
			// 
			this.toolStripMenuItemSetPassenger.Name = "toolStripMenuItemSetPassenger";
			this.toolStripMenuItemSetPassenger.Size = new System.Drawing.Size(153, 22);
			this.toolStripMenuItemSetPassenger.Text = "Set Passenger";
			// 
			// mapDisplay
			// 
			this.mapDisplay.AutoScroll = true;
			this.mapDisplay.DisplayCoordinates = false;
			this.mapDisplay.DisplayShadows = false;
			this.mapDisplay.DisplayShadowsAll = false;
			this.mapDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapDisplay.Location = new System.Drawing.Point(0, 25);
			this.mapDisplay.Name = "mapDisplay";
			this.mapDisplay.Size = new System.Drawing.Size(1050, 831);
			this.mapDisplay.TabIndex = 2;
			// 
			// toolStripMuteSound
			// 
			this.toolStripMuteSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripMuteSound.Image = global::Windwardopolis.Status.loudspeaker;
			this.toolStripMuteSound.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripMuteSound.Name = "toolStripMuteSound";
			this.toolStripMuteSound.Size = new System.Drawing.Size(23, 22);
			this.toolStripMuteSound.Text = "Mute sounds";
			this.toolStripMuteSound.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = global::Windwardopolis.Status.gauge;
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton1.Text = "Full Speed";
			// 
			// toolStripFullSpeed
			// 
			this.toolStripFullSpeed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripFullSpeed.Image = global::Windwardopolis.Status.gauge;
			this.toolStripFullSpeed.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripFullSpeed.Name = "toolStripFullSpeed";
			this.toolStripFullSpeed.Size = new System.Drawing.Size(23, 22);
			this.toolStripFullSpeed.Text = "Full speed";
			this.toolStripFullSpeed.Click += new System.EventHandler(this.toolStripFullSpeed_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1050, 856);
			this.Controls.Add(this.mapDisplay);
			this.Controls.Add(this.toolStripMainMenu);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainWindow";
			this.Text = "Windwardopolis";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameDisplay_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
			this.Load += new System.EventHandler(this.MainWindow_Load);
			this.toolStripMainMenu.ResumeLayout(false);
			this.toolStripMainMenu.PerformLayout();
			this.contextMenuRMB.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStripMainMenu;
		private System.Windows.Forms.ToolStripButton toolStripButtonPlay;
		private System.Windows.Forms.ToolStripButton toolStripButtonPause;
		private System.Windows.Forms.ToolStripButton toolStripButtonStop;
		private System.Windows.Forms.ToolStripButton toolStripButtonJoinOpened;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButtonSpeed;
		private System.Windows.Forms.ToolStripButton toolStripButtonJoinClosed;
		private System.Windows.Forms.ToolStripButton toolStripButtonStep;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton toolStripButtonScoreboard;
		private System.Windows.Forms.ToolStripButton toolStripButtonStatusList;
		private WindwardopolisLibrary.map.MapDisplay mapDisplay;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownZoom;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom200;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom100;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom75;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom50;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCoordinates;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLimoShadow;
		private System.Windows.Forms.ToolStripButton toolStripButtonDebugWindow;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLimoShadowAll;
		private System.Windows.Forms.ToolStripButton toolStripDebugRun;
		private System.Windows.Forms.ContextMenuStrip contextMenuRMB;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMoveCar;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetDest;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetPassenger;
		private System.Windows.Forms.ToolStripButton toolStripDebugReset;
		private System.Windows.Forms.ToolStripButton toolStripMuteSound;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripFullSpeed;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
	}
}
