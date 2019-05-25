using System;
using System.Collections.Generic;
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

	internal enum ControlAxis
	{
		X,
		Y,
		Z
	}

	internal class Parameters
	{
		private string configFilename = "config.xml";
		private string defaultFilename = "default.xml";
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

		internal double TablesTotalLength
		{
			get
			{
				return StartOffset + Table1Length + MiddleGap + Table2Length + EndOffset;
			}
		}

		internal Parameters()
		{
			if (!ReadFromFile(configFilename))
			{
				Create();
				WriteToFile();
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
		}

		internal void SetDefaults()
		{
			if (!ReadFromFile(defaultFilename))
			{
				Create();
				WriteToFile(defaultFilename);
			}
		}

		private bool ReadFromFile(string filename)
		{
			try
			{
				var doc = new XmlDocument();
				doc.Load(filename);

				StartOffset = double.Parse(doc["Parameters"]["StartOffset"].InnerText); //mm
				Table1Length = double.Parse(doc["Parameters"]["Table1Length"].InnerText); //mm
				MiddleGap = double.Parse(doc["Parameters"]["MiddleGap"].InnerText); //mm
				Table2Length = double.Parse(doc["Parameters"]["Table2Length"].InnerText); //mm
				EndOffset = double.Parse(doc["Parameters"]["EndOffset"].InnerText); //mm
				SerialPortString = doc["Parameters"]["SerialPortString"].InnerText;
				Baudrate = int.Parse(doc["Parameters"]["Baudrate"].InnerText);
				ControlAxis = (ControlAxis)Enum.Parse(typeof(ControlAxis), doc["Parameters"]["ControlAxis"].InnerText);
				PurgeDuration = double.Parse(doc["Parameters"]["PurgeDuration"].InnerText); //sec
				StepPulseTime = double.Parse(doc["Parameters"]["StepPulseTime"].InnerText); //usec
				StepIdleDelay = double.Parse(doc["Parameters"]["StepIdleDelay"].InnerText); //msec
				StepPortInvert = (Mask)int.Parse(doc["Parameters"]["StepPortInvert"].InnerText);
				DirectionPortInvert = (Mask)int.Parse(doc["Parameters"]["DirectionPortInvert"].InnerText);
				StepEnableInvert = bool.Parse(doc["Parameters"]["StepEnableInvert"].InnerText);
				LimitPinsInvert = bool.Parse(doc["Parameters"]["LimitPinsInvert"].InnerText);
				ProbePinInvert = bool.Parse(doc["Parameters"]["ProbePinInvert"].InnerText);
				JunctionDeviation = double.Parse(doc["Parameters"]["JunctionDeviation"].InnerText); //mm
				ArcTolerance = double.Parse(doc["Parameters"]["ArcTolerance"].InnerText); //mm
				ReportInches = bool.Parse(doc["Parameters"]["ReportInches"].InnerText);
				SoftLimits = bool.Parse(doc["Parameters"]["SoftLimits"].InnerText);
				HardLimits = bool.Parse(doc["Parameters"]["HardLimits"].InnerText);
				HomingCycle = bool.Parse(doc["Parameters"]["HomingCycle"].InnerText);
				HomingDirectionInvert = (Mask)int.Parse(doc["Parameters"]["HomingDirectionInvert"].InnerText);
				HomingFeed = double.Parse(doc["Parameters"]["HomingFeed"].InnerText); //mm/min
				HomingSeek = double.Parse(doc["Parameters"]["HomingSeek"].InnerText); //mm/min
				HomingDebounce = double.Parse(doc["Parameters"]["HomingDebounce"].InnerText); //msec //msec
				HomingPullOff = double.Parse(doc["Parameters"]["HomingPullOff"].InnerText); //mm
				MaximumSpindleSpeed = double.Parse(doc["Parameters"]["MaximumSpindleSpeed"].InnerText); //RPM
				MinimumSpindleSpeed = double.Parse(doc["Parameters"]["MinimumSpindleSpeed"].InnerText); //RPM
				LaserMode = bool.Parse(doc["Parameters"]["LaserMode"].InnerText);
				XSteps = double.Parse(doc["Parameters"]["XSteps"].InnerText); // /mm
				YSteps = double.Parse(doc["Parameters"]["YSteps"].InnerText); // /mm
				ZSteps = double.Parse(doc["Parameters"]["ZSteps"].InnerText); // /mm
				XFeedRate = double.Parse(doc["Parameters"]["XFeedRate"].InnerText); //mm/min
				YFeedRate = double.Parse(doc["Parameters"]["YFeedRate"].InnerText); //mm/min
				ZFeedRate = double.Parse(doc["Parameters"]["ZFeedRate"].InnerText); //mm/min
				XAcceleration = double.Parse(doc["Parameters"]["XAcceleration"].InnerText); //mm/sec^2
				YAcceleration = double.Parse(doc["Parameters"]["YAcceleration"].InnerText); //mm/sec^2
				ZAcceleration = double.Parse(doc["Parameters"]["ZAcceleration"].InnerText); //mm/sec^2
				XMaximumTravel = double.Parse(doc["Parameters"]["XMaximumTravel"].InnerText); //mm
				YMaximumTravel = double.Parse(doc["Parameters"]["YMaximumTravel"].InnerText); //mm
				ZMaximumTravel = double.Parse(doc["Parameters"]["ZMaximumTravel"].InnerText); //mm
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		internal void WriteToFile()
		{
			WriteToFile(configFilename);
		}

		private void WriteToFile(string filename)
		{
			new XDocument(
				new XElement("Parameters",
					new XElement("StartOffset", StartOffset.ToString()),
					new XElement("Table1Length", Table1Length.ToString()),
					new XElement("MiddleGap", MiddleGap.ToString()),
					new XElement("Table2Length", Table2Length.ToString()),
					new XElement("EndOffset", EndOffset.ToString()),
					new XElement("SerialPortString", SerialPortString.ToString()),
					new XElement("Baudrate", Baudrate.ToString()),
					new XElement("ControlAxis", ControlAxis.ToString()),
					new XElement("PurgeDuration", PurgeDuration.ToString()),
					new XElement("StepPulseTime", StepPulseTime.ToString()),
					new XElement("StepIdleDelay", StepIdleDelay.ToString()),
					new XElement("StepPortInvert", ((int)StepPortInvert).ToString()),
					new XElement("DirectionPortInvert", ((int)DirectionPortInvert).ToString()),
					new XElement("StepEnableInvert", StepEnableInvert.ToString()),
					new XElement("LimitPinsInvert", LimitPinsInvert.ToString()),
					new XElement("ProbePinInvert", ProbePinInvert.ToString()),
					new XElement("JunctionDeviation", JunctionDeviation.ToString()),
					new XElement("ArcTolerance", ArcTolerance.ToString()),
					new XElement("ReportInches", ReportInches.ToString()),
					new XElement("SoftLimits", SoftLimits.ToString()),
					new XElement("HardLimits", HardLimits.ToString()),
					new XElement("HomingCycle", HomingCycle.ToString()),
					new XElement("HomingDirectionInvert", ((int)HomingDirectionInvert).ToString()),
					new XElement("HomingFeed", HomingFeed.ToString()),
					new XElement("HomingSeek", HomingSeek.ToString()),
					new XElement("HomingDebounce", HomingDebounce.ToString()),
					new XElement("HomingPullOff", HomingPullOff.ToString()),
					new XElement("MaximumSpindleSpeed", MaximumSpindleSpeed.ToString()),
					new XElement("MinimumSpindleSpeed", MinimumSpindleSpeed.ToString()),
					new XElement("LaserMode", LaserMode.ToString()),
					new XElement("XSteps", XSteps.ToString()),
					new XElement("YSteps", YSteps.ToString()),
					new XElement("ZSteps", ZSteps.ToString()),
					new XElement("XFeedRate", XFeedRate.ToString()),
					new XElement("YFeedRate", YFeedRate.ToString()),
					new XElement("ZFeedRate", ZFeedRate.ToString()),
					new XElement("XAcceleration", XAcceleration.ToString()),
					new XElement("YAcceleration", YAcceleration.ToString()),
					new XElement("ZAcceleration", ZAcceleration.ToString()),
					new XElement("XMaximumTravel", XMaximumTravel.ToString()),
					new XElement("YMaximumTravel", YMaximumTravel.ToString()),
					new XElement("ZMaximumTravel", ZMaximumTravel.ToString())
				)).Save(filename);
		}
	}
}
