using System;
using System.Threading;
using System.Windows.Forms;

namespace GrblController
{
	internal class GeometryController
	{
		private ManualResetEvent willStop = new ManualResetEvent(false);
		private ManualResetEvent stopped = new ManualResetEvent(false);
		private ManualResetEvent targetReached = new ManualResetEvent(false);

		private double targetCoord; //can be negative.
		private bool exiting;

		private Thread thread;

		internal void Start()
		{
			thread = new Thread(new ThreadStart(ThreadFunc));

			exiting = false;

			Main.Instance.Connection.onStatusChanged += Connection_onStatusChanged;

			willStop.Reset();
			thread.Start();

			Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { ConnectionState = ConnectionState.ConnectedStarted });
		}

		private void Connection_onStatusChanged(Status oldStatus, Status newStatus)
		{
			if (Math.Abs(newStatus.MachineCoordinate - targetCoord) < 0.1)
			{
				targetReached.Set();
			}
		}

		internal void Stop()
		{
			exiting = true;

			Main.Instance.Connection.onStatusChanged -= Connection_onStatusChanged;

			Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { ConnectionState = ConnectionState.ConnectedStopped });

			if (thread != null && thread.IsAlive)
			{
				stopped.Reset();
				willStop.Set();
				stopped.WaitOne();
			}
		}

		private void WaitXPosition(double targetCoord)
		{
			targetReached.Reset();
			this.targetCoord = (Main.Instance.Parameters.ReverseFeed ? -1 : 1) * targetCoord;
			WaitHandle.WaitAny(new WaitHandle[] { targetReached, willStop });
		}

		private void ThreadFunc()
		{
			Main.Instance.Connection.Send("$X");

			Main.Instance.AddLog("Go to beginning of Table 1 paint area.");
			Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + (Main.Instance.Parameters.ReverseFeed ? "-" : "") + Main.Instance.Parameters.StartOffset.ToString("0.0"));
			WaitXPosition(Main.Instance.Parameters.StartOffset);

			if (exiting)
			{
				stopped.Set();
				return;
			}

			if (Main.Instance.Connection.Status.ConnectionState != ConnectionState.ConnectedStarted)
			{
				Main.Instance.AddLog("Set status to Stopped.");
				stopped.Set();
				return;
			}

			Main.Instance.AddLog("Start sprayer.");
			Main.Instance.Connection.Send("M3");
			Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = true });

			Main.Instance.AddLog("Go to end of Table 1 paint area.");
			var target = Main.Instance.Parameters.StartOffset + Main.Instance.Table1PaintedAreaRatio * Main.Instance.Parameters.Table1Length;
			Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + (Main.Instance.Parameters.ReverseFeed ? "-" : "") + target.ToString("0.0"));
			WaitXPosition(target);

			if (exiting)
			{
				stopped.Set();
				return;
			}

			if (Main.Instance.Connection.Status.ConnectionState != ConnectionState.ConnectedStarted)
			{
				Main.Instance.AddLog("Stop sprayer.");
				Main.Instance.Connection.Send("M5");
				Main.Instance.AddLog("Set status to Stopped.");
				Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
				stopped.Set();
				return;
			}

			Main.Instance.AddLog("Stop sprayer.");
			Main.Instance.Connection.Send("M5");
			Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });

			Main.Instance.AddLog("Go to beginning of Table 2 paint area.");
			target = Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap;
			Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + (Main.Instance.Parameters.ReverseFeed ? "-" : "") + target.ToString("0.0"));
			WaitXPosition(target);

			if (exiting)
			{
				stopped.Set();
				return;
			}

			if (Main.Instance.Connection.Status.ConnectionState != ConnectionState.ConnectedStarted)
			{
				Main.Instance.AddLog("Stop sprayer.");
				Main.Instance.Connection.Send("M5");
				Main.Instance.AddLog("Set status to Stopped.");
				Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
				stopped.Set();
				return;
			}

			Main.Instance.AddLog("Start sprayer.");
			Main.Instance.Connection.Send("M3");
			Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = true });

			Main.Instance.AddLog("Go to end of Table 2 paint area.");
			target = Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap + Main.Instance.Table2PaintedAreaRatio * Main.Instance.Parameters.Table2Length;
			Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + (Main.Instance.Parameters.ReverseFeed ? "-" : "") + target.ToString("0.0"));
			WaitXPosition(target);

			if (exiting)
			{
				stopped.Set();
				return;
			}

			if (Main.Instance.Connection.Status.ConnectionState != ConnectionState.ConnectedStarted)
			{
				Main.Instance.AddLog("Stop sprayer.");
				Main.Instance.Connection.Send("M5");
				Main.Instance.AddLog("Set status to Stopped.");
				Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
				stopped.Set();
				return;
			}

			Main.Instance.AddLog("Stop sprayer.");
			Main.Instance.Connection.Send("M5");
			Main.Instance.AddLog("Set status to Stopped.");
			Main.Instance.Connection.Status.ConnectionState = ConnectionState.ConnectedStopped;
			Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
		}

		internal void GoToCoordinate(double machineCoordinate)
		{
			Main.Instance.AddLog("Going to machine coordinate " + Main.Instance.Parameters.ControlAxis.ToString() + "=" + Math.Abs(Main.Instance.Connection.Status.MachineCoordinate));
			var target = Main.Instance.Connection.Status.MachineCoordinate;
			Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis.ToString() + (Main.Instance.Parameters.ReverseFeed ? "-" : "") + target.ToString("0.0"));
		}
	}
}
