using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

namespace Dicom.PrintScp {
	public static class AppUtility {
		public static bool IsAutoStartup(string appName) {
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true)) {
				foreach (string name in key.GetValueNames()) {
					if (name == appName)
						return true;
				}
				return false;
			}
		}

		public static void SetAutoStartup(string appName, bool autoStartup) {
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true)) {
				if (IsAutoStartup(appName)) {
					if (!autoStartup)
						key.DeleteValue(appName);
				} else
					if (autoStartup)
						key.SetValue(appName, String.Format("\"{0}\"", Application.ExecutablePath));
			}
		}
	}
}
