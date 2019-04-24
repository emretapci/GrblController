using System;
using System.Windows.Forms;

namespace CncController
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

			startOffset.Value = (decimal)parameters.StartOffset;
			table1Length.Value = (decimal)parameters.Table1Length;
			middleGapLength.Value = (decimal)parameters.MiddleGap;
			table2Length.Value = (decimal)parameters.Table2Length;
			endOffset.Value = (decimal)parameters.EndOffset;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			var parameters = Parameters.ReadFromFile();

			parameters.StartOffset = (double)startOffset.Value;
			parameters.Table1Length = (double)table1Length.Value;
			parameters.MiddleGap = (double)middleGapLength.Value;
			parameters.Table2Length = (double)table2Length.Value;
			parameters.EndOffset = (double)endOffset.Value;

			Parameters.WriteToFile(parameters);

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
