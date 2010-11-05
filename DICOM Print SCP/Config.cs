using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Dicom.PrintScp {
	[Serializable]
	public class DicomPrintConfig {
		public string AETitle;
		public PrinterSettings PrinterSettings;
		public string PaperSource;
		public bool PreviewOnly;

		public DicomPrintConfig() {
			AETitle = "PRINT_SCP";
			PreviewOnly = false;

			PrintDocument document = new PrintDocument();
			PrinterSettings = document.PrinterSettings;
			PaperSource = PrinterSettings.PaperSources[0].SourceName;
		}

		public string PrinterName {
			get { return PrinterSettings.PrinterName; }
		}
	}

	[Serializable]
	public class Config {
		public static Config Instance;
		private static string ConfigPath = Path.Combine(Dicom.Debug.GetStartDirectory(), "printscp.cfg");

		public int Port;
		public int MaxPduSize;
		public int SocketTimeout;
		public int DimseTimeout;
		public int ThrottleSpeed;
		public List<DicomPrintConfig> Printers;

		public Config() {
			Port = 104;
			MaxPduSize = 65536;
			SocketTimeout = 30;
			DimseTimeout = 180;
			ThrottleSpeed = 0;
			Printers = new List<DicomPrintConfig>();
		}

		public DicomPrintConfig FindPrinter(string aeTitle) {
			foreach (DicomPrintConfig config in Printers) {
				if (config.AETitle == aeTitle)
					return config;
			}
			return null;
		}

		public static Config Load() {
			if (!File.Exists(ConfigPath)) {
				Instance = new Config();
				return Instance;
			}

			try {
				BinaryFormatter serializer = new BinaryFormatter();
				using (FileStream fs = new FileStream(ConfigPath, FileMode.Open)) {
					Instance = (Config)serializer.Deserialize(fs);
				}
			}
			catch (Exception e) {
				Dicom.Debug.Log.Error(e.Message);
				Instance = new Config();
			}

			//try {
			//    XmlSerializer serializer = new XmlSerializer(typeof(Config));
			//    using (FileStream fs = new FileStream(ConfigPath, FileMode.Open)) {
			//        Instance = (Config)serializer.Deserialize(fs);
			//    }
			//}
			//catch (Exception e) {
			//    Dicom.Debug.Log.Error(e.Message);
			//    Instance = new Config();
			//}

			return Instance;
		}

		public static void Save() {
			if (Instance == null)
				Instance = new Config();

			try {
				BinaryFormatter serializer = new BinaryFormatter();
				using (FileStream fs = File.Create(ConfigPath)) {
					serializer.Serialize(fs, Instance);
					fs.Flush();
				}
			}
			catch (Exception e) {
				Dicom.Debug.Log.Error(e.Message);
			}

			//try {
			//    XmlSerializer serializer = new XmlSerializer(typeof(Config));
			//    using (FileStream fs = File.Create(ConfigPath)) {
			//        serializer.Serialize(fs, Instance);
			//        fs.Flush();
			//    }
			//}
			//catch (Exception e) {
			//    Dicom.Debug.Log.Error(e.Message);
			//}
		}
	}
}
