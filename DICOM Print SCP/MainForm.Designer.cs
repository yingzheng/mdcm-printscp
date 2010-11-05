namespace Dicom.PrintScp {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.linkDicomLog = new System.Windows.Forms.LinkLabel();
			this.gbDicomSettings = new System.Windows.Forms.GroupBox();
			this.nuThrottle = new System.Windows.Forms.NumericUpDown();
			this.nuDimseTo = new System.Windows.Forms.NumericUpDown();
			this.nuSocketTo = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.nuMaxPduSize = new System.Windows.Forms.NumericUpDown();
			this.nuDicomPort = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.bttnStartStop = new System.Windows.Forms.Button();
			this.bttnPrinterSettings = new System.Windows.Forms.Button();
			this.cbAutoStart = new System.Windows.Forms.CheckBox();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.menuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.NotifyMenuOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.NotifyMenuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.lvPrinters = new System.Windows.Forms.ListView();
			this.colPrinterAE = new System.Windows.Forms.ColumnHeader();
			this.colPrinterName = new System.Windows.Forms.ColumnHeader();
			this.colPrinterTray = new System.Windows.Forms.ColumnHeader();
			this.colPrinterPreview = new System.Windows.Forms.ColumnHeader();
			this.bttnAddPrinter = new System.Windows.Forms.Button();
			this.bttnDeletePrinter = new System.Windows.Forms.Button();
			this.gbDicomSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nuThrottle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nuDimseTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nuSocketTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nuMaxPduSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nuDicomPort)).BeginInit();
			this.menuNotify.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkDicomLog
			// 
			this.linkDicomLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkDicomLog.AutoSize = true;
			this.linkDicomLog.Location = new System.Drawing.Point(369, 201);
			this.linkDicomLog.Name = "linkDicomLog";
			this.linkDicomLog.Size = new System.Drawing.Size(63, 13);
			this.linkDicomLog.TabIndex = 0;
			this.linkDicomLog.TabStop = true;
			this.linkDicomLog.Text = "DICOM Log";
			this.linkDicomLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnDicomLogClicked);
			// 
			// gbDicomSettings
			// 
			this.gbDicomSettings.Controls.Add(this.nuThrottle);
			this.gbDicomSettings.Controls.Add(this.nuDimseTo);
			this.gbDicomSettings.Controls.Add(this.nuSocketTo);
			this.gbDicomSettings.Controls.Add(this.label6);
			this.gbDicomSettings.Controls.Add(this.label5);
			this.gbDicomSettings.Controls.Add(this.label4);
			this.gbDicomSettings.Controls.Add(this.label3);
			this.gbDicomSettings.Controls.Add(this.nuMaxPduSize);
			this.gbDicomSettings.Controls.Add(this.nuDicomPort);
			this.gbDicomSettings.Controls.Add(this.label1);
			this.gbDicomSettings.Location = new System.Drawing.Point(15, 12);
			this.gbDicomSettings.Name = "gbDicomSettings";
			this.gbDicomSettings.Size = new System.Drawing.Size(300, 202);
			this.gbDicomSettings.TabIndex = 1;
			this.gbDicomSettings.TabStop = false;
			this.gbDicomSettings.Text = "DICOM Settings";
			// 
			// nuThrottle
			// 
			this.nuThrottle.Location = new System.Drawing.Point(140, 164);
			this.nuThrottle.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
			this.nuThrottle.Name = "nuThrottle";
			this.nuThrottle.Size = new System.Drawing.Size(100, 20);
			this.nuThrottle.TabIndex = 11;
			// 
			// nuDimseTo
			// 
			this.nuDimseTo.Location = new System.Drawing.Point(140, 129);
			this.nuDimseTo.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.nuDimseTo.Name = "nuDimseTo";
			this.nuDimseTo.Size = new System.Drawing.Size(100, 20);
			this.nuDimseTo.TabIndex = 10;
			this.nuDimseTo.Value = new decimal(new int[] {
            180,
            0,
            0,
            0});
			// 
			// nuSocketTo
			// 
			this.nuSocketTo.Location = new System.Drawing.Point(140, 94);
			this.nuSocketTo.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.nuSocketTo.Name = "nuSocketTo";
			this.nuSocketTo.Size = new System.Drawing.Size(100, 20);
			this.nuSocketTo.TabIndex = 9;
			this.nuSocketTo.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(55, 166);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(79, 13);
			this.label6.TabIndex = 8;
			this.label6.Text = "Throttle (KB/s):";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(49, 131);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "DIMSE Timeout:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(49, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(85, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Socket Timeout:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(31, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(103, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Maximum PDU Size:";
			// 
			// nuMaxPduSize
			// 
			this.nuMaxPduSize.Location = new System.Drawing.Point(140, 59);
			this.nuMaxPduSize.Maximum = new decimal(new int[] {
            16777216,
            0,
            0,
            0});
			this.nuMaxPduSize.Name = "nuMaxPduSize";
			this.nuMaxPduSize.Size = new System.Drawing.Size(100, 20);
			this.nuMaxPduSize.TabIndex = 4;
			this.nuMaxPduSize.Value = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			// 
			// nuDicomPort
			// 
			this.nuDicomPort.Location = new System.Drawing.Point(140, 24);
			this.nuDicomPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.nuDicomPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nuDicomPort.Name = "nuDicomPort";
			this.nuDicomPort.Size = new System.Drawing.Size(100, 20);
			this.nuDicomPort.TabIndex = 1;
			this.nuDicomPort.Value = new decimal(new int[] {
            104,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(67, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "DICOM Port:";
			// 
			// bttnStartStop
			// 
			this.bttnStartStop.Location = new System.Drawing.Point(332, 12);
			this.bttnStartStop.Name = "bttnStartStop";
			this.bttnStartStop.Size = new System.Drawing.Size(100, 23);
			this.bttnStartStop.TabIndex = 3;
			this.bttnStartStop.Text = "Start";
			this.bttnStartStop.UseVisualStyleBackColor = true;
			this.bttnStartStop.Click += new System.EventHandler(this.OnToggleServiceClick);
			// 
			// bttnPrinterSettings
			// 
			this.bttnPrinterSettings.Location = new System.Drawing.Point(332, 360);
			this.bttnPrinterSettings.Name = "bttnPrinterSettings";
			this.bttnPrinterSettings.Size = new System.Drawing.Size(100, 23);
			this.bttnPrinterSettings.TabIndex = 4;
			this.bttnPrinterSettings.Text = "Printer Settings";
			this.bttnPrinterSettings.UseVisualStyleBackColor = true;
			this.bttnPrinterSettings.Click += new System.EventHandler(this.OnClickPrinterSettings);
			// 
			// cbAutoStart
			// 
			this.cbAutoStart.AutoSize = true;
			this.cbAutoStart.Location = new System.Drawing.Point(340, 41);
			this.cbAutoStart.Name = "cbAutoStart";
			this.cbAutoStart.Size = new System.Drawing.Size(73, 17);
			this.cbAutoStart.TabIndex = 6;
			this.cbAutoStart.Text = "Auto Start";
			this.cbAutoStart.UseVisualStyleBackColor = true;
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.menuNotify;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "DICOM Print SCP";
			this.notifyIcon.Visible = true;
			this.notifyIcon.DoubleClick += new System.EventHandler(this.OnNotifyMenuOpen);
			// 
			// menuNotify
			// 
			this.menuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NotifyMenuOpen,
            this.NotifyMenuExit});
			this.menuNotify.Name = "menuNotify";
			this.menuNotify.Size = new System.Drawing.Size(112, 48);
			// 
			// NotifyMenuOpen
			// 
			this.NotifyMenuOpen.Name = "NotifyMenuOpen";
			this.NotifyMenuOpen.Size = new System.Drawing.Size(111, 22);
			this.NotifyMenuOpen.Text = "&Open";
			this.NotifyMenuOpen.Click += new System.EventHandler(this.OnNotifyMenuOpen);
			// 
			// NotifyMenuExit
			// 
			this.NotifyMenuExit.Name = "NotifyMenuExit";
			this.NotifyMenuExit.Size = new System.Drawing.Size(111, 22);
			this.NotifyMenuExit.Text = "&Exit";
			this.NotifyMenuExit.Click += new System.EventHandler(this.OnClickNotifyMenuExit);
			// 
			// lvPrinters
			// 
			this.lvPrinters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPrinterAE,
            this.colPrinterName,
            this.colPrinterTray,
            this.colPrinterPreview});
			this.lvPrinters.FullRowSelect = true;
			this.lvPrinters.Location = new System.Drawing.Point(15, 230);
			this.lvPrinters.MultiSelect = false;
			this.lvPrinters.Name = "lvPrinters";
			this.lvPrinters.Size = new System.Drawing.Size(417, 124);
			this.lvPrinters.TabIndex = 7;
			this.lvPrinters.UseCompatibleStateImageBehavior = false;
			this.lvPrinters.View = System.Windows.Forms.View.Details;
			this.lvPrinters.DoubleClick += new System.EventHandler(this.OnClickPrinterSettings);
			// 
			// colPrinterAE
			// 
			this.colPrinterAE.Text = "AE Title";
			this.colPrinterAE.Width = 100;
			// 
			// colPrinterName
			// 
			this.colPrinterName.Text = "Printer";
			this.colPrinterName.Width = 150;
			// 
			// colPrinterTray
			// 
			this.colPrinterTray.Text = "Tray";
			this.colPrinterTray.Width = 80;
			// 
			// colPrinterPreview
			// 
			this.colPrinterPreview.Text = "Preview";
			// 
			// bttnAddPrinter
			// 
			this.bttnAddPrinter.Location = new System.Drawing.Point(15, 360);
			this.bttnAddPrinter.Name = "bttnAddPrinter";
			this.bttnAddPrinter.Size = new System.Drawing.Size(100, 23);
			this.bttnAddPrinter.TabIndex = 8;
			this.bttnAddPrinter.Text = "&Add Printer";
			this.bttnAddPrinter.UseVisualStyleBackColor = true;
			this.bttnAddPrinter.Click += new System.EventHandler(this.OnClickAddPrinter);
			// 
			// bttnDeletePrinter
			// 
			this.bttnDeletePrinter.Location = new System.Drawing.Point(121, 360);
			this.bttnDeletePrinter.Name = "bttnDeletePrinter";
			this.bttnDeletePrinter.Size = new System.Drawing.Size(50, 23);
			this.bttnDeletePrinter.TabIndex = 9;
			this.bttnDeletePrinter.Text = "&Delete";
			this.bttnDeletePrinter.UseVisualStyleBackColor = true;
			this.bttnDeletePrinter.Click += new System.EventHandler(this.OnClickDeletePrinter);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(446, 395);
			this.Controls.Add(this.bttnDeletePrinter);
			this.Controls.Add(this.bttnAddPrinter);
			this.Controls.Add(this.lvPrinters);
			this.Controls.Add(this.cbAutoStart);
			this.Controls.Add(this.bttnStartStop);
			this.Controls.Add(this.bttnPrinterSettings);
			this.Controls.Add(this.gbDicomSettings);
			this.Controls.Add(this.linkDicomLog);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.Text = "DICOM Print SCP";
			this.Load += new System.EventHandler(this.OnLoad);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
			this.gbDicomSettings.ResumeLayout(false);
			this.gbDicomSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nuThrottle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nuDimseTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nuSocketTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nuMaxPduSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nuDicomPort)).EndInit();
			this.menuNotify.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel linkDicomLog;
		private System.Windows.Forms.GroupBox gbDicomSettings;
		private System.Windows.Forms.NumericUpDown nuMaxPduSize;
		private System.Windows.Forms.NumericUpDown nuDicomPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nuThrottle;
		private System.Windows.Forms.NumericUpDown nuDimseTo;
		private System.Windows.Forms.NumericUpDown nuSocketTo;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button bttnStartStop;
		private System.Windows.Forms.Button bttnPrinterSettings;
		private System.Windows.Forms.CheckBox cbAutoStart;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ContextMenuStrip menuNotify;
		private System.Windows.Forms.ToolStripMenuItem NotifyMenuOpen;
		private System.Windows.Forms.ToolStripMenuItem NotifyMenuExit;
		private System.Windows.Forms.ListView lvPrinters;
		private System.Windows.Forms.ColumnHeader colPrinterAE;
		private System.Windows.Forms.ColumnHeader colPrinterName;
		private System.Windows.Forms.ColumnHeader colPrinterPreview;
		private System.Windows.Forms.ColumnHeader colPrinterTray;
		private System.Windows.Forms.Button bttnAddPrinter;
		private System.Windows.Forms.Button bttnDeletePrinter;
	}
}

