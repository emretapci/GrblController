using System;
using System.Drawing;
using System.Windows.Forms;

namespace CncController
{
	public partial class Main : Form
	{
		private Label[] table1Labels;
		private Label[] table2Labels;
		private Parameters parameters;
		private Connection connection;

		public Main()
		{
			InitializeComponent();
			table1Labels = new Label[] { table1Label0, table1Label1d4, table1Label1d2, table1Label3d4, table1LabelFull };
			table2Labels = new Label[] { table2Label0, table2Label1d4, table2Label1d2, table2Label3d4, table2LabelFull };
			parameters = Parameters.ReadFromFile();
			connection = new Connection(AddLog);
		}

		private void Main_Load(object sender, EventArgs e)
		{
			parameters = Parameters.ReadFromFile();
			table1Slider_ValueChanged(null, null);
			table2Slider_ValueChanged(null, null);

			connection.onStatusChanged += StatusChanged;

			connection.Initialize(parameters);
		}

		private void StatusChanged(Status oldStatus, Status newStatus)
		{
			if(InvokeRequired)
			{
				Invoke(new Action(() =>
				{
					StatusChanged(oldStatus, newStatus);
				}));
				return;
			}

			switch (connection.Status)
			{
				case Status.DisconnectedCannotConnect:
					connectDisconnectButton.Text = "Connect";
					connectDisconnectButton.Enabled = false;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					break;
				case Status.DisconnectedCanConnect:
					connectDisconnectButton.Text = "Connect";
					connectDisconnectButton.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					break;
				case Status.Connecting:
					connectDisconnectButton.Text = "Connecting (stop)";
					connectDisconnectButton.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					AddLog("Connecting...");
					break;
				case Status.ConnectedStarted:
					connectDisconnectButton.Text = "Disconnect";
					connectDisconnectButton.Enabled = true;
					startStopButton.Text = "Stop";
					startStopButton.Enabled = true;
					break;
				case Status.ConnectedStopped:
					connectDisconnectButton.Text = "Disconnect";
					connectDisconnectButton.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = true;
					break;
			}

			if ((oldStatus == Status.Connecting || oldStatus == Status.DisconnectedCanConnect || oldStatus == Status.DisconnectedCannotConnect) &&
				(newStatus == Status.ConnectedStarted || newStatus == Status.ConnectedStopped))
			{
				AddLog("Connected.");
			}

			if ((oldStatus == Status.Connecting || oldStatus == Status.ConnectedStarted || oldStatus == Status.ConnectedStopped) &&
				(newStatus == Status.DisconnectedCanConnect || newStatus == Status.DisconnectedCannotConnect))
			{
				AddLog("Disconnected.");
			}

			if (oldStatus != Status.ConnectedStarted && newStatus == Status.ConnectedStarted)
			{
				AddLog("Started sending G codes.");
			}

			if (oldStatus == Status.ConnectedStarted && newStatus != Status.ConnectedStarted)
			{
				AddLog("Stopped sending G codes.");
			}
		}

		private void table1Slider_ValueChanged(object sender, EventArgs e)
		{
			foreach (var label in table1Labels)
			{
				label.ForeColor = SystemColors.ControlDark;
			}

			table1Labels[table1Slider.Value].ForeColor = Color.Black;
			ResizeTablePanels();
		}

		private void table2Slider_ValueChanged(object sender, EventArgs e)
		{
			foreach (var label in table2Labels)
			{
				label.ForeColor = SystemColors.ControlDark;
			}

			table2Labels[table2Slider.Value].ForeColor = Color.Black;
			ResizeTablePanels();
		}

		private void ResizeTablePanels()
		{
			double dTotalLength = parameters.StartOffset + parameters.Table1Length + parameters.MiddleGap + parameters.Table2Length + parameters.EndOffset;

			table1Panel.Location = new Point(0, (int)(tablePanel.Height * parameters.StartOffset / dTotalLength));
			table1Panel.Size = new Size(tablePanel.Width - 2, (int)(tablePanel.Height * parameters.Table1Length / dTotalLength));

			table2Panel.Location = new Point(0, (int)(tablePanel.Height * (parameters.StartOffset + parameters.Table1Length + parameters.MiddleGap) / dTotalLength));
			table2Panel.Size = new Size(tablePanel.Width - 2, (int)(tablePanel.Height * parameters.Table2Length / dTotalLength));

			table1PanelPaintedArea.Location = new Point(0, 0);
			table1PanelPaintedArea.Size = new Size(table1Panel.Width - 2, (int)(table1Panel.Size.Height * (table1Slider.Value / 4.0)));

			table2PanelPaintedArea.Location = new Point(0, 0);
			table2PanelPaintedArea.Size = new Size(table1Panel.Width - 2, (int)(table2Panel.Size.Height * (table2Slider.Value / 4.0)));
		}

		private void tableLengthsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((new TableLengthSettings()).ShowDialog() == DialogResult.OK)
			{
				Main_Load(null, null);
			}
		}

		private void serialPortToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((new SerialPortSettings()).ShowDialog() == DialogResult.OK)
			{
				Main_Load(null, null);
			}
		}

		private void gRBLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((new GrblSettings()).ShowDialog() == DialogResult.OK)
			{
				Main_Load(null, null);
			}
		}

		private void connectDisconnectButton_Click(object sender, EventArgs e)
		{
			switch (connection.Status)
			{
				case Status.Connecting:
				case Status.ConnectedStarted:
				case Status.ConnectedStopped:
					connection.Disconnect();
					break;
				case Status.DisconnectedCanConnect:
					connection.Connect();
					break;
			}
		}

		private void startStopButton_Click(object sender, EventArgs e)
		{
			if (connection.Status == Status.ConnectedStopped)
			{
				connection.Start();
			}
			else if (connection.Status == Status.ConnectedStarted)
			{
				connection.Stop();
			}
		}

		private void AddLog(string s)
		{
			if (InvokeRequired)
			{
				Invoke(new Action(() =>
				{
					AddLog(s);
				}));
				return;
			}

			activityLog.Items.Add(DateTime.Now.ToString("hh:mm:ss") + " " + s);
			int visibleItems = activityLog.ClientSize.Height / activityLog.ItemHeight;
			activityLog.TopIndex = Math.Max(activityLog.Items.Count - visibleItems + 1, 0);
		}
	}
}
