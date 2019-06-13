using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace GrblController
{
	public partial class Main : Form
	{
		private Label[] table1Labels;
		private Label[] table2Labels;

		internal static Main Instance { get; private set; }

		internal GeometryController GeometryController { get; private set; }
		internal Connection Connection { get; private set; }

		internal double Table1PaintedArea //0-4
		{
			get
			{
				return table1Slider.Value;
			}
		}

		internal double Table2PaintedArea //0-4
		{
			get
			{
				return table2Slider.Value;
			}
		}

		public Main()
		{
			InitializeComponent();
			table1Labels = new Label[] { table1Label0, table1Label1d4, table1Label1d2, table1Label3d4, table1LabelFull };
			table2Labels = new Label[] { table2Label0, table2Label1d4, table2Label1d2, table2Label3d4, table2LabelFull };
			Connection = new Connection();
			GeometryController = new GeometryController();
			Instance = this;
		}

		private void Main_Load(object sender, EventArgs e)
		{
			Status.onStatusChanged += StatusChanged;

			Connection.Initialize();

			SetTables();

			UpdateMachinePositionPanel();

			Status.SetStatus(new Status());
		}

		private void SetTables()
		{
			table2Panel.Visible = Parameters.Instance.DoubleTable;
			table2PanelPaintedArea.Visible = Parameters.Instance.DoubleTable;
			table2Slider.Visible = Parameters.Instance.DoubleTable;

			ResizeSliders();

			table1Slider_ValueChanged(null, null);
			table2Slider_ValueChanged(null, null);
		}

		private void ResizeSliders()
		{
			tablePanel.Size = new Size(tablePanel.Size.Width,
				(homeButton.Location.Y + homeButton.Size.Height - activityLog.Location.Y) / (Parameters.Instance.DoubleTable ? 1 : 2));

			slider1Panel.Size = new Size(slider1Panel.Size.Width, Parameters.Instance.DoubleTable ? (tablePanel.Height - 5) / 2 : tablePanel.Height);
			slider1Panel.Location = new Point(tablePanel.Location.X + tablePanel.Size.Width + 5, tablePanel.Location.Y);

			slider2Panel.Size = slider1Panel.Size;
			slider2Panel.Location = new Point(slider1Panel.Location.X, slider1Panel.Location.Y + slider1Panel.Size.Height + 5);

			slider2Panel.Visible = Parameters.Instance.DoubleTable;
			foreach (var label in table2Labels)
				label.Visible = Parameters.Instance.DoubleTable;

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
			if (InvokeRequired)
			{
				BeginInvoke(new Action(() =>
				{
					StatusChanged(oldStatus, newStatus);
				}));
				return;
			}

			if (Status.Instance.ConnectionState == ConnectionState.Disconnected)
			{
				startStopButton.Text = "Start";
				startStopButton.Enabled = false;
				homeButton.Enabled = false;
				resetButton.Enabled = false;
				purgeButton.Enabled = false;
				calibrateToolStripMenuItem.Enabled = false;
				menuStrip1.Enabled = true;
				Text = "GRBL Controller [Disconnected]";
			}

			if (Status.Instance.ConnectionState == ConnectionState.Connecting)
			{
				startStopButton.Text = "Start";
				startStopButton.Enabled = false;
				homeButton.Enabled = false;
				resetButton.Enabled = false;
				purgeButton.Enabled = false;
				calibrateToolStripMenuItem.Enabled = false;
				menuStrip1.Enabled = true;
				Text = "GRBL Controller [Connecting...]";
			}

			if (Status.Instance.ConnectionState == ConnectionState.Connected)
			{
				if (Status.Instance.RunState == RunState.Running)
				{
					startStopButton.Text = "Stop";
					startStopButton.Enabled = true;
					homeButton.Enabled = false;
					resetButton.Enabled = true;
					purgeButton.Enabled = false;
					calibrateToolStripMenuItem.Enabled = true;
					menuStrip1.Enabled = true;
					Text = "GRBL Controller [Connected] [Running]";
				}
				else if (Status.Instance.RunState == RunState.Stopped)
				{
					startStopButton.Text = "Start";
					startStopButton.Enabled = true;
					homeButton.Enabled = true;
					resetButton.Enabled = true;
					purgeButton.Enabled = true;
					calibrateToolStripMenuItem.Enabled = true;
					menuStrip1.Enabled = true;
					Text = "GRBL Controller [Connected]";
				}
				else if (Status.Instance.RunState == RunState.Calibrating)
				{
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					homeButton.Enabled = false;
					resetButton.Enabled = true;
					purgeButton.Enabled = false;
					calibrateToolStripMenuItem.Enabled = false;
					menuStrip1.Enabled = false;
					Text = "GRBL Controller [Calibrating]";
				}
				else if (Status.Instance.RunState == RunState.Purging)
				{
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					homeButton.Enabled = false;
					resetButton.Enabled = true;
					purgeButton.Enabled = false;
					calibrateToolStripMenuItem.Enabled = false;
					menuStrip1.Enabled = false;
					Text = "GRBL Controller [Purging]";
				}
			}
			portStatusLabel.Text = Status.Instance.ConnectingPort;
			portStatusLabel.Visible = !string.IsNullOrWhiteSpace(Status.Instance.ConnectingPort);

			if (oldStatus.ConnectionState != ConnectionState.Connecting && newStatus.ConnectionState == ConnectionState.Connecting)
			{
				AddLog("Connecting...");
			}

			if (oldStatus.ConnectionState != ConnectionState.Connected && newStatus.ConnectionState == ConnectionState.Connected)
			{
				AddLog("Connected.");
			}

			if (oldStatus.ConnectionState != ConnectionState.Disconnected && newStatus.ConnectionState == ConnectionState.Disconnected)
			{
				AddLog("Disconnected.");
			}

			table1Slider.Enabled = table2Slider.Enabled = !(Status.Instance.ConnectionState == ConnectionState.Connected && Status.Instance.RunState == RunState.Running);

			machineCoordinate.Text = "Machine position: " + (double.IsNaN(newStatus.MachineCoordinate) ? "Unknown" : Math.Abs(newStatus.MachineCoordinate).ToString("0.0 mm"));
			machineState.Text = "Machine state: " + newStatus.MachineState.ToString();

			UpdateMachinePositionPanel();

			if (statusUpdatedAlarmPanel.BackColor == Color.Red)
				statusUpdatedAlarmPanel.BackColor = DefaultBackColor;
			else
				statusUpdatedAlarmPanel.BackColor = Color.Red;
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
			double dTotalLength;
			if(Parameters.Instance.DoubleTable)
				dTotalLength = Parameters.Instance.StartOffset + Parameters.Instance.Table1Length + Parameters.Instance.MiddleGap + Parameters.Instance.Table2Length + Parameters.Instance.EndOffset;
			else
				dTotalLength = Parameters.Instance.StartOffset + Parameters.Instance.Table1Length + Parameters.Instance.EndOffset;

			table1Panel.Location = new Point(0, (int)(tablePanel.Height * Parameters.Instance.StartOffset / dTotalLength));
			table1Panel.Size = new Size(tablePanel.Width - 2, (int)(tablePanel.Height * Parameters.Instance.Table1Length / dTotalLength));

			table2Panel.Location = new Point(0, (int)(tablePanel.Height * (Parameters.Instance.StartOffset + Parameters.Instance.Table1Length + Parameters.Instance.MiddleGap) / dTotalLength));
			table2Panel.Size = new Size(tablePanel.Width - 2, (int)(tablePanel.Height * Parameters.Instance.Table2Length / dTotalLength));

			table1PanelPaintedArea.Location = new Point(0, 0);
			table1PanelPaintedArea.Size = new Size(table1Panel.Width - 2, (int)(table1Panel.Size.Height * Table1PaintedArea / 4.0));

			table2PanelPaintedArea.Location = new Point(0, 0);
			table2PanelPaintedArea.Size = new Size(table1Panel.Width - 2, (int)(table2Panel.Size.Height * Table2PaintedArea / 4.0));
		}

		private void UpdateMachinePositionPanel()
		{
			machinePositionPanel.Location = new Point(0, (int)(Math.Abs(Status.Instance.MachineCoordinate) / Parameters.Instance.TablesTotalLength * tablePanel.Height - machinePositionPanel.Size.Height / 2) - 1);
			machinePositionPanel.BackColor = Status.Instance.Painting.Value ? Color.Red : Color.Blue;
		}

		private void startStopButton_Click(object sender, EventArgs e)
		{
			if (Status.Instance.ConnectionState == ConnectionState.Connected)
			{
				if (Status.Instance.RunState == RunState.Stopped)
				{
					Connection.Calibrate(() => GeometryController.Start());
				}
				else if (Status.Instance.RunState == RunState.Running)
				{
					GeometryController.Stop();
				}
			}
		}

		private void homeButton_Click(object sender, EventArgs e)
		{
			Connection.Calibrate(null);
		}

		private void resetButton_Click(object sender, EventArgs e)
		{
			AddLog("Resetting...");
			Connection.Reset();
		}

		private void purgeButton_Click(object sender, EventArgs e)
		{
			AddLog("Purging...");
			Connection.Purge();
		}

		internal void AddLog(string s)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Action(() =>
				{
					AddLog(s);
				}));
				return;
			}

			while (activityLog.Items.Count >= 50)
			{
				activityLog.Items.RemoveAt(0);
			}

			activityLog.Items.Add(DateTime.Now.ToString("hh:mm:ss") + "     " + s);
			int visibleItems = activityLog.ClientSize.Height / activityLog.ItemHeight;
			activityLog.TopIndex = Math.Max(activityLog.Items.Count - visibleItems + 1, 0);
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			Status.onStatusChanged -= StatusChanged;
			Connection.Disconnect();
			GeometryController.Stop();
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var settingsForm = new Settings();
			if (settingsForm.ShowDialog() == DialogResult.OK)
			{
				SetTables();
				Status.Instance.MachineCoordinate = settingsForm.MachinePosition;
				if (Status.Instance.ConnectionState == ConnectionState.Connected)
				{
					GeometryController.GoToCoordinate(Status.Instance.MachineCoordinate);
				}
				machineCoordinate.Text = "Machine " + Parameters.Instance.ControlAxis.ToString() + " position.";
			}
		}

		private void calibrateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Connection.Calibrate(null);
		}
	}
}
