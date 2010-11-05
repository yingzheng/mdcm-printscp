using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NLog;
using NLog.Config;
using NLog.Targets;

namespace Dicom.Forms {
	public partial class LogForm : Form {
		public LogForm() {
			InitializeComponent();
			InitializeLog();
		}

		private void InitializeLog() {
			LoggingConfiguration config = new LoggingConfiguration();

			DicomRichTextBoxTarget target = new DicomRichTextBoxTarget();
			target.UseDefaultRowColoringRules = true;
			target.Layout = "${message}";
			target.Control = rtbLog;

			config.AddTarget("DicomRichTextBox", target);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

			LogManager.Configuration = config;
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				e.Cancel = true;
				Hide();
			}
		}
	}
}
