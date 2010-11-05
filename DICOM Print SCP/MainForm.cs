using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using Dicom.Forms;
using Dicom.Network;
using Dicom.Network.Server;

namespace Dicom.PrintScp {
	public partial class MainForm : Form {
		private LogForm _log;
		private DcmServer<NPrintService> _server;
		private bool _closing;

		public MainForm() {
			InitializeComponent();
			InitializeLog();
		}

		private void InitializeLog() {
			_log = new LogForm();
			_log.Show();
			_log.Hide();
		}

		private void LoadSettings() {
			try {
				Config.Load();
				nuDicomPort.Value = Config.Instance.Port;
				nuMaxPduSize.Value = Config.Instance.MaxPduSize;
				nuSocketTo.Value = Config.Instance.SocketTimeout;
				nuDimseTo.Value = Config.Instance.DimseTimeout;
				nuThrottle.Value = Config.Instance.ThrottleSpeed;
				cbAutoStart.Checked = AppUtility.IsAutoStartup("DICOM Print SCP");
				RefreshPrinters();
			} catch (Exception ex) {
#if DEBUG
				Dicom.Debug.Log.Error("Error: " + ex.ToString());
#else
				Dicom.Debug.Log.Error("Error: " + ex.Message);
#endif
			}
		}

		private void SaveSettings() {
			try {
				Config.Instance.Port = (int)nuDicomPort.Value;
				Config.Instance.MaxPduSize = (int)nuMaxPduSize.Value;
				Config.Instance.SocketTimeout = (int)nuSocketTo.Value;
				Config.Instance.DimseTimeout = (int)nuDimseTo.Value;
				Config.Instance.ThrottleSpeed = (int)nuThrottle.Value;
				Config.Save();

				AppUtility.SetAutoStartup("DICOM Print SCP", cbAutoStart.Checked);
				RefreshPrinters();
			} catch (Exception ex) {
#if DEBUG
				Dicom.Debug.Log.Error("Error: " + ex.ToString());
#else
				Dicom.Debug.Log.Error("Error: " + ex.Message);
#endif
			}
		}

		private void EnableControls(bool enabled) {
			cbAutoStart.Enabled = enabled;
			bttnPrinterSettings.Enabled = enabled;
			gbDicomSettings.Enabled = enabled;
			lvPrinters.Enabled = enabled;
			bttnAddPrinter.Enabled = enabled;
			bttnDeletePrinter.Enabled = enabled;
			bttnPrinterSettings.Enabled = enabled;
		}

		private void ToggleService() {
			if (_server == null) {
				SaveSettings();

				if (Config.Instance.Printers.Count == 0) {
					MessageBox.Show(this, "Please configure your printer before starting the DICOM server.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				EnableControls(false);
				bttnStartStop.Text = "Stop";

				try {
					_server = new DcmServer<NPrintService>();
					_server.AddPort((int)nuDicomPort.Value, DcmSocketType.TCP);
					_server.Start();
					return;
				}
				catch (Exception ex) {
					MessageBox.Show(this, ex.Message, "DICOM Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			_server.Stop();
			_server = null;

			EnableControls(true);
			bttnStartStop.Text = "Start";
		}

		private void OnLoad(object sender, EventArgs e) {
			LoadSettings();

			if (Config.Instance.Printers.Count == 0)
			    OnClickAddPrinter(sender, e);

			if (Config.Instance.Printers.Count > 0) {
				ToggleService();
				Hide();
			}
		}

		private void OnDicomLogClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			_log.Show();
			_log.Focus();
		}

		private void OnToggleServiceClick(object sender, EventArgs e) {
			ToggleService();
		}

		private void OnClosing(object sender, FormClosingEventArgs e) {
			if (_closing || 
				e.CloseReason == CloseReason.WindowsShutDown || 
				e.CloseReason == CloseReason.TaskManagerClosing || 
				e.CloseReason == CloseReason.ApplicationExitCall)
			{
				if (_server != null) {
					ToggleService();
				} else {
					Config.Save();
				}
			} else {
				if (_server != null) {
					e.Cancel = true;
					Hide();
				} else {
					Config.Save();
				}
			}
		}

		private void OnClickNotifyMenuExit(object sender, EventArgs e) {
			_closing = true;
			Application.Exit();
		}

		private void OnNotifyMenuOpen(object sender, EventArgs e) {
			Show();
			Focus();
		}

		private void RefreshPrinters() {
			lvPrinters.BeginUpdate();

			lvPrinters.Items.Clear();
			foreach (DicomPrintConfig config in Config.Instance.Printers) {
				ListViewItem lvi = new ListViewItem(config.AETitle);
				lvi.SubItems.Add(config.PrinterName);
				lvi.SubItems.Add(config.PaperSource);
				lvi.SubItems.Add(config.PreviewOnly.ToString());
				lvi.Tag = config;
				lvPrinters.Items.Add(lvi);
			}

			lvPrinters.Sort();

			lvPrinters.EndUpdate();
		}

		private void OnClickAddPrinter(object sender, EventArgs e) {
			DicomPrintConfig config = new DicomPrintConfig();
			PrinterSettingsForm psf = new PrinterSettingsForm(config);
			if (psf.ShowDialog(this) == DialogResult.OK) {
				Config.Instance.Printers.Add(config);
				SaveSettings();
			}
		}
		
		private void OnClickPrinterSettings(object sender, EventArgs e) {
			if (lvPrinters.SelectedItems.Count == 0)
				return;

			ListViewItem lvi = lvPrinters.SelectedItems[0];
			DicomPrintConfig config = (DicomPrintConfig)lvi.Tag;
			PrinterSettingsForm psf = new PrinterSettingsForm(config);
			if (psf.ShowDialog(this) == DialogResult.OK) {
				SaveSettings();
			} else {
				LoadSettings();
			}
		}

		private void OnClickDeletePrinter(object sender, EventArgs e) {
			if (lvPrinters.SelectedItems.Count == 0)
				return;

			if (MessageBox.Show(this, "Are you sure you want to delete the selected printer?", "Delete Printer", 
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				return;

			ListViewItem lvi = lvPrinters.SelectedItems[0];
			DicomPrintConfig config = (DicomPrintConfig)lvi.Tag;
			Config.Instance.Printers.Remove(config);
			SaveSettings();
		}
	}
}
