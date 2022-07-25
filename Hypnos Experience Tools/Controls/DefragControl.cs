using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Hypnos.Experience.Tools
{
	public partial class DefragControl : UserControl
	{
		public DefragControl()
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

        private ConsoleRedirector DefragCR { get; set; }

        #endregion

        #region Public methods

        public void UpdateUI()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => { UpdateUI(); }));
                return;
            }

            btnDefrag.Enabled = Env.EngineValidated && !string.IsNullOrEmpty(textBoxExperienceFile.Text) && File.Exists(textBoxExperienceFile.Text);
            btnDefrag.Text = DefragCR == null ? "Defrag" : "Stop";
        }

        #endregion

        #region Priovate method

        private void btnBrowseExperienceFile_Click(object sender, EventArgs e)
		{
            using (ButtonLock bl = new ButtonLock(btnBrowseExperienceFile, false))
            {
                string currentPath = null;
                if (!string.IsNullOrEmpty(textBoxExperienceFile.Text))
                {
                    try
                    {
                        currentPath = Path.GetFullPath(textBoxExperienceFile.Text);
                        if (!Directory.Exists(currentPath))
                            currentPath = null;
                    }
                    catch
                    {
                        currentPath = null;
                    }
                }

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = Constants.ExperienceExtension;
                ofd.Filter = string.Format("{0} experience files (*.{1})|*.{1}", Constants.EngineName, Constants.ExperienceExtension);
                ofd.InitialDirectory = currentPath;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string newEngineName = Path.GetFullPath(ofd.FileName);
                if (string.Compare(newEngineName, textBoxExperienceFile.Text, true) == 0)
                    return;

                textBoxExperienceFile.Text = Path.GetFullPath(ofd.FileName);
            }

            UpdateUI();
        }

		private void btnDefrag_Click(object sender, EventArgs e)
		{
            btnDefrag.Enabled = false;

            if (DefragCR == null)
            {
                DefragCR = new ConsoleRedirector();

                UpdateUI();

                Parent.AddEngineOutput("");
                Parent.AddControlText("Starting defragmentation");

                //Start defragmentation in a thread
                new Thread(() =>
                {
                    DefragCR.ProcessOutputReceived += new DataReceivedEventHandler((_s, _e) =>
                    {
                        if (string.IsNullOrEmpty(_e.Data))
                            return;

                        Parent.AddEngineOutput(_e.Data);
                    });

                    DefragCR.ProcessExited += new EventHandler((_s, _e) =>
                    {
                        if (_s == DefragCR)
                        {
                            DefragCR = null;
                            UpdateUI();

                            Parent.AddControlText("Defragmentation ended");
                        }
                    });

                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("defrag \"{0}\"", textBoxExperienceFile.Text);

                    DefragCR.Start(Env.EnginePath, sb.ToString(), null);
                    DefragCR.WaitForExit(Timeout.Infinite, false);
                }).Start();
            }
            else
            {
                ConsoleRedirector cr = DefragCR;

                //Just in case the process existed immediately before we obtained the reference to 'DefragCR'
                if (cr != null)
                    cr.Kill();

                //Just to be sure
                DefragCR = null;
            }

            UpdateUI();
		}

        #endregion
    }
}
