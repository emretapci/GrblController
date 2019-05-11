using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace GrblController
{
	public partial class Settings : Form
	{
		internal double MachinePosition { get; private set; }

		public Settings()
		{
			InitializeComponent();
		}

		private void GrblSettings_Load(object sender, EventArgs e)
		{
			LoadSettings(Parameters.Instance);
		}

		private void LoadSettings(Parameters parameters)
		{
			#region Load GRBL settings to form

			stepPulseTime.Text = parameters.StepPulseTime.ToString();
			stepIdleDelay.Text = parameters.StepIdleDelay.ToString();
			stepPortInvertX.Checked = parameters.StepPortInvert.HasFlag(Mask.X);
			stepPortInvertY.Checked = parameters.StepPortInvert.HasFlag(Mask.Y);
			stepPortInvertZ.Checked = parameters.StepPortInvert.HasFlag(Mask.Z);
			directionPortInvertX.Checked = parameters.DirectionPortInvert.HasFlag(Mask.X);
			directionPortInvertY.Checked = parameters.DirectionPortInvert.HasFlag(Mask.Y);
			directionPortInvertZ.Checked = parameters.DirectionPortInvert.HasFlag(Mask.Z);
			stepEnableInvert.Checked = parameters.StepEnableInvert;
			limitPinsInvert.Checked = parameters.LimitPinsInvert;
			probePinInvert.Checked = parameters.ProbePinInvert;
			junctionDeviation.Text = parameters.JunctionDeviation.ToString();
			arcTolerance.Text = parameters.ArcTolerance.ToString();
			reportInches.Checked = parameters.ReportInches;
			softLimits.Checked = parameters.SoftLimits;
			hardLimits.Checked = parameters.HardLimits;
			homingCycle.Checked = parameters.HomingCycle;
			homingDirectionInvertX.Checked = parameters.HomingDirectionInvert.HasFlag(Mask.X);
			homingDirectionInvertY.Checked = parameters.HomingDirectionInvert.HasFlag(Mask.Y);
			homingDirectionInvertZ.Checked = parameters.HomingDirectionInvert.HasFlag(Mask.Z);
			homingFeed.Text = parameters.HomingFeed.ToString();
			homingSeek.Text = parameters.HomingSeek.ToString();
			homingDebounce.Text = parameters.HomingDebounce.ToString();
			homingPullOff.Text = parameters.HomingPullOff.ToString();
			maximumSpindleSpeed.Text = parameters.MaximumSpindleSpeed.ToString();
			minimumSpindleSpeed.Text = parameters.MinimumSpindleSpeed.ToString();
			laserMode.Checked = parameters.LaserMode;
			xSteps.Text = parameters.XSteps.ToString();
			ySteps.Text = parameters.YSteps.ToString();
			zSteps.Text = parameters.ZSteps.ToString();
			xMaximumRate.Text = parameters.XFeedRate.ToString();
			yMaximumRate.Text = parameters.YFeedRate.ToString();
			zMaximumRate.Text = parameters.ZFeedRate.ToString();
			xAcceleration.Text = parameters.XAcceleration.ToString();
			yAcceleration.Text = parameters.YAcceleration.ToString();
			zAcceleration.Text = parameters.ZAcceleration.ToString();
			xMaximumTravel.Text = parameters.XMaximumTravel.ToString();
			yMaximumTravel.Text = parameters.YMaximumTravel.ToString();
			zMaximumTravel.Text = parameters.ZMaximumTravel.ToString();

			#endregion

			#region Load Table settings to form

			startOffset.Text = parameters.StartOffset.ToString();
			table1Length.Text = parameters.Table1Length.ToString();
			middleGapLength.Text = parameters.MiddleGap.ToString();
			table2Length.Text = parameters.Table2Length.ToString();
			endOffset.Text = parameters.EndOffset.ToString();

			#endregion

			#region Load Machine settings to form

			controlAxisLabel.Text = "Machine " + Parameters.Instance.ControlAxis.ToString() + " position";
			machinePositionTextBox.Text = double.IsNaN(Status.Instance.MachineCoordinate) ? "Unknown" : Math.Abs(Status.Instance.MachineCoordinate).ToString("0.0");
			controlAxis.SelectedIndex = (int)parameters.ControlAxis;

			#endregion

			#region Load Serial port settings to form

			serialPortCombobox.Items.AddRange(SerialPort.GetPortNames());
			serialPortCombobox.SelectedIndex = Array.IndexOf(SerialPort.GetPortNames(), parameters.SerialPortString);

			if (serialPortCombobox.SelectedIndex == -1 && serialPortCombobox.Items.Count > 0)
			{
				serialPortCombobox.SelectedIndex = 0;
			}

			baudrateCombobox.Items.AddRange(new string[] { "110", "300", "600", "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" });
			baudrateCombobox.SelectedItem = parameters.Baudrate.ToString();

			#endregion
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			var oldParameters = new Parameters();

			#region Get GRBL settings from form

			Parameters.Instance.StepPulseTime = double.Parse(stepPulseTime.Text);
			Parameters.Instance.StepIdleDelay = double.Parse(stepIdleDelay.Text);
			Parameters.Instance.StepPortInvert = (stepPortInvertX.Checked ? Mask.X : 0) | (stepPortInvertY.Checked ? Mask.Y : 0) | (stepPortInvertZ.Checked ? Mask.Z : 0);
			Parameters.Instance.DirectionPortInvert = (directionPortInvertX.Checked ? Mask.X : 0) | (directionPortInvertY.Checked ? Mask.Y : 0) | (directionPortInvertZ.Checked ? Mask.Z : 0);
			Parameters.Instance.StepEnableInvert = stepEnableInvert.Checked;
			Parameters.Instance.LimitPinsInvert = limitPinsInvert.Checked;
			Parameters.Instance.ProbePinInvert = probePinInvert.Checked;
			Parameters.Instance.JunctionDeviation = double.Parse(junctionDeviation.Text);
			Parameters.Instance.ArcTolerance = double.Parse(arcTolerance.Text);
			Parameters.Instance.ReportInches = reportInches.Checked;
			Parameters.Instance.SoftLimits = softLimits.Checked;
			Parameters.Instance.HardLimits = hardLimits.Checked;
			Parameters.Instance.HomingCycle = homingCycle.Checked;
			Parameters.Instance.HomingDirectionInvert = (homingDirectionInvertX.Checked ? Mask.X : 0) | (homingDirectionInvertY.Checked ? Mask.Y : 0) | (homingDirectionInvertZ.Checked ? Mask.Z : 0);
			Parameters.Instance.HomingFeed = double.Parse(homingFeed.Text);
			Parameters.Instance.HomingSeek = double.Parse(homingSeek.Text);
			Parameters.Instance.HomingDebounce = double.Parse(homingDebounce.Text);
			Parameters.Instance.HomingPullOff = double.Parse(homingPullOff.Text);
			Parameters.Instance.MaximumSpindleSpeed = double.Parse(maximumSpindleSpeed.Text);
			Parameters.Instance.MinimumSpindleSpeed = double.Parse(minimumSpindleSpeed.Text);
			Parameters.Instance.LaserMode = laserMode.Checked;
			Parameters.Instance.XSteps = double.Parse(xSteps.Text);
			Parameters.Instance.YSteps = double.Parse(ySteps.Text);
			Parameters.Instance.ZSteps = double.Parse(zSteps.Text);
			Parameters.Instance.XFeedRate = double.Parse(xMaximumRate.Text);
			Parameters.Instance.YFeedRate = double.Parse(yMaximumRate.Text);
			Parameters.Instance.ZFeedRate = double.Parse(zMaximumRate.Text);
			Parameters.Instance.XAcceleration = double.Parse(xAcceleration.Text);
			Parameters.Instance.YAcceleration = double.Parse(yAcceleration.Text);
			Parameters.Instance.ZAcceleration = double.Parse(zAcceleration.Text);
			Parameters.Instance.XMaximumTravel = double.Parse(xMaximumTravel.Text);
			Parameters.Instance.YMaximumTravel = double.Parse(yMaximumTravel.Text);
			Parameters.Instance.ZMaximumTravel = double.Parse(zMaximumTravel.Text);

			#endregion

			#region Get Table settings from form

			try
			{
				Parameters.Instance.StartOffset = double.Parse(startOffset.Text);
				Parameters.Instance.Table1Length = double.Parse(table1Length.Text);
				Parameters.Instance.MiddleGap = double.Parse(middleGapLength.Text);
				Parameters.Instance.Table2Length = double.Parse(table2Length.Text);
				Parameters.Instance.EndOffset = double.Parse(endOffset.Text);
			}
			catch
			{
				MessageBox.Show("Error in parameters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			#endregion

			#region Get Machine settings from form

			double d;
			if (double.TryParse(machinePositionTextBox.Text, out d))
			{
				if (d < 0)
				{
					MessageBox.Show("\"Machine coordinate\" cannot be negative.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				else if (d <= Parameters.Instance.TablesTotalLength)
				{
					Parameters.Instance.ControlAxis = (ControlAxis)controlAxis.SelectedIndex;
					MachinePosition = d;
				}
				else
				{
					MessageBox.Show("\"Machine coordinate\" cannot be bigger than tables' total length.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			else if (machinePositionTextBox.Text != "Unknown")
			{
				MessageBox.Show("Invalid value for \"Machine coordinate\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			#endregion

			#region Get Serial port settings from form

			Parameters.Instance.SerialPortString = serialPortCombobox.SelectedItem != null ? serialPortCombobox.SelectedItem.ToString() : "";
			Parameters.Instance.Baudrate = int.Parse(baudrateCombobox.SelectedItem.ToString());

			#endregion

			#region Write settings to board

			if (Status.Instance.ConnectionState == ConnectionState.Connected)
			{
				if (Parameters.Instance.StepPulseTime != oldParameters.StepPulseTime)
				{
					Main.Instance.Connection.SendSetting(0, Parameters.Instance.StepPulseTime.ToString());
				}
				if (Parameters.Instance.StepIdleDelay != oldParameters.StepIdleDelay)
				{
					Main.Instance.Connection.SendSetting(1, Parameters.Instance.StepIdleDelay.ToString());
				}
				if (Parameters.Instance.StepPortInvert != oldParameters.StepPortInvert)
				{
					Main.Instance.Connection.SendSetting(2, ((int)Parameters.Instance.StepPortInvert).ToString());
				}
				if (Parameters.Instance.DirectionPortInvert != oldParameters.DirectionPortInvert)
				{
					Main.Instance.Connection.SendSetting(3, ((int)Parameters.Instance.DirectionPortInvert).ToString());
				}
				if (Parameters.Instance.StepEnableInvert != oldParameters.StepEnableInvert)
				{
					Main.Instance.Connection.SendSetting(4, Parameters.Instance.StepEnableInvert ? "1" : "0");
				}
				if (Parameters.Instance.LimitPinsInvert != oldParameters.LimitPinsInvert)
				{
					Main.Instance.Connection.SendSetting(5, Parameters.Instance.LimitPinsInvert ? "1" : "0");
				}
				if (Parameters.Instance.ProbePinInvert != oldParameters.ProbePinInvert)
				{
					Main.Instance.Connection.SendSetting(6, Parameters.Instance.ProbePinInvert ? "1" : "0");
				}
				if (Parameters.Instance.JunctionDeviation != oldParameters.JunctionDeviation)
				{
					Main.Instance.Connection.SendSetting(11, Parameters.Instance.JunctionDeviation.ToString());
				}
				if (Parameters.Instance.ArcTolerance != oldParameters.ArcTolerance)
				{
					Main.Instance.Connection.SendSetting(12, Parameters.Instance.ArcTolerance.ToString());
				}
				if (Parameters.Instance.ReportInches != oldParameters.ReportInches)
				{
					Main.Instance.Connection.SendSetting(13, Parameters.Instance.ReportInches ? "1" : "0");
				}
				if (Parameters.Instance.SoftLimits != oldParameters.SoftLimits)
				{
					Main.Instance.Connection.SendSetting(20, Parameters.Instance.SoftLimits ? "1" : "0");
				}
				if (Parameters.Instance.HardLimits != oldParameters.HardLimits)
				{
					Main.Instance.Connection.SendSetting(21, Parameters.Instance.HardLimits ? "1" : "0");
				}
				if (Parameters.Instance.HomingCycle != oldParameters.HomingCycle)
				{
					Main.Instance.Connection.SendSetting(22, Parameters.Instance.HomingCycle ? "1" : "0");
				}
				if (Parameters.Instance.HomingDirectionInvert != oldParameters.HomingDirectionInvert)
				{
					Main.Instance.Connection.SendSetting(23, ((int)Parameters.Instance.HomingDirectionInvert).ToString());
				}
				if (Parameters.Instance.HomingFeed != oldParameters.HomingFeed)
				{
					Main.Instance.Connection.SendSetting(24, Parameters.Instance.HomingFeed.ToString());
				}
				if (Parameters.Instance.HomingSeek != oldParameters.HomingSeek)
				{
					Main.Instance.Connection.SendSetting(25, Parameters.Instance.HomingSeek.ToString());
				}
				if (Parameters.Instance.HomingDebounce != oldParameters.HomingDebounce)
				{
					Main.Instance.Connection.SendSetting(26, Parameters.Instance.HomingDebounce.ToString());
				}
				if (Parameters.Instance.HomingPullOff != oldParameters.HomingPullOff)
				{
					Main.Instance.Connection.SendSetting(27, Parameters.Instance.HomingPullOff.ToString());
				}
				if (Parameters.Instance.MaximumSpindleSpeed != oldParameters.MaximumSpindleSpeed)
				{
					Main.Instance.Connection.SendSetting(30, Parameters.Instance.MaximumSpindleSpeed.ToString());
				}
				if (Parameters.Instance.MinimumSpindleSpeed != oldParameters.MinimumSpindleSpeed)
				{
					Main.Instance.Connection.SendSetting(31, Parameters.Instance.MinimumSpindleSpeed.ToString());
				}
				if (Parameters.Instance.LaserMode != oldParameters.LaserMode)
				{
					Main.Instance.Connection.SendSetting(32, Parameters.Instance.LaserMode ? "1" : "0");
				}
				if (Parameters.Instance.XSteps != oldParameters.XSteps)
				{
					Main.Instance.Connection.SendSetting(100, Parameters.Instance.XSteps.ToString());
				}
				if (Parameters.Instance.YSteps != oldParameters.YSteps)
				{
					Main.Instance.Connection.SendSetting(101, Parameters.Instance.YSteps.ToString());
				}
				if (Parameters.Instance.ZSteps != oldParameters.ZSteps)
				{
					Main.Instance.Connection.SendSetting(102, Parameters.Instance.ZSteps.ToString());
				}
				if (Parameters.Instance.XFeedRate != oldParameters.XFeedRate)
				{
					Main.Instance.Connection.SendSetting(110, Parameters.Instance.XFeedRate.ToString());
				}
				if (Parameters.Instance.YFeedRate != oldParameters.YFeedRate)
				{
					Main.Instance.Connection.SendSetting(111, Parameters.Instance.YFeedRate.ToString());
				}
				if (Parameters.Instance.ZFeedRate != oldParameters.ZFeedRate)
				{
					Main.Instance.Connection.SendSetting(112, Parameters.Instance.ZFeedRate.ToString());
				}
				if (Parameters.Instance.XAcceleration != oldParameters.XAcceleration)
				{
					Main.Instance.Connection.SendSetting(120, Parameters.Instance.XAcceleration.ToString());
				}
				if (Parameters.Instance.YAcceleration != oldParameters.YAcceleration)
				{
					Main.Instance.Connection.SendSetting(121, Parameters.Instance.YAcceleration.ToString());
				}
				if (Parameters.Instance.ZAcceleration != oldParameters.ZAcceleration)
				{
					Main.Instance.Connection.SendSetting(122, Parameters.Instance.ZAcceleration.ToString());
				}
				if (Parameters.Instance.XMaximumTravel != oldParameters.XMaximumTravel)
				{
					Main.Instance.Connection.SendSetting(130, Parameters.Instance.XMaximumTravel.ToString());
				}
				if (Parameters.Instance.YMaximumTravel != oldParameters.YMaximumTravel)
				{
					Main.Instance.Connection.SendSetting(131, Parameters.Instance.YMaximumTravel.ToString());
				}
				if (Parameters.Instance.ZMaximumTravel != oldParameters.ZMaximumTravel)
				{
					Main.Instance.Connection.SendSetting(132, Parameters.Instance.ZMaximumTravel.ToString());
				}
			}

			#endregion

			Parameters.Instance.WriteToFile();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void defaultButton_Click(object sender, EventArgs e)
		{
			Parameters.Instance.SetDefaults();
			LoadSettings(Parameters.Instance);
		}
	}
}
