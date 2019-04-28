using System;

namespace GrblController
{
	internal class GeometryController
	{
		private double xPosition;

		internal event Action<double, double> onXPositionChanged;

		internal double XPosition
		{
			get
			{
				return xPosition;
			}
			set
			{
				var old = xPosition;
				xPosition = value;
				onXPositionChanged?.Invoke(old, xPosition);
			}
		}

		internal void Start()
		{
			Main.Instance.Connection.Status = Status.ConnectedStarted;

			Main.Instance.AddLog("Go to beginning of Table 1 paint area.");
			Main.Instance.Connection.Send("G0X" + Main.Instance.Parameters.StartOffset.ToString("0.0"));

			Main.Instance.AddLog("Start sprayer.");
			Main.Instance.Connection.Send("M3");

			Main.Instance.AddLog("Go to end of Table 1 paint area.");
			Main.Instance.Connection.Send("G0X" + (Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length).ToString("0.0"));

			Main.Instance.AddLog("Stop sprayer.");
			Main.Instance.Connection.Send("M5");

			Main.Instance.AddLog("Go to beginning of Table 2 paint area.");
			Main.Instance.Connection.Send("G0X" + (Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap).ToString("0.0"));

			Main.Instance.AddLog("Start sprayer.");
			Main.Instance.Connection.Send("M3");

			Main.Instance.AddLog("Go to end of Table 2 paint area.");
			Main.Instance.Connection.Send("G0X" + (Main.Instance.Parameters.StartOffset + Main.Instance.Parameters.Table1Length + Main.Instance.Parameters.MiddleGap + Main.Instance.Parameters.Table2Length).ToString("0.0"));

			Main.Instance.AddLog("Stop sprayer.");
			Main.Instance.Connection.Send("M5");

			Main.Instance.AddLog("Set status to Stopped.");
			Main.Instance.Connection.Status = Status.ConnectedStopped;
		}

		internal void Stop()
		{
			Main.Instance.Connection.Status = Status.ConnectedStopped;

			//Stop sending G codes.

			//Send any necessary footer G codes.
		}
	}
}
