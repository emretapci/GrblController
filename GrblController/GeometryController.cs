using System;
using System.Threading;

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

			Status.onStatusChanged += Connection_onStatusChanged;

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

			Status.onStatusChanged -= Connection_onStatusChanged;

			Status.SetStatus(new Status() { ConnectionState = ConnectionState.Connected, RunState = RunState.Stopped });

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
				Status.SetStatus(new Status() { ConnectionState = ConnectionState.Connected, RunState = RunState.Running });
			}

			if (Main.Instance.Table1PaintedArea > 0)
			{
				Main.Instance.AddLog("Go to beginning of Table 1 paint area.");
				Main.Instance.Connection.Send("G0" + Parameters.Instance.ControlAxis + Parameters.Instance.StartOffset.ToString("0.0"));
				WaitXPosition(Parameters.Instance.StartOffset);

				if (exiting)
				{
					Main.Instance.AddLog("Aborted.");
					Status.SetStatus(new Status() { Painting = false });
					stopped.Set();
					return;
				}

				Main.Instance.AddLog("Start sprayer.");
				Main.Instance.Connection.Send("M8");
				Status.SetStatus(new Status() { Painting = true });

				Main.Instance.AddLog("Go to end of Table 1 paint area.");
				var target = Parameters.Instance.StartOffset + Main.Instance.Table1PaintedArea / 4.0 * Parameters.Instance.Table1Length;
				Main.Instance.Connection.Send("G0" + Parameters.Instance.ControlAxis + target.ToString("0.0"));
				WaitXPosition(target);

				if (exiting)
				{
					Main.Instance.AddLog("Stop sprayer.");
					Main.Instance.AddLog("Aborted.");
					Main.Instance.Connection.Send("M9");
					Status.SetStatus(new Status() { Painting = false });
					stopped.Set();
					return;
				}

				Main.Instance.AddLog("Stop sprayer.");
				Main.Instance.Connection.Send("M9");
				Status.SetStatus(new Status() { Painting = false });
			}

			if (Main.Instance.Table2PaintedArea > 0)
			{
				Main.Instance.AddLog("Go to beginning of Table 2 paint area.");
				var target = Parameters.Instance.StartOffset + Parameters.Instance.Table1Length + Parameters.Instance.MiddleGap;
				Main.Instance.Connection.Send("G0" + Parameters.Instance.ControlAxis + target.ToString("0.0"));
				WaitXPosition(target);

				if (exiting)
				{
					Main.Instance.AddLog("Aborted.");
					Status.SetStatus(new Status() { Painting = false });
					stopped.Set();
					return;
				}

				Main.Instance.AddLog("Start sprayer.");
				Main.Instance.Connection.Send("M8");
				Status.SetStatus(new Status() { Painting = true });

				Main.Instance.AddLog("Go to end of Table 2 paint area.");
				target = Parameters.Instance.StartOffset + Parameters.Instance.Table1Length + Parameters.Instance.MiddleGap + Main.Instance.Table2PaintedArea / 4.0 * Parameters.Instance.Table2Length;
				Main.Instance.Connection.Send("G0" + Parameters.Instance.ControlAxis + target.ToString("0.0"));
				WaitXPosition(target);

				if (exiting)
				{
					Main.Instance.AddLog("Stop sprayer.");
					Main.Instance.AddLog("Aborted.");
					Main.Instance.Connection.Send("M9");
					Status.SetStatus(new Status() { Painting = false });
					stopped.Set();
					return;
				}

				Main.Instance.AddLog("Stop sprayer.");
			}

			Main.Instance.Connection.Send("M9");
			Main.Instance.AddLog("Job finished.");

			Status.SetStatus(new Status() { Painting = false });
			Main.Instance.AddLog("Returning to 5 mm.");
			Main.Instance.Connection.Send("G0" + Parameters.Instance.ControlAxis + "5");
			WaitXPosition(5);

			stopped.Set();
			Status.SetStatus(new Status() { ConnectionState = ConnectionState.Connected, RunState = RunState.Stopped });
		}

		internal void GoToCoordinate(double coordinate)
		{
			Main.Instance.Connection.Unlock();

			Main.Instance.AddLog("Going to machine coordinate " + Parameters.Instance.ControlAxis.ToString() + "=" + Math.Abs(Status.Instance.MachineCoordinate));
			var target = Status.Instance.MachineCoordinate;
			var command = "G0" + Parameters.Instance.ControlAxis.ToString() + target.ToString("0.0");
			Main.Instance.Connection.Send(command);
		}
	}
}
