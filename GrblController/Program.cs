using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace GrblController
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			CultureInfo ci = new CultureInfo("en-GB");
			Thread.CurrentThread.CurrentCulture = ci;
			Thread.CurrentThread.CurrentUICulture = ci;

			//Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Main());
		}

		static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			// Log the exception, display it, etc
			MessageBox.Show(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			// Log the exception, display it, etc
			MessageBox.Show((e.ExceptionObject as Exception).Message + Environment.NewLine + (e.ExceptionObject as Exception).StackTrace);
		}
	}
}
