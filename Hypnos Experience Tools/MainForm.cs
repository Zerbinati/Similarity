using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Hypnos.Experience.Tools
{
    public struct LogInfo
	{
        public static readonly LogInfo Empty = new LogInfo(-1, 0);

        public int Start;
        public int Length;

        public LogInfo(int start, int length)
		{
            Start = start;
            Length = length;
		}
    }

	public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

#if true
            //Allow Cross-thread operations (We will syncronize internally)
            CheckForIllegalCrossThreadCalls = false;
#endif

            //Set main form title
            Text = string.Format("{0} {1}", Constants.ApplicationName, Constants.ApplicationVersion);

            //Center exit button
            btnExit.Left = (Width - btnExit.Width) / 2;
        }

        #region Public methods

        public void UpdateUI(Control sender)
        {
            if (sender != (Control)defragControl)
                defragControl.UpdateUI();

            if (sender != (Control)mergeControl)
                mergeControl.UpdateUI();

            if (sender != (Control)expFromPgnControl)
                expFromPgnControl.UpdateUI();
        }

        public LogInfo AddEngineOutput(string output, params object[] args)
        {
            if (output == null)
                return LogInfo.Empty;

            lock (richEngineOutput)
            {
                string log = string.Format(output, args);
                LogInfo li = new LogInfo(richEngineOutput.Text.Length, log.Length);

                //Add data
                richEngineOutput.AppendText(log + Environment.NewLine);

                //Refresh
                richEngineOutput.Refresh();

                return li;
            }
        }

        public void AddEngineOutput(ref LogInfo logInfo, string output, params object[] args)
        {
            if (output == null)
                return;

            lock (richEngineOutput)
            {
                richEngineOutput.Select(logInfo.Start, logInfo.Length);

                string log = string.Format(output, args);
                logInfo.Length = log.Length;

                richEngineOutput.SelectedText = log;

                //Refresh
                richEngineOutput.Refresh();
            }
        }

        public void AddControlText(string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return;

            lock (richEngineOutput)
            {
                richEngineOutput.SelectionStart = richEngineOutput.TextLength;
                richEngineOutput.SelectionLength = 0;

                richEngineOutput.SelectionColor = Color.Yellow;
                richEngineOutput.SelectionBackColor = Color.Red;
                richEngineOutput.AppendText(string.Format(text, args) + Environment.NewLine);

                //Scroll to end
                richEngineOutput.SelectionStart = richEngineOutput.Text.Length;
                richEngineOutput.ScrollToCaret();

                //Refresh
                richEngineOutput.Refresh();
            }
        }

        #endregion

        #region Handlers
        private void btnBrowseEngine_Click(object sender, System.EventArgs e)
        {
            using (ButtonLock bl = new ButtonLock(btnBrowseEngine, false))
            {
                try
                {
                    #region Browse

                    string currentPath = null;
                    if (!string.IsNullOrEmpty(Env.EnginePath))
                    {
                        try
                        {
                            currentPath = Path.GetFullPath(Env.EnginePath);
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
                    ofd.DefaultExt = "exe";
                    ofd.Filter = string.Format("{0} engine ({0}*.exe)|{0}*.exe", Constants.EngineNameShort);
                    ofd.InitialDirectory = currentPath;

                    if (ofd.ShowDialog() != DialogResult.OK)
                        return;

                    string newEnginePath = Path.GetFullPath(ofd.FileName);
                    if (string.Compare(newEnginePath, Env.EnginePath, true) == 0)
                        return;

                    Env.EnginePath = textEnginePath.Text = Path.GetFullPath(ofd.FileName);

                    #endregion

                    #region Validate

                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        Env.EngineValidated = false;

                        using (ConsoleRedirector validator = new ConsoleRedirector())
                        {
                            List<string> engineOutput = new List<string>();

                            validator.ProcessOutputReceived += new DataReceivedEventHandler((_s, _e) =>
                            {
                                if (string.IsNullOrEmpty(_e.Data))
                                    return;

                                engineOutput.Add(_e.Data);
                            });

                            validator.ProcessStarted += new EventHandler((_s, _e) =>
                            {
                                validator.SendLineAndWaitForData("uci", "uciok", 2500, 5000);
                                validator.SendLine("quit");
                            });

                            AddControlText("Validating engine");

                            //Start and verify engine
                            validator.Start(Env.EnginePath, null, null);
                            validator.WaitForExit(5000, false);
                            validator.Kill();

                            //Check engine output
                            Env.EngineValidated = engineOutput.Exists(_x => _x.ToLower().Contains(Constants.EngineName.ToLower()));

                            AddControlText(Env.EngineValidated ? "Engine is valid" : "Engine is not valid");

                            if (!Env.EngineValidated)
                            {
                                MessageBox.Show(string.Format("Please select {0} engine", Constants.EngineName), "Wrong engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }

                    #endregion
                }
                finally
                {
                    UpdateUI(this);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
