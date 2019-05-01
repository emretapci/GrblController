using System;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GrblController
{
	internal enum ConnectionState
	{
		DisconnectedCannotConnect,
		DisconnectedCanConnect,
		Connecting,
		ConnectedStarted,
		ConnectedStopped
	}

	internal enum MachineState
	{
		Unknown,
		Idle,
		Run,
		Hold,
		Jog,
		Alarm,
		Door,
		Check,
		Home,
		Sleep
	}

	internal class Status
	{
		internal ConnectionState ConnectionState { get; set; }
		internal MachineState MachineState { get; set; }
		internal Vector3D MachinePosition { get; set; }
		internal bool Painting { get; set; }

		internal double MachineCoordinate
		{
			get
			{
				switch(Main.Instance.Parameters.ControlAxis)
				{
					case ControlAxis.X:
						return MachinePosition.X;
					case ControlAxis.Y:
						return MachinePosition.Y;
					case ControlAxis.Z:
						return MachinePosition.Z;
				}
				return double.NaN;
			}
			set
			{
				switch (Main.Instance.Parameters.ControlAxis)
				{
					case ControlAxis.X:
						MachinePosition.X = value;
						break;
					case ControlAxis.Y:
						MachinePosition.Y = value;
						break;
					case ControlAxis.Z:
						MachinePosition.Z = value;
						break;
				}
			}
		}

		internal Status()
		{
		}

		internal Status(Status status)
		{
			ConnectionState = status.ConnectionState;
			MachineState = status.MachineState;
			MachinePosition = new Vector3D(status.MachinePosition);
			Painting = status.Painting;
		}
	}

	internal class Connection
	{
		private SerialPort serialPort;
		private ManualResetEvent responseReceived = new ManualResetEvent(false);
		private string response;
		private byte[] buffer = new byte[1000];
		private int length = 0;
		private System.Timers.Timer statusTimer;

		internal event Action<Status, Status> onStatusChanged;

		internal Status Status { get; private set; }

		internal void Initialize()
		{
			Status = new Status()
			{
				ConnectionState = ConnectionState.DisconnectedCannotConnect,
				MachineState = MachineState.Unknown,
				MachinePosition = new Vector3D(0, 0, 0),
				Painting = false
			};

			Disconnect();

			statusTimer = new System.Timers.Timer(1000);
			statusTimer.Interval = 500;
			statusTimer.AutoReset = true;
			statusTimer.Elapsed += (sender, e) =>
			{
				Send("?");
			};
		}

		internal void SetStatus(Status status)
		{
			var old = Status;
			Status = status;
			onStatusChanged?.Invoke(old, Status);
		}

		internal void Connect()
		{
			if (Status.ConnectionState == ConnectionState.ConnectedStarted || Status.ConnectionState == ConnectionState.ConnectedStopped || Status.ConnectionState == ConnectionState.Connecting)
			{
				return;
			}

			//Connect with new parameters.
			serialPort = new SerialPort(Main.Instance.Parameters.SerialPortString, Main.Instance.Parameters.Baudrate, Parity.None, 8, StopBits.One);

			try
			{
				serialPort.Open();
			}
			catch
			{
				MessageBox.Show("Cannot open " + Main.Instance.Parameters.SerialPortString + ". Maybe it is already open.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Disconnect();
				return;
			}

			serialPort.DataReceived += ProcessData;

			SetStatus(new Status(Status) { ConnectionState = ConnectionState.Connecting });

			serialPort.Write(new byte[] { 0x18 }, 0, 1);
		}

		internal void Send(string command)
		{
			if (serialPort != null && serialPort.IsOpen)
			{
				serialPort.WriteLine(command);
			}
		}

		private void ProcessData(object sender, SerialDataReceivedEventArgs e)
		{
			int readBytes = serialPort.Read(buffer, length, serialPort.BytesToRead);
			length += readBytes;

			while (true)
			{
				int newlineIndex = Math.Min(Array.IndexOf(buffer, (byte)'\n'), Array.IndexOf(buffer, (byte)'\r'));
				if (newlineIndex >= length || newlineIndex < 0)
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
					bool processed = false;

					if (Status.ConnectionState == ConnectionState.Connecting && versionRegex.IsMatch(line))
					{
						var version = versionRegex.Match(line).Groups["version"].Value;
						Main.Instance.AddLog("GRBL version received: " + version);
						SetStatus(new Status(Status) { ConnectionState = ConnectionState.ConnectedStopped });

						//not necessary anymore. will be flushed anyway.
						//Main.Instance.AddLog("Receiving settings.");
						//Send("$$");
						statusTimer.Start();
						processed = true;
					}
					else if (Status.ConnectionState == ConnectionState.ConnectedStopped && settingRegex.IsMatch(line))
					{
						var setting = settingRegex.Match(line).Groups["setting"].Value;
						var val = settingRegex.Match(line).Groups["value"].Value;

						//settings in the config file is the master. config on the board is always overridden.
						//RecordSetting(setting, val);
						//Main.Instance.AddLog("$" + setting + "=" + val);

						processed = true;
					}
					else if (Status.ConnectionState == ConnectionState.ConnectedStopped && okRegex.IsMatch(line))
					{
						response = line.Trim();
						responseReceived.Set();
						processed = true;
					}
					else if (Status.ConnectionState == ConnectionState.ConnectedStopped || Status.ConnectionState == ConnectionState.ConnectedStarted)
					{
						var statusMatches = StatusMatch(line);
						if (statusMatches.ContainsKey("MPosX") && statusMatches.ContainsKey("MPosY") && statusMatches.ContainsKey("MPosZ"))
						{
							SetStatus(new Status(Status) { MachinePosition = new Vector3D(-double.Parse(statusMatches["MPosX"]), -double.Parse(statusMatches["MPosY"]), -double.Parse(statusMatches["MPosZ"])) });
							processed = true;
						}
					}

					/*if (!processed && line.Trim() != "ok")
					{
						Main.Instance.AddLog(line);
					}*/
				}
			}
		}

		internal void WriteSettingsToBoard()
		{
			SendSetting(0, Main.Instance.Parameters.StepPulseTime.ToString());
			SendSetting(1, Main.Instance.Parameters.StepIdleDelay.ToString());
			SendSetting(2, ((int)Main.Instance.Parameters.StepPortInvert).ToString());
			SendSetting(3, ((int)Main.Instance.Parameters.DirectionPortInvert).ToString());
			SendSetting(4, Main.Instance.Parameters.StepEnableInvert ? "1" : "0");
			SendSetting(5, Main.Instance.Parameters.LimitPinsInvert ? "1" : "0");
			SendSetting(6, Main.Instance.Parameters.ProbePinInvert ? "1" : "0");
			SendSetting(10, ((int)Main.Instance.Parameters.StatusReport).ToString());
			SendSetting(11, Main.Instance.Parameters.JunctionDeviation.ToString());
			SendSetting(12, Main.Instance.Parameters.ArcTolerance.ToString());
			SendSetting(13, Main.Instance.Parameters.ReportInches ? "1" : "0");
			SendSetting(20, Main.Instance.Parameters.SoftLimits ? "1" : "0");
			SendSetting(21, Main.Instance.Parameters.HardLimits ? "1" : "0");
			SendSetting(22, Main.Instance.Parameters.HomingCycle ? "1" : "0");
			SendSetting(23, ((int)Main.Instance.Parameters.HomingDirectionInvert).ToString());
			SendSetting(24, Main.Instance.Parameters.HomingFeed.ToString());
			SendSetting(25, Main.Instance.Parameters.HomingSeek.ToString());
			SendSetting(26, Main.Instance.Parameters.HomingDebounce.ToString());
			SendSetting(27, Main.Instance.Parameters.HomingPullOff.ToString());
			SendSetting(30, Main.Instance.Parameters.MaximumSpindleSpeed.ToString());
			SendSetting(31, Main.Instance.Parameters.MinimumSpindleSpeed.ToString());
			SendSetting(32, Main.Instance.Parameters.LaserMode ? "1" : "0");
			SendSetting(100, Main.Instance.Parameters.XSteps.ToString());
			SendSetting(101, Main.Instance.Parameters.YSteps.ToString());
			SendSetting(102, Main.Instance.Parameters.ZSteps.ToString());
			SendSetting(110, Main.Instance.Parameters.XFeedRate.ToString());
			SendSetting(111, Main.Instance.Parameters.YFeedRate.ToString());
			SendSetting(112, Main.Instance.Parameters.ZFeedRate.ToString());
			SendSetting(120, Main.Instance.Parameters.XAcceleration.ToString());
			SendSetting(121, Main.Instance.Parameters.YAcceleration.ToString());
			SendSetting(122, Main.Instance.Parameters.ZAcceleration.ToString());
			SendSetting(130, Main.Instance.Parameters.XMaximumTravel.ToString());
			SendSetting(131, Main.Instance.Parameters.YMaximumTravel.ToString());
			SendSetting(132, Main.Instance.Parameters.ZMaximumTravel.ToString());
		}

		private Dictionary<string, string> StatusMatch(string line)
		{
			var regex = new Regex[] {
				new Regex(@"(?<state>(Idle|Run|Hold|Jog|Alarm|Door|Check|Home|Sleep))"),
				new Regex(@"(MPos:(?<MPosX>\-?\d+(\.\d*)?),(?<MPosY>\-?\d+(\.\d*)?),(?<MPosZ>\-?\d+(\.\d*)?))"),
				new Regex(@"(Ov:(?<OvX>\-?\d+(\.\d*)?),(?<OvY>\-?\d+(\.\d*)?),(?<OvZ>\-?\d+(\.\d*)?))"),
				new Regex(@"(WCO:(?<WCOX>\-?\d+(\.\d*)?),(?<WCOY>\-?\d+(\.\d*)?),(?<WCOZ>\-?\d+(\.\d*)?))"),
				new Regex(@"(Bf:(?<BfAvBl>\d+),(?<BfAvByRxBuf>\d+))"),
				new Regex(@"(Ln:(?<Line>\d+))"),
				new Regex(@"((F:(?<FeedRate1>\d+))|(FS:(?<FeedRate2>\d+),(?<SpnSpd>\d+)))"),
				new Regex(@"(Pn:(?<Pins>[XYZPDHRSA]*))"),
				new Regex(@"(A:(?<AccState>[(S|C)FM]*))")
			};

			var matches = regex.Select(r => r.Matches(line)).Where(m => m.Count > 0).Select(m => m[0]);
			var namedMatches = new string[] { "state", "MPosX", "MPosY", "MPosZ", "OvX", "OvY", "OvZ", "WCO", "Bf", "Ln", "FeedRate1", "FeedRate2", "SpnSpd", "Pins", "AccState" };

			var matchesDict = new Dictionary<string, string>();

			foreach (var match in matches)
			{
				foreach (var match2 in namedMatches.Where(nm => !string.IsNullOrWhiteSpace(match.Groups[nm].Value)))
				{
					matchesDict.Add(match2, match.Groups[match2].Value);
				}
			}

			return matchesDict;
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
		}

		internal void SendSetting(int setting, string val)
		{
			Send("$" + setting + "=" + val);
			if (responseReceived.WaitOne(1000))
			{
				if (response == "ok")
				{
					//Main.Instance.AddLog("\"$" + setting + "=" + val + "\" command accepted.");
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
			if (Status.ConnectionState == ConnectionState.ConnectedStarted)
			{
				Main.Instance.GeometryController.Stop();
			}

			statusTimer?.Stop();

			if ((Status.ConnectionState == ConnectionState.ConnectedStarted || Status.ConnectionState == ConnectionState.ConnectedStopped ||
				Status.ConnectionState == ConnectionState.Connecting) && serialPort != null)
			{
				serialPort.DataReceived -= ProcessData;
				serialPort.Dispose();
				serialPort = null;
			}

			if (Array.IndexOf(SerialPort.GetPortNames(), Main.Instance.Parameters.SerialPortString) >= 0)
			{
				Status.ConnectionState = ConnectionState.DisconnectedCanConnect;
				SetStatus(new Status(Status) { ConnectionState = ConnectionState.DisconnectedCanConnect });
			}
			else
			{
				SetStatus(new Status(Status) { ConnectionState = ConnectionState.DisconnectedCannotConnect });
			}
		}
	}
}
