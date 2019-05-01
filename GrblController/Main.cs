using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrblController
{
	public partial class Main : Form
	{
		private Label[] table1Labels;
		private Label[] table2Labels;

		internal static Main Instance { get; private set; }

		internal GeometryController GeometryController { get; private set; }
		internal Connection Connection { get; private set; }
		internal Parameters Parameters { get; private set; }

		internal double Table1PaintedAreaRatio
		{
			get
			{
				return table1Slider.Value / 4.0;
			}
		}

		internal double Table2PaintedAreaRatio
		{
			get
			{
				return table2Slider.Value / 4.0;
			}
		}

		public Main()
		{
			InitializeComponent();
			table1Labels = new Label[] { table1Label0, table1Label1d4, table1Label1d2, table1Label3d4, table1LabelFull };
			table2Labels = new Label[] { table2Label0, table2Label1d4, table2Label1d2, table2Label3d4, table2LabelFull };
			Parameters = Parameters.ReadFromFile();
			Connection = new Connection();
			GeometryController = new GeometryController();
			Instance = this;
		}

		private void Main_Load(object sender, EventArgs e)
		{
			Connection.Initialize();

			Parameters = Parameters.ReadFromFile();
			table1Slider_ValueChanged(null, null);
			table2Slider_ValueChanged(null, null);
			UpdateXPanel();

			Connection.onStatusChanged += StatusChanged;

			label2.Text = "Machine " + Parameters.ControlAxis.ToString() + " position.";
			StatusChanged(Connection.Status, Connection.Status);
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

			switch (Connection.Status.ConnectionState)
			{
				case ConnectionState.DisconnectedCannotConnect:
					connectDisconnectButton.Text = "Connect";
					connectDisconnectButton.Enabled = false;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					break;
				case ConnectionState.DisconnectedCanConnect:
					connectDisconnectButton.Text = "Connect";
					connectDisconnectButton.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					break;
				case ConnectionState.Connecting:
					connectDisconnectButton.Text = "Connecting (stop)";
					connectDisconnectButton.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					AddLog("Connecting...");
					break;
				case ConnectionState.ConnectedStarted:
					connectDisconnectButton.Text = "Disconnect";
					connectDisconnectButton.Enabled = true;
					startStopButton.Text = "Stop";
					startStopButton.Enabled = true;
					break;
				case ConnectionState.ConnectedStopped:
					connectDisconnectButton.Text = "Disconnect";
					connectDisconnectButton.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = true;
					break;
			}

			if ((oldStatus.ConnectionState == ConnectionState.Connecting || oldStatus.ConnectionState == ConnectionState.DisconnectedCanConnect
					|| oldStatus.ConnectionState == ConnectionState.DisconnectedCannotConnect) &&
				(newStatus.ConnectionState == ConnectionState.ConnectedStarted || newStatus.ConnectionState == ConnectionState.ConnectedStopped))
			{
				AddLog("Connected.");
			}

			if ((oldStatus.ConnectionState == ConnectionState.Connecting || oldStatus.ConnectionState == ConnectionState.ConnectedStarted
					|| oldStatus.ConnectionState == ConnectionState.ConnectedStopped) &&
				(newStatus.ConnectionState == ConnectionState.DisconnectedCanConnect || newStatus.ConnectionState == ConnectionState.DisconnectedCannotConnect))
			{
				AddLog("Disconnected.");
			}

			if (oldStatus.ConnectionState != ConnectionState.ConnectedStarted && newStatus.ConnectionState == ConnectionState.ConnectedStarted)
			{
				AddLog("Started sending G codes.");
			}

			if (oldStatus.ConnectionState == ConnectionState.ConnectedStarted && newStatus.ConnectionState != ConnectionState.ConnectedStarted)
			{
				AddLog("Stopped sending G codes.");
			}

			Parameters = Parameters.ReadFromFile();

			switch (Parameters.ControlAxis)
			{
				case ControlAxis.X:
					machinePositionLabel.Text = newStatus.MachinePosition.X.ToString("0.0 mm");
					break;
				case ControlAxis.Y:
					machinePositionLabel.Text = newStatus.MachinePosition.Y.ToString("0.0 mm");
					break;
				case ControlAxis.Z:
					machinePositionLabel.Text = newStatus.MachinePosition.Z.ToString("0.0 mm");
					break;
			}

			UpdateXPanel();
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
			double dTotalLength = Parameters.StartOffset + Parameters.Table1Length + Parameters.MiddleGap + Parameters.Table2Length + Parameters.EndOffset;

			table1Panel.Location = new Point(0, (int)(tablePanel.Height * Parameters.StartOffset / dTotalLength));
			table1Panel.Size = new Size(tablePanel.Width - 2, (int)(tablePanel.Height * Parameters.Table1Length / dTotalLength));

			table2Panel.Location = new Point(0, (int)(tablePanel.Height * (Parameters.StartOffset + Parameters.Table1Length + Parameters.MiddleGap) / dTotalLength));
			table2Panel.Size = new Size(tablePanel.Width - 2, (int)(tablePanel.Height * Parameters.Table2Length / dTotalLength));

			table1PanelPaintedArea.Location = new Point(0, 0);
			table1PanelPaintedArea.Size = new Size(table1Panel.Width - 2, (int)(table1Panel.Size.Height * Table1PaintedAreaRatio));

			table2PanelPaintedArea.Location = new Point(0, 0);
			table2PanelPaintedArea.Size = new Size(table1Panel.Width - 2, (int)(table2Panel.Size.Height * Table2PaintedAreaRatio));
		}

		private void UpdateXPanel()
		{
			machinePositionPanel.Location = new Point(0, (int)(Connection.Status.MachinePosition.Y / Parameters.TablesTotalLength * tablePanel.Height - machinePositionPanel.Size.Height / 2) - 1);
			machinePositionPanel.BackColor = Connection.Status.Painting ? Color.Red : Color.Blue;
		}

		private void tableLengthsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((new TableLengthSettings()).ShowDialog() == DialogResult.OK)
			{
				table1Slider_ValueChanged(null, null);
				table2Slider_ValueChanged(null, null);
				Parameters = Parameters.ReadFromFile();
			}
		}

		private void serialPortToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((new SerialPortSettings()).ShowDialog() == DialogResult.OK)
			{
				table1Slider_ValueChanged(null, null);
				table2Slider_ValueChanged(null, null);
				Parameters = Parameters.ReadFromFile();
				Connection.Disconnect();
			}
		}

		private void gRBLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var grblSettingsForm = new GrblSettings();
			grblSettingsForm.ShowDialog();
		}

		private void setMachinePositionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var setMachinePosition = new SetMachinePosition();
			if (setMachinePosition.ShowDialog() == DialogResult.OK)
			{
				Parameters = Parameters.ReadFromFile();

				switch (Parameters.ControlAxis)
				{
					case ControlAxis.X:
						Connection.Status.MachinePosition.X = setMachinePosition.MachinePosition;
						break;
					case ControlAxis.Y:
						Connection.Status.MachinePosition.Y = setMachinePosition.MachinePosition;
						break;
					case ControlAxis.Z:
						Connection.Status.MachinePosition.Z = setMachinePosition.MachinePosition;
						break;
				}

				Connection.SetStatus(Connection.Status);

				label2.Text = "Machine " + Parameters.ControlAxis.ToString() + " position.";
			}
		}

		private void connectDisconnectButton_Click(object sender, EventArgs e)
		{
			switch (Connection.Status.ConnectionState)
			{
				case ConnectionState.Connecting:
				case ConnectionState.ConnectedStarted:
				case ConnectionState.ConnectedStopped:
					Connection.Disconnect();
					break;
				case ConnectionState.DisconnectedCanConnect:
					Connection.Initialize();
					Connection.Connect();
					break;
			}
		}

		private void startStopButton_Click(object sender, EventArgs e)
		{
			if (Connection.Status.ConnectionState == ConnectionState.ConnectedStopped)
			{
				GeometryController.Start();
			}
			else if (Connection.Status.ConnectionState == ConnectionState.ConnectedStarted)
			{
				GeometryController.Stop();
			}
		}

		internal void AddLog(string s)
		{
			if (InvokeRequired)
			{
				Invoke(new Action(() =>
				{
					AddLog(s);
				}));
				return;
			}

			activityLog.Items.Add(DateTime.Now.ToString("hh:mm:ss") + "     " + s);
			int visibleItems = activityLog.ClientSize.Height / activityLog.ItemHeight;
			activityLog.TopIndex = Math.Max(activityLog.Items.Count - visibleItems + 1, 0);
		}
	}
}
