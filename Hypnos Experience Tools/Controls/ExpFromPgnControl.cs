using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Hypnos.Experience.Tools
{
	public partial class ExpFromPgnControl : UserControl
	{
		public ExpFromPgnControl()
		{
			InitializeComponent();

			UpdateUI();
		}

		#region Private variables

		Dictionary<string, long> _pgnFiles = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
		private string _lastPgnFolder = null;

		bool _working = false;
		bool _abort = false;

		#endregion


		#region Private properties

		private new MainForm Parent
		{
			get
			{
				return (((UserControl)this).Parent.Parent.Parent) as MainForm;
			}
		}

		#endregion

		#region Public methods

		public void UpdateUI()
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(() => { UpdateUI(); }));
				return;
			}

			btnStartStop.Enabled = Env.EngineValidated && _pgnFiles.Count > 0 && !string.IsNullOrEmpty(txtExpFilePath.Text);
			btnStartStop.Text = _working ? "Stop" : "Start";

			//Generate the info label
			long totalSize = 0;
			foreach (KeyValuePair<string, long> kvp in _pgnFiles)
				totalSize += kvp.Value;

			lblPgnInfo.Text = string.Format("{0} PGN file{1} (size = {2})", _pgnFiles.Count, _pgnFiles.Count == 1 ? string.Empty : "s", Util.FormatBytes(totalSize));
		}

		#endregion

		#region Private methods

		private bool RunCleanPgnStep(string workFolder, string cleanPgnFile)
		{
			string pgnExtractExe = Path.Combine(workFolder, "my-pgn-extract-x64.exe");
			string pgnFileList = Path.Combine(workFolder, "pgn_file_list.txt");

			Parent.AddEngineOutput("Cleaning {0}", lblPgnInfo.Text);

			#region Clean PGN

			#region Extract pgn-extract exe

			try
			{
				File.WriteAllBytes(pgnExtractExe, Properties.Resources.pgn_extract);
			}
			catch (Exception ex)
			{
				Parent.AddEngineOutput("Could not extract required tool to: {0}\r\nError from: {1}\r\nError message: {2}", pgnExtractExe, ex.StackTrace, ex.Message);
				return false;
			}

			#endregion

			#region Create file list to be used with pgn-extract

			string[] allPgnFiles = new string[_pgnFiles.Count];

			int idx = 0;
			foreach (string fileName in _pgnFiles.Keys)
			{
				allPgnFiles[idx] = fileName;
				idx++;
			}

			try
			{
				File.WriteAllLines(pgnFileList, allPgnFiles);
			}
			catch (Exception ex)
			{
				Parent.AddEngineOutput("Could not write PGN file list to: {0}\r\nError from: {1}\r\nError message: {2}", pgnFileList, ex.StackTrace, ex.Message);
				return false;
			}

			#endregion

			#region Run pgn-extract

			string pgnExtractArgs = string.Format("-w102400 --quiet --novars --fixresulttags -Wlalg -f\"{0}\" --output \"{1}\"", pgnFileList, cleanPgnFile);

			try
			{
				LogInfo myLog = Parent.AddEngineOutput("Clean PGN size: 0 B");
				DateTime lastProgress = DateTime.Now;

				using (ConsoleRedirector cr = new ConsoleRedirector())
				{
					cr.Start(pgnExtractExe, pgnExtractArgs, null);

					while (cr != null && !cr.WaitForExit(50, false))
					{
						Application.DoEvents();

						if (_abort)
							cr.Kill();

						if ((DateTime.Now - lastProgress).TotalSeconds > 1)
						{
							lastProgress = DateTime.Now;
							Parent.AddEngineOutput(ref myLog, "Clean PGN size: {0}", Util.FormatBytes((new FileInfo(cleanPgnFile)).Length));
						}
					}

					if (!_abort)
						Parent.AddEngineOutput(ref myLog, "Clean PGN size: {0}", Util.FormatBytes((new FileInfo(cleanPgnFile)).Length));
				}
			}
			catch (Exception ex)
			{
				string errorMessage = string.Format("Error while running pgn-extract.exe from: {0}\r\nError from: {1}\r\nError message: {2}", pgnExtractExe, ex.StackTrace, ex.Message);

				Parent.AddEngineOutput(errorMessage);

				return false;
			}

			#endregion

			#endregion

			return !_abort;
		}

		private bool RunCompactPgnStep(string cleanPgnFile, string compactPgnFile)
		{
			LogInfo myLog = Parent.AddEngineOutput("Generating compact PGN");

			bool success = PgnProcessor.PgnToCpgn(
				cleanPgnFile,
				compactPgnFile,
				delegate (string timeRemaining, long completedGames, long rejectedGames, double gamesPerSecond, int errors, double completedPercentage)
				{
					if (_abort)
						return false;

					Parent.AddEngineOutput(
						ref myLog,
						"Generating compact PGN -> {0:0.000}%, Games: {1} (rejected: {2}, errors: {3}). Games/second: {4:0.0}. Time remaining: {5}",
						completedPercentage,
						completedGames,
						rejectedGames,
						errors,
						gamesPerSecond,
						timeRemaining);

					Application.DoEvents();

					return true;
				});

			return success && !_abort;
		}

		private bool RunConvertCompactPgnToExpStep(string compactPgnFile, string expFile)
		{
			Parent.AddEngineOutput("Generating experience from compact PGN");

			try
			{
				using (ConsoleRedirector cr = new ConsoleRedirector())
				{
					cr.ProcessOutputReceived += new DataReceivedEventHandler((_s, _e) =>
					{
						if (_e.Data == null)
							return;

						Parent.AddEngineOutput(_e.Data);
					});

					string HypnosArguments = string.Format("convert_compact_pgn \"{0}\" \"{1}\"", compactPgnFile, expFile);
					cr.Start(Env.EnginePath, HypnosArguments, null);

					while (cr != null && !cr.WaitForExit(50, false))
					{
						Application.DoEvents();

						if (_abort)
							cr.Kill();
					}
				}
			}
			catch (Exception ex)
			{
				Parent.AddEngineOutput("Error while running Hypnos engine from: {0}\r\nError from: {1}\r\nError message: {2}", Env.EnginePath, ex.StackTrace, ex.Message);
				return false;
			}

			return !_abort;
		}

		#endregion

		private void btnAddPgnFiles_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.CheckPathExists = true;
			ofd.Multiselect = true;
			ofd.DefaultExt = Constants.ExperienceExtension;
			ofd.Filter = "PGN files (*.pgn)|*.pgn";

			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			listBoxPgnFiles.BeginUpdate();
			foreach (string newFile in ofd.FileNames)
			{
				if (!_pgnFiles.ContainsKey(newFile))
				{
					_pgnFiles.Add(newFile, new FileInfo(newFile).Length);
					listBoxPgnFiles.Items.Add(newFile);
				}
			}
			listBoxPgnFiles.EndUpdate();

			UpdateUI();
		}

		private void btnAddPgnFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.Description = "Browse folder";
			fbd.ShowNewFolderButton = false;
			fbd.RootFolder = Environment.SpecialFolder.MyComputer;
			fbd.SelectedPath = _lastPgnFolder;

			if (fbd.ShowDialog() != DialogResult.OK)
				return;

			_lastPgnFolder = fbd.SelectedPath;

			listBoxPgnFiles.BeginUpdate();
			foreach (string _newFile in Directory.EnumerateFiles(Path.GetFullPath(_lastPgnFolder), "*.pgn", SearchOption.AllDirectories))
			{
				string newFile = Path.GetFullPath(_newFile);
				if (!_pgnFiles.ContainsKey(newFile))
				{
					_pgnFiles.Add(newFile, new FileInfo(newFile).Length);
					listBoxPgnFiles.Items.Add(newFile);
				}
			}
			listBoxPgnFiles.EndUpdate();

			UpdateUI();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			List<string> filesToRemove = new List<string>();
			foreach (object o in listBoxPgnFiles.SelectedItems)
				filesToRemove.Add(o as string);

			listBoxPgnFiles.BeginUpdate();
			foreach (string s in filesToRemove)
			{
				_pgnFiles.Remove(s);
				listBoxPgnFiles.Items.Remove(s);
			}
			listBoxPgnFiles.EndUpdate();

			UpdateUI();
		}

		private void btnBrowseExpFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.CheckFileExists = false;
			ofd.CheckPathExists = true;
			ofd.Multiselect = false;
			ofd.DefaultExt = Constants.ExperienceExtension;
			ofd.Filter = string.Format("{0} experience files (*.{1})|*.{1}", Constants.EngineName, Constants.ExperienceExtension);

			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			txtExpFilePath.Text = Path.GetFullPath(ofd.FileName);

			UpdateUI();
		}

		private void btnStartStop_Click(object sender, System.EventArgs e)
		{
			if(_working)
			{
				Parent.AddEngineOutput("PGN to EXP -> Aborting");
				_abort = true;
				return;
			}

			//Change button label
			_working = true;
			btnStartStop.Text = "Stop";

			Parent.AddControlText("PGN to EXP -> Starting");

			try
			{
				string tempFolder = Path.Combine(Path.GetTempPath(), "Hypnos UI Tools");				
				string cleanPgnFile = Path.Combine(tempFolder, "clean.cpgn");
				string compactPgnFile = Path.Combine(tempFolder, "compact.cpgn");

				#region Create temporary folder

				try
				{
					Util.RemoveDirectory(tempFolder);
				}
				catch (Exception ex)
				{
					string errorMessage = string.Format("Could not delete pre-existing temporary directory: {0}\r\nError from: {1}\r\nError message: {2}", tempFolder, ex.StackTrace, ex.Message);
					MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				try
				{
					Directory.CreateDirectory(tempFolder);
				}
				catch (Exception ex)
				{
					string errorMessage = string.Format("Could not create temporary directory: {0}\r\nError from: {1}\r\nError message: {2}", tempFolder, ex.StackTrace, ex.Message);
					MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				#endregion

				bool ok = RunCleanPgnStep(tempFolder, cleanPgnFile);
				if (!ok)
					return;

				ok = RunCompactPgnStep(cleanPgnFile, compactPgnFile);
				if (!ok)
					return;

				ok = RunConvertCompactPgnToExpStep(compactPgnFile, txtExpFilePath.Text);
				if (!ok)
					return;
			}
			finally
			{
				Parent.AddControlText("PGN to EXP -> {0}", _abort ? "Aborted" : "Completed");

				_working = false;
				_abort = false;
				
				UpdateUI();
			}
		}
	}
}
