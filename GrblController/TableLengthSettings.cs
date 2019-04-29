using System;
using System.Threading;
using System.Windows.Forms;

namespace GrblController
{
	public partial class TableLengthSettings : Form
	{
		public TableLengthSettings()
		{
			InitializeComponent();
		}

		private void TableLengthSettings_Load(object sender, EventArgs e)
		{
			var parameters = Parameters.ReadFromFile();

			startOffset.Text = parameters.StartOffset.ToString();
			table1Length.Text = parameters.Table1Length.ToString();
			middleGapLength.Text = parameters.MiddleGap.ToString();
			table2Length.Text = parameters.Table2Length.ToString();
			endOffset.Text = parameters.EndOffset.ToString();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			var parameters = Parameters.ReadFromFile();

			try
			{
				parameters.StartOffset = double.Parse(startOffset.Text);
				parameters.Table1Length = double.Parse(table1Length.Text);
				parameters.MiddleGap = double.Parse(middleGapLength.Text);
				parameters.Table2Length = double.Parse(table2Length.Text);
				parameters.EndOffset = double.Parse(endOffset.Text);

				Parameters.WriteToFile(parameters);

				DialogResult = DialogResult.OK;
				Close();
			}
			catch
			{
				MessageBox.Show("Error in parameters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
