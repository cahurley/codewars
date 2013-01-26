namespace WindwardopolisLibrary.map
{
	partial class MapDisplay
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
			this.SuspendLayout();
			// 
			// MapDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.DoubleBuffered = true;
			this.Name = "MapDisplay";
			this.Size = new System.Drawing.Size(421, 561);
			this.Load += new System.EventHandler(this.MapDisplay_Load);
			this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.MapDisplay_Scroll);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MapDisplay_Paint);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MapDisplay_MouseClick);
			this.Resize += new System.EventHandler(this.MapDisplay_Resize);
			this.ResumeLayout(false);

		}

		#endregion

	}
}
