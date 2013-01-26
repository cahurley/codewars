using System.Windows.Forms;

namespace Windwardopolis
{
	public partial class StatusMessages : Form
	{
		public StatusMessages()
		{
			InitializeComponent();
		}

		public void AddMessage(string message)
		{
			listBoxStatus.Items.Add(message);
			listBoxStatus.TopIndex = listBoxStatus.Items.Count - 1;
		}

        private void StatusMessages_Load(object sender, System.EventArgs e)
        {
            MainWindow.RestoreWindow(this, "status");
        }
	}
}
