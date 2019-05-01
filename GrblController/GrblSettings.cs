using System;
using System.Threading;
using System.Windows.Forms;

namespace GrblController
{
	public partial class GrblSettings : Form
	{
		public GrblSettings()
		{
			InitializeComponent();
		}

		private void GrblSettings_Load(object sender, EventArgs e)
		{
			var parameters = Parameters.ReadFromFile();

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
			statusReport.SelectedIndex = (int)parameters.StatusReport;
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
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			var oldParameters = Parameters.ReadFromFile();
			var newParameters = Parameters.ReadFromFile();

			newParameters.StepPulseTime = double.Parse(stepPulseTime.Text);
			newParameters.StepIdleDelay = double.Parse(stepIdleDelay.Text);
			newParameters.StepPortInvert = (stepPortInvertX.Checked ? Mask.X : 0) | (stepPortInvertY.Checked ? Mask.Y : 0) | (stepPortInvertZ.Checked ? Mask.Z : 0);
			newParameters.DirectionPortInvert = (directionPortInvertX.Checked ? Mask.X : 0) | (directionPortInvertY.Checked ? Mask.Y : 0) | (directionPortInvertZ.Checked ? Mask.Z : 0);
			newParameters.StepEnableInvert = stepEnableInvert.Checked;
			newParameters.LimitPinsInvert = limitPinsInvert.Checked;
			newParameters.ProbePinInvert = probePinInvert.Checked;
			newParameters.StatusReport = (StatusReport)statusReport.SelectedIndex;
			newParameters.JunctionDeviation = double.Parse(junctionDeviation.Text);
			newParameters.ArcTolerance = double.Parse(arcTolerance.Text);
			newParameters.ReportInches = reportInches.Checked;
			newParameters.SoftLimits = softLimits.Checked;
			newParameters.HardLimits = hardLimits.Checked;
			newParameters.HomingCycle = homingCycle.Checked;
			newParameters.HomingDirectionInvert = (homingDirectionInvertX.Checked ? Mask.X : 0) | (homingDirectionInvertY.Checked ? Mask.Y : 0) | (homingDirectionInvertZ.Checked ? Mask.Z : 0);
			newParameters.HomingFeed = double.Parse(homingFeed.Text);
			newParameters.HomingSeek = double.Parse(homingSeek.Text);
			newParameters.HomingDebounce = double.Parse(homingDebounce.Text);
			newParameters.HomingPullOff = double.Parse(homingPullOff.Text);
			newParameters.MaximumSpindleSpeed = double.Parse(maximumSpindleSpeed.Text);
			newParameters.MinimumSpindleSpeed = double.Parse(minimumSpindleSpeed.Text);
			newParameters.LaserMode = laserMode.Checked;
			newParameters.XSteps = double.Parse(xSteps.Text);
			newParameters.YSteps = double.Parse(ySteps.Text);
			newParameters.ZSteps = double.Parse(zSteps.Text);
			newParameters.XFeedRate = double.Parse(xMaximumRate.Text);
			newParameters.YFeedRate = double.Parse(yMaximumRate.Text);
			newParameters.ZFeedRate = double.Parse(zMaximumRate.Text);
			newParameters.XAcceleration = double.Parse(xAcceleration.Text);
			newParameters.YAcceleration = double.Parse(yAcceleration.Text);
			newParameters.ZAcceleration = double.Parse(zAcceleration.Text);
			newParameters.XMaximumTravel = double.Parse(xMaximumTravel.Text);
			newParameters.YMaximumTravel = double.Parse(yMaximumTravel.Text);
			newParameters.ZMaximumTravel = double.Parse(zMaximumTravel.Text);

			Parameters.WriteToFile(newParameters);

			bool settingsChanged = false;

			if (newParameters.StepPulseTime != oldParameters.StepPulseTime)
			{
				Main.Instance.Connection.SendSetting(0, newParameters.StepPulseTime.ToString());
				settingsChanged = true;
			}
			if (newParameters.StepIdleDelay != oldParameters.StepIdleDelay)
			{
				Main.Instance.Connection.SendSetting(1, newParameters.StepIdleDelay.ToString());
				settingsChanged = true;
			}
			if (newParameters.StepPortInvert != oldParameters.StepPortInvert)
			{
				Main.Instance.Connection.SendSetting(2, ((int)newParameters.StepPortInvert).ToString());
				settingsChanged = true;
			}
			if (newParameters.DirectionPortInvert != oldParameters.DirectionPortInvert)
			{
				Main.Instance.Connection.SendSetting(3, ((int)newParameters.DirectionPortInvert).ToString());
				settingsChanged = true;
			}
			if (newParameters.StepEnableInvert != oldParameters.StepEnableInvert)
			{
				Main.Instance.Connection.SendSetting(4, newParameters.StepEnableInvert ? "1" : "0");
				settingsChanged = true;
			}
			if (newParameters.LimitPinsInvert != oldParameters.LimitPinsInvert)
			{
				Main.Instance.Connection.SendSetting(5, newParameters.LimitPinsInvert ? "1" : "0");
				settingsChanged = true;
			}
			if (newParameters.ProbePinInvert != oldParameters.ProbePinInvert)
			{
				Main.Instance.Connection.SendSetting(6, newParameters.ProbePinInvert ? "1" : "0");
				settingsChanged = true;
			}
			if (newParameters.StatusReport != oldParameters.StatusReport)
			{
				Main.Instance.Connection.SendSetting(10, ((int)newParameters.StatusReport).ToString());
				settingsChanged = true;
			}
			if (newParameters.JunctionDeviation != oldParameters.JunctionDeviation)
			{
				Main.Instance.Connection.SendSetting(11, newParameters.JunctionDeviation.ToString());
				settingsChanged = true;
			}
			if (newParameters.ArcTolerance != oldParameters.ArcTolerance)
			{
				Main.Instance.Connection.SendSetting(12, newParameters.ArcTolerance.ToString());
				settingsChanged = true;
			}
			if (newParameters.ReportInches != oldParameters.ReportInches)
			{
				Main.Instance.Connection.SendSetting(13, newParameters.ReportInches ? "1" : "0");
				settingsChanged = true;
			}
			if (newParameters.SoftLimits != oldParameters.SoftLimits)
			{
				Main.Instance.Connection.SendSetting(20, newParameters.SoftLimits ? "1" : "0");
				settingsChanged = true;
			}
			if (newParameters.HardLimits != oldParameters.HardLimits)
			{
				Main.Instance.Connection.SendSetting(21, newParameters.HardLimits ? "1" : "0");
				settingsChanged = true;
			}
			if (newParameters.HomingCycle != oldParameters.HomingCycle)
			{
				Main.Instance.Connection.SendSetting(22, newParameters.HomingCycle ? "1" : "0");
				settingsChanged = true;
			}
			if (newParameters.HomingDirectionInvert != oldParameters.HomingDirectionInvert)
			{
				Main.Instance.Connection.SendSetting(23, ((int)newParameters.HomingDirectionInvert).ToString());
				settingsChanged = true;
			}
			if (newParameters.HomingFeed != oldParameters.HomingFeed)
			{
				Main.Instance.Connection.SendSetting(24, newParameters.HomingFeed.ToString());
				settingsChanged = true;
			}
			if (newParameters.HomingSeek != oldParameters.HomingSeek)
			{
				Main.Instance.Connection.SendSetting(25, newParameters.HomingSeek.ToString());
				settingsChanged = true;
			}
			if (newParameters.HomingDebounce != oldParameters.HomingDebounce)
			{
				Main.Instance.Connection.SendSetting(26, newParameters.HomingDebounce.ToString());
				settingsChanged = true;
			}
			if (newParameters.HomingPullOff != oldParameters.HomingPullOff)
			{
				Main.Instance.Connection.SendSetting(27, newParameters.HomingPullOff.ToString());
				settingsChanged = true;
			}
			if (newParameters.MaximumSpindleSpeed != oldParameters.MaximumSpindleSpeed)
			{
				Main.Instance.Connection.SendSetting(30, newParameters.MaximumSpindleSpeed.ToString());
				settingsChanged = true;
			}
			if (newParameters.MinimumSpindleSpeed != oldParameters.MinimumSpindleSpeed)
			{
				Main.Instance.Connection.SendSetting(31, newParameters.MinimumSpindleSpeed.ToString());
				settingsChanged = true;
			}
			if (newParameters.LaserMode != oldParameters.LaserMode)
			{
				Main.Instance.Connection.SendSetting(32, newParameters.LaserMode ? "1" : "0");
				settingsChanged = true;
			}
			if (newParameters.XSteps != oldParameters.XSteps)
			{
				Main.Instance.Connection.SendSetting(100, newParameters.XSteps.ToString());
				settingsChanged = true;
			}
			if (newParameters.YSteps != oldParameters.YSteps)
			{
				Main.Instance.Connection.SendSetting(101, newParameters.YSteps.ToString());
				settingsChanged = true;
			}
			if (newParameters.ZSteps != oldParameters.ZSteps)
			{
				Main.Instance.Connection.SendSetting(102, newParameters.ZSteps.ToString());
				settingsChanged = true;
			}
			if (newParameters.XFeedRate != oldParameters.XFeedRate)
			{
				Main.Instance.Connection.SendSetting(110, newParameters.XFeedRate.ToString());
				settingsChanged = true;
			}
			if (newParameters.YFeedRate != oldParameters.YFeedRate)
			{
				Main.Instance.Connection.SendSetting(111, newParameters.YFeedRate.ToString());
				settingsChanged = true;
			}
			if (newParameters.ZFeedRate != oldParameters.ZFeedRate)
			{
				Main.Instance.Connection.SendSetting(112, newParameters.ZFeedRate.ToString());
				settingsChanged = true;
			}
			if (newParameters.XAcceleration != oldParameters.XAcceleration)
			{
				Main.Instance.Connection.SendSetting(120, newParameters.XAcceleration.ToString());
				settingsChanged = true;
			}
			if (newParameters.YAcceleration != oldParameters.YAcceleration)
			{
				Main.Instance.Connection.SendSetting(121, newParameters.YAcceleration.ToString());
				settingsChanged = true;
			}
			if (newParameters.ZAcceleration != oldParameters.ZAcceleration)
			{
				Main.Instance.Connection.SendSetting(122, newParameters.ZAcceleration.ToString());
				settingsChanged = true;
			}
			if (newParameters.XMaximumTravel != oldParameters.XMaximumTravel)
			{
				Main.Instance.Connection.SendSetting(130, newParameters.XMaximumTravel.ToString());
				settingsChanged = true;
			}
			if (newParameters.YMaximumTravel != oldParameters.YMaximumTravel)
			{
				Main.Instance.Connection.SendSetting(131, newParameters.YMaximumTravel.ToString());
				settingsChanged = true;
			}
			if (newParameters.ZMaximumTravel != oldParameters.ZMaximumTravel)
			{
				Main.Instance.Connection.SendSetting(132, newParameters.ZMaximumTravel.ToString());
				settingsChanged = true;
			}

			if (settingsChanged)
			{
				Main.Instance.Connection.Disconnect();
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		private void defaultButton_Click(object sender, EventArgs e)
		{
			Parameters.WriteToFile(new Parameters());
			GrblSettings_Load(null, null);
		}
	}
}
