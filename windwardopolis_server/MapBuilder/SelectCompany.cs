using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindwardopolisLibrary.units;

namespace MapBuilder
{
	public partial class SelectCompany : Form
	{
		/// <summary>
		/// Create the dialog box.
		/// </summary>
		/// <param name="companies">All companies.</param>
		public SelectCompany(IList<Company> companies)
		{
			InitializeComponent();

			foreach (var companyOn in companies)
				listBoxCompanies.Items.Add(companyOn);

			btnOk.Enabled = listBoxCompanies.SelectedIndex != -1;
		}

		/// <summary>
		/// The selected company.
		/// </summary>
		public Company Company
		{
			get { return listBoxCompanies.SelectedItem as Company; }
		}

		private void listBoxCompanies_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnOk.Enabled = listBoxCompanies.SelectedIndex != -1;
		}

		private void listBoxCompanies_DoubleClick(object sender, EventArgs e)
		{
			if (listBoxCompanies.SelectedIndex == -1)
				return;
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
