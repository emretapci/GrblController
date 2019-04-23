namespace CncController
{
	partial class Settings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.startOffset = new System.Windows.Forms.NumericUpDown();
			this.table1Length = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.middleGapLength = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.table2Length = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.endOffset = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.startOffset)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.table1Length)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.middleGapLength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.table2Length)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.endOffset)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(67, 26);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "Start offset";
			// 
			// startOffset
			// 
			this.startOffset.Location = new System.Drawing.Point(163, 24);
			this.startOffset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.startOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.startOffset.Name = "startOffset";
			this.startOffset.Size = new System.Drawing.Size(154, 27);
			this.startOffset.TabIndex = 1;
			// 
			// table1Length
			// 
			this.table1Length.Location = new System.Drawing.Point(163, 68);
			this.table1Length.Margin = new System.Windows.Forms.Padding(4);
			this.table1Length.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.table1Length.Name = "table1Length";
			this.table1Length.Size = new System.Drawing.Size(154, 27);
			this.table1Length.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(47, 70);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(108, 19);
			this.label2.TabIndex = 2;
			this.label2.Text = "Table 1 length";
			// 
			// middleGapLength
			// 
			this.middleGapLength.Location = new System.Drawing.Point(163, 112);
			this.middleGapLength.Margin = new System.Windows.Forms.Padding(4);
			this.middleGapLength.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.middleGapLength.Name = "middleGapLength";
			this.middleGapLength.Size = new System.Drawing.Size(154, 27);
			this.middleGapLength.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(19, 114);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 19);
			this.label3.TabIndex = 4;
			this.label3.Text = "Middle gap length";
			// 
			// table2Length
			// 
			this.table2Length.Location = new System.Drawing.Point(163, 156);
			this.table2Length.Margin = new System.Windows.Forms.Padding(4);
			this.table2Length.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.table2Length.Name = "table2Length";
			this.table2Length.Size = new System.Drawing.Size(154, 27);
			this.table2Length.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(47, 158);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(108, 19);
			this.label4.TabIndex = 6;
			this.label4.Text = "Table 2 length";
			// 
			// endOffset
			// 
			this.endOffset.Location = new System.Drawing.Point(163, 200);
			this.endOffset.Margin = new System.Windows.Forms.Padding(4);
			this.endOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.endOffset.Name = "endOffset";
			this.endOffset.Size = new System.Drawing.Size(154, 27);
			this.endOffset.TabIndex = 9;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(72, 202);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(83, 19);
			this.label5.TabIndex = 8;
			this.label5.Text = "End offset";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(91, 264);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(110, 35);
			this.buttonOK.TabIndex = 10;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(207, 264);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(110, 35);
			this.buttonCancel.TabIndex = 11;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// Settings
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(353, 333);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.endOffset);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.table2Length);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.middleGapLength);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.table1Length);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.startOffset);
			this.Controls.Add(this.label1);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Settings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Settings";
			this.Load += new System.EventHandler(this.Settings_Load);
			((System.ComponentModel.ISupportInitialize)(this.startOffset)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.table1Length)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.middleGapLength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.table2Length)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.endOffset)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown startOffset;
		private System.Windows.Forms.NumericUpDown table1Length;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown middleGapLength;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown table2Length;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown endOffset;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}