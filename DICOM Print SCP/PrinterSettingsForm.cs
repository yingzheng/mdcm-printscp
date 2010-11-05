using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace Dicom.PrintScp {
	public partial class PrinterSettingsForm : Form {
		private DicomPrintConfig _config;

		public PrinterSettingsForm(DicomPrintConfig config) {
			InitializeComponent();

			_config = config;
			tbAETitle.Text = _config.AETitle;
			tbPrinterName.Text = _config.PrinterName;
			cbPaperSource.Items.Clear();
			cbPaperSource.Items.Add("Driver Select");
			foreach (PaperSource source in _config.PrinterSettings.PaperSources) {
				if (!String.IsNullOrEmpty(source.SourceName))
					cbPaperSource.Items.Add(source.SourceName);
			}
			cbPaperSource.SelectedItem = _config.PaperSource;
			if (cbPaperSource.SelectedIndex == -1)
				cbPaperSource.SelectedIndex = 0;
			cbPreviewOnly.Checked = _config.PreviewOnly;
		}

		private void OnSelectPrinter(object sender, EventArgs e) {
			PrintDialog pd = new PrintDialog();
			pd.UseEXDialog = true;
			if (_config.PrinterSettings != null)
				pd.PrinterSettings = _config.PrinterSettings;
			if (pd.ShowDialog(this) == DialogResult.OK) {
				_config.PrinterSettings = pd.PrinterSettings;
				tbPrinterName.Text = _config.PrinterName;

				cbPaperSource.Items.Clear();
				cbPaperSource.Items.Add("Driver Select");
				foreach (PaperSource source in _config.PrinterSettings.PaperSources) {
					if (!String.IsNullOrEmpty(source.SourceName))
						cbPaperSource.Items.Add(source.SourceName);
				}
				cbPaperSource.SelectedIndex = 0;
			}
		}

		private void OnSelectTray(object sender, EventArgs e) {
			if (cbPaperSource.SelectedIndex == 0)
				_config.PaperSource = null;
			else
				_config.PaperSource = cbPaperSource.Text;
		}

		private void OnChangeAETitle(object sender, EventArgs e) {
			_config.AETitle = tbAETitle.Text;
		}

		private void OnPreviewOnlyChanged(object sender, EventArgs e) {
			_config.PreviewOnly = cbPreviewOnly.Checked;
		}
	}
}
