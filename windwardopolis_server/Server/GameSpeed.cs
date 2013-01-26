
using System;
using System.Windows.Forms;

namespace Windwardopolis
{
	public partial class GameSpeed : Form
	{
		public GameSpeed(int movesPerSecond)
		{
			InitializeComponent();
			spinMovesPerSec.Value = Math.Min (1000, movesPerSecond);
		}

		public int MovesPerSecond
		{
			get { return (int) spinMovesPerSec.Value; }
		}
	}
}
