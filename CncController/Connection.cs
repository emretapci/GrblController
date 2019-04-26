using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CncController
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
		private ManualResetEvent readLoopExit = new ManualResetEvent(false);
		private ManualResetEvent readLoopExited = new ManualResetEvent(false);
		private Action<string> logFunction;

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

		internal Connection(Action<string> logFunction)
		{
			this.logFunction = logFunction;
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
			serialPort.Open();
			serialPort.DataReceived += (object sender, SerialDataReceivedEventArgs e) =>
			{
				var line = serialPort.ReadLine();
				var versionRegex = new Regex(@"Grbl\s(?<version>[\d\.a-z]*)\s\[\'\$\'\sfor\shelp\]");
				var version = versionRegex.Match(line).Groups["version"].Value;

				if(!string.IsNullOrWhiteSpace(version))
				{
					logFunction("GRBL version received: " + version);
					Status = Status.ConnectedStopped;
					SendGrblSettings();
				}
				else
				{
					logFunction("ERROR: Board does not respond with GRBL version.");
				}
			};

			Status = Status.Connecting;
		}

		internal void Disconnect()
		{
			if(Status == Status.ConnectedStarted)
			{
				Stop();
			}

			if (Status == Status.ConnectedStarted || Status == Status.ConnectedStopped || Status == Status.Connecting)
			{
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

		private void SendGrblSettings()
		{
			logFunction("Sending GRBL settings.");
			//TODO: Send grbl settings. If good response comes from board, set Status=Connected.
		}

		internal void Start()
		{
			Status = Status.ConnectedStarted;

			//Send any necessary header G codes.
			//Start sending G codes.
		}

		internal void Stop()
		{
			Status = Status.ConnectedStopped;

			//Stop sending G codes.
			//Send any necessary footer G codes.
		}
	}
}
