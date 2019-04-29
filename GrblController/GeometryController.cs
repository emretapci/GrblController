﻿using System;
using System.Threading;

namespace GrblController
{
	internal class GeometryController
	{
		private ManualResetEvent willStop = new ManualResetEvent(false);
		private ManualResetEvent stopped = new ManualResetEvent(false);
		private ManualResetEvent xReached = new ManualResetEvent(false);

		private double targetX;

		private Thread thread;

		internal void Start()
		{
			thread = new Thread(new ThreadStart(ThreadFunc));

			Main.Instance.Connection.onStatusChanged += Connection_onStatusChanged;

			willStop.Reset();
			thread.Start();

			Main.Instance.Connection.Status.ConnectionState = ConnectionState.ConnectedStarted;
			Main.Instance.Connection.SetStatus(Main.Instance.Connection.Status);
		}

		private void Connection_onStatusChanged(Status oldStatus, Status newStatus)
		{
			if (newStatus.MachinePosition.X == targetX)
			{
				xReached.Set();
			}
		}

		internal void Stop()
		{
			Main.Instance.Connection.onStatusChanged -= Connection_onStatusChanged;

			willStop.Set();
			stopped.WaitOne();

			Main.Instance.Connection.Status.ConnectionState = ConnectionState.ConnectedStopped;
			Main.Instance.Connection.SetStatus(Main.Instance.Connection.Status);
		}

		private void WaitXPosition(double targetX)
		{
			xReached.Reset();
			this.targetX = targetX;

			if (WaitHandle.WaitAny(new WaitHandle[] { xReached, willStop }) == 1)
			{
				stopped.Set();
			}
		}

		private void ThreadFunc()
		{
			Main.Instance.Connection.Send("$X");

			Main.Instance.AddLog("Go to beginning of Table 1 paint area.");
			Main.Instance.Connection.Send("G0X" + Main.Instance.Parameters.StartOffset.ToString("0.0"));
			WaitXPosition(Main.Instance.Parameters.StartOffset);

			if(Main.Instance.Connection.Status.ConnectionState != ConnectionState.ConnectedStarted)
			{
				Main.Instance.AddLog("Set status to Stopped.");
				return;
			}

			Main.Instance.AddLog("Start sprayer.");
			Main.Instance.Connection.Send("M3");

			Main.Instance.AddLog("Go to end of Table 1 paint area.");
			Main.Instance.Connection.Send("G0X" + (Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length).ToString("0.0"));
			WaitXPosition(Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length);

			if (Main.Instance.Connection.Status.ConnectionState != ConnectionState.ConnectedStarted)
			{
				Main.Instance.AddLog("Stop sprayer.");
				Main.Instance.Connection.Send("M5");
				Main.Instance.AddLog("Set status to Stopped.");
				return;
			}

			Main.Instance.AddLog("Stop sprayer.");
			Main.Instance.Connection.Send("M5");

			Main.Instance.AddLog("Go to beginning of Table 2 paint area.");
			Main.Instance.Connection.Send("G0X" + (Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap).ToString("0.0"));
			WaitXPosition(Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap);

			if (Main.Instance.Connection.Status.ConnectionState != ConnectionState.ConnectedStarted)
			{
				Main.Instance.AddLog("Stop sprayer.");
				Main.Instance.Connection.Send("M5");
				Main.Instance.AddLog("Set status to Stopped.");
				return;
			}

			Main.Instance.AddLog("Start sprayer.");
			Main.Instance.Connection.Send("M3");

			Main.Instance.AddLog("Go to end of Table 2 paint area.");
			Main.Instance.Connection.Send("G0X" + (Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap + Main.Instance.Parameters.Table2Length).ToString("0.0"));
			WaitXPosition(Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap + Main.Instance.Parameters.Table2Length);

			if (Main.Instance.Connection.Status.ConnectionState != ConnectionState.ConnectedStarted)
			{
				Main.Instance.AddLog("Stop sprayer.");
				Main.Instance.Connection.Send("M5");
				Main.Instance.AddLog("Set status to Stopped.");
				return;
			}

			Main.Instance.AddLog("Stop sprayer.");
			Main.Instance.Connection.Send("M5");
			Main.Instance.AddLog("Set status to Stopped.");
			Main.Instance.Connection.Status.ConnectionState = ConnectionState.ConnectedStopped;
			Main.Instance.Connection.SetStatus(Main.Instance.Connection.Status);
		}
	}
}