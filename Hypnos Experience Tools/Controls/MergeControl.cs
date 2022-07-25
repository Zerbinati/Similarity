using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Hypnos.Experience.Tools
{
	public partial class MergeControl : UserControl
	{
		public MergeControl()
		{
			InitializeComponent();

			UpdateUI();
		}

		#region Private properties

		private new MainForm Parent
		{
			get
			{
				return (((UserControl)this).Parent.Parent.Parent) as MainForm;
			}
		}

		private ConsoleRedirector MergeCR { get; set; }

		List<string> ExperienceFilesToBeMerged
		{
			get
			{
				List<string> list = new List<string>();
				for (int i = 0; i < listBoxExperienceFiles.Items.Count; i++)
					list.Add(listBoxExperienceFiles.Items[i] as string);

				return list;
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

			btnMerge.Enabled = Env.EngineValidated && listBoxExperienceFiles.Items.Count > 0;
			btnMerge.Text = MergeCR == null ? "Merge" : "Stop";
		}

		#endregion

		#region Private methods


		#endregion

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.CheckPathExists = true;
			ofd.Multiselect = true;
			ofd.DefaultExt = Constants.ExperienceExtension;
			ofd.Filter = string.Format("{0} experience files (*.{1})|*.{1}", Constants.EngineName, Constants.ExperienceExtension);

			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			foreach (string newFile in ofd.FileNames)
			{
				if (ExperienceFilesToBeMerged.Exists(x => string.Compare(x, newFile, true) == 0))
					continue;

				listBoxExperienceFiles.Items.Add(newFile);
			}

			UpdateUI();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			List<string> filesToRemove = new List<string>();
			foreach (object o in listBoxExperienceFiles.SelectedItems)
				filesToRemove.Add(o as string);

			foreach (string s in filesToRemove)
				listBoxExperienceFiles.Items.Remove(s);

			UpdateUI();
		}

		private void btnMerge_Click(object sender, System.EventArgs e)
		{
			btnMerge.Enabled = false;

			if (MergeCR == null)
			{
				MergeCR = new ConsoleRedirector();

				UpdateUI();

				Parent.AddEngineOutput("");
				Parent.AddControlText("Starting Merge");

				//Start Mergementation in a thread
				new Thread(() =>
				{
					MergeCR.ProcessOutputReceived += new DataReceivedEventHandler((_s, _e) =>
					{
						if (string.IsNullOrEmpty(_e.Data))
							return;

						Parent.AddEngineOutput(_e.Data);
					});

					MergeCR.ProcessExited += new EventHandler((_s, _e) =>
					{
						if (_s == MergeCR)
						{
							MergeCR = null;
							UpdateUI();

							Parent.AddControlText("Merge ended");
						}
					});

					StringBuilder sb = new StringBuilder();
					sb.Append("merge");

					foreach (string s in ExperienceFilesToBeMerged)
						sb.AppendFormat(" \"{0}\"", s);

					MergeCR.Start(Env.EnginePath, sb.ToString(), null);
					MergeCR.WaitForExit(Timeout.Infinite, false);
				}).Start();
			}
			else
			{
				ConsoleRedirector cr = MergeCR;

				//Just in case the process existed immediately before we obtained the reference to 'MergeCR'
				if (cr != null)
					cr.Kill();

				//Just to be sure
				MergeCR = null;
			}

			UpdateUI();
		}
	}
}
