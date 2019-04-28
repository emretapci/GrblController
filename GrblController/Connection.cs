using System;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace GrblController
{
	internal enum Status
	{
		DisconnectedCannotConnect,
		DisconnectedCanConnect,
		Connecting,
		ConnectedStarted,
		ConnectedStopped
	}

	internal class Connection
	{
		private Status status;
		private Parameters parameters;
		private SerialPort serialPort;
		private ManualResetEvent responseReceived = new ManualResetEvent(false);
		private string response;
		private byte[] buffer = new byte[1000];
		private int length = 0;

		internal event Action<Status, Status> onStatusChanged;

		internal Status Status
		{
			get
			{
				return status;
			}
			set
			{
				var old = status;
				status = value;
				onStatusChanged?.Invoke(old, status);
			}
		}

		internal void Initialize(Parameters parameters)
		{
			this.parameters = parameters;
			Disconnect();
		}

		internal void Connect()
		{
			if (Status == Status.ConnectedStarted || Status == Status.ConnectedStopped || Status == Status.Connecting)
			{
				return;
			}

			//Connect with new parameters.
			serialPort = new SerialPort(parameters.SerialPortString, parameters.Baudrate, Parity.None, 8, StopBits.One);

			try
			{
				serialPort.Open();
			}
			catch
			{
				MessageBox.Show("Cannot open " + parameters.SerialPortString + ". Maybe it is already open.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Disconnect();
				return;
			}

			serialPort.DataReceived += ProcessData;

			Status = Status.Connecting;

			serialPort.Write(new byte[] { 0x18 }, 0, 1);
		}

		internal void Send(string command)
		{
			serialPort.WriteLine(command);
		}

		private void ProcessData(object sender, SerialDataReceivedEventArgs e)
		{
			int readBytes = serialPort.Read(buffer, length, serialPort.BytesToRead);
			length += readBytes;

			while (true)
			{
				int newlineIndex = Array.IndexOf(buffer, (byte)'\n');
				if(newlineIndex >= length)
				{
					break;
				}

				var line = Encoding.ASCII.GetString(buffer, 0, newlineIndex).Trim();
				length -= newlineIndex + 1;
				Array.Copy(buffer, newlineIndex + 1, buffer, 0, length);

				if (!string.IsNullOrWhiteSpace(line))
				{
					var versionRegex = new Regex(@"Grbl\s(?<version>[\d\.a-z]*)\s\[\'\$\'\sfor\shelp\]");
					var settingRegex = new Regex(@"\$(?<setting>\d+)=(?<value>\d+(\.\d*)?)");
					var okRegex = new Regex(@"ok");

					if (Status == Status.Connecting && versionRegex.IsMatch(line))
					{
						var version = versionRegex.Match(line).Groups["version"].Value;
						Main.Instance.AddLog("GRBL version received: " + version);
						Status = Status.ConnectedStopped;
						Main.Instance.AddLog("Receiving settings.");
						Send("$$");
					}
					else if (Status == Status.ConnectedStopped && settingRegex.IsMatch(line))
					{
						var setting = settingRegex.Match(line).Groups["setting"].Value;
						var val = settingRegex.Match(line).Groups["value"].Value;
						RecordSetting(setting, val);
						Main.Instance.AddLog("$" + setting + "=" + val);
					}
					else if (Status == Status.ConnectedStopped && okRegex.IsMatch(line))
					{
						responseReceived.Set();
						response = line.Trim();
					}
				}
			}
		}

		private void RecordSetting(string setting, string val)
		{
			var parameters = Parameters.ReadFromFile();

			switch (setting)
			{
				case "0":
					parameters.StepPulseTime = double.Parse(val);
					break;
				case "1":
					parameters.StepIdleDelay = double.Parse(val);
					break;
				case "2":
					parameters.StepPortInvert = (Mask)(int.Parse(val) & 7);
					break;
				case "3":
					parameters.DirectionPortInvert = (Mask)(int.Parse(val) & 7);
					break;
				case "4":
					parameters.StepEnableInvert = (int.Parse(val) > 0);
					break;
				case "5":
					parameters.LimitPinsInvert = (int.Parse(val) > 0);
					break;
				case "6":
					parameters.ProbePinInvert = (int.Parse(val) > 0);
					break;
				case "10":
					parameters.StatusReport = (StatusReport)(int.Parse(val) & 3);
					break;
				case "11":
					parameters.JunctionDeviation = double.Parse(val);
					break;
				case "12":
					parameters.ArcTolerance = double.Parse(val);
					break;
				case "13":
					parameters.ReportInches = (int.Parse(val) > 0);
					break;
				case "20":
					parameters.SoftLimits = (int.Parse(val) > 0);
					break;
				case "21":
					parameters.HardLimits = (int.Parse(val) > 0);
					break;
				case "22":
					parameters.HomingCycle = (int.Parse(val) > 0);
					break;
				case "23":
					parameters.HomingDirectionInvert = (Mask)(int.Parse(val) & 7);
					break;
				case "24":
					parameters.HomingFeed = double.Parse(val);
					break;
				case "25":
					parameters.HomingSeek = double.Parse(val);
					break;
				case "26":
					parameters.HomingDebounce = double.Parse(val);
					break;
				case "27":
					parameters.HomingPullOff = double.Parse(val);
					break;
				case "30":
					parameters.MaximumSpindleSpeed = double.Parse(val);
					break;
				case "31":
					parameters.MinimumSpindleSpeed = double.Parse(val);
					break;
				case "32":
					parameters.LaserMode = (int.Parse(val) > 0);
					break;
				case "100":
					parameters.XSteps = double.Parse(val);
					break;
				case "101":
					parameters.YSteps = double.Parse(val);
					break;
				case "102":
					parameters.ZSteps = double.Parse(val);
					break;
				case "110":
					parameters.XFeedRate = double.Parse(val);
					break;
				case "111":
					parameters.YFeedRate = double.Parse(val);
					break;
				case "112":
					parameters.ZFeedRate = double.Parse(val);
					break;
				case "120":
					parameters.XAcceleration = double.Parse(val);
					break;
				case "121":
					parameters.YAcceleration = double.Parse(val);
					break;
				case "122":
					parameters.ZAcceleration = double.Parse(val);
					break;
				case "130":
					parameters.XMaximumTravel = double.Parse(val);
					break;
				case "131":
					parameters.YMaximumTravel = double.Parse(val);
					break;
				case "132":
					parameters.ZMaximumTravel = double.Parse(val);
					break;
			}

			Parameters.WriteToFile(parameters);
			this.parameters = parameters;
		}

		internal void SendSettings(Parameters parameters)
		{
			bool settingsChanged = false;

			if (this.parameters.StepPulseTime != parameters.StepPulseTime)
			{
				SendSetting(0, parameters.StepPulseTime.ToString());
				settingsChanged = true;
			}
			if (this.parameters.StepIdleDelay != parameters.StepIdleDelay)
			{
				SendSetting(1, parameters.StepIdleDelay.ToString());
				settingsChanged = true;
			}
			if (this.parameters.StepPortInvert != parameters.StepPortInvert)
			{
				SendSetting(2, ((int)parameters.StepPortInvert).ToString());
				settingsChanged = true;
			}
			if (this.parameters.DirectionPortInvert != parameters.DirectionPortInvert)
			{
				SendSetting(3, ((int)parameters.DirectionPortInvert).ToString());
				settingsChanged = true;
			}
			if (this.parameters.StepEnableInvert != parameters.StepEnableInvert)
			{
				SendSetting(4, parameters.StepEnableInvert ? "1" : "0");
				settingsChanged = true;
			}
			if (this.parameters.LimitPinsInvert != parameters.LimitPinsInvert)
			{
				SendSetting(5, parameters.LimitPinsInvert ? "1" : "0");
				settingsChanged = true;
			}
			if (this.parameters.ProbePinInvert != parameters.ProbePinInvert)
			{
				SendSetting(6, parameters.ProbePinInvert ? "1" : "0");
				settingsChanged = true;
			}
			if (this.parameters.StatusReport != parameters.StatusReport)
			{
				SendSetting(10, ((int)parameters.StatusReport).ToString());
				settingsChanged = true;
			}
			if (this.parameters.JunctionDeviation != parameters.JunctionDeviation)
			{
				SendSetting(11, parameters.JunctionDeviation.ToString());
				settingsChanged = true;
			}
			if (this.parameters.ArcTolerance != parameters.ArcTolerance)
			{
				SendSetting(12, parameters.ArcTolerance.ToString());
				settingsChanged = true;
			}
			if (this.parameters.ReportInches != parameters.ReportInches)
			{
				SendSetting(13, parameters.ReportInches ? "1" : "0");
				settingsChanged = true;
			}
			if (this.parameters.SoftLimits != parameters.SoftLimits)
			{
				SendSetting(20, parameters.SoftLimits ? "1" : "0");
				settingsChanged = true;
			}
			if (this.parameters.HardLimits != parameters.HardLimits)
			{
				SendSetting(21, parameters.HardLimits ? "1" : "0");
				settingsChanged = true;
			}
			if (this.parameters.HomingCycle != parameters.HomingCycle)
			{
				SendSetting(22, parameters.HomingCycle ? "1" : "0");
				settingsChanged = true;
			}
			if (this.parameters.HomingDirectionInvert != parameters.HomingDirectionInvert)
			{
				SendSetting(23, ((int)parameters.HomingDirectionInvert).ToString());
				settingsChanged = true;
			}
			if (this.parameters.HomingFeed != parameters.HomingFeed)
			{
				SendSetting(24, parameters.HomingFeed.ToString());
				settingsChanged = true;
			}
			if (this.parameters.HomingSeek != parameters.HomingSeek)
			{
				SendSetting(25, parameters.HomingSeek.ToString());
				settingsChanged = true;
			}
			if (this.parameters.HomingDebounce != parameters.HomingDebounce)
			{
				SendSetting(26, parameters.HomingDebounce.ToString());
				settingsChanged = true;
			}
			if (this.parameters.HomingPullOff != parameters.HomingPullOff)
			{
				SendSetting(27, parameters.HomingPullOff.ToString());
				settingsChanged = true;
			}
			if (this.parameters.MaximumSpindleSpeed != parameters.MaximumSpindleSpeed)
			{
				SendSetting(30, parameters.MaximumSpindleSpeed.ToString());
				settingsChanged = true;
			}
			if (this.parameters.MinimumSpindleSpeed != parameters.MinimumSpindleSpeed)
			{
				SendSetting(31, parameters.MinimumSpindleSpeed.ToString());
				settingsChanged = true;
			}
			if (this.parameters.LaserMode != parameters.LaserMode)
			{
				SendSetting(32, parameters.LaserMode ? "1" : "0");
				settingsChanged = true;
			}
			if (this.parameters.XSteps != parameters.XSteps)
			{
				SendSetting(100, parameters.XSteps.ToString());
				settingsChanged = true;
			}
			if (this.parameters.YSteps != parameters.YSteps)
			{
				SendSetting(101, parameters.YSteps.ToString());
				settingsChanged = true;
			}
			if (this.parameters.ZSteps != parameters.ZSteps)
			{
				SendSetting(102, parameters.ZSteps.ToString());
				settingsChanged = true;
			}
			if (this.parameters.XFeedRate != parameters.XFeedRate)
			{
				SendSetting(110, parameters.XFeedRate.ToString());
				settingsChanged = true;
			}
			if (this.parameters.YFeedRate != parameters.YFeedRate)
			{
				SendSetting(111, parameters.YFeedRate.ToString());
				settingsChanged = true;
			}
			if (this.parameters.ZFeedRate != parameters.ZFeedRate)
			{
				SendSetting(112, parameters.ZFeedRate.ToString());
				settingsChanged = true;
			}
			if (this.parameters.XAcceleration != parameters.XAcceleration)
			{
				SendSetting(120, parameters.XAcceleration.ToString());
				settingsChanged = true;
			}
			if (this.parameters.YAcceleration != parameters.YAcceleration)
			{
				SendSetting(121, parameters.YAcceleration.ToString());
				settingsChanged = true;
			}
			if (this.parameters.ZAcceleration != parameters.ZAcceleration)
			{
				SendSetting(122, parameters.ZAcceleration.ToString());
				settingsChanged = true;
			}
			if (this.parameters.XMaximumTravel != parameters.XMaximumTravel)
			{
				SendSetting(130, parameters.XMaximumTravel.ToString());
				settingsChanged = true;
			}
			if (this.parameters.YMaximumTravel != parameters.YMaximumTravel)
			{
				SendSetting(131, parameters.YMaximumTravel.ToString());
				settingsChanged = true;
			}
			if (this.parameters.ZMaximumTravel != parameters.ZMaximumTravel)
			{
				SendSetting(132, parameters.ZMaximumTravel.ToString());
				settingsChanged = true;
			}

			if(settingsChanged)
			{
				Disconnect();
			}
		}

		private void SendSetting(int setting, string val)
		{
			Send("$" + setting + "=" + val);
			if (responseReceived.WaitOne(1000))
			{
				if (response == "ok")
				{
					Main.Instance.AddLog("\"$" + setting + "=" + val + "\" command accepted.");
				}
				else
				{
					Main.Instance.AddLog("ERROR: Board did not accept \"$" + setting + "=" + val + "\".");
				}
			}
			else
			{
				Main.Instance.AddLog("ERROR: Board did not respond to command \"$" + setting + "=" + val + "\".");
			}
		}

		internal void Disconnect()
		{
			if (Status == Status.ConnectedStarted)
			{
				Main.Instance.GeometryController.Stop();
			}

			if ((Status == Status.ConnectedStarted || Status == Status.ConnectedStopped || Status == Status.Connecting) && serialPort != null)
			{
				serialPort.DataReceived -= ProcessData;
				serialPort.Close();
				serialPort.Dispose();
				serialPort = null;
			}

			if (Array.IndexOf(SerialPort.GetPortNames(), parameters.SerialPortString) >= 0)
			{
				Status = Status.DisconnectedCanConnect;
			}
			else
			{
				Status = Status.DisconnectedCannotConnect;
			}
		}
	}
}
