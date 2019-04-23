using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CncController
{
	public partial class Settings : Form
	{
		public Settings()
		{
			InitializeComponent();
		}

		private void Settings_Load(object sender, EventArgs e)
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
			Parameters.WriteToFile(new Parameters()
			{
				StartOffset = (double)startOffset.Value,
				Table1Length = (double)table1Length.Value,
				MiddleGap = (double)middleGapLength.Value,
				Table2Length = (double)table2Length.Value,
				EndOffset = (double)endOffset.Value
			});

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
