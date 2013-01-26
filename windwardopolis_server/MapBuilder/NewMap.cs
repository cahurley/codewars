using System;

using System.Windows.Forms;

namespace MapBuilder
{
	public partial class NewMap : Form
	{
		public NewMap()
		{
			InitializeComponent();
			textBoxSize_TextChanged(null, null);
		}

		public int MapHeight
		{
			get { return int.Parse(textBoxHeight.Text); }
		}

		public int MapWidth
		{
			get { return int.Parse(textBoxWidth.Text); }
		}

		private void textBoxSize_TextChanged(object sender, EventArgs e)
		{

			bool enabled = true;
			int num;
			if (!int.TryParse(textBoxHeight.Text, out num))
				enabled = false;
			else
				if (!int.TryParse(textBoxWidth.Text, out num))
					enabled = false;
			btnOk.Enabled = enabled;
		}
	}
}
