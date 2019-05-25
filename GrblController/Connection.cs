using System;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;

namespace GrblController
{
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
		private List<string> serialPorts = new List<string>();

		internal void Initialize()
		{
			statusTimer = new System.Timers.Timer(1000);
			statusTimer.Interval = 500;
			statusTimer.Enabled = false;
			statusTimer.AutoReset = true;
			statusTimer.Elapsed += (sender, e) =>
			{
				Send("?");
			};

			statusCheckTimer = new System.Timers.Timer(1000);
			statusCheckTimer.Interval = 1000;
			statusCheckTimer.Enabled = false;
			statusCheckTimer.AutoReset = true;
			statusCheckTimer.Elapsed += (sender, e) =>
			{
				statusCheckTimer.Enabled = false;
				if (DateTime.Now.Subtract(lastStatusReportTime).TotalSeconds >= 3)
				{
					Reconnect();
				}
				statusCheckTimer.Enabled = true;
			};

			serialPorts = SerialPort.GetPortNames().ToList();
			var index = serialPorts.IndexOf(Parameters.Instance.SerialPortString);
			if (index >= 0)
			{
				serialPorts.RemoveAt(index);
				serialPorts.Insert(0, Parameters.Instance.SerialPortString);
			}

			statusCheckTimer.Enabled = true;
			lastStatusReportTime = DateTime.MinValue;
		}

		private void Reconnect()
		{
			statusTimer.Stop();
			Status.SetStatus(new Status() { ConnectionState = ConnectionState.Connecting });

			if (serialPorts.Count > 0)
			{
				TryPort(serialPorts[0]);
			}

			//Rearrange serial ports and reconnect
			var serialPorts2 = SerialPort.GetPortNames().ToList();
			var newSerialPorts = serialPorts2.Where(s => serialPorts.IndexOf(s) < 0).ToList();
			var intersection = serialPorts.Where(s => serialPorts2.IndexOf(s) >= 0).ToList();
			if (intersection.Count >= 2)
			{
				intersection.Add(intersection[0]);
				intersection.RemoveAt(0);
			}

			newSerialPorts.AddRange(intersection);
			serialPorts = newSerialPorts;

			lastStatusReportTime = DateTime.Now;
		}

		private void TryPort(string port)
		{
			if (serialPort != null && serialPort.IsOpen)
			{
				serialPort.Close();
			}

			serialPort = new SerialPort(port, Parameters.Instance.Baudrate, Parity.None, 8, StopBits.One);

			try
			{
				serialPort.Open();
				Status.SetStatus(new Status() { ConnectingPort = port });
				serialPort.DataReceived += ProcessData;
				Reset();
			}
			catch (Exception e)
			{
			}
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

		internal void Purge()
		{
			willStop.Reset();

			(new Thread(new ThreadStart(() =>
			{
				Main.Instance.AddLog("Starting purge.");
				Main.Instance.SetMenuEnabled(false);
				Main.Instance.GeometryController.Stop();
				Status.SetStatus(new Status() { Painting = true, ConnectionState = ConnectionState.Connected, RunState = RunState.Purging });

				if (!Unlock())
				{
					return;
				}

				#region Purging

				Main.Instance.Connection.Send("M8");

				if (willStop.WaitOne(4000))
				{
					Main.Instance.AddLog("Aborted.");
					Main.Instance.Connection.Send("M9");
					Status.SetStatus(new Status() { Painting = false });
					stopped.Set();
					return;
				}

				#endregion

				stopped.Set();
				Main.Instance.Connection.Send("M9");
				Main.Instance.AddLog("Purge completed.");
				Status.SetStatus(new Status() { ConnectionState = ConnectionState.Connected, RunState = RunState.Stopped });
			}))).Start();
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
								statusTimer.Start();
								lastStatusReportTime = DateTime.Now;
								Status.SetStatus(new Status() { ConnectionState = ConnectionState.Connected, RunState = RunState.Stopped });

								Parameters.Instance.SerialPortString = Status.Instance.ConnectingPort;
								Parameters.Instance.WriteToFile();
							}
							else
							{
								Main.Instance.AddLog("ERROR: Could not write settings to board.");
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
					else if (Status.Instance.ConnectionState == ConnectionState.Connected && settingRegex.IsMatch(line))
					{
						var setting = settingRegex.Match(line).Groups["setting"].Value;
						var val = settingRegex.Match(line).Groups["value"].Value;
					}
					else if (line.Trim() == "ok")
					{
						response = "ok";
						responseReceived.Set();
					}
					else if (Status.Instance.ConnectionState == ConnectionState.Connected)
					{
						lastStatusReportTime = DateTime.Now;
						var statusMatches = StatusMatch(line);
						if (statusMatches.ContainsKey("WPosX") && statusMatches.ContainsKey("WPosY") && statusMatches.ContainsKey("WPosZ"))
						{
							Status.SetStatus(new Status()
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
			result &= SendSetting(0, Parameters.Instance.StepPulseTime.ToString());
			result &= SendSetting(1, Parameters.Instance.StepIdleDelay.ToString());
			result &= SendSetting(2, ((int)Parameters.Instance.StepPortInvert).ToString());
			result &= SendSetting(3, ((int)Parameters.Instance.DirectionPortInvert).ToString());
			result &= SendSetting(4, Parameters.Instance.StepEnableInvert ? "1" : "0");
			result &= SendSetting(5, Parameters.Instance.LimitPinsInvert ? "1" : "0");
			result &= SendSetting(6, Parameters.Instance.ProbePinInvert ? "1" : "0");
			result &= SendSetting(10, "0"); //Work coordinates
			result &= SendSetting(11, Parameters.Instance.JunctionDeviation.ToString());
			result &= SendSetting(12, Parameters.Instance.ArcTolerance.ToString());
			result &= SendSetting(13, Parameters.Instance.ReportInches ? "1" : "0");
			result &= SendSetting(20, Parameters.Instance.SoftLimits ? "1" : "0");
			result &= SendSetting(21, Parameters.Instance.HardLimits ? "1" : "0");
			result &= SendSetting(22, Parameters.Instance.HomingCycle ? "1" : "0");
			result &= SendSetting(23, ((int)Parameters.Instance.HomingDirectionInvert).ToString());
			result &= SendSetting(24, Parameters.Instance.HomingFeed.ToString());
			result &= SendSetting(25, Parameters.Instance.HomingSeek.ToString());
			result &= SendSetting(26, Parameters.Instance.HomingDebounce.ToString());
			result &= SendSetting(27, Parameters.Instance.HomingPullOff.ToString());
			result &= SendSetting(30, Parameters.Instance.MaximumSpindleSpeed.ToString());
			result &= SendSetting(31, Parameters.Instance.MinimumSpindleSpeed.ToString());
			result &= SendSetting(32, Parameters.Instance.LaserMode ? "1" : "0");
			result &= SendSetting(100, Parameters.Instance.XSteps.ToString());
			result &= SendSetting(101, Parameters.Instance.YSteps.ToString());
			result &= SendSetting(102, Parameters.Instance.ZSteps.ToString());
			result &= SendSetting(110, Parameters.Instance.XFeedRate.ToString());
			result &= SendSetting(111, Parameters.Instance.YFeedRate.ToString());
			result &= SendSetting(112, Parameters.Instance.ZFeedRate.ToString());
			result &= SendSetting(120, Parameters.Instance.XAcceleration.ToString());
			result &= SendSetting(121, Parameters.Instance.YAcceleration.ToString());
			result &= SendSetting(122, Parameters.Instance.ZAcceleration.ToString());
			result &= SendSetting(130, Parameters.Instance.XMaximumTravel.ToString());
			result &= SendSetting(131, Parameters.Instance.YMaximumTravel.ToString());
			result &= SendSetting(132, Parameters.Instance.ZMaximumTravel.ToString());
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
			if (Status.Instance.ConnectionState == ConnectionState.Connected && Status.Instance.RunState == RunState.Calibrating)
			{
				stopped.Reset();
				willStop.Set();
				stopped.WaitOne();
			}

			if (Status.Instance.ConnectionState == ConnectionState.Connected && Status.Instance.RunState == RunState.Running)
			{
				Main.Instance.GeometryController.Stop();
			}

			if (serialPort != null)
			{
				serialPort.DataReceived -= ProcessData;
				serialPort.Close();
				serialPort = null;
			}

			Status.SetStatus(new Status() { ConnectionState = ConnectionState.Disconnected });
		}

		internal void Calibrate(Action callback)
		{
			willStop.Reset();

			(new Thread(new ThreadStart(() =>
			{
				Main.Instance.AddLog("Starting calibration.");
				Main.Instance.SetMenuEnabled(false);
				Main.Instance.GeometryController.Stop();
				Status.SetStatus(new Status() { Painting = false, ConnectionState = ConnectionState.Connected, RunState = RunState.Calibrating });

				if (!Unlock())
				{
					return;
				}

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
				Status.SetStatus(new Status() { ConnectionState = ConnectionState.Connected, RunState = RunState.Stopped });
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
			if (Status.Instance.MachineState == MachineState.Alarm || Status.Instance.MachineState == MachineState.Unknown)
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
