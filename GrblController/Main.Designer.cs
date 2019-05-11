namespace GrblController
{
	partial class Main
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
			this.tablePanel = new System.Windows.Forms.Panel();
			this.machinePositionPanel = new System.Windows.Forms.Panel();
			this.table2Panel = new System.Windows.Forms.Panel();
			this.table2PanelPaintedArea = new System.Windows.Forms.Panel();
			this.table1Panel = new System.Windows.Forms.Panel();
			this.table1PanelPaintedArea = new System.Windows.Forms.Panel();
			this.table1Slider = new System.Windows.Forms.VScrollBar();
			this.table2Slider = new System.Windows.Forms.VScrollBar();
			this.startStopButton = new System.Windows.Forms.Button();
			this.activityLog = new System.Windows.Forms.ListBox();
			this.slider2Panel = new System.Windows.Forms.Panel();
			this.slider1Panel = new System.Windows.Forms.Panel();
			this.table1Label0 = new System.Windows.Forms.Label();
			this.table1LabelFull = new System.Windows.Forms.Label();
			this.table2LabelFull = new System.Windows.Forms.Label();
			this.table1Label3d4 = new System.Windows.Forms.Label();
			this.table2Label3d4 = new System.Windows.Forms.Label();
			this.table1Label1d4 = new System.Windows.Forms.Label();
			this.table2Label1d2 = new System.Windows.Forms.Label();
			this.table1Label1d2 = new System.Windows.Forms.Label();
			this.table2Label1d4 = new System.Windows.Forms.Label();
			this.table2Label0 = new System.Windows.Forms.Label();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.machineState = new System.Windows.Forms.ToolStripStatusLabel();
			this.machineCoordinate = new System.Windows.Forms.ToolStripStatusLabel();
			this.portStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.calibrateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusUpdatedAlarmPanel = new System.Windows.Forms.Panel();
			this.homeButton = new System.Windows.Forms.Button();
			this.resetButton = new System.Windows.Forms.Button();
			this.tablePanel.SuspendLayout();
			this.table2Panel.SuspendLayout();
			this.table1Panel.SuspendLayout();
			this.slider2Panel.SuspendLayout();
			this.slider1Panel.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tablePanel
			// 
			this.tablePanel.BackColor = System.Drawing.Color.LightGray;
			this.tablePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tablePanel.Controls.Add(this.machinePositionPanel);
			this.tablePanel.Controls.Add(this.table2Panel);
			this.tablePanel.Controls.Add(this.table1Panel);
			this.tablePanel.Location = new System.Drawing.Point(14, 38);
			this.tablePanel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.tablePanel.Name = "tablePanel";
			this.tablePanel.Size = new System.Drawing.Size(231, 464);
			this.tablePanel.TabIndex = 0;
			// 
			// machinePositionPanel
			// 
			this.machinePositionPanel.BackColor = System.Drawing.Color.Red;
			this.machinePositionPanel.Location = new System.Drawing.Point(0, 426);
			this.machinePositionPanel.Name = "machinePositionPanel";
			this.machinePositionPanel.Size = new System.Drawing.Size(231, 4);
			this.machinePositionPanel.TabIndex = 3;
			// 
			// table2Panel
			// 
			this.table2Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.table2Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.table2Panel.Controls.Add(this.table2PanelPaintedArea);
			this.table2Panel.Location = new System.Drawing.Point(0, 148);
			this.table2Panel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.table2Panel.Name = "table2Panel";
			this.table2Panel.Size = new System.Drawing.Size(229, 270);
			this.table2Panel.TabIndex = 2;
			// 
			// table2PanelPaintedArea
			// 
			this.table2PanelPaintedArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.table2PanelPaintedArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.table2PanelPaintedArea.Dock = System.Windows.Forms.DockStyle.Top;
			this.table2PanelPaintedArea.Location = new System.Drawing.Point(0, 0);
			this.table2PanelPaintedArea.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.table2PanelPaintedArea.Name = "table2PanelPaintedArea";
			this.table2PanelPaintedArea.Size = new System.Drawing.Size(227, 70);
			this.table2PanelPaintedArea.TabIndex = 1;
			// 
			// table1Panel
			// 
			this.table1Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.table1Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.table1Panel.Controls.Add(this.table1PanelPaintedArea);
			this.table1Panel.Location = new System.Drawing.Point(0, 0);
			this.table1Panel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.table1Panel.Name = "table1Panel";
			this.table1Panel.Size = new System.Drawing.Size(228, 118);
			this.table1Panel.TabIndex = 0;
			// 
			// table1PanelPaintedArea
			// 
			this.table1PanelPaintedArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.table1PanelPaintedArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.table1PanelPaintedArea.Location = new System.Drawing.Point(0, 0);
			this.table1PanelPaintedArea.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.table1PanelPaintedArea.Name = "table1PanelPaintedArea";
			this.table1PanelPaintedArea.Size = new System.Drawing.Size(448, 70);
			this.table1PanelPaintedArea.TabIndex = 0;
			// 
			// table1Slider
			// 
			this.table1Slider.Cursor = System.Windows.Forms.Cursors.Hand;
			this.table1Slider.Dock = System.Windows.Forms.DockStyle.Fill;
			this.table1Slider.LargeChange = 1;
			this.table1Slider.Location = new System.Drawing.Point(0, 0);
			this.table1Slider.Maximum = 4;
			this.table1Slider.Name = "table1Slider";
			this.table1Slider.Size = new System.Drawing.Size(38, 187);
			this.table1Slider.TabIndex = 1;
			this.table1Slider.ValueChanged += new System.EventHandler(this.table1Slider_ValueChanged);
			// 
			// table2Slider
			// 
			this.table2Slider.Cursor = System.Windows.Forms.Cursors.Hand;
			this.table2Slider.Dock = System.Windows.Forms.DockStyle.Fill;
			this.table2Slider.LargeChange = 1;
			this.table2Slider.Location = new System.Drawing.Point(0, 0);
			this.table2Slider.Maximum = 4;
			this.table2Slider.Name = "table2Slider";
			this.table2Slider.Size = new System.Drawing.Size(38, 265);
			this.table2Slider.TabIndex = 1;
			this.table2Slider.ValueChanged += new System.EventHandler(this.table2Slider_ValueChanged);
			// 
			// startStopButton
			// 
			this.startStopButton.Location = new System.Drawing.Point(342, 215);
			this.startStopButton.Name = "startStopButton";
			this.startStopButton.Size = new System.Drawing.Size(440, 213);
			this.startStopButton.TabIndex = 15;
			this.startStopButton.Text = "Start";
			this.startStopButton.UseVisualStyleBackColor = true;
			this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);
			// 
			// activityLog
			// 
			this.activityLog.FormattingEnabled = true;
			this.activityLog.ItemHeight = 19;
			this.activityLog.Location = new System.Drawing.Point(343, 38);
			this.activityLog.Name = "activityLog";
			this.activityLog.Size = new System.Drawing.Size(438, 156);
			this.activityLog.TabIndex = 19;
			// 
			// slider2Panel
			// 
			this.slider2Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.slider2Panel.Controls.Add(this.table2Slider);
			this.slider2Panel.Location = new System.Drawing.Point(255, 236);
			this.slider2Panel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.slider2Panel.Name = "slider2Panel";
			this.slider2Panel.Size = new System.Drawing.Size(40, 267);
			this.slider2Panel.TabIndex = 3;
			// 
			// slider1Panel
			// 
			this.slider1Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.slider1Panel.Controls.Add(this.table1Slider);
			this.slider1Panel.Location = new System.Drawing.Point(254, 38);
			this.slider1Panel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.slider1Panel.Name = "slider1Panel";
			this.slider1Panel.Size = new System.Drawing.Size(40, 189);
			this.slider1Panel.TabIndex = 2;
			// 
			// table1Label0
			// 
			this.table1Label0.AutoSize = true;
			this.table1Label0.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table1Label0.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table1Label0.Location = new System.Drawing.Point(303, 73);
			this.table1Label0.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table1Label0.Name = "table1Label0";
			this.table1Label0.Size = new System.Drawing.Size(18, 19);
			this.table1Label0.TabIndex = 20;
			this.table1Label0.Text = "0";
			// 
			// table1LabelFull
			// 
			this.table1LabelFull.AutoSize = true;
			this.table1LabelFull.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table1LabelFull.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table1LabelFull.Location = new System.Drawing.Point(303, 256);
			this.table1LabelFull.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table1LabelFull.Name = "table1LabelFull";
			this.table1LabelFull.Size = new System.Drawing.Size(32, 19);
			this.table1LabelFull.TabIndex = 24;
			this.table1LabelFull.Text = "full";
			// 
			// table2LabelFull
			// 
			this.table2LabelFull.AutoSize = true;
			this.table2LabelFull.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table2LabelFull.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table2LabelFull.Location = new System.Drawing.Point(303, 466);
			this.table2LabelFull.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table2LabelFull.Name = "table2LabelFull";
			this.table2LabelFull.Size = new System.Drawing.Size(32, 19);
			this.table2LabelFull.TabIndex = 29;
			this.table2LabelFull.Text = "full";
			// 
			// table1Label3d4
			// 
			this.table1Label3d4.AutoSize = true;
			this.table1Label3d4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table1Label3d4.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table1Label3d4.Location = new System.Drawing.Point(303, 210);
			this.table1Label3d4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table1Label3d4.Name = "table1Label3d4";
			this.table1Label3d4.Size = new System.Drawing.Size(32, 19);
			this.table1Label3d4.TabIndex = 23;
			this.table1Label3d4.Text = "3/4";
			// 
			// table2Label3d4
			// 
			this.table2Label3d4.AutoSize = true;
			this.table2Label3d4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table2Label3d4.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table2Label3d4.Location = new System.Drawing.Point(303, 420);
			this.table2Label3d4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table2Label3d4.Name = "table2Label3d4";
			this.table2Label3d4.Size = new System.Drawing.Size(32, 19);
			this.table2Label3d4.TabIndex = 28;
			this.table2Label3d4.Text = "3/4";
			// 
			// table1Label1d4
			// 
			this.table1Label1d4.AutoSize = true;
			this.table1Label1d4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table1Label1d4.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table1Label1d4.Location = new System.Drawing.Point(303, 118);
			this.table1Label1d4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table1Label1d4.Name = "table1Label1d4";
			this.table1Label1d4.Size = new System.Drawing.Size(32, 19);
			this.table1Label1d4.TabIndex = 21;
			this.table1Label1d4.Text = "1/4";
			// 
			// table2Label1d2
			// 
			this.table2Label1d2.AutoSize = true;
			this.table2Label1d2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table2Label1d2.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table2Label1d2.Location = new System.Drawing.Point(303, 374);
			this.table2Label1d2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table2Label1d2.Name = "table2Label1d2";
			this.table2Label1d2.Size = new System.Drawing.Size(32, 19);
			this.table2Label1d2.TabIndex = 27;
			this.table2Label1d2.Text = "1/2";
			// 
			// table1Label1d2
			// 
			this.table1Label1d2.AutoSize = true;
			this.table1Label1d2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table1Label1d2.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table1Label1d2.Location = new System.Drawing.Point(303, 164);
			this.table1Label1d2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table1Label1d2.Name = "table1Label1d2";
			this.table1Label1d2.Size = new System.Drawing.Size(32, 19);
			this.table1Label1d2.TabIndex = 22;
			this.table1Label1d2.Text = "1/2";
			// 
			// table2Label1d4
			// 
			this.table2Label1d4.AutoSize = true;
			this.table2Label1d4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table2Label1d4.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table2Label1d4.Location = new System.Drawing.Point(303, 329);
			this.table2Label1d4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table2Label1d4.Name = "table2Label1d4";
			this.table2Label1d4.Size = new System.Drawing.Size(32, 19);
			this.table2Label1d4.TabIndex = 26;
			this.table2Label1d4.Text = "1/4";
			// 
			// table2Label0
			// 
			this.table2Label0.AutoSize = true;
			this.table2Label0.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.table2Label0.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.table2Label0.Location = new System.Drawing.Point(303, 283);
			this.table2Label0.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.table2Label0.Name = "table2Label0";
			this.table2Label0.Size = new System.Drawing.Size(18, 19);
			this.table2Label0.TabIndex = 25;
			this.table2Label0.Text = "0";
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.machineState,
            this.machineCoordinate,
            this.portStatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 518);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
			this.statusStrip1.Size = new System.Drawing.Size(795, 29);
			this.statusStrip1.TabIndex = 38;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// machineState
			// 
			this.machineState.Name = "machineState";
			this.machineState.Size = new System.Drawing.Size(99, 24);
			this.machineState.Text = "machineState";
			// 
			// machineCoordinate
			// 
			this.machineCoordinate.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.machineCoordinate.Name = "machineCoordinate";
			this.machineCoordinate.Size = new System.Drawing.Size(235, 24);
			this.machineCoordinate.Text = "Machine/Work position: Unknown";
			// 
			// portStatusLabel
			// 
			this.portStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.portStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.portStatusLabel.ForeColor = System.Drawing.Color.Red;
			this.portStatusLabel.Name = "portStatusLabel";
			this.portStatusLabel.Size = new System.Drawing.Size(4, 24);
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(795, 27);
			this.menuStrip1.TabIndex = 39;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// menuToolStripMenuItem
			// 
			this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.calibrateToolStripMenuItem});
			this.menuToolStripMenuItem.Font = new System.Drawing.Font("Arial", 10.2F);
			this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
			this.menuToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
			this.menuToolStripMenuItem.Text = "Menu";
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(149, 26);
			this.settingsToolStripMenuItem.Text = "Settings";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// calibrateToolStripMenuItem
			// 
			this.calibrateToolStripMenuItem.Name = "calibrateToolStripMenuItem";
			this.calibrateToolStripMenuItem.Size = new System.Drawing.Size(149, 26);
			this.calibrateToolStripMenuItem.Text = "Calibrate";
			this.calibrateToolStripMenuItem.Click += new System.EventHandler(this.calibrateToolStripMenuItem_Click);
			// 
			// statusUpdatedAlarmPanel
			// 
			this.statusUpdatedAlarmPanel.Location = new System.Drawing.Point(343, 207);
			this.statusUpdatedAlarmPanel.Name = "statusUpdatedAlarmPanel";
			this.statusUpdatedAlarmPanel.Size = new System.Drawing.Size(4, 4);
			this.statusUpdatedAlarmPanel.TabIndex = 40;
			// 
			// homeButton
			// 
			this.homeButton.Location = new System.Drawing.Point(343, 434);
			this.homeButton.Name = "homeButton";
			this.homeButton.Size = new System.Drawing.Size(216, 69);
			this.homeButton.TabIndex = 41;
			this.homeButton.Text = "Home";
			this.homeButton.UseVisualStyleBackColor = true;
			this.homeButton.Click += new System.EventHandler(this.homeButton_Click);
			// 
			// resetButton
			// 
			this.resetButton.Location = new System.Drawing.Point(565, 434);
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(216, 69);
			this.resetButton.TabIndex = 42;
			this.resetButton.Text = "Reset";
			this.resetButton.UseVisualStyleBackColor = true;
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(795, 547);
			this.Controls.Add(this.resetButton);
			this.Controls.Add(this.homeButton);
			this.Controls.Add(this.statusUpdatedAlarmPanel);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.table1Label0);
			this.Controls.Add(this.table1LabelFull);
			this.Controls.Add(this.table2LabelFull);
			this.Controls.Add(this.table1Label3d4);
			this.Controls.Add(this.table2Label3d4);
			this.Controls.Add(this.table1Label1d4);
			this.Controls.Add(this.table2Label1d2);
			this.Controls.Add(this.table1Label1d2);
			this.Controls.Add(this.table2Label1d4);
			this.Controls.Add(this.table2Label0);
			this.Controls.Add(this.activityLog);
			this.Controls.Add(this.tablePanel);
			this.Controls.Add(this.startStopButton);
			this.Controls.Add(this.slider1Panel);
			this.Controls.Add(this.slider2Panel);
			this.Font = new System.Drawing.Font("Arial", 10.2F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.Name = "Main";
			this.Text = "GRBL Controller";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.Load += new System.EventHandler(this.Main_Load);
			this.tablePanel.ResumeLayout(false);
			this.table2Panel.ResumeLayout(false);
			this.table1Panel.ResumeLayout(false);
			this.slider2Panel.ResumeLayout(false);
			this.slider1Panel.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel tablePanel;
		private System.Windows.Forms.VScrollBar table1Slider;
		private System.Windows.Forms.VScrollBar table2Slider;
		private System.Windows.Forms.Panel table2Panel;
		private System.Windows.Forms.Panel table1Panel;
		private System.Windows.Forms.Panel table2PanelPaintedArea;
		private System.Windows.Forms.Panel table1PanelPaintedArea;
		private System.Windows.Forms.Button startStopButton;
		private System.Windows.Forms.ListBox activityLog;
		private System.Windows.Forms.Panel slider2Panel;
		private System.Windows.Forms.Panel slider1Panel;
		private System.Windows.Forms.Label table1Label0;
		private System.Windows.Forms.Label table1LabelFull;
		private System.Windows.Forms.Label table2LabelFull;
		private System.Windows.Forms.Label table1Label3d4;
		private System.Windows.Forms.Label table2Label3d4;
		private System.Windows.Forms.Label table1Label1d4;
		private System.Windows.Forms.Label table2Label1d2;
		private System.Windows.Forms.Label table1Label1d2;
		private System.Windows.Forms.Label table2Label1d4;
		private System.Windows.Forms.Label table2Label0;
		private System.Windows.Forms.Panel machinePositionPanel;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel machineState;
		private System.Windows.Forms.ToolStripStatusLabel machineCoordinate;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem calibrateToolStripMenuItem;
		private System.Windows.Forms.Panel statusUpdatedAlarmPanel;
		private System.Windows.Forms.Button homeButton;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.ToolStripStatusLabel portStatusLabel;
	}
}

