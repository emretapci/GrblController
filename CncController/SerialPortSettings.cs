using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace CncController
{
	public partial class SerialPortSettings : Form
	{
		public SerialPortSettings()
		{
			InitializeComponent();
		}

		private void Settings_Load(object sender, EventArgs e)
		{
			var parameters = Parameters.ReadFromFile();

			serialPortCombobox.Items.AddRange(SerialPort.GetPortNames());
			serialPortCombobox.SelectedIndex = Array.IndexOf(SerialPort.GetPortNames(), parameters.SerialPortString);

			baudrateCombobox.Items.AddRange(new string[] { "110", "300", "600", "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" });
			baudrateCombobox.SelectedItem = parameters.Baudrate.ToString();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			var parameters = Parameters.ReadFromFile();

			parameters.SerialPortString = serialPortCombobox.SelectedItem != null ? serialPortCombobox.SelectedItem.ToString() : "";
			parameters.Baudrate = uint.Parse(baudrateCombobox.SelectedItem.ToString());

			Parameters.WriteToFile(parameters);

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
