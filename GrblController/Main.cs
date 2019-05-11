﻿using System;
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
			Connection.Initialize();

			ResizeSliders();

			table1Slider_ValueChanged(null, null);
			table2Slider_ValueChanged(null, null);
			UpdateMachinePositionPanel();

			Status.onStatusChanged += StatusChanged;

			Status.SetStatus(new Status());
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

		internal void SetSlidersEnabled(bool enabled)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Action(() =>
				{
					SetSlidersEnabled(enabled);
				}));
				return;
			}

			table1Slider.Enabled = enabled;
			table2Slider.Enabled = enabled;
		}

		internal void SetMenuEnabled(bool enabled)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Action(() =>
				{
					SetMenuEnabled(enabled);
				}));
				return;
			}

			menuStrip1.Enabled = enabled;
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
				calibrateToolStripMenuItem.Enabled = false;
				Text = "GRBL Controller [Disconnected]";
			}

			if (Status.Instance.ConnectionState == ConnectionState.Connecting)
			{
				startStopButton.Text = "Start";
				startStopButton.Enabled = false;
				homeButton.Enabled = false;
				resetButton.Enabled = false;
				AddLog("Connecting...");
				calibrateToolStripMenuItem.Enabled = false;
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
					calibrateToolStripMenuItem.Enabled = true;
					Text = "GRBL Controller [Connected] [Running]";
				}
				else if (Status.Instance.RunState == RunState.Stopped)
				{
					startStopButton.Text = "Start";
					startStopButton.Enabled = true;
					homeButton.Enabled = true;
					resetButton.Enabled = true;
					calibrateToolStripMenuItem.Enabled = true;
					Text = "GRBL Controller [Connected]";
				}
				else if (Status.Instance.RunState == RunState.Calibrating)
				{
					startStopButton.Text = "Start";
					startStopButton.Enabled = false;
					homeButton.Enabled = false;
					resetButton.Enabled = true;
					calibrateToolStripMenuItem.Enabled = false;
					Text = "GRBL Controller [Calibrating]";
				}
			}

			if (oldStatus.ConnectionState != ConnectionState.Connected && newStatus.ConnectionState == ConnectionState.Connected)
			{
				AddLog("Connected.");
			}

			if (oldStatus.ConnectionState == ConnectionState.Connected && newStatus.ConnectionState != ConnectionState.Connected)
			{
				AddLog("Disconnected.");
			}

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
			double dTotalLength = Parameters.Instance.StartOffset + Parameters.Instance.Table1Length + Parameters.Instance.MiddleGap + Parameters.Instance.Table2Length + Parameters.Instance.EndOffset;

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
				table1Slider_ValueChanged(null, null);
				table2Slider_ValueChanged(null, null);
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
