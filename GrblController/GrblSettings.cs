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
			var parameters = Parameters.ReadFromFile();

			parameters.StepPulseTime = double.Parse(CorrectDecimalSep(stepPulseTime.Text));
			parameters.StepIdleDelay = double.Parse(CorrectDecimalSep(stepIdleDelay.Text));
			parameters.StepPortInvert = (stepPortInvertX.Checked ? Mask.X : 0) | (stepPortInvertY.Checked ? Mask.Y : 0) | (stepPortInvertZ.Checked ? Mask.Z : 0);
			parameters.DirectionPortInvert = (directionPortInvertX.Checked ? Mask.X : 0) | (directionPortInvertY.Checked ? Mask.Y : 0) | (directionPortInvertZ.Checked ? Mask.Z : 0);
			parameters.StepEnableInvert = stepEnableInvert.Checked;
			parameters.LimitPinsInvert = limitPinsInvert.Checked;
			parameters.ProbePinInvert = probePinInvert.Checked;
			parameters.StatusReport = (StatusReport)statusReport.SelectedIndex;
			parameters.JunctionDeviation = double.Parse(CorrectDecimalSep(junctionDeviation.Text));
			parameters.ArcTolerance = double.Parse(CorrectDecimalSep(arcTolerance.Text));
			parameters.ReportInches = reportInches.Checked;
			parameters.SoftLimits = softLimits.Checked;
			parameters.HardLimits = hardLimits.Checked;
			parameters.HomingCycle = homingCycle.Checked;
			parameters.HomingDirectionInvert = (homingDirectionInvertX.Checked ? Mask.X : 0) | (homingDirectionInvertY.Checked ? Mask.Y : 0) | (homingDirectionInvertZ.Checked ? Mask.Z : 0);
			parameters.HomingFeed = double.Parse(CorrectDecimalSep(homingFeed.Text));
			parameters.HomingSeek = double.Parse(CorrectDecimalSep(homingSeek.Text));
			parameters.HomingDebounce = double.Parse(CorrectDecimalSep(homingDebounce.Text));
			parameters.HomingPullOff = double.Parse(CorrectDecimalSep(homingPullOff.Text));
			parameters.MaximumSpindleSpeed = double.Parse(CorrectDecimalSep(maximumSpindleSpeed.Text));
			parameters.MinimumSpindleSpeed = double.Parse(CorrectDecimalSep(minimumSpindleSpeed.Text));
			parameters.LaserMode = laserMode.Checked;
			parameters.XSteps = double.Parse(CorrectDecimalSep(xSteps.Text));
			parameters.YSteps = double.Parse(CorrectDecimalSep(ySteps.Text));
			parameters.ZSteps = double.Parse(CorrectDecimalSep(zSteps.Text));
			parameters.XFeedRate = double.Parse(CorrectDecimalSep(xMaximumRate.Text));
			parameters.YFeedRate = double.Parse(CorrectDecimalSep(yMaximumRate.Text));
			parameters.ZFeedRate = double.Parse(CorrectDecimalSep(zMaximumRate.Text));
			parameters.XAcceleration = double.Parse(CorrectDecimalSep(xAcceleration.Text));
			parameters.YAcceleration = double.Parse(CorrectDecimalSep(yAcceleration.Text));
			parameters.ZAcceleration = double.Parse(CorrectDecimalSep(zAcceleration.Text));
			parameters.XMaximumTravel = double.Parse(CorrectDecimalSep(xMaximumTravel.Text));
			parameters.YMaximumTravel = double.Parse(CorrectDecimalSep(yMaximumTravel.Text));
			parameters.ZMaximumTravel = double.Parse(CorrectDecimalSep(zMaximumTravel.Text));

			Parameters.WriteToFile(parameters);

			DialogResult = DialogResult.OK;
			Close();
		}

		private string CorrectDecimalSep(string s)
		{
			return s.Replace(".", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
		}
	}
}
