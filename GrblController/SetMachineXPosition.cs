using System;
using System.Windows.Forms;

namespace GrblController
{
	public partial class SetMachineXPosition : Form
	{
		internal double MachineXPosition { get; private set; }

		internal SetMachineXPosition()
		{
			InitializeComponent();
		}

		private void SetMachineXPosition_Load(object sender, EventArgs e)
		{
			machineXPositionTextBox.Text = Main.Instance.GeometryController.XPosition.ToString("0.0");
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
					MachineXPosition = d;
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
	}
}
