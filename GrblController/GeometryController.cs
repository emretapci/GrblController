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
			this.targetCoord = targetCoord;
			WaitHandle.WaitAny(new WaitHandle[] { targetReached, willStop });
		}

		private void ThreadFunc()
		{
			Main.Instance.Connection.Unlock();

			if (Main.Instance.Table1PaintedArea == 0 && Main.Instance.Table2PaintedArea == 0)
			{
				Main.Instance.AddLog("Nothing to do.");
				stopped.Set();
				return;
			}
			else
			{
				Main.Instance.AddLog("Job started.");
				Main.Instance.SetSlidersEnabled(false);
				Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { ConnectionState = ConnectionState.ConnectedStarted });
			}

			if (Main.Instance.Table1PaintedArea > 0)
			{
				Main.Instance.AddLog("Go to beginning of Table 1 paint area.");
				Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + Main.Instance.Parameters.StartOffset.ToString("0.0"));
				WaitXPosition(Main.Instance.Parameters.StartOffset);

				if (exiting)
				{
					Main.Instance.AddLog("Aborted.");
					Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
					stopped.Set();
					Main.Instance.SetSlidersEnabled(true);
					return;
				}

				Main.Instance.AddLog("Start sprayer.");
				Main.Instance.Connection.Send("M3");
				Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = true });

				Main.Instance.AddLog("Go to end of Table 1 paint area.");
				var target = Main.Instance.Parameters.StartOffset + Main.Instance.Table1PaintedArea / 4.0 * Main.Instance.Parameters.Table1Length;
				Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + target.ToString("0.0"));
				WaitXPosition(target);

				if (exiting)
				{
					Main.Instance.AddLog("Stop sprayer.");
					Main.Instance.AddLog("Aborted.");
					Main.Instance.Connection.Send("M5");
					Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
					stopped.Set();
					Main.Instance.SetSlidersEnabled(true);
					return;
				}

				Main.Instance.AddLog("Stop sprayer.");
				Main.Instance.Connection.Send("M5");
				Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
			}

			if (Main.Instance.Table2PaintedArea > 0)
			{
				Main.Instance.AddLog("Go to beginning of Table 2 paint area.");
				var target = Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap;
				Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + target.ToString("0.0"));
				WaitXPosition(target);

				if (exiting)
				{
					Main.Instance.AddLog("Aborted.");
					Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
					stopped.Set();
					Main.Instance.SetSlidersEnabled(true);
					return;
				}

				Main.Instance.AddLog("Start sprayer.");
				Main.Instance.Connection.Send("M3");
				Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = true });

				Main.Instance.AddLog("Go to end of Table 2 paint area.");
				target = Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap + Main.Instance.Table2PaintedArea / 4.0 * Main.Instance.Parameters.Table2Length;
				Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + target.ToString("0.0"));
				WaitXPosition(target);

				if (exiting)
				{
					Main.Instance.AddLog("Stop sprayer.");
					Main.Instance.AddLog("Aborted.");
					Main.Instance.Connection.Send("M5");
					Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
					stopped.Set();
					Main.Instance.SetSlidersEnabled(true);
					return;
				}

				Main.Instance.AddLog("Stop sprayer.");
			}

			Main.Instance.Connection.Send("M5");
			Main.Instance.AddLog("Job finished.");

			Main.Instance.Connection.SetStatus(new Status(Main.Instance.Connection.Status) { Painting = false });
			Main.Instance.AddLog("Returning to 5 mm.");
			Main.Instance.Connection.Send("G0" + Main.Instance.Parameters.ControlAxis + "5");
			WaitXPosition(5);

			stopped.Set();
			Main.Instance.SetSlidersEnabled(true);
			Main.Instance.Connection.Status.ConnectionState = ConnectionState.ConnectedStopped;
		}

		internal void GoToCoordinate(double coordinate)
		{
			Main.Instance.Connection.Unlock();

			Main.Instance.AddLog("Going to machine coordinate " + Main.Instance.Parameters.ControlAxis.ToString() + "=" + Math.Abs(Main.Instance.Connection.Status.MachineCoordinate));
			var target = Main.Instance.Connection.Status.MachineCoordinate;
			var command = "G0" + Main.Instance.Parameters.ControlAxis.ToString() + target.ToString("0.0");
			Main.Instance.Connection.Send(command);
		}
	}
}
