using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Dicom;
using Dicom.Data;
using Dicom.Imaging;
using Dicom.Network;
using Dicom.Network.Server;
using Dicom.Utility;

namespace Dicom.PrintScp {
	class NPrintState {
		public byte PresentationID;
		public DcmPrintJob Job;
		public DcmPrintDocument Document;
	}

	class NPrintService : DcmServiceBase {
		#region Private Members
		private DcmFilmSession _session;
		private List<DcmPrintJob> _jobs;
		private DicomPrintConfig _config;
		#endregion

		#region Public Constructors
		public NPrintService() : base() {
			LogID = "N-Print SCP";
			_jobs = new List<DcmPrintJob>();
		}
		#endregion

		#region Protected Overrides
		protected override void OnReceiveAssociateRequest(DcmAssociate association) {
			association.NegotiateAsyncOps = false;
			LogID = association.CallingAE;

			_config = Config.Instance.FindPrinter(association.CalledAE);
			if (_config == null) {
				SendAssociateReject(DcmRejectResult.Permanent, DcmRejectSource.ServiceUser, DcmRejectReason.CalledAENotRecognized);
				return;
			}

			foreach (DcmPresContext pc in association.GetPresentationContexts()) {
				if (pc.AbstractSyntax == DicomUID.VerificationSOPClass ||
					pc.AbstractSyntax == DicomUID.BasicColorPrintManagementMetaSOPClass ||
					pc.AbstractSyntax == DicomUID.BasicGrayscalePrintManagementMetaSOPClass ||
					pc.AbstractSyntax == DicomUID.PrinterSOPClass ||
					//pc.AbstractSyntax == DicomUID.PrinterConfigurationRetrieval ||
					//pc.AbstractSyntax == DicomUID.PrintJob ||
					pc.AbstractSyntax == DicomUID.BasicFilmSessionSOPClass ||
					pc.AbstractSyntax == DicomUID.BasicFilmBoxSOPClass ||
					pc.AbstractSyntax == DicomUID.BasicGrayscaleImageBoxSOPClass ||
					pc.AbstractSyntax == DicomUID.BasicColorImageBoxSOPClass) {
					pc.SetResult(DcmPresContextResult.Accept);
				} else {
					pc.SetResult(DcmPresContextResult.RejectAbstractSyntaxNotSupported);
				}
			}
			SendAssociateAccept(association);
		}

		protected override void OnReceiveCEchoRequest(byte presentationID, ushort messageID, DcmPriority priority) {
			SendCEchoResponse(presentationID, messageID, DcmStatus.Success);
		}

		protected override void OnReceiveNCreateRequest(byte presentationID, ushort messageID, 
			DicomUID affectedClass, DicomUID affectedInstance, DcmDataset dataset) {
			DicomUID sopClass = Associate.GetAbstractSyntax(presentationID);

			if (affectedClass == DicomUID.BasicFilmSessionSOPClass) {
				if (_session != null) {
					Log.Error("{0} -> Attempted to create second Basic Film Session on association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				_session = new DcmFilmSession(sopClass, affectedInstance, dataset);

				SendNCreateResponse(presentationID, messageID, affectedClass, _session.SOPInstanceUID, null, DcmStatus.Success);
				return;
			}

			if (affectedClass == DicomUID.BasicFilmBoxSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				DcmFilmBox box = _session.CreateFilmBox(affectedInstance, dataset);
				if (!box.Initialize()) {
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				SendNCreateResponse(presentationID, messageID, affectedClass, box.SOPInstanceUID, dataset, DcmStatus.Success);
				return;
			}

			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected override void OnReceiveNDeleteRequest(byte presentationID, ushort messageID, 
			DicomUID requestedClass, DicomUID requestedInstance) {

			if (requestedClass == DicomUID.BasicFilmSessionSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				_session = null;

				SendNDeleteResponse(presentationID, messageID, requestedClass, requestedInstance, DcmStatus.Success);
				return;
			}

			if (requestedClass == DicomUID.BasicFilmBoxSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				_session.DeleteFilmBox(requestedInstance);

				SendNDeleteResponse(presentationID, messageID, requestedClass, requestedInstance, DcmStatus.Success);
				return;
			}

			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected override void OnReceiveNSetRequest(byte presentationID, ushort messageID, 
			DicomUID requestedClass, DicomUID requestedInstance, DcmDataset dataset) {

			if (requestedClass == DicomUID.BasicFilmSessionSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				_session.Dataset.Merge(dataset);

				SendNSetResponse(presentationID, messageID, requestedClass, requestedInstance, null, DcmStatus.Success);
				return;
			}

			if (requestedClass == DicomUID.BasicFilmBoxSOPClass) {
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				DcmFilmBox box = _session.FindFilmBox(requestedInstance);
				if (box == null) {
					Log.Error("{0} -> Received N-SET request for invalid film box", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				box.Dataset.Merge(dataset);

				SendNSetResponse(presentationID, messageID, requestedClass, requestedInstance, null, DcmStatus.Success);
				return;
			}

			if (requestedClass == DicomUID.BasicColorImageBoxSOPClass ||
				requestedClass == DicomUID.BasicGrayscaleImageBoxSOPClass)
			{
				if (_session == null) {
					Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				DcmImageBox box = _session.FindImageBox(requestedInstance);
				if (box == null) {
					Log.Error("{0} -> Received N-SET request for invalid image box", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				box.Dataset.Merge(dataset);

				SendNSetResponse(presentationID, messageID, requestedClass, requestedInstance, null, DcmStatus.Success);
				return;
			}

			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected override void OnReceiveNGetRequest(byte presentationID, ushort messageID,
			DicomUID requestedClass, DicomUID requestedInstance, DicomTag[] attributes) {

			if (requestedClass == DicomUID.PrinterSOPClass && requestedInstance == DicomUID.PrinterSOPInstance) {
				DcmDataset ds = new DcmDataset(DicomTransferSyntax.ImplicitVRLittleEndian);
				ds.AddElementWithValue(DicomTags.PrinterStatus, "NORMAL");
				ds.AddElementWithValue(DicomTags.PrinterStatus, "NORMAL");
				ds.AddElementWithValue(DicomTags.PrinterName, _config.PrinterName);
				ds.AddElementWithValue(DicomTags.Manufacturer, "N/A");
				ds.AddElementWithValue(DicomTags.ManufacturersModelName, "N/A");
				ds.AddElementWithValue(DicomTags.DeviceSerialNumber, "N/A");
				ds.AddElementWithValue(DicomTags.SoftwareVersions, "N/A");
				ds.SetDateTime(DicomTags.DateOfLastCalibration, DicomTags.TimeOfLastCalibration, DateTime.Now);

				SendNGetResponse(presentationID, messageID, requestedClass, requestedInstance, ds, DcmStatus.Success);
				return;
			}

			if (requestedClass == DicomUID.PrintJobSOPClass) {
				DcmPrintJob job = null;

				foreach (DcmPrintJob pj in _jobs) {
					if (pj.SOPInstanceUID == requestedInstance) {
						job = pj;
						break;
					}
				}

				if (job == null) {
					job = new DcmPrintJob(requestedInstance);
					job.ExecutionStatus = "DONE";
					job.CreationDateTime = DateTime.Today;
					job.PrintPriority = _session.PrintPriority;
					job.PrinterName = _config.PrinterName;
					job.Originator = Associate.CallingAE;
				}

				SendNGetResponse(presentationID, messageID, requestedClass, requestedInstance, job.Dataset, DcmStatus.Success);
				return;
			}

			if (requestedClass == DicomUID.PrinterConfigurationRetrievalSOPClass && requestedInstance == DicomUID.PrinterConfigurationRetrievalSOPInstance) {
				DcmDataset ds = new DcmDataset(DicomTransferSyntax.ImplicitVRLittleEndian);
				DcmDataset config = new DcmDataset(DicomTransferSyntax.ImplicitVRLittleEndian);


				DcmItemSequence sq = new DcmItemSequence(DicomTags.PrinterConfigurationSequence);
				sq.AddSequenceItem(config);
				ds.AddItem(sq);

				SendNGetResponse(presentationID, messageID, requestedClass, requestedInstance, ds, DcmStatus.Success);
				return;
			}

			SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
		}

		protected override void OnReceiveNActionRequest(byte presentationID, ushort messageID, 
			DicomUID requestedClass, DicomUID requestedInstance, ushort actionTypeID, DcmDataset dataset) {

			if (_session == null) {
				Log.Error("{0} -> A Basic Film Session does not exist for this association", LogID);
				SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
				return;
			}

			DcmPrintDocument document = new DcmPrintDocument(_config, _session.Clone());

			if (requestedClass == DicomUID.BasicFilmSessionSOPClass && actionTypeID == 0x0001) {
				foreach (DcmFilmBox box in _session.BasicFilmBoxes)
					document.AddFilmBox(box.Clone());
			}
			
			else if (requestedClass == DicomUID.BasicFilmBoxSOPClass && actionTypeID == 0x0001) {
				DcmFilmBox box = _session.FindFilmBox(requestedInstance);
				if (box == null) {
					Log.Error("{0} -> Received N-ACTION request for invalid film box", LogID);
					SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
					return;
				}

				document.AddFilmBox(box.Clone());
			}

			else {
				SendAbort(DcmAbortSource.ServiceProvider, DcmAbortReason.NotSpecified);
			}

			DcmDataset result = null;

			NPrintState state = new NPrintState();
			state.PresentationID = presentationID;
			state.Document = document;

			//DicomUID jobUid = DicomUID.Generate(_session.SOPInstanceUID, 9999);
			//jobUid = DicomUID.Generate(jobUid, _jobs.Count + 1);

			//DcmPrintJob job = new DcmPrintJob(jobUid);
			//job.ExecutionStatus = "PENDING";
			//job.ExecutionStatusInfo = "QUEUED";
			//job.CreationDateTime = DateTime.Now;
			//job.PrintPriority = _session.PrintPriority;
			//job.PrinterName = Config.Instance.PrinterSettings.PrinterName;
			//job.Originator = Associate.CallingAE;

			//result = new DcmDataset(DicomTransferSyntax.ImplicitVRLittleEndian);
			//result.AddReferenceSequenceItem(DicomTags.ReferencedPrintJobSequenceRETIRED, DicomUID.PrintJob, job.SOPInstanceUID);

			//state.Job = job;
			//_jobs.Add(job);
			
			new Thread(PrintJobProc).Start(state);

			SendNActionResponse(presentationID, messageID, requestedClass, requestedInstance, actionTypeID, result, DcmStatus.Success);

			if (state.Job != null) {
				SendNEventReportRequest(presentationID, NextMessageID(), DicomUID.PrintJobSOPClass, state.Job.SOPInstanceUID, 1, state.Job.Dataset);
			}
		}

		protected override void OnReceiveNEventReportResponse(byte presentationID, ushort messageIdRespondedTo, 
			DicomUID affectedClass, DicomUID affectedInstance, ushort eventTypeID, DcmDataset dataset, DcmStatus status) {
			if (affectedClass == DicomUID.PrintJobSOPClass) {
				DcmPrintJob job = null;

				foreach (DcmPrintJob pj in _jobs) {
					if (pj.SOPInstanceUID == affectedInstance) {
						job = pj;
						break;
					}
				}

				if (job != null && (job.ExecutionStatus == "DONE" || job.ExecutionStatus == "FAILURE"))
					_jobs.Remove(job);

				return;
			}
		}
		#endregion

		#region Private Methods
		private void PrintJobProc(object state) {
			NPrintState print = (NPrintState)state;

			if (print.Job != null) {
				print.Job.ExecutionStatus = "PRINTING";
				print.Job.ExecutionStatusInfo = "PRINTING";
				SendNEventReportRequest(print.PresentationID, NextMessageID(), DicomUID.PrintJobSOPClass, print.Job.SOPInstanceUID, 2, print.Job.Dataset);
			}

			try {
				print.Document.Print();

				if (print.Job != null) {
					print.Job.ExecutionStatus = "DONE";
					print.Job.ExecutionStatusInfo = "SUCCESS";
					SendNEventReportRequest(print.PresentationID, NextMessageID(), DicomUID.PrintJobSOPClass, print.Job.SOPInstanceUID, 3, print.Job.Dataset);
				}
			} catch (Exception e) {
				if (print.Job != null) {
					print.Job.ExecutionStatus = "FAILURE";
					print.Job.ExecutionStatusInfo = e.Message;
					SendNEventReportRequest(print.PresentationID, NextMessageID(), DicomUID.PrintJobSOPClass, print.Job.SOPInstanceUID, 4, print.Job.Dataset);
				}
			}
		}
		#endregion
	}

	public class DcmPrintDocument {
		#region Private Members
		private DcmFilmSession _session;
		private List<DcmFilmBox> _filmBoxes;
		private int _current;
		private PrintPreviewDialog _previewDialog;
		private DicomPrintConfig _config;
		#endregion

		#region Public Constructors
		public DcmPrintDocument(DicomPrintConfig config, DcmFilmSession session) {
			_session = session;
			_config = config;
			_filmBoxes = new List<DcmFilmBox>();
		}
		#endregion

		#region Public Properties

		#endregion

		#region Public Methods
		public void AddFilmBox(DcmFilmBox box) {
			_filmBoxes.Add(box);
		}

		public void Print() {
			try {
				_current = 0;

				PrintDocument document = new PrintDocument();
				document.PrinterSettings = _config.PrinterSettings;
				document.PrinterSettings.Collate = true;
				document.PrinterSettings.Copies = (short)_session.NumberOfCopies;
				//document.DefaultPageSettings.Margins = new Margins(100, 100, 100, 100);
				document.DefaultPageSettings.Margins = new Margins(25, 25, 25, 25);
				document.QueryPageSettings += OnQueryPageSettings;
				document.PrintPage += OnPrintPage;

				if (!String.IsNullOrEmpty(_config.PaperSource)) {
					foreach (PaperSource source in document.PrinterSettings.PaperSources) {
						if (source.SourceName == _config.PaperSource)
							document.DefaultPageSettings.PaperSource = source;
					}
				}

				if (_config.PreviewOnly) {
					if (Application.OpenForms.Count > 0)
						Application.OpenForms[0].BeginInvoke(new WaitCallback(PreviewProc), document);
				} else {
					document.Print();
				}
			} catch (Exception ex) {
#if DEBUG
				Dicom.Debug.Log.Error("DICOM Print Error: " + ex.ToString());
#else
				Dicom.Debug.Log.Error("DICOM Print Error: " + ex.Message);
#endif
			}
		}
		#endregion

		#region Private Methods
		private void PreviewProc(object state) {
			try {
				PrintDocument document = (PrintDocument)state;

				_previewDialog = new PrintPreviewDialog();
				_previewDialog.Text = "DICOM Print Preview";
				_previewDialog.ShowInTaskbar = true;
				_previewDialog.WindowState = FormWindowState.Maximized;
				_previewDialog.Document = document;
				_previewDialog.FormClosed += delegate(object sender, FormClosedEventArgs e) {
					_previewDialog = null;
				};
				_previewDialog.Show(Application.OpenForms[0]);
				_previewDialog.BringToFront();
				_previewDialog.Focus();
			} catch (Exception ex) {
#if DEBUG
				Dicom.Debug.Log.Error("DICOM Print Error: " + ex.ToString());
#else
				Dicom.Debug.Log.Error("DICOM Print Error: " + ex.Message);
#endif
			}
		}

		private void OnQueryPageSettings(object sender, QueryPageSettingsEventArgs e) {
			DcmFilmBox filmBox = _filmBoxes[_current];

			e.PageSettings.Landscape = (filmBox.FilmOrientation == "LANDSCAPE");
		}

		#region Thank You Dr. Dobb's!
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, DeviceCapsIndex index);

		enum DeviceCapsIndex {
			PhysicalOffsetX = 112,
			PhysicalOffsetY = 113,
		}

		// Adjust MarginBounds rectangle when printing based
		// on the physical characteristics of the printer
		static Rectangle GetRealMarginBounds(PrintPageEventArgs e, bool preview) {
			if (preview) return e.MarginBounds;

			int cx = 0;
			int cy = 0;
			IntPtr hdc = e.Graphics.GetHdc();

			try {
				// Both of these come back as device units and are not
				// scaled to 1/100th of an inch
				cx = GetDeviceCaps(hdc, DeviceCapsIndex.PhysicalOffsetX);
				cy = GetDeviceCaps(hdc, DeviceCapsIndex.PhysicalOffsetY);
			} finally {
				e.Graphics.ReleaseHdc(hdc);
			}

			// Create the real margin bounds by scaling the offset
			// by the printer resolution and then rescaling it
			// back to 1/100th of an inch
			Rectangle marginBounds = e.MarginBounds;
			int dpiX = (int)e.Graphics.DpiX;
			int dpiY = (int)e.Graphics.DpiY;
			marginBounds.Offset(-cx * 100 / dpiX, -cy * 100 / dpiY);
			return marginBounds;
		}

		static Rectangle GetRealPageBounds(PrintPageEventArgs e, bool preview) {
			// Return in units of 1/100th of an inch
			if (preview) return e.PageBounds;

			// Translate to units of 1/100th of an inch
			RectangleF vpb = e.Graphics.VisibleClipBounds;
			PointF[] bottomRight = { new PointF(vpb.Size.Width, vpb.Size.Height) };
			e.Graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.Page, bottomRight);
			float dpiX = e.Graphics.DpiX;
			float dpiY = e.Graphics.DpiY;
			return new Rectangle(0, 0,
								(int)(bottomRight[0].X * 100 / dpiX),
								(int)(bottomRight[0].Y * 100 / dpiY));
		}
		#endregion

		private void OnPrintPage(object sender, PrintPageEventArgs e) {
			DcmFilmBox filmBox = _filmBoxes[_current];

			//if (filmBox.MagnificationType == "CUBIC")
			//    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			//else
			//    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

			e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

			int dpiX = e.PageSettings.PrinterResolution.X;
			int dpiY = e.PageSettings.PrinterResolution.Y;

			Rectangle bounds = GetRealPageBounds(e, _config.PreviewOnly);
			bounds.X += (int)e.PageSettings.HardMarginX;
			bounds.Width -= (int)e.PageSettings.HardMarginX * 2;
			bounds.Y += (int)e.PageSettings.HardMarginY;
			bounds.Height -= (int)e.PageSettings.HardMarginY * 2;

			//Rectangle bounds = GetRealMarginBounds(e, _config.PreviewOnly);

			string format = filmBox.ImageDisplayFormat;

			if (String.IsNullOrEmpty(format))
				return;

			string[] parts = format.Split('\\');

			if (parts[0] == "STANDARD" && parts.Length == 2) {
				parts = parts[1].Split(',');
				if (parts.Length == 2) {
					try {
						int rows = int.Parse(parts[0]);
						int cols = int.Parse(parts[1]);

						int rowSize = bounds.Height / rows;
						int colSize = bounds.Width / cols;

						int imageBox = 0;

						for (int r = 0; r < rows; r++) {
							for (int c = 0; c < cols; c++) {
								Point position = new Point();
								position.Y = bounds.Top + (r * rowSize);
								position.X = bounds.Left + (c * colSize);

								if (imageBox < filmBox.BasicImageBoxes.Count)
									DrawImageBox(filmBox.BasicImageBoxes[imageBox], e.Graphics, position, colSize, rowSize, dpiX, dpiY);

								imageBox++;
							}
						}
					}
					catch (Exception ex) {
#if DEBUG
						Dicom.Debug.Log.Error("DICOM Print Error: " + ex.ToString());
#else
						Dicom.Debug.Log.Error("DICOM Print Error: " + ex.Message);
#endif
					}
				}

			}

			if (parts[0] == "ROW" && parts.Length == 2) {
				try {
					parts = parts[1].Split(',');

					int rows = parts.Length;
					int rowSize = bounds.Height / rows;

					int imageBox = 0;

					for (int r = 0; r < rows; r++) {
						int cols = int.Parse(parts[r]);
						int colSize = bounds.Width / cols;

						for (int c = 0; c < cols; c++) {
                            Point position = new Point();
                            position.Y = bounds.Top + (r * rowSize);
                            position.X = bounds.Left + (c * colSize);

							if (imageBox < filmBox.BasicImageBoxes.Count)
								DrawImageBox(filmBox.BasicImageBoxes[imageBox], e.Graphics, position, colSize, rowSize, dpiX, dpiY);

							imageBox++;
						}
					}
				} catch (Exception ex) {
#if DEBUG
					Dicom.Debug.Log.Error("DICOM Print Error: " + ex.ToString());
#else
					Dicom.Debug.Log.Error("DICOM Print Error: " + ex.Message);
#endif
				}
			}

			if (parts[0] == "COL" && parts.Length == 2) {
				try {
					parts = parts[1].Split(',');

					int cols = parts.Length;
					int colSize = bounds.Width / cols;

					int imageBox = 0;

					for (int c = 0; c < cols; c++) {
						int rows = int.Parse(parts[c]);
						int rowSize = bounds.Height / rows;

						for (int r = 0; r < rows; r++) {
							Point position = new Point();
                            position.Y = bounds.Top + (r * rowSize);
                            position.X = bounds.Left + (c * colSize);

							if (imageBox < filmBox.BasicImageBoxes.Count)
								DrawImageBox(filmBox.BasicImageBoxes[imageBox], e.Graphics, position, colSize, rowSize, dpiX, dpiY);

							imageBox++;
						}
					}
				} catch (Exception ex) {
#if DEBUG
					Dicom.Debug.Log.Error("DICOM Print Error: " + ex.ToString());
#else
					Dicom.Debug.Log.Error("DICOM Print Error: " + ex.Message);
#endif
				}
			}

			_current++;
			e.HasMorePages = _current < _filmBoxes.Count;
			if (!e.HasMorePages)
				_current = 0;
		}

		private void DrawImageBox(DcmImageBox imageBox, Graphics graphics, Point position, int width, int height, int dpiX, int dpiY) {
			DcmDataset dataset = imageBox.ImageSequence;
			if (!dataset.Contains(DicomTags.PixelData))
				return;

			DcmPixelData pixelData = new DcmPixelData(dataset);

			PinnedIntArray pixelBuffer = null;
			Bitmap bitmap = null;

			if (pixelData.SamplesPerPixel == 3) {
				pixelBuffer = new PinnedIntArray(pixelData.GetFrameDataS32(0));
				bitmap = new Bitmap(pixelData.ImageWidth, pixelData.ImageHeight,
					pixelData.ImageWidth * sizeof(int), PixelFormat.Format32bppRgb, pixelBuffer.Pointer);
			} else {
                bool invert = (pixelData.PhotometricInterpretation == "MONOCHROME1");
				if (imageBox.Polarity == "REVERSE")
					invert = !invert;

				byte[] pixelsOut = null;

				if (pixelData.BitsAllocated == 8) {					
					pixelsOut = pixelData.GetFrameDataU8(0);
				} else {
                    ushort[] pixels = pixelData.GetFrameDataU16(0);
					pixelsOut = new byte[pixels.Length];
					double scale = 256.0 / 4096.0;

                    int pixel = 0;
                    for (int y = 0; y < pixelData.ImageHeight; y++) {
                        for (int x = 0; x < pixelData.ImageWidth; x++) {
							pixelsOut[pixel] = (byte)(pixels[pixel] * scale);
                            pixel++;
                        }
                    }

					pixels = null;
				}

				bitmap = new Bitmap(pixelData.ImageWidth, pixelData.ImageHeight, PixelFormat.Format8bppIndexed);

				if (invert)
					ColorTable.Apply(bitmap, ColorTable.Monochrome1);
				else
					ColorTable.Apply(bitmap, ColorTable.Monochrome2);

				BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, pixelData.ImageWidth, pixelData.ImageHeight),
					ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

				IntPtr pos = bmData.Scan0;
				for (int i = 0, c = pixelsOut.Length; i < c; i += bmData.Width) {
					Marshal.Copy(pixelsOut, i, pos, bmData.Width);
					pos = new IntPtr(pos.ToInt64() + bmData.Stride);
				}

				bitmap.UnlockBits(bmData);
			}

			//bitmap.SetResolution(dpiX, dpiY);

			int border = 3;

			double factor = Math.Min((double)height / (double)bitmap.Height,
									 (double)width / (double)bitmap.Width);

			int drawWidth = (int)(bitmap.Width * factor) - (border * 2);
			int drawHeight = (int)(bitmap.Height * factor) - (border * 2);

			int drawX = position.X + ((width - drawWidth) / 2);
			int drawY = position.Y + ((height - drawHeight) / 2);

			graphics.DrawImage(bitmap, drawX, drawY, drawWidth, drawHeight);
		}
		#endregion
	}
}
