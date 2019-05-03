using System;
using System.Drawing;
using System.Threading;
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
		internal string ConfigFilename { get { return "config.xml"; } }
		internal string DefaultFilename { get { return "default.xml"; } }

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
			Parameters.ReadFromFile(DefaultFilename); //if non-existent, create one.
			Parameters = Parameters.ReadFromFile(ConfigFilename);
			Connection = new Connection();
			GeometryController = new GeometryController();
			Instance = this;
		}

		private void Main_Load(object sender, EventArgs e)
		{
			Connection.Initialize();

			ResizeSliders();

			Parameters = Parameters.ReadFromFile(Main.Instance.ConfigFilename);
			table1Slider_ValueChanged(null, null);
			table2Slider_ValueChanged(null, null);
			UpdateMachinePositionPanel();

			Connection.onStatusChanged += StatusChanged;

			machineCoordinate.Text = "Machine " + Parameters.ControlAxis.ToString() + " position.";
			StatusChanged(Connection.Status, Connection.Status);
		}

		private void ResizeSliders()
		{
			slider1Panel.Size = new Size(slider1Panel.Size.Width, (tablePanel.Height - 5) / 2);
			slider1Panel.Location = new Point(tablePanel.Location.X + tablePanel.Size.Width + 5, tablePanel.Location.Y);

			slider2Panel.Size = slider1Panel.Size;
			slider2Panel.Location = new Point(slider1Panel.Location.X, slider1Panel.Location.Y + slider1Panel.Size.Height + 5);

			double ratio = 0.65;
			for (int i = 0; i < table1Labels.Length; i++)
			{
				table1Labels[i].Location = new Point(slider1Panel.Location.X + slider1Panel.Size.Width + 5, (int)(slider1Panel.Size.Height * (1 - ratio) / 2 + slider1Panel.Location.Y + i * ratio * slider1Panel.Height / (table1Labels.Length - 1) - table1Labels[i].Size.Height / 2));
			}
			for (int i = 0; i < table2Labels.Length; i++)
			{
				table2Labels[i].Location = new Point(slider2Panel.Location.X + slider2Panel.Size.Width + 5, (int)(slider2Panel.Size.Height * (1 - ratio) / 2 + slider2Panel.Location.Y + i * ratio * slider2Panel.Height / (table2Labels.Length - 1) - table2Labels[i].Size.Height / 2));
			}
		}

		private void StatusChanged(Status oldStatus, Status newStatus)
		{
			if (InvokeRequired && !IsDisposed)
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
					connectToolStripMenuItem.Text = "Connect";
					connectToolStripMenuItem.Enabled = false;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					calibrateToolStripMenuItem.Enabled = false;
					Text = "GRBL Controller [Disconnected]";
					break;
				case ConnectionState.DisconnectedCanConnect:
					connectToolStripMenuItem.Text = "Connect";
					connectToolStripMenuItem.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					calibrateToolStripMenuItem.Enabled = false;
					Text = "GRBL Controller [Disconnected]";
					break;
				case ConnectionState.Connecting:
					connectToolStripMenuItem.Text = "Connecting (stop)";
					connectToolStripMenuItem.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					AddLog("Connecting...");
					calibrateToolStripMenuItem.Enabled = false;
					Text = "GRBL Controller [Connecting...]";
					break;
				case ConnectionState.ConnectedStarted:
					connectToolStripMenuItem.Text = "Disconnect";
					connectToolStripMenuItem.Enabled = true;
					startStopButton.Text = "Stop";
					startStopButton.Enabled = true;
					calibrateToolStripMenuItem.Enabled = true;
					Text = "GRBL Controller [Connected] [Running]";
					break;
				case ConnectionState.ConnectedStopped:
					connectToolStripMenuItem.Text = "Disconnect";
					connectToolStripMenuItem.Enabled = true;
					startStopButton.Text = "Start";
					startStopButton.Enabled = true;
					calibrateToolStripMenuItem.Enabled = true;
					Text = "GRBL Controller [Connected]";
					break;
				case ConnectionState.ConnectedCalibrating:
					connectToolStripMenuItem.Text = "Disconnect";
					connectToolStripMenuItem.Enabled = false;
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					calibrateToolStripMenuItem.Enabled = false;
					Text = "GRBL Controller [Calibrating]";
					break;
			}

			Parameters = Parameters.ReadFromFile(ConfigFilename);

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

			machineCoordinate.Text = newStatus.MachineCoordinate.ToString("0.0 mm");
			machineState.Text = "Machine state: " + newStatus.MachineState.ToString();

			UpdateMachinePositionPanel();
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

		private void UpdateMachinePositionPanel()
		{
			machinePositionPanel.Location = new Point(0, (int)(Math.Abs(Connection.Status.MachineCoordinate) / Parameters.TablesTotalLength * tablePanel.Height - machinePositionPanel.Size.Height / 2) - 1);
			machinePositionPanel.BackColor = Connection.Status.Painting ? Color.Red : Color.Blue;
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
			if (InvokeRequired && !IsDisposed)
			{
				Invoke(new Action(() =>
				{
					AddLog(s);
				}));
				return;
			}

			while(activityLog.Items.Count >= 50)
			{
				activityLog.Items.RemoveAt(0);
			}

			activityLog.Items.Add(DateTime.Now.ToString("hh:mm:ss") + "     " + s);
			int visibleItems = activityLog.ClientSize.Height / activityLog.ItemHeight;
			activityLog.TopIndex = Math.Max(activityLog.Items.Count - visibleItems + 1, 0);
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			Connection.Disconnect();
			GeometryController.Stop();
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var settingsForm = new Settings();
			if (settingsForm.ShowDialog() == DialogResult.OK)
			{
				table1Slider_ValueChanged(null, null);
				table2Slider_ValueChanged(null, null);
				Parameters = Parameters.ReadFromFile(ConfigFilename);
				Connection.Status.MachineCoordinate = settingsForm.MachinePosition;
				machineCoordinate.Text = "Machine " + Parameters.ControlAxis.ToString() + " position.";
			}
		}

		private void connectToolStripMenuItem_Click(object sender, EventArgs e)
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

		private void calibrateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Connection.Calibrate();
		}
	}
}
