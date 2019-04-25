using System;
using System.IO.Ports;
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
			serialPort.Open();

			Status = Status.Connecting;

			SendGrblSettings();
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
			MessageBox.Show(parameters.SerialPortString + " port opened.\nWill now send GRBL settings and wait for a good response.", "TODO", MessageBoxButtons.OK, MessageBoxIcon.Information);
			//TODO: Send grbl settings. If good response comes from board, set Status=Connected.
		}

		private void Start()
		{
			//Send any necessary header G codes.
			//Start sending G codes.
		}

		private void Stop()
		{
			//Stop sending G codes.
			//Send any necessary footer G codes.
		}
	}
}
