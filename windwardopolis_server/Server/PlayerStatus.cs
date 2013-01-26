// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.

using System.Drawing;
using System.Windows.Forms;
using WindwardopolisLibrary.units;

namespace Windwardopolis
{
	/// <summary>
	/// The window that displays a player's status.
	/// </summary>
	internal partial class PlayerStatus : UserControl
	{
		private static readonly Bitmap[] avatars = new [] { Avatars.avatar1, Avatars.avatar2, Avatars.avatar3, Avatars.avatar4 };
		private static int nextAvatar;

		private Passenger passengerOn;
		private Company nextBusStopOn;
		private bool firstTime = true;

		/// <summary>
		/// Create the window.
		/// </summary>
		/// <param name="player">The player this window is for.</param>
		public PlayerStatus(Player player)
		{
			InitializeComponent();
			Player = player;
		}

		/// <summary>
		/// The player this is showing status for.
		/// </summary>
		public Player Player { get; private set; }

		/// <summary>
		/// Redraw this window. Call when status has changed.
		/// </summary>
        public void UpdateStats()
		{

		    labelScore.Text = Player.Score.ToString("0.##");

			pictNoConnection.Visible = ! Player.IsConnected;

		    if (Player.Passenger != passengerOn || Player.NextBusStop != nextBusStopOn || firstTime)
		    {
		    	firstTime = false;

		        if (Player.Passenger == null)
		        {
		            labelPassenger.Text = @"{none}";
		            pictPassenger.Image = null;
					if (Player.NextBusStop != null)
					{
						labelDestination.Text = Player.NextBusStop.Name;
						pictDestination.Image = Player.NextBusStop.Logo;
					}
					else
					{
						labelDestination.Text = @"{none}";
						pictDestination.Image = null;
					}
		        }
		        else
		        {
		            labelPassenger.Text = Player.Passenger.Name;
		            pictPassenger.Image = Player.Passenger.Logo;
		            labelDestination.Text = Player.Passenger.Destination.Name;
                    pictDestination.Image = Player.Passenger.Destination.Logo;
                }
		        passengerOn = Player.Passenger;
		    	nextBusStopOn = Player.NextBusStop;
		    }
            Invalidate(true);
        }

	    private void PlayerStatus_Load(object sender, System.EventArgs e)
		{
//			BackColor = Player.SpriteColor;
			if ((Player.Avatar != null) && (Player.Avatar.Width == 32) && (Player.Avatar.Height == 32))
				pictureBoxAvatar.Image = Player.Avatar;
			else
			{
				pictureBoxAvatar.Image = avatars[nextAvatar++];
				if (nextAvatar >= avatars.Length)
					nextAvatar = 0;
			}
			pictureBoxRobot.Image = Player.Limo.VehicleBitmap;
			labelName.Text = Player.Name;

			if (Player.SpriteColor.R > 250 & Player.SpriteColor.G > 250)
				labelName.BackColor = Player.SpriteColor;
			else
				labelName.ForeColor = Player.SpriteColor;
			// colors
			/*
			if (Player.SpriteColor.GetBrightness() < 0.5)
			{
				labelScore.ForeColor = Color.White;
				labelName.ForeColor = Color.White;
				labelPassenger.ForeColor = Color.White;
				labelDestination.ForeColor = Color.White;
			}
			else
			{
				labelScore.ForeColor = Color.Black;
				labelName.ForeColor = Color.Black;
				labelPassenger.ForeColor = Color.Black;
				labelDestination.ForeColor = Color.Black;
			}
			 * */
		}

		private void PlayerStatus_Paint(object sender, PaintEventArgs pe)
		{
			// Delivered passenger avatars - 97, 4 - space = 3
			for (int ind = 0; ind < Player.PassengersDelivered.Count; ind++)
			{
				Bitmap bmp = Player.PassengersDelivered[ind].Logo;
				int width, height;
				if (bmp.Width >= bmp.Height)
				{
					width = 24;
					height = (bmp.Height*24)/bmp.Width;
				}
				else
				{
					height = 24;
					width = (bmp.Width*24)/bmp.Height;
				}
				pe.Graphics.DrawImage(bmp, new Rectangle(4 + ind*(24 + 6) + (24 - width) / 2, 82 + (24 - height), width, height));
			}
		}
	}
}
