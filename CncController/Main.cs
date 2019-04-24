using System;
using System.Drawing;
using System.Windows.Forms;

namespace CncController
{
	public partial class Main : Form
	{
		private Label[] table1Labels;
		private Label[] table2Labels;
		private Parameters parameters;

		public Main()
		{
			InitializeComponent();
			table1Labels = new Label[] { table1Label0, table1Label1d4, table1Label1d2, table1Label3d4, table1LabelFull };
			table2Labels = new Label[] { table2Label0, table2Label1d4, table2Label1d2, table2Label3d4, table2LabelFull };
			parameters = Parameters.ReadFromFile();
		}

		private void Main_Load(object sender, EventArgs e)
		{
			parameters = Parameters.ReadFromFile();
			table1Slider_ValueChanged(null, null);
			table2Slider_ValueChanged(null, null);
		}


		private void table1Slider_ValueChanged(object sender, EventArgs e)
		{
			foreach (var label in table1Labels)
			{
				label.ForeColor = SystemColors.ControlDark;
			}

			table1Labels[table1Slider.Value].ForeColor = Color.Black;
			ResizeTablePanels();
		}

		private void table2Slider_ValueChanged(object sender, EventArgs e)
		{
			foreach (var label in table2Labels)
			{
				label.ForeColor = SystemColors.ControlDark;
			}

			table2Labels[table2Slider.Value].ForeColor = Color.Black;
			ResizeTablePanels();
		}

		private void ResizeTablePanels()
		{
			double dTotalLength = parameters.StartOffset + parameters.Table1Length + parameters.MiddleGap + parameters.Table2Length + parameters.EndOffset;

			table1Panel.Location = new Point(0, (int)(tablePanel.Height * parameters.StartOffset / dTotalLength));
			table1Panel.Size = new Size(tablePanel.Width, (int)(tablePanel.Height * parameters.Table1Length / dTotalLength));

			table2Panel.Location = new Point(0, (int)(tablePanel.Height * (parameters.StartOffset + parameters.Table1Length + parameters.MiddleGap) / dTotalLength));
			table2Panel.Size = new Size(tablePanel.Width, (int)(tablePanel.Height * parameters.Table2Length / dTotalLength));

			table1PanelPaintedArea.Location = new Point(0, 0);
			table1PanelPaintedArea.Size = new Size(table1Panel.Width, (int)(table1Panel.Size.Height * (table1Slider.Value / 4.0)));

			table2PanelPaintedArea.Location = new Point(0, 0);
			table2PanelPaintedArea.Size = new Size(table1Panel.Width, (int)(table2Panel.Size.Height * (table2Slider.Value / 4.0)));
		}

		private void tableLengthsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((new TableLengthSettings()).ShowDialog() == DialogResult.OK)
			{
				Main_Load(null, null);
			}
		}

		private void serialPortToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((new SerialPortSettings()).ShowDialog() == DialogResult.OK)
			{
				Main_Load(null, null);
			}
		}
	}
}
