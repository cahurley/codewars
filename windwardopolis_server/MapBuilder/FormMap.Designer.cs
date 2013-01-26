namespace MapBuilder
{
	partial class FormMap
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.objectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.roadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.busStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.companyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.parkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.stopSignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontalStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.verticalStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.trafficLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.startPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.northStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.eastStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.southStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.westStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemZoom200 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemZoom100 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemZoom75 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemZoom50 = new System.Windows.Forms.ToolStripMenuItem();
			this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.trimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addRowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mapDisplay = new WindwardopolisLibrary.map.MapDisplay();
			this.rotate90ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.objectToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.mapToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(742, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.newToolStripMenuItem.Text = "New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.openToolStripMenuItem.Text = "Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.closeToolStripMenuItem.Text = "Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(120, 6);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.saveAsToolStripMenuItem.Text = "Save As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(120, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// objectToolStripMenuItem
			// 
			this.objectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.roadToolStripMenuItem,
            this.busStopToolStripMenuItem,
            this.companyToolStripMenuItem,
            this.parkToolStripMenuItem,
            this.toolStripSeparator3,
            this.stopSignToolStripMenuItem,
            this.trafficLightToolStripMenuItem,
            this.toolStripSeparator4,
            this.startPositionToolStripMenuItem,
            this.toolStripSeparator5,
            this.clearToolStripMenuItem});
			this.objectToolStripMenuItem.Name = "objectToolStripMenuItem";
			this.objectToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.objectToolStripMenuItem.Text = "Object";
			// 
			// roadToolStripMenuItem
			// 
			this.roadToolStripMenuItem.Name = "roadToolStripMenuItem";
			this.roadToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.roadToolStripMenuItem.Text = "Road";
			this.roadToolStripMenuItem.Click += new System.EventHandler(this.roadToolStripMenuItem_Click);
			// 
			// busStopToolStripMenuItem
			// 
			this.busStopToolStripMenuItem.Name = "busStopToolStripMenuItem";
			this.busStopToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.busStopToolStripMenuItem.Text = "Bus Stop";
			this.busStopToolStripMenuItem.Click += new System.EventHandler(this.busStopToolStripMenuItem_Click);
			// 
			// companyToolStripMenuItem
			// 
			this.companyToolStripMenuItem.Name = "companyToolStripMenuItem";
			this.companyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.companyToolStripMenuItem.Text = "Company";
			this.companyToolStripMenuItem.Click += new System.EventHandler(this.companyToolStripMenuItem_Click);
			// 
			// parkToolStripMenuItem
			// 
			this.parkToolStripMenuItem.Name = "parkToolStripMenuItem";
			this.parkToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.parkToolStripMenuItem.Text = "Park";
			this.parkToolStripMenuItem.Click += new System.EventHandler(this.parkToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
			// 
			// stopSignToolStripMenuItem
			// 
			this.stopSignToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.horizontalStopToolStripMenuItem,
            this.verticalStopToolStripMenuItem});
			this.stopSignToolStripMenuItem.Name = "stopSignToolStripMenuItem";
			this.stopSignToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.stopSignToolStripMenuItem.Text = "Stop Sign";
			// 
			// horizontalStopToolStripMenuItem
			// 
			this.horizontalStopToolStripMenuItem.Name = "horizontalStopToolStripMenuItem";
			this.horizontalStopToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.horizontalStopToolStripMenuItem.Text = "Horizontal";
			this.horizontalStopToolStripMenuItem.Click += new System.EventHandler(this.horizontalStopToolStripMenuItem_Click);
			// 
			// verticalStopToolStripMenuItem
			// 
			this.verticalStopToolStripMenuItem.Name = "verticalStopToolStripMenuItem";
			this.verticalStopToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.verticalStopToolStripMenuItem.Text = "Vertical";
			this.verticalStopToolStripMenuItem.Click += new System.EventHandler(this.verticalStopToolStripMenuItem_Click);
			// 
			// trafficLightToolStripMenuItem
			// 
			this.trafficLightToolStripMenuItem.Name = "trafficLightToolStripMenuItem";
			this.trafficLightToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.trafficLightToolStripMenuItem.Text = "Traffic Light";
			this.trafficLightToolStripMenuItem.Click += new System.EventHandler(this.trafficLightToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
			// 
			// startPositionToolStripMenuItem
			// 
			this.startPositionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.northStartToolStripMenuItem,
            this.eastStartToolStripMenuItem,
            this.southStartToolStripMenuItem,
            this.westStartToolStripMenuItem});
			this.startPositionToolStripMenuItem.Name = "startPositionToolStripMenuItem";
			this.startPositionToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.startPositionToolStripMenuItem.Text = "Start Position";
			// 
			// northStartToolStripMenuItem
			// 
			this.northStartToolStripMenuItem.Name = "northStartToolStripMenuItem";
			this.northStartToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.northStartToolStripMenuItem.Text = "North";
			this.northStartToolStripMenuItem.Click += new System.EventHandler(this.northStartToolStripMenuItem_Click);
			// 
			// eastStartToolStripMenuItem
			// 
			this.eastStartToolStripMenuItem.Name = "eastStartToolStripMenuItem";
			this.eastStartToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.eastStartToolStripMenuItem.Text = "East";
			this.eastStartToolStripMenuItem.Click += new System.EventHandler(this.eastStartToolStripMenuItem_Click);
			// 
			// southStartToolStripMenuItem
			// 
			this.southStartToolStripMenuItem.Name = "southStartToolStripMenuItem";
			this.southStartToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.southStartToolStripMenuItem.Text = "South";
			this.southStartToolStripMenuItem.Click += new System.EventHandler(this.southStartToolStripMenuItem_Click);
			// 
			// westStartToolStripMenuItem
			// 
			this.westStartToolStripMenuItem.Name = "westStartToolStripMenuItem";
			this.westStartToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.westStartToolStripMenuItem.Text = "West";
			this.westStartToolStripMenuItem.Click += new System.EventHandler(this.westStartToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(141, 6);
			// 
			// clearToolStripMenuItem
			// 
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.clearToolStripMenuItem.Text = "Clear";
			this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.toolStripSeparator6,
            this.toolStripMenuItemZoom200,
            this.toolStripMenuItemZoom100,
            this.toolStripMenuItemZoom75,
            this.toolStripMenuItemZoom50});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.refreshToolStripMenuItem.Text = "Refresh";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(110, 6);
			// 
			// toolStripMenuItemZoom200
			// 
			this.toolStripMenuItemZoom200.Name = "toolStripMenuItemZoom200";
			this.toolStripMenuItemZoom200.Size = new System.Drawing.Size(113, 22);
			this.toolStripMenuItemZoom200.Text = "200%";
			this.toolStripMenuItemZoom200.Click += new System.EventHandler(this.toolStripMenuItemZoom200_Click);
			// 
			// toolStripMenuItemZoom100
			// 
			this.toolStripMenuItemZoom100.Name = "toolStripMenuItemZoom100";
			this.toolStripMenuItemZoom100.Size = new System.Drawing.Size(113, 22);
			this.toolStripMenuItemZoom100.Text = "100%";
			this.toolStripMenuItemZoom100.Click += new System.EventHandler(this.toolStripMenuItemZoom100_Click);
			// 
			// toolStripMenuItemZoom75
			// 
			this.toolStripMenuItemZoom75.Name = "toolStripMenuItemZoom75";
			this.toolStripMenuItemZoom75.Size = new System.Drawing.Size(113, 22);
			this.toolStripMenuItemZoom75.Text = "75%";
			this.toolStripMenuItemZoom75.Click += new System.EventHandler(this.toolStripMenuItemZoom50_Click);
			// 
			// toolStripMenuItemZoom50
			// 
			this.toolStripMenuItemZoom50.Name = "toolStripMenuItemZoom50";
			this.toolStripMenuItemZoom50.Size = new System.Drawing.Size(113, 22);
			this.toolStripMenuItemZoom50.Text = "50%";
			this.toolStripMenuItemZoom50.Click += new System.EventHandler(this.toolStripMenuItemZoom25_Click);
			// 
			// mapToolStripMenuItem
			// 
			this.mapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trimToolStripMenuItem,
            this.addRowsToolStripMenuItem,
            this.addColumnToolStripMenuItem,
            this.rotate90ToolStripMenuItem});
			this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
			this.mapToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.mapToolStripMenuItem.Text = "Map";
			// 
			// trimToolStripMenuItem
			// 
			this.trimToolStripMenuItem.Name = "trimToolStripMenuItem";
			this.trimToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.trimToolStripMenuItem.Text = "Trim";
			this.trimToolStripMenuItem.Click += new System.EventHandler(this.trimToolStripMenuItem_Click);
			// 
			// addRowsToolStripMenuItem
			// 
			this.addRowsToolStripMenuItem.Name = "addRowsToolStripMenuItem";
			this.addRowsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.addRowsToolStripMenuItem.Text = "Add Row";
			this.addRowsToolStripMenuItem.Click += new System.EventHandler(this.addRowsToolStripMenuItem_Click);
			// 
			// addColumnToolStripMenuItem
			// 
			this.addColumnToolStripMenuItem.Name = "addColumnToolStripMenuItem";
			this.addColumnToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.addColumnToolStripMenuItem.Text = "Add Column";
			this.addColumnToolStripMenuItem.Click += new System.EventHandler(this.addColumnToolStripMenuItem_Click);
			// 
			// mapDisplay
			// 
			this.mapDisplay.AutoScroll = true;
			this.mapDisplay.DisplayCoordinates = false;
			this.mapDisplay.DisplayShadows = false;
			this.mapDisplay.DisplayShadowsAll = false;
			this.mapDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapDisplay.Location = new System.Drawing.Point(0, 24);
			this.mapDisplay.Name = "mapDisplay";
			this.mapDisplay.Size = new System.Drawing.Size(742, 713);
			this.mapDisplay.TabIndex = 1;
			this.mapDisplay.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapDisplay_MouseClick);
			this.mapDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMap_MouseDown);
			this.mapDisplay.MouseLeave += new System.EventHandler(this.FormMap_MouseLeave);
			this.mapDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMap_MouseMove);
			this.mapDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMap_MouseUp);
			// 
			// rotate90ToolStripMenuItem
			// 
			this.rotate90ToolStripMenuItem.Name = "rotate90ToolStripMenuItem";
			this.rotate90ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.rotate90ToolStripMenuItem.Text = "Rotate 90";
			this.rotate90ToolStripMenuItem.Click += new System.EventHandler(this.rotate90ToolStripMenuItem_Click);
			// 
			// FormMap
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(742, 737);
			this.Controls.Add(this.mapDisplay);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormMap";
			this.Text = "Windwardopolis Map Builder - [{0}]";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMap_FormClosing);
			this.Load += new System.EventHandler(this.FormMap_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem objectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem roadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem busStopToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem companyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem parkToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private WindwardopolisLibrary.map.MapDisplay mapDisplay;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem stopSignToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem horizontalStopToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verticalStopToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem trafficLightToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem startPositionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem northStartToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem eastStartToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem southStartToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem westStartToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom200;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom100;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom75;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom50;
		private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem trimToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addRowsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addColumnToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rotate90ToolStripMenuItem;
	}
}

