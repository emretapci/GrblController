using System;

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
		Calibrating,
		Purging
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
		private static Status instance;

		internal ConnectionState? ConnectionState { get; set; }
		internal string ConnectingPort { get; set; }
		internal RunState? RunState { get; set; }
		internal MachineState? MachineState { get; set; }
		internal Vector3D MachinePosition { get; set; } //can be negative.
		internal bool? Painting { get; set; }

		internal static event Action<Status, Status> onStatusChanged;

		internal static Status Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Status()
					{
						ConnectionState = GrblController.ConnectionState.Disconnected,
						ConnectingPort = "",
						RunState = GrblController.RunState.Stopped,
						MachineState = GrblController.MachineState.Unknown,
						MachinePosition = new Vector3D(),
						Painting = false
					};
				}
				return instance;
			}
		}

		internal double MachineCoordinate //can be negative.
		{
			get
			{
				switch (Parameters.Instance.ControlAxis)
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
				switch (Parameters.Instance.ControlAxis)
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
			ConnectionState = null;
			ConnectingPort = null;
			RunState = null;
			MachineState = null;
			MachinePosition = null;
			Painting = null;
		}

		internal Status(Status status)
		{
			ConnectionState = status.ConnectionState;
			ConnectingPort = status.ConnectingPort;
			RunState = status.RunState;
			MachineState = status.MachineState;
			MachinePosition = new Vector3D(status.MachinePosition);
			Painting = status.Painting;
		}

		internal static void SetStatus(Status statusDiff)
		{
			var oldStatus = new Status(Instance);
			var newStatus = new Status(Instance);

			if (statusDiff.ConnectionState.HasValue)
			{
				newStatus.ConnectionState = statusDiff.ConnectionState.Value;
			}
			if (statusDiff.ConnectingPort != null)
			{
				newStatus.ConnectingPort = statusDiff.ConnectingPort;
			}
			if (statusDiff.RunState.HasValue)
			{
				newStatus.RunState = statusDiff.RunState.Value;
			}
			if (statusDiff.MachineState.HasValue)
			{
				newStatus.MachineState = statusDiff.MachineState.Value;
			}
			if (statusDiff.MachinePosition != null)
			{
				newStatus.MachinePosition = new Vector3D(statusDiff.MachinePosition);
			}
			if (statusDiff.Painting.HasValue)
			{
				newStatus.Painting = statusDiff.Painting.Value;
			}

			instance = new Status(newStatus);
			onStatusChanged?.Invoke(oldStatus, newStatus);
		}
	}
}
