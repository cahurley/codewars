
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WindwardopolisLibrary.units;

namespace Windwardopolis
{
	public partial class PlayerScore : Form
	{
		private const int INDEX_FIRST_SCORE_COL = 3;

		private readonly List<PlayerStatus> playerStats = new List<PlayerStatus>();
		private DataTable table;
		private readonly Framework framework; 

		public PlayerScore(Framework framework)
		{
			InitializeComponent();
			this.framework = framework;
		}

		/// <summary>
		/// Starting a game.
		/// </summary>
		public void SetupGame()
		{

			// if have stats windows - dispose them
			while (playerStats.Count > 0)
			{
				PlayerStatus statOn = playerStats[0];
				playerStats.RemoveAt(0);
				panelPlayers.Controls.Remove(statOn);
				statOn.Dispose();
			}

			// create player stats windows
			NewPlayerAdded();
		}

		/// <summary>
		/// New player added to the game
		/// </summary>
		public void NewPlayerAdded()
		{

			if (framework.GameEngine == null)
				return;

			// create player stats windows
			foreach (Player playerOn in framework.GameEngine.Players)
			{
				if (playerStats.Any(psOn => psOn.Player == playerOn))
					continue;

				PlayerStatus ps = new PlayerStatus(playerOn) {Width = panelPlayers.Width/2};

				ps.Top = (playerStats.Count/2)*ps.Height;
				if ((playerStats.Count & 1) == 0)
					ps.Left = 0;
				else
					ps.Left = panelPlayers.Width/2;
				playerStats.Add(ps);
				panelPlayers.Controls.Add(ps);
			}

			// order them by name
			int height = -1;
			int winOn = 0;
			panelPlayers.SuspendLayout();
			playerStats.Sort((a,b) => (a.Player.Name.CompareTo(b.Player.Name)));
			foreach (PlayerStatus psOn in playerStats)
			{
				// set up top position
				if (height == -1)
					height = psOn.Height;

				psOn.Top = (winOn/2)*height;
				if ((winOn & 1) == 0)
					psOn.Left = 0;
				else
					psOn.Left = panelPlayers.Width/2;
				winOn++;
			}
			if (playerStats.Count > 0)
				panelPlayers.Height = ((playerStats.Count + 1) / 2) * playerStats[0].Height;
			panelPlayers.ResumeLayout();
			UpdatePlayers();
		}

		/// <summary>
		/// Update (re-draw) the player status windows.
		/// </summary>
		public void UpdatePlayers()
		{

			if (playerStats.Count == 0)
				return;

			foreach (PlayerStatus ps in playerStats)
				ps.UpdateStats();
			panelPlayers.Invalidate();
			panelPlayers.Update();

			UpdateScoreboard();
		}

		/// <summary>
		/// Called to delete all player status windows the player status windows.
		/// </summary>
		public void ResetPlayers()
		{

			if (table != null)
			{
				table.Dispose();
				table = null;
				UpdateScoreboard();
			}

			if (playerStats.Count != 0)
			{
				foreach (PlayerStatus ps in playerStats)
					ps.Dispose();
				playerStats.Clear();

				panelPlayers.Invalidate();
				panelPlayers.Update();
			}
		}

		private void UpdateScoreboard()
		{

			if (playerStats.Count == 0)
				return;

			bool incGameOn = framework.Mode != Framework.COMM_STATE.GAME_OVER;

			// create a new table (new game or player added)?
			if ((table == null) || (table.Rows.Count != playerStats.Count) ||
									(table.Columns.Count != playerStats[0].Player.Scoreboard.Count + (incGameOn ? 1 : 0) + INDEX_FIRST_SCORE_COL + 1))
			{

				table = new DataTable();
				table.Columns.Add("Player", typeof(string));
				table.Columns.Add("School", typeof(string));
				table.Columns.Add("Lang", typeof(string));
				for (int ind = 0; ind < playerStats[0].Player.Scoreboard.Count + (incGameOn ? 1 : 0); ind++)
					table.Columns.Add("Game " + (ind + 1), typeof(float));
				table.Columns.Add("Total", typeof(float));
				foreach (PlayerStatus plyrOn in playerStats.OrderBy(pl => pl.Player.Name))
				{
					List<object> cells = new List<object> {plyrOn.Player.Name, plyrOn.Player.School, plyrOn.Player.Language};
					float total = 0;
					foreach (float score in plyrOn.Player.Scoreboard)
					{
						cells.Add(score);
						total += score;
					}
					if (incGameOn)
					{
						cells.Add(plyrOn.Player.Score);
						total += plyrOn.Player.Score;
					}
					cells.Add(total);
					table.Rows.Add(cells.ToArray());
				}

				dataGridView.SuspendLayout();
				dataGridView.DataSource = null;
				dataGridView.DataSource = table;
                for (int ind = 1; ind < dataGridView.Columns.Count; ind++ )
                    dataGridView.Columns[ind].DefaultCellStyle.Format = "0.##";
                dataGridView.ResumeLayout();
				return;
			}

			// update existing table
			dataGridView.SuspendLayout();
			foreach (PlayerStatus plyrOn in playerStats.OrderBy(pl => pl.Player.Name))
			{
				DataRow row = table.Rows.Cast<DataRow>().FirstOrDefault(rowOn => (string)rowOn[0] == plyrOn.Player.Name);
				if (row == null)
					continue;
				float total = 0;
				int colOn = INDEX_FIRST_SCORE_COL;
				foreach (float score in plyrOn.Player.Scoreboard)
				{
					row[colOn++] = score;
					total += score;
				}
				if (incGameOn)
				{
					row[colOn++] = plyrOn.Player.Score;
					total += plyrOn.Player.Score;
				}
				row[colOn] = total;
			}
			dataGridView.ResumeLayout();
		}

        private void PlayerScore_Load(object sender, System.EventArgs e)
        {
            MainWindow.RestoreWindow(this, "scoreboard");
        }

        private void panelPlayers_SizeChanged(object sender, System.EventArgs e)
        {
        	int wid = ClientRectangle.Width / 2;
        	for (int index = 0; index < playerStats.Count; index++)
        	{
        		PlayerStatus psOn = playerStats[index];
        		psOn.Left = (index & 0x01) == 0x01 ? wid : 0;
				psOn.Width = wid;
        	}
        }
	}
}
