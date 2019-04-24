using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace CncController
{
	internal class Parameters
	{
		internal double StartOffset { get; set; } //mm
		internal double Table1Length { get; set; } //mm
		internal double MiddleGap { get; set; } //mm
		internal double Table2Length { get; set; } //mm
		internal double EndOffset { get; set; } //mm

		internal string SerialPortString { get; set; }
		internal uint Baudrate { get; set; }

		internal Parameters()
		{
			StartOffset = 100;
			Table1Length = 500;
			MiddleGap = 100;
			Table2Length = 500;
			EndOffset = 100;
			SerialPortString = "COM1";
			Baudrate = 115200;
		}

		internal static Parameters ReadFromFile()
		{
			if(!File.Exists("config.xml"))
			{
				WriteToFile(new Parameters());
			}

			var doc = new XmlDocument();
			doc.Load("config.xml");

			var parameters = new Parameters()
			{
				StartOffset = double.Parse(doc["Parameters"]["StartOffset"].InnerText),
				Table1Length = double.Parse(doc["Parameters"]["Table1Length"].InnerText),
				MiddleGap = double.Parse(doc["Parameters"]["MiddleGap"].InnerText),
				Table2Length = double.Parse(doc["Parameters"]["Table2Length"].InnerText),
				EndOffset = double.Parse(doc["Parameters"]["EndOffset"].InnerText),
				SerialPortString = doc["Parameters"]["SerialPortString"].InnerText,
				Baudrate = uint.Parse(doc["Parameters"]["Baudrate"].InnerText)
			};

			return parameters;
		}

		internal static void WriteToFile(Parameters parameters)
		{
			new XDocument(
				new XElement("Parameters",
					new XElement("StartOffset", parameters.StartOffset.ToString()),
					new XElement("Table1Length", parameters.Table1Length.ToString()),
					new XElement("MiddleGap", parameters.MiddleGap.ToString()),
					new XElement("Table2Length", parameters.Table2Length.ToString()),
					new XElement("EndOffset", parameters.EndOffset.ToString()),
					new XElement("SerialPortString", parameters.SerialPortString.ToString()),
					new XElement("Baudrate", parameters.Baudrate.ToString())
				)).Save("config.xml");
		}
	}
}
