using System;

namespace GrblController
{
	[Flags]
	internal enum Mask
	{
		X = 1,
		Y = 2,
		Z = 4
	}

	internal enum ControlAxis
	{
		X,
		Y,
		Z
	}

	internal class Parameters
	{
		private static Parameters instance;

		internal static Parameters Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Parameters();
				}
				return instance;
			}
		}

		internal double StartOffset { get; set; }
		internal double Table1Length { get; set; }
		internal double MiddleGap { get; set; }
		internal double Table2Length { get; set; }
		internal double EndOffset { get; set; }
		internal string SerialPortString { get; set; }
		internal int Baudrate { get; set; }
		internal ControlAxis ControlAxis { get; set; }
		internal double PurgeDuration { get; set; }
		internal double StepPulseTime { get; set; }
		internal double StepIdleDelay { get; set; }
		internal Mask StepPortInvert { get; set; }
		internal Mask DirectionPortInvert { get; set; }
		internal bool StepEnableInvert { get; set; }
		internal bool LimitPinsInvert { get; set; }
		internal bool ProbePinInvert { get; set; }
		internal double JunctionDeviation { get; set; }
		internal double ArcTolerance { get; set; }
		internal bool ReportInches { get; set; }
		internal bool SoftLimits { get; set; }
		internal bool HardLimits { get; set; }
		internal bool HomingCycle { get; set; }
		internal Mask HomingDirectionInvert { get; set; }
		internal double HomingFeed { get; set; }
		internal double HomingSeek { get; set; }
		internal double HomingDebounce { get; set; }
		internal double HomingPullOff { get; set; }
		internal double MaximumSpindleSpeed { get; set; }
		internal double MinimumSpindleSpeed { get; set; }
		internal bool LaserMode { get; set; }
		internal double XSteps { get; set; }
		internal double YSteps { get; set; }
		internal double ZSteps { get; set; }
		internal double XFeedRate { get; set; }
		internal double YFeedRate { get; set; }
		internal double ZFeedRate { get; set; }
		internal double XAcceleration { get; set; }
		internal double YAcceleration { get; set; }
		internal double ZAcceleration { get; set; }
		internal double XMaximumTravel { get; set; }
		internal double YMaximumTravel { get; set; }
		internal double ZMaximumTravel { get; set; }
		internal bool DoubleTable { get; set; }

		internal double TablesTotalLength
		{
			get
			{
				return StartOffset + Table1Length + EndOffset + (DoubleTable ? (MiddleGap + Table2Length) : 0);
			}
		}

		internal Parameters()
		{
			if (!ReadFromFile())
			{
				SetDefaults();
			}
		}

		private void Create()
		{
			StartOffset = 100;
			Table1Length = 500;
			MiddleGap = 100;
			Table2Length = 500;
			EndOffset = 100;
			SerialPortString = "COM1";
			Baudrate = 115200;
			ControlAxis = ControlAxis.Y;
			PurgeDuration = 4;
			StepPulseTime = 10;
			StepIdleDelay = 255;
			StepPortInvert = 0;
			DirectionPortInvert = Mask.Y | Mask.Z;
			StepEnableInvert = true;
			LimitPinsInvert = false;
			ProbePinInvert = false;
			JunctionDeviation = 0.02;
			ArcTolerance = 0.002;
			ReportInches = false;
			SoftLimits = false;
			HardLimits = true;
			HomingCycle = false;
			HomingDirectionInvert = Mask.X;
			HomingFeed = 25;
			HomingSeek = 500;
			HomingDebounce = 250;
			HomingPullOff = 1;
			MaximumSpindleSpeed = 12000;
			MinimumSpindleSpeed = 0;
			LaserMode = false;
			XSteps = 200;
			YSteps = 200;
			ZSteps = 200;
			XFeedRate = 3000;
			YFeedRate = 3000;
			ZFeedRate = 2000;
			XAcceleration = 25;
			YAcceleration = 25;
			ZAcceleration = 50;
			XMaximumTravel = 200;
			YMaximumTravel = 200;
			ZMaximumTravel = 200;
			DoubleTable = true;
		}

		internal void SetDefaults()
		{
			Create();
			WriteToFile();
		}

		private bool ReadFromFile()
		{
			try
			{
				Properties.Settings.Default.Reload();

				StartOffset = Properties.Settings.Default.StartOffset; //mm
				Table1Length = Properties.Settings.Default.Table1Length; //mm
				MiddleGap = Properties.Settings.Default.MiddleGap; //mm
				Table2Length = Properties.Settings.Default.Table2Length; //mm
				EndOffset = Properties.Settings.Default.EndOffset; //mm
				SerialPortString = Properties.Settings.Default.SerialPortString;
				Baudrate = Properties.Settings.Default.Baudrate;
				ControlAxis = (ControlAxis)Enum.Parse(typeof(ControlAxis), Properties.Settings.Default.ControlAxis);
				PurgeDuration = Properties.Settings.Default.PurgeDuration; //sec
				StepPulseTime = Properties.Settings.Default.StepPulseTime; //usec
				StepIdleDelay = Properties.Settings.Default.StepIdleDelay; //msec
				StepPortInvert = (Mask)Properties.Settings.Default.StepPortInvert;
				DirectionPortInvert = (Mask)Properties.Settings.Default.DirectionPortInvert;
				StepEnableInvert = Properties.Settings.Default.StepEnableInvert;
				LimitPinsInvert = Properties.Settings.Default.LimitPinsInvert;
				ProbePinInvert = Properties.Settings.Default.ProbePinInvert;
				JunctionDeviation = Properties.Settings.Default.JunctionDeviation; //mm
				ArcTolerance = Properties.Settings.Default.ArcTolerance; //mm
				ReportInches = Properties.Settings.Default.ReportInches;
				SoftLimits = Properties.Settings.Default.SoftLimits;
				HardLimits = Properties.Settings.Default.HardLimits;
				HomingCycle = Properties.Settings.Default.HomingCycle;
				HomingDirectionInvert = (Mask)Properties.Settings.Default.HomingDirectionInvert;
				HomingFeed = Properties.Settings.Default.HomingFeed; //mm/min
				HomingSeek = Properties.Settings.Default.HomingSeek; //mm/min
				HomingDebounce = Properties.Settings.Default.HomingDebounce; //msec //msec
				HomingPullOff = Properties.Settings.Default.HomingPullOff; //mm
				MaximumSpindleSpeed = Properties.Settings.Default.MaximumSpindleSpeed; //RPM
				MinimumSpindleSpeed = Properties.Settings.Default.MinimumSpindleSpeed; //RPM
				LaserMode = Properties.Settings.Default.LaserMode;
				XSteps = Properties.Settings.Default.XSteps; // /mm
				YSteps = Properties.Settings.Default.YSteps; // /mm
				ZSteps = Properties.Settings.Default.ZSteps; // /mm
				XFeedRate = Properties.Settings.Default.XFeedRate; //mm/min
				YFeedRate = Properties.Settings.Default.YFeedRate; //mm/min
				ZFeedRate = Properties.Settings.Default.ZFeedRate; //mm/min
				XAcceleration = Properties.Settings.Default.XAcceleration; //mm/sec^2
				YAcceleration = Properties.Settings.Default.YAcceleration; //mm/sec^2
				ZAcceleration = Properties.Settings.Default.ZAcceleration; //mm/sec^2
				XMaximumTravel = Properties.Settings.Default.XMaximumTravel; //mm
				YMaximumTravel = Properties.Settings.Default.YMaximumTravel; //mm
				ZMaximumTravel = Properties.Settings.Default.ZMaximumTravel; //mm
				DoubleTable = Properties.Settings.Default.DoubleTable;
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		internal void WriteToFile()
		{
			Properties.Settings.Default.StartOffset = StartOffset; //mm
			Properties.Settings.Default.Table1Length = Table1Length; //mm
			Properties.Settings.Default.MiddleGap = MiddleGap; //mm
			Properties.Settings.Default.Table2Length = Table2Length; //mm
			Properties.Settings.Default.EndOffset = EndOffset; //mm
			Properties.Settings.Default.SerialPortString = SerialPortString;
			Properties.Settings.Default.Baudrate = Baudrate;
			Properties.Settings.Default.ControlAxis = ControlAxis.ToString();
			Properties.Settings.Default.PurgeDuration = PurgeDuration; //sec
			Properties.Settings.Default.StepPulseTime = StepPulseTime; //usec
			Properties.Settings.Default.StepIdleDelay = StepIdleDelay; //msec
			Properties.Settings.Default.StepPortInvert = (int)StepPortInvert;
			Properties.Settings.Default.DirectionPortInvert = (int)DirectionPortInvert;
			Properties.Settings.Default.StepEnableInvert = StepEnableInvert;
			Properties.Settings.Default.LimitPinsInvert = LimitPinsInvert;
			Properties.Settings.Default.ProbePinInvert = ProbePinInvert;
			Properties.Settings.Default.JunctionDeviation = JunctionDeviation; //mm
			Properties.Settings.Default.ArcTolerance = ArcTolerance; //mm
			Properties.Settings.Default.ReportInches = ReportInches;
			Properties.Settings.Default.SoftLimits = SoftLimits;
			Properties.Settings.Default.HardLimits = HardLimits;
			Properties.Settings.Default.HomingCycle = HomingCycle;
			Properties.Settings.Default.HomingDirectionInvert = (int)HomingDirectionInvert;
			Properties.Settings.Default.HomingFeed = HomingFeed; //mm/min
			Properties.Settings.Default.HomingSeek = HomingSeek; //mm/min
			Properties.Settings.Default.HomingDebounce = HomingDebounce; //msec //msec
			Properties.Settings.Default.HomingPullOff = HomingPullOff; //mm
			Properties.Settings.Default.MaximumSpindleSpeed = MaximumSpindleSpeed; //RPM
			Properties.Settings.Default.MinimumSpindleSpeed = MinimumSpindleSpeed; //RPM
			Properties.Settings.Default.LaserMode = LaserMode;
			Properties.Settings.Default.XSteps = XSteps; // /mm
			Properties.Settings.Default.YSteps = YSteps; // /mm
			Properties.Settings.Default.ZSteps = ZSteps; // /mm
			Properties.Settings.Default.XFeedRate = XFeedRate; //mm/min
			Properties.Settings.Default.YFeedRate = YFeedRate; //mm/min
			Properties.Settings.Default.ZFeedRate = ZFeedRate; //mm/min
			Properties.Settings.Default.XAcceleration = XAcceleration; //mm/sec^2
			Properties.Settings.Default.YAcceleration = YAcceleration; //mm/sec^2
			Properties.Settings.Default.ZAcceleration = ZAcceleration; //mm/sec^2
			Properties.Settings.Default.XMaximumTravel = XMaximumTravel; //mm
			Properties.Settings.Default.YMaximumTravel = YMaximumTravel; //mm
			Properties.Settings.Default.ZMaximumTravel = ZMaximumTravel; //mm
			Properties.Settings.Default.DoubleTable = DoubleTable;

			Properties.Settings.Default.Save();
		}
	}
}
