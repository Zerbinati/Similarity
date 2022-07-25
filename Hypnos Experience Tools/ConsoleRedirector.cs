using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Hypnos.Experience.Tools
{
	public class ConsoleRedirector : IDisposable
	{
		#region Private variables

		private string _executablePath;
		private string _arguments;
		private Process _process = null;

		private DateTime _lastOutputReceived = DateTime.MinValue;

        private string _waitForData = null;
        private ManualResetEventSlim _waitForEvent = new ManualResetEventSlim(false);

		private object _sendingInputLock = new object();

		#endregion

		#region Private methods

		private void _OnProcessStarted()
		{
			if(ProcessStarted != null)
				ProcessStarted(this, EventArgs.Empty);
		}

		void _OnProcessExited()
		{
			if (ProcessExited != null)
				ProcessExited(this, EventArgs.Empty);
		}

		private void _OnOutputReceived(object sender, DataReceivedEventArgs e)
		{
			//Mark the date and time
			_lastOutputReceived = DateTime.Now;

			//Check if we were waiting for this output and falg it
			if (_waitForData != null && e.Data != null && _waitForData == e.Data)
				_waitForEvent.Set();

			if(ProcessOutputReceived != null)
			{
				ProcessOutputReceived(this, e);
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			Kill();
		}

		#endregion

		#region Events

		public event EventHandler ProcessStarted;
		public event EventHandler ProcessExited;
		public event DataReceivedEventHandler ProcessOutputReceived;

		#endregion

		#region Public properties

		public object Tag { get; set; }

		public string ExecutablePath
		{
			get
			{
				return _executablePath;
			}
		}

		public string Arguments
		{
			get
			{
				return _arguments;
			}
		}

		public bool IsRunning
		{
			get
			{
				Process p = _process;
				if (p == null)
					return false;

				if (p.HasExited)
					return false;

				if (p.WaitForExit(0))
					return false;

				return true;
			}
		}

		#endregion

		#region Public methods

		public virtual void Start(string executablePath, string arguments, ProcessPriorityClass? processPriority)
		{
			if (_process != null)
				throw new Exception("Cannot start a new redirected process when one is already started");

			_executablePath = Path.GetFullPath(executablePath);
			_arguments = arguments;

			Debug.WriteLine("** Starting \"{0}\" with arguments: {1}", _executablePath, string.IsNullOrEmpty(_arguments) ? "<N/A>" : _arguments);

			_process = new Process();
            _process.StartInfo.UseShellExecute = false;
			_process.StartInfo.CreateNoWindow = true;
			_process.StartInfo.WorkingDirectory = Path.GetDirectoryName(executablePath);
			_process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.RedirectStandardInput = true;
			_process.OutputDataReceived += _OnOutputReceived;
			_process.ErrorDataReceived += _OnOutputReceived;

			_process.StartInfo.FileName = _executablePath;
			_process.StartInfo.Arguments = _arguments;

			bool? processStarted = null;

			//Thread to monitor the process
			new Thread((_p) =>
			{
				Process p = _p as Process;

				//Set this thread as background thread
				Thread.CurrentThread.IsBackground = true;

				Debug.WriteLine("** Waiting for process to start -> Exe: \"{0}\" with arguments: {1}", _executablePath, string.IsNullOrEmpty(_arguments) ? "<N/A>" : _arguments);

				//Wait for the thread to start
				while (!processStarted.HasValue)
					Thread.Sleep(0);

				if (!processStarted.Value)
				{
					Debug.WriteLine("** Process failed to start -> Exe: \"{0}\" with arguments: {1}", _executablePath, string.IsNullOrEmpty(_arguments) ? "<N/A>" : _arguments);
					return;
				}

				//Trigger the process started event
				_OnProcessStarted();
				Debug.WriteLine("** Process started -> Exe: \"{0}\" with arguments: {1}", _executablePath, string.IsNullOrEmpty(_arguments) ? "<N/A>" : _arguments);

				//Wait for the process to finish
				Debug.WriteLine("** Waiting for process to finish -> Exe: \"{0}\" with arguments: {1}", _executablePath, string.IsNullOrEmpty(_arguments) ? "<N/A>" : _arguments);
				p.WaitForExit(Timeout.Infinite);

				//Trigger the process existed event
				_OnProcessExited();
				Debug.WriteLine("** Process finished -> Exe: \"{0}\" with arguments: {1}", _executablePath, string.IsNullOrEmpty(_arguments) ? "<N/A>" : _arguments);
			}).Start(_process);

			processStarted = _process.Start();

			if(processPriority.HasValue)
				_process.PriorityClass = processPriority.Value;

            _process.BeginOutputReadLine();
			_process.BeginErrorReadLine();
		}

		public virtual void Kill()
		{
			if (!IsRunning)
				return;

			try
			{
				Process p = _process;

				//Stop Output and Error async redirection, also try to close the process
				try
				{
					p.CancelOutputRead();
					p.CancelErrorRead();
					p.CloseMainWindow();
				}
				catch { }

				//Return if the process has stopped already
				if (!IsRunning)
					return;

				//Wait for the process to close
				try
				{
					p.WaitForExit(500);
				}
				catch { }

				//Return if the process has stopped already
				if (!IsRunning)
					return;

				//Kill if the process is still running
				try
				{
					p.Kill();
				}
				catch { }
			}
			finally
			{
				_process = null;
			}
		}

		public virtual bool WaitForExit(int timeoutMS, bool forceExit)
		{
			if (!IsRunning)
				return true;

			Process p = _process;

			Stopwatch sw = Stopwatch.StartNew();
			int waitStep = timeoutMS > 500 ? 500 : timeoutMS;
			while (true)
			{
				//Wait some time
				int thisWaitStep = timeoutMS - sw.ElapsedMilliseconds > waitStep ? waitStep : Math.Max(0, (int)(timeoutMS - sw.ElapsedMilliseconds));
				p.WaitForExit(thisWaitStep);
				
				//Check if process has exited
				if (!IsRunning)
					return true;

				//Can we wait more?
				if (timeoutMS != Timeout.Infinite && sw.ElapsedMilliseconds > timeoutMS)
					break;
			}

			if (IsRunning && forceExit)
			{
				Kill();
				return true;
			}

			return p.HasExited;
        }

		public virtual void SendLine(string input)
		{
			Process p = _process;
			if (p == null || p.HasExited)
				return;

			lock (_sendingInputLock)
				p.StandardInput.WriteLine(input);
		}

        public bool SendLineAndWaitForData(string line, string data, int timeoutMsSinceLastOutput, int totalTimeoutMs)
        {
            if (_waitForData != null)
                throw new Exception("Cannot wait for data twice");

            bool dataReceived = false;
            Thread t = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                _waitForEvent.Reset();
                _waitForData = data;

                SendLine(line);

				DateTime beginDateTime = DateTime.Now;

				do
				{
					DateTime beginThisIteration = DateTime.Now;
					dataReceived = _waitForEvent.Wait(timeoutMsSinceLastOutput);

					if (dataReceived || !IsRunning)
						break;

					if ((beginThisIteration - _lastOutputReceived).TotalMilliseconds > timeoutMsSinceLastOutput)
						break;

					if ((DateTime.Now - beginDateTime).TotalMilliseconds > totalTimeoutMs)
						break;

				} while (true);
            });

            t.Start();
            t.Join();

            return dataReceived;
        }

		#endregion

		#region Constructor

		public ConsoleRedirector()
		{
        }

		#endregion
	}
}
