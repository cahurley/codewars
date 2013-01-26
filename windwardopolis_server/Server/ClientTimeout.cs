using System;
using System.Windows.Forms;

namespace Windwardopolis
{
	public partial class ClientTimeout : Form
	{
		public ClientTimeout(int movesPerSecond)
		{
			InitializeComponent();
#if DEBUG_MODE
			spinTimeoutSeconds.Maximum = int.MaxValue;
#else
			spinTimeoutSeconds.Maximum = 30;
#endif
			spinTimeoutSeconds.Value = Math.Min(Math.Max(1, movesPerSecond), spinTimeoutSeconds.Maximum);
		}

		public int TimeoutSeconds
		{
			get { return (int) spinTimeoutSeconds.Value; }
		}
	}
}
