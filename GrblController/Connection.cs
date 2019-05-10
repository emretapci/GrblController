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
		Disconnected,
		Connecting,
		Connected
	}

	internal enum RunState
	{
		Running,
		Stopped,
		Calibrating
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
		internal RunState RunState { get; set; }
		internal MachineState MachineState { get; set; }
		internal Vector3D MachinePosition { get; set; } //can be negative.
		internal bool Painting { get; set; }

		internal double MachineCoordinate //can be negative.
		{
			get
			{
				switch (Main.Instance.Parameters.ControlAxis)
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
			RunState = status.RunState;
			MachineState = status.MachineState;
			MachinePosition = new Vector3D(status.MachinePosition);
			Painting = status.Painting;
		}
	}

	internal class Connection
	{
		private ManualResetEvent willStop = new ManualResetEvent(false);
		private ManualResetEvent stopped = new ManualResetEvent(false);
		private SerialPort serialPort;
		private ManualResetEvent responseReceived = new ManualResetEvent(false);
		private ManualResetEvent versionStringReceived = new ManualResetEvent(false);
		private ManualResetEvent probeReceived = new ManualResetEvent(false);
		private string response;
		private byte[] buffer = new byte[1000];
		private int length = 0;
		private System.Timers.Timer statusTimer;
		private System.Timers.Timer statusCheckTimer;
		private DateTime lastStatusReportTime;

		internal event Action<Status, Status> onStatusChanged;

		internal Status Status { get; private set; }

		internal void Initialize()
		{
			Status = new Status()
			{
				ConnectionState = ConnectionState.Disconnected,
				MachineState = MachineState.Unknown,
				MachinePosition = new Vector3D(),
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

			statusCheckTimer = new System.Timers.Timer(1000);
			statusCheckTimer.Interval = 500;
			statusCheckTimer.AutoReset = true;
			statusCheckTimer.Elapsed += (sender, e) =>
			{
				if (DateTime.Now.Subtract(lastStatusReportTime).TotalSeconds > 2)
				{
					SetStatus(new Status() { ConnectionState = ConnectionState.Disconnected });
				}
			};

			onStatusChanged += (oldStatus, newStatus) =>
			{
				if (oldStatus.ConnectionState == ConnectionState.Connected && newStatus.ConnectionState == ConnectionState.Disconnected)
				{
					SetStatus(new Status() { ConnectionState = ConnectionState.Connecting });
				}
				if (oldStatus.ConnectionState == ConnectionState.Disconnected && newStatus.ConnectionState == ConnectionState.Connecting)
				{
					Connect();
				}
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
			(new Thread(new ThreadStart(() =>
			{
				while (Status.ConnectionState == ConnectionState.Connecting)
				{
					var ports = SerialPort.GetPortNames().ToList();
					var configPortIndex = ports.IndexOf(Parameters["SerialPortString"]);
					if (configPortIndex >= 0)
					{
						ports.RemoveAt(configPortIndex);
						ports.Insert(0, Parameters["SerialPortString"]);
					}

					foreach (var port in ports)
					{
						if (TryPort(port))
						{
							Parameters["SerialPortString"] = port;
							Reset();
							break;
						}
					}
				}
			}))).Start();
		}

		private bool TryPort(string port)
		{
			//Connect with new parameters.
			serialPort = new SerialPort(port, Main.Instance.Parameters.Baudrate, Parity.None, 8, StopBits.One);

			try
			{
				serialPort.Open();
			}
			catch
			{
				return false;
			}

			serialPort.DataReceived += ProcessData;

			Thread.Sleep(2000);

			return Status.ConnectionState == ConnectionState.Connected;
		}

		internal void Reset()
		{
			willStop.Set();

			if (serialPort != null && serialPort.IsOpen)
			{
				try
				{
					serialPort.Write(new byte[] { 0x18 }, 0, 1);
				}
				catch (Exception e)
				{
					Main.Instance.AddLog("ERROR: " + e.Message);
				}
			}
		}

		internal void Send(string command)
		{
			if (serialPort != null && serialPort.IsOpen)
			{
				try
				{
					serialPort.WriteLine(command);
				}
				catch (Exception e)
				{
					Main.Instance.AddLog("ERROR: " + e.Message);
				}
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
					var probeRegex = new Regex(@"\[PRB:\-?\d+(\.\d*)?,\-?\d+(\.\d*)?,\-?\d+(\.\d*)?(:\d)?\]");
					var errorRegex = new Regex(@"error:\s*\d*");

					if (versionRegex.IsMatch(line))
					{
						versionStringReceived.Set();
						var version = versionRegex.Match(line).Groups["version"].Value;

						Main.Instance.AddLog("GRBL version received: " + version);

						(new Thread(new ThreadStart(() =>
						{
							willStop.Reset();
							Thread.Sleep(1000);
							if (WriteSettingsToBoard())
							{
								if (Unlock())
								{
									Thread.Sleep(500);
									if (SendGCodeAndWaitForOk("G54"))
									{
										Main.Instance.AddLog("Work coordinates selected.");
									}
								}
								SetStatus(new Status(Status) { ConnectionState = ConnectionState.Connected });
							}
							else
							{
								Disconnect();
							}
						}))).Start();

						statusTimer.Start();
					}
					else if (errorRegex.IsMatch(line))
					{
						Main.Instance.AddLog(line);
					}
					else if (probeRegex.IsMatch(line))
					{
						probeReceived.Set();
					}
					else if (Status.ConnectionState == ConnectionState.Connected && settingRegex.IsMatch(line))
					{
						var setting = settingRegex.Match(line).Groups["setting"].Value;
						var val = settingRegex.Match(line).Groups["value"].Value;
					}
					else if (line.Trim() == "ok")
					{
						response = "ok";
						responseReceived.Set();
					}
					else if (Status.ConnectionState == ConnectionState.Connected)
					{
						lastStatusReportTime = DateTime.Now;
						var statusMatches = StatusMatch(line);
						if (statusMatches.ContainsKey("WPosX") && statusMatches.ContainsKey("WPosY") && statusMatches.ContainsKey("WPosZ"))
						{
							SetStatus(new Status(Status)
							{
								MachinePosition = new Vector3D(double.Parse(statusMatches["WPosX"]), double.Parse(statusMatches["WPosY"]), double.Parse(statusMatches["WPosZ"])),
								MachineState = (MachineState)Enum.Parse(typeof(MachineState), statusMatches["state"])
							});
						}
					}
				}
			}
		}

		private bool WriteSettingsToBoard()
		{
			Main.Instance.AddLog("Writing settings to board.");

			bool result = true;
			result &= SendSetting(0, Main.Instance.Parameters.StepPulseTime.ToString());
			result &= SendSetting(1, Main.Instance.Parameters.StepIdleDelay.ToString());
			result &= SendSetting(2, ((int)Main.Instance.Parameters.StepPortInvert).ToString());
			result &= SendSetting(3, ((int)Main.Instance.Parameters.DirectionPortInvert).ToString());
			result &= SendSetting(4, Main.Instance.Parameters.StepEnableInvert ? "1" : "0");
			result &= SendSetting(5, Main.Instance.Parameters.LimitPinsInvert ? "1" : "0");
			result &= SendSetting(6, Main.Instance.Parameters.ProbePinInvert ? "1" : "0");
			result &= SendSetting(10, "0"); //Work coordinates
			result &= SendSetting(11, Main.Instance.Parameters.JunctionDeviation.ToString());
			result &= SendSetting(12, Main.Instance.Parameters.ArcTolerance.ToString());
			result &= SendSetting(13, Main.Instance.Parameters.ReportInches ? "1" : "0");
			result &= SendSetting(20, Main.Instance.Parameters.SoftLimits ? "1" : "0");
			result &= SendSetting(21, Main.Instance.Parameters.HardLimits ? "1" : "0");
			result &= SendSetting(22, Main.Instance.Parameters.HomingCycle ? "1" : "0");
			result &= SendSetting(23, ((int)Main.Instance.Parameters.HomingDirectionInvert).ToString());
			result &= SendSetting(24, Main.Instance.Parameters.HomingFeed.ToString());
			result &= SendSetting(25, Main.Instance.Parameters.HomingSeek.ToString());
			result &= SendSetting(26, Main.Instance.Parameters.HomingDebounce.ToString());
			result &= SendSetting(27, Main.Instance.Parameters.HomingPullOff.ToString());
			result &= SendSetting(30, Main.Instance.Parameters.MaximumSpindleSpeed.ToString());
			result &= SendSetting(31, Main.Instance.Parameters.MinimumSpindleSpeed.ToString());
			result &= SendSetting(32, Main.Instance.Parameters.LaserMode ? "1" : "0");
			result &= SendSetting(100, Main.Instance.Parameters.XSteps.ToString());
			result &= SendSetting(101, Main.Instance.Parameters.YSteps.ToString());
			result &= SendSetting(102, Main.Instance.Parameters.ZSteps.ToString());
			result &= SendSetting(110, Main.Instance.Parameters.XFeedRate.ToString());
			result &= SendSetting(111, Main.Instance.Parameters.YFeedRate.ToString());
			result &= SendSetting(112, Main.Instance.Parameters.ZFeedRate.ToString());
			result &= SendSetting(120, Main.Instance.Parameters.XAcceleration.ToString());
			result &= SendSetting(121, Main.Instance.Parameters.YAcceleration.ToString());
			result &= SendSetting(122, Main.Instance.Parameters.ZAcceleration.ToString());
			result &= SendSetting(130, Main.Instance.Parameters.XMaximumTravel.ToString());
			result &= SendSetting(131, Main.Instance.Parameters.YMaximumTravel.ToString());
			result &= SendSetting(132, Main.Instance.Parameters.ZMaximumTravel.ToString());
			return result;
		}

		private Dictionary<string, string> StatusMatch(string line)
		{
			var regex = new Regex[] {
				new Regex(@"(?<state>(Idle|Run|Hold|Jog|Alarm|Door|Check|Home|Sleep))"),
				new Regex(@"(MPos:(?<MPosX>\-?\d+(\.\d*)?),(?<MPosY>\-?\d+(\.\d*)?),(?<MPosZ>\-?\d+(\.\d*)?))"),
				new Regex(@"(WPos:(?<WPosX>\-?\d+(\.\d*)?),(?<WPosY>\-?\d+(\.\d*)?),(?<WPosZ>\-?\d+(\.\d*)?))"),
				new Regex(@"(Ov:(?<OvX>\-?\d+(\.\d*)?),(?<OvY>\-?\d+(\.\d*)?),(?<OvZ>\-?\d+(\.\d*)?))"),
				new Regex(@"(WCO:(?<WCOX>\-?\d+(\.\d*)?),(?<WCOY>\-?\d+(\.\d*)?),(?<WCOZ>\-?\d+(\.\d*)?))"),
				new Regex(@"(Bf:(?<BfAvBl>\d+),(?<BfAvByRxBuf>\d+))"),
				new Regex(@"(Ln:(?<Line>\d+))"),
				new Regex(@"((F:(?<FeedRate1>\d+))|(FS:(?<FeedRate2>\d+),(?<SpnSpd>\d+)))"),
				new Regex(@"(Pn:(?<Pins>[XYZPDHRSA]*))"),
				new Regex(@"(A:(?<AccState>[(S|C)FM]*))")
			};

			var matches = regex.Select(r => r.Matches(line)).Where(m => m.Count > 0).Select(m => m[0]);
			var namedMatches = new string[] { "state", "MPosX", "MPosY", "MPosZ", "WPosX", "WPosY", "WPosZ", "OvX", "OvY", "OvZ", "WCO", "Bf", "Ln", "FeedRate1", "FeedRate2", "SpnSpd", "Pins", "AccState" };

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

		internal bool SendSetting(int setting, string val)
		{
			responseReceived.Reset();
			Send("$" + setting + "=" + val);
			if (responseReceived.WaitOne(100))
			{
				if (response == "ok")
				{
					return true;
				}
				else
				{
					Main.Instance.AddLog("ERROR: Board did not accept \"$" + setting + "=" + val + "\".");
					return false;
				}
			}
			else
			{
				Main.Instance.AddLog("ERROR: Board did not respond to command \"$" + setting + "=" + val + "\".");
				return false;
			}
		}

		internal void Disconnect()
		{
			if (Status.ConnectionState == ConnectionState.Connected && Status.RunState == RunState.Calibrating)
			{
				stopped.Reset();
				willStop.Set();
				stopped.WaitOne();
			}

			if (Status.ConnectionState == ConnectionState.Connected && Status.RunState == RunState.Running)
			{
				Main.Instance.GeometryController.Stop();
			}

			statusTimer?.Stop();
			statusCheckTimer?.Stop();

			if (serialPort != null)
			{
				serialPort.DataReceived -= ProcessData;
				serialPort.Close();
				serialPort = null;
			}

			SetStatus(new Status(Status) { ConnectionState = ConnectionState.Disconnected });
		}

		internal void CheckCanConnect()
		{
			if (Status.ConnectionState == ConnectionState.DisconnectedCanConnect || Status.ConnectionState == ConnectionState.DisconnectedCannotConnect)
			{
				if (Array.IndexOf(SerialPort.GetPortNames(), Main.Instance.Parameters.SerialPortString) >= 0)
				{
					SetStatus(new Status(Status) { ConnectionState = ConnectionState.DisconnectedCanConnect });
				}
				else
				{
					SetStatus(new Status(Status) { ConnectionState = ConnectionState.DisconnectedCannotConnect });
				}
			}
		}

		internal void Calibrate(Action callback)
		{
			willStop.Reset();

			(new Thread(new ThreadStart(() =>
			{
				Main.Instance.AddLog("Starting calibration.");
				Main.Instance.SetSlidersEnabled(false);
				Main.Instance.SetMenuEnabled(false);
				Main.Instance.GeometryController.Stop();
				Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false, ConnectionState = ConnectionState.ConnectedCalibrating });

				if (!Unlock())
					return;

				#region Probing

				probeReceived.Reset();
				Main.Instance.AddLog("Probing now...");
				Main.Instance.Connection.Send("G38.2 Y-10000 F500");

				if (WaitHandle.WaitAny(new WaitHandle[] { probeReceived, willStop }) == 1)
				{
					stopped.Set();
					return;
				}

				#endregion

				#region Set feed rate

				Main.Instance.AddLog("Setting feed rate.");
				if (!SendGCodeAndWaitForOk("G94 F100"))
				{
					Main.Instance.AddLog("ERROR: Invalid response. (" + response + ")");
					return;
				}

				#endregion

				Thread.Sleep(1000);

				#region Zeroize

				Main.Instance.AddLog("Zeroizing.");
				if (!SendGCodeAndWaitForOk("G10 P0 L20 Y0"))
				{
					Main.Instance.AddLog("ERROR: Invalid response. (" + response + ")");
					return;
				}

				#endregion

				#region Jog to 5 mm

				Main.Instance.AddLog("Jogging to 5 mm");
				if (!SendGCodeAndWaitForOk("$J=G91 Y5 F100"))
				{
					Main.Instance.AddLog("ERROR: Invalid response. (" + response + ")");
					return;
				}

				#endregion

				stopped.Set();
				Main.Instance.AddLog("Calibration successful.");
				SetStatus(new Status(Status) { ConnectionState = ConnectionState.ConnectedStopped });
				Main.Instance.SetSlidersEnabled(true);
				Main.Instance.SetMenuEnabled(true);

				callback?.Invoke();
			}))).Start();
		}

		private bool SendGCodeAndWaitForOk(string command)
		{
			responseReceived.Reset();
			response = "";
			Main.Instance.Connection.Send(command);

			if (WaitHandle.WaitAny(new WaitHandle[] { responseReceived, willStop }) == 1)
			{
				stopped.Set();
				return false;
			}

			if (response != "ok")
			{
				stopped.Set();
				return false;
			}

			stopped.Set();
			return true;
		}

		internal bool Unlock()
		{
			if (Status.MachineState == MachineState.Alarm || Status.MachineState == MachineState.Unknown)
			{
				responseReceived.Reset();
				response = "";
				Main.Instance.AddLog("Unlocking.");
				Main.Instance.Connection.Send("$X");

				if (WaitHandle.WaitAny(new WaitHandle[] { responseReceived, willStop }) == 1)
				{
					stopped.Set();
					return false;
				}

				if (response != "ok")
				{
					Main.Instance.AddLog("ERROR: Invalid response. (" + response + ")");
					stopped.Set();
					return false;
				}
			}
			return true;
		}
	}
}
