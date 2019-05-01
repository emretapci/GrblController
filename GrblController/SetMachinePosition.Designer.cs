namespace GrblController
{
	partial class SetMachinePosition
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.machineXPositionTextBox = new System.Windows.Forms.TextBox();
			this.controlAxisLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.controlAxis = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(195, 87);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(110, 35);
			this.buttonCancel.TabIndex = 13;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(79, 87);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(110, 35);
			this.buttonOK.TabIndex = 12;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// machineXPositionTextBox
			// 
			this.machineXPositionTextBox.Location = new System.Drawing.Point(148, 42);
			this.machineXPositionTextBox.Name = "machineXPositionTextBox";
			this.machineXPositionTextBox.Size = new System.Drawing.Size(121, 23);
			this.machineXPositionTextBox.TabIndex = 14;
			// 
			// controlAxisLabel
			// 
			this.controlAxisLabel.AutoSize = true;
			this.controlAxisLabel.Location = new System.Drawing.Point(16, 45);
			this.controlAxisLabel.Name = "controlAxisLabel";
			this.controlAxisLabel.Size = new System.Drawing.Size(126, 16);
			this.controlAxisLabel.TabIndex = 15;
			this.controlAxisLabel.Text = "Machine X position";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(275, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(30, 16);
			this.label2.TabIndex = 16;
			this.label2.Text = "mm";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(32, 19);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 16);
			this.label3.TabIndex = 17;
			this.label3.Text = "Control axis";
			// 
			// controlAxis
			// 
			this.controlAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.controlAxis.FormattingEnabled = true;
			this.controlAxis.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z"});
			this.controlAxis.Location = new System.Drawing.Point(148, 12);
			this.controlAxis.Name = "controlAxis";
			this.controlAxis.Size = new System.Drawing.Size(121, 24);
			this.controlAxis.TabIndex = 18;
			this.controlAxis.SelectedIndexChanged += new System.EventHandler(this.controlAxis_SelectedIndexChanged);
			// 
			// SetMachinePosition
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(326, 144);
			this.Controls.Add(this.controlAxis);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.controlAxisLabel);
			this.Controls.Add(this.machineXPositionTextBox);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetMachinePosition";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set machine position";
			this.Load += new System.EventHandler(this.SetMachinePosition_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.TextBox machineXPositionTextBox;
		private System.Windows.Forms.Label controlAxisLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox controlAxis;
	}
}