using System;
using System.Xml;
using System.Xml.Linq;

namespace GrblController
{
	[Flags]
	internal enum Mask
	{
		X = 1,
		Y = 2,
		Z = 4
	}

	internal enum StatusReport
	{
		WorkPosition,
		MachinePosition,
		WorkPositionWithBuffer
	}

	internal enum ControlAxis
	{
		X,
		Y,
		Z
	}

	internal class Parameters
	{
		internal double TablesTotalLength
		{
			get
			{
				return StartOffset + Table1Length + MiddleGap + Table2Length + EndOffset;
			}
		}

		internal double StartOffset { get; set; } //mm
		internal double Table1Length { get; set; } //mm
		internal double MiddleGap { get; set; } //mm
		internal double Table2Length { get; set; } //mm
		internal double EndOffset { get; set; } //mm

		internal string SerialPortString { get; set; }
		internal int Baudrate { get; set; }

		internal ControlAxis ControlAxis { get; set; }
		internal bool ReverseFeed { get; set; }

		internal double StepPulseTime { get; set; } //usec
		internal double StepIdleDelay { get; set; } //msec
		internal Mask StepPortInvert { get; set; }
		internal Mask DirectionPortInvert { get; set; } //usec
		internal bool StepEnableInvert { get; set; } //usec
		internal bool LimitPinsInvert { get; set; } //usec
		internal bool ProbePinInvert { get; set; } //usec
		internal StatusReport StatusReport { get; set; }
		internal double JunctionDeviation { get; set; } //mm
		internal double ArcTolerance { get; set; } //mm
		internal bool ReportInches { get; set; }
		internal bool SoftLimits { get; set; }
		internal bool HardLimits { get; set; }
		internal bool HomingCycle { get; set; }
		internal Mask HomingDirectionInvert { get; set; }
		internal double HomingFeed { get; set; } //mm/min
		internal double HomingSeek { get; set; } //mm/min
		internal double HomingDebounce { get; set; } //msec
		internal double HomingPullOff { get; set; } //mm
		internal double MaximumSpindleSpeed { get; set; } //RPM
		internal double MinimumSpindleSpeed { get; set; } //RPM
		internal bool LaserMode { get; set; }
		internal double XSteps { get; set; } // /mm
		internal double YSteps { get; set; } // /mm
		internal double ZSteps { get; set; } // /mm
		internal double XFeedRate { get; set; } //mm/min
		internal double YFeedRate { get; set; } //mm/min
		internal double ZFeedRate { get; set; } //mm/min
		internal double XAcceleration { get; set; } //mm/sec^2
		internal double YAcceleration { get; set; } //mm/sec^2
		internal double ZAcceleration { get; set; } //mm/sec^2
		internal double XMaximumTravel { get; set; } //mm
		internal double YMaximumTravel { get; set; } //mm
		internal double ZMaximumTravel { get; set; } //mm

		internal string CalibrateBeforeHit { get; set; }
		internal string Zeroize { get; set; }
		internal string CalibrateAfterHit { get; set; }

		internal Parameters()
		{
			StartOffset = 100;
			Table1Length = 500;
			MiddleGap = 100;
			Table2Length = 500;
			EndOffset = 100;

			SerialPortString = "COM1";
			Baudrate = 115200;

			ControlAxis = ControlAxis.Y;
			ReverseFeed = false;

			StepPulseTime = 10;
			StepIdleDelay = 255;
			StepPortInvert = 0;
			DirectionPortInvert = Mask.Y | Mask.Z;
			StepEnableInvert = true;
			LimitPinsInvert = false;
			ProbePinInvert = false;
			StatusReport = StatusReport.MachinePosition;
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

			CalibrateBeforeHit = "";
			Zeroize = "";
			CalibrateAfterHit = "";
		}

		internal static Parameters ReadFromFile(string filename)
		{
			try
			{
				var doc = new XmlDocument();
				doc.Load(filename);

				var parameters = new Parameters()
				{
					StartOffset = double.Parse(doc["Parameters"]["StartOffset"].InnerText),
					Table1Length = double.Parse(doc["Parameters"]["Table1Length"].InnerText),
					MiddleGap = double.Parse(doc["Parameters"]["MiddleGap"].InnerText),
					Table2Length = double.Parse(doc["Parameters"]["Table2Length"].InnerText),
					EndOffset = double.Parse(doc["Parameters"]["EndOffset"].InnerText),

					SerialPortString = doc["Parameters"]["SerialPortString"].InnerText,
					Baudrate = int.Parse(doc["Parameters"]["Baudrate"].InnerText),

					ControlAxis = (ControlAxis)Enum.Parse(typeof(ControlAxis), doc["Parameters"]["ControlAxis"].InnerText),
					ReverseFeed = bool.Parse(doc["Parameters"]["ReverseFeed"].InnerText),

					StepPulseTime = double.Parse(doc["Parameters"]["StepPulseTime"].InnerText),
					StepIdleDelay = double.Parse(doc["Parameters"]["StepIdleDelay"].InnerText),
					StepPortInvert = (Mask)int.Parse(doc["Parameters"]["StepPortInvert"].InnerText),
					DirectionPortInvert = (Mask)int.Parse(doc["Parameters"]["DirectionPortInvert"].InnerText),
					StepEnableInvert = bool.Parse(doc["Parameters"]["StepEnableInvert"].InnerText),
					LimitPinsInvert = bool.Parse(doc["Parameters"]["LimitPinsInvert"].InnerText),
					ProbePinInvert = bool.Parse(doc["Parameters"]["ProbePinInvert"].InnerText),
					StatusReport = (StatusReport)Enum.Parse(typeof(StatusReport), doc["Parameters"]["StatusReport"].InnerText),
					JunctionDeviation = double.Parse(doc["Parameters"]["JunctionDeviation"].InnerText),
					ArcTolerance = double.Parse(doc["Parameters"]["ArcTolerance"].InnerText),
					ReportInches = bool.Parse(doc["Parameters"]["ReportInches"].InnerText),
					SoftLimits = bool.Parse(doc["Parameters"]["SoftLimits"].InnerText),
					HardLimits = bool.Parse(doc["Parameters"]["HardLimits"].InnerText),
					HomingCycle = bool.Parse(doc["Parameters"]["HomingCycle"].InnerText),
					HomingDirectionInvert = (Mask)int.Parse(doc["Parameters"]["HomingDirectionInvert"].InnerText),
					HomingFeed = double.Parse(doc["Parameters"]["HomingFeed"].InnerText),
					HomingSeek = double.Parse(doc["Parameters"]["HomingSeek"].InnerText),
					HomingDebounce = double.Parse(doc["Parameters"]["HomingDebounce"].InnerText),
					HomingPullOff = double.Parse(doc["Parameters"]["HomingPullOff"].InnerText),
					MaximumSpindleSpeed = double.Parse(doc["Parameters"]["MaximumSpindleSpeed"].InnerText),
					MinimumSpindleSpeed = double.Parse(doc["Parameters"]["MinimumSpindleSpeed"].InnerText),
					LaserMode = bool.Parse(doc["Parameters"]["LaserMode"].InnerText),
					XSteps = double.Parse(doc["Parameters"]["XSteps"].InnerText),
					YSteps = double.Parse(doc["Parameters"]["YSteps"].InnerText),
					ZSteps = double.Parse(doc["Parameters"]["ZSteps"].InnerText),
					XFeedRate = double.Parse(doc["Parameters"]["XFeedRate"].InnerText),
					YFeedRate = double.Parse(doc["Parameters"]["YFeedRate"].InnerText),
					ZFeedRate = double.Parse(doc["Parameters"]["ZFeedRate"].InnerText),
					XAcceleration = double.Parse(doc["Parameters"]["XAcceleration"].InnerText),
					YAcceleration = double.Parse(doc["Parameters"]["YAcceleration"].InnerText),
					ZAcceleration = double.Parse(doc["Parameters"]["ZAcceleration"].InnerText),
					XMaximumTravel = double.Parse(doc["Parameters"]["XMaximumTravel"].InnerText),
					YMaximumTravel = double.Parse(doc["Parameters"]["YMaximumTravel"].InnerText),
					ZMaximumTravel = double.Parse(doc["Parameters"]["ZMaximumTravel"].InnerText),

					CalibrateBeforeHit = doc["Parameters"]["CalibrateBeforeHit"].InnerText,
					Zeroize = doc["Parameters"]["Zeroize"].InnerText,
					CalibrateAfterHit = doc["Parameters"]["CalibrateAfterHit"].InnerText
				};
				return parameters;
			}
			catch
			{
				var parameters = new Parameters();
				WriteToFile(filename, parameters);
				return parameters;
			}
		}

		internal static void WriteToFile(string filename, Parameters parameters)
		{
			new XDocument(
				new XElement("Parameters",
					new XElement("StartOffset", parameters.StartOffset.ToString()),
					new XElement("Table1Length", parameters.Table1Length.ToString()),
					new XElement("MiddleGap", parameters.MiddleGap.ToString()),
					new XElement("Table2Length", parameters.Table2Length.ToString()),
					new XElement("EndOffset", parameters.EndOffset.ToString()),

					new XElement("SerialPortString", parameters.SerialPortString.ToString()),
					new XElement("Baudrate", parameters.Baudrate.ToString()),

					new XElement("ControlAxis", parameters.ControlAxis.ToString()),
					new XElement("ReverseFeed", parameters.ReverseFeed.ToString()),

					new XElement("StepPulseTime", parameters.StepPulseTime.ToString()),
					new XElement("StepIdleDelay", parameters.StepIdleDelay.ToString()),
					new XElement("StepPortInvert", ((int)parameters.StepPortInvert).ToString()),
					new XElement("DirectionPortInvert", ((int)parameters.DirectionPortInvert).ToString()),
					new XElement("StepEnableInvert", parameters.StepEnableInvert.ToString()),
					new XElement("LimitPinsInvert", parameters.LimitPinsInvert.ToString()),
					new XElement("ProbePinInvert", parameters.ProbePinInvert.ToString()),
					new XElement("StatusReport", ((int)parameters.StatusReport).ToString()),
					new XElement("JunctionDeviation", parameters.JunctionDeviation.ToString()),
					new XElement("ArcTolerance", parameters.ArcTolerance.ToString()),
					new XElement("ReportInches", parameters.ReportInches.ToString()),
					new XElement("SoftLimits", parameters.SoftLimits.ToString()),
					new XElement("HardLimits", parameters.HardLimits.ToString()),
					new XElement("HomingCycle", parameters.HomingCycle.ToString()),
					new XElement("HomingDirectionInvert", ((int)parameters.HomingDirectionInvert).ToString()),
					new XElement("HomingFeed", parameters.HomingFeed.ToString()),
					new XElement("HomingSeek", parameters.HomingSeek.ToString()),
					new XElement("HomingDebounce", parameters.HomingDebounce.ToString()),
					new XElement("HomingPullOff", parameters.HomingPullOff.ToString()),
					new XElement("MaximumSpindleSpeed", parameters.MaximumSpindleSpeed.ToString()),
					new XElement("MinimumSpindleSpeed", parameters.MinimumSpindleSpeed.ToString()),
					new XElement("LaserMode", parameters.LaserMode.ToString()),
					new XElement("XSteps", parameters.XSteps.ToString()),
					new XElement("YSteps", parameters.YSteps.ToString()),
					new XElement("ZSteps", parameters.ZSteps.ToString()),
					new XElement("XFeedRate", parameters.XFeedRate.ToString()),
					new XElement("YFeedRate", parameters.YFeedRate.ToString()),
					new XElement("ZFeedRate", parameters.ZFeedRate.ToString()),
					new XElement("XAcceleration", parameters.XAcceleration.ToString()),
					new XElement("YAcceleration", parameters.YAcceleration.ToString()),
					new XElement("ZAcceleration", parameters.ZAcceleration.ToString()),
					new XElement("XMaximumTravel", parameters.XMaximumTravel.ToString()),
					new XElement("YMaximumTravel", parameters.YMaximumTravel.ToString()),
					new XElement("ZMaximumTravel", parameters.ZMaximumTravel.ToString()),
					new XElement("CalibrateBeforeHit", parameters.CalibrateBeforeHit.ToString()),
					new XElement("Zeroize", parameters.Zeroize.ToString()),
					new XElement("CalibrateAfterHit", parameters.CalibrateAfterHit.ToString())
				)).Save(filename);
		}
	}
}
