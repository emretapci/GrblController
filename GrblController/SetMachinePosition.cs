using System;
using System.Windows.Forms;

namespace GrblController
{
	public partial class SetMachinePosition : Form
	{
		internal double MachinePosition { get; private set; }

		internal SetMachinePosition()
		{
			InitializeComponent();
		}

		private void SetMachinePosition_Load(object sender, EventArgs e)
		{
			var parameters = Parameters.ReadFromFile();

			machineXPositionTextBox.Text = Main.Instance.Connection.Status.MachinePosition.X.ToString("0.0");
			controlAxis.SelectedIndex = (int)parameters.ControlAxis;
			reverseFeed.Checked = parameters.ReverseFeed;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			double d;
			if (double.TryParse(machineXPositionTextBox.Text, out d))
			{
				if (d < 0)
				{
					MessageBox.Show("Machine X coordinate cannot be negative.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else if (d <= Main.Instance.Parameters.TablesTotalLength)
				{
					var parameters = Parameters.ReadFromFile();

					parameters.ControlAxis = (ControlAxis)controlAxis.SelectedIndex;

					MachinePosition = d;

					parameters.ReverseFeed = reverseFeed.Checked;

					Parameters.WriteToFile(parameters);

					DialogResult = DialogResult.OK;
					Close();
				}
				else
				{
					MessageBox.Show("Machine X coordinate cannot be bigger than tables' total length.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("Invalid value for Machine X coordinate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void controlAxis_SelectedIndexChanged(object sender, EventArgs e)
		{
			controlAxisLabel.Text = "Machine " + controlAxis.SelectedItem.ToString() + " position";
		}
	}
}
