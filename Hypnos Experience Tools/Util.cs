using System;
using System.Diagnostics;
using System.IO;

namespace Hypnos.Experience.Tools
{
	public delegate bool ProcessorCallback(string log);

	public static class Util
	{
		#region Private variables

		private static readonly string[] _byteSizes = new string[] { "B", "KB", "MB", "GB", "TB" };

		#endregion

		public static string FormatBytes(long size)
		{
			double s = size;
			int order = 0;
			while (s >= 1024.0 && order < _byteSizes.Length - 1)
			{
				order++;
				s /= 1024.0;
			}

			return string.Format("{0:0.00} {1}", s, _byteSizes[order]);
		}

		public static Process RunConsolePocess(string executablePath, string arguments, ProcessorCallback processorCallback)
		{
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.RedirectStandardOutput = (processorCallback != null);
			p.StartInfo.RedirectStandardError = (processorCallback != null);
			p.StartInfo.RedirectStandardInput = false;

			p.StartInfo.FileName = executablePath;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.WorkingDirectory = Path.GetDirectoryName(executablePath);

			if (processorCallback != null)
			{
				DataReceivedEventHandler dataReceivedEventHandler = new DataReceivedEventHandler((s, e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
						processorCallback.Invoke(e.Data);
				});

				p.OutputDataReceived += dataReceivedEventHandler;
				p.ErrorDataReceived += dataReceivedEventHandler;
			}

			p.Start();

			if (processorCallback != null)
			{
				p.BeginOutputReadLine();
				p.BeginErrorReadLine();
			}

			return p;
		}

		public static bool RemoveDirectory(string path)
		{
			if (!Directory.Exists(path))
				return true;

			string comSpec = Environment.ExpandEnvironmentVariables("%ComSpec%");
			string commandLine = string.Format("/C RMDIR /S /Q \"{0}\"", path);

			try
			{
				Process p = RunConsolePocess(comSpec, commandLine, null);
				p.WaitForExit();
			}
			catch
			{
				//Do nothing
			}

			return !Directory.Exists(path);
		}
	}
}
