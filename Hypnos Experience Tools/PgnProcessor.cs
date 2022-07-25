using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Hypnos.Experience.Tools
{
	public delegate bool PgnProcessorProgressCallback(string timeRemaining, long completedGames, long rejectedGames, double gamesPerSecond, int errors, double completedPercentage);

	public static class PgnProcessor
	{
		#region Public methods

		public static bool PgnToCpgn(string cleanPgnFilename, string cpgnFilename, PgnProcessorProgressCallback progressCallback)
		{			
			DateTime startTime = DateTime.Now;
			TimeSpan timeTaken = TimeSpan.Zero;
			int totalGames = 0;
			int rejectedGames = 0;
			int totalErrors = 0;
			long lines = 0;

			long fileSize = new FileInfo(cleanPgnFilename).Length;
			long approximateConsumedSize = 0;

			bool doCallback(long? elapsedMilliseconds)
			{
				double approximatePercentageDone = elapsedMilliseconds.HasValue ? Math.Min(100.0, (double)approximateConsumedSize / (double)fileSize * 100.0d) : 100.0;

				TimeSpan remaining_time = TimeSpan.Zero;
				if (elapsedMilliseconds.HasValue)
				{
					long expected_total_time_ms = (long)(elapsedMilliseconds.Value * 100 / approximatePercentageDone);
					long expected_remaining_time = Math.Max(0, expected_total_time_ms - elapsedMilliseconds.Value);
					remaining_time = TimeSpan.FromMilliseconds(expected_remaining_time); 
				}

				return progressCallback(
					string.Format("{0:00}:{1:00}:{2:00}", remaining_time.Hours, remaining_time.Minutes, remaining_time.Seconds),
					totalGames,
					rejectedGames,
					totalGames / (DateTime.Now - startTime).TotalSeconds,
					totalErrors,
					approximatePercentageDone);
			};

			using (StreamWriter cpgnStream = new StreamWriter(cpgnFilename, false, Encoding.ASCII))
			{
				Stopwatch sw = Stopwatch.StartNew();
				StringBuilder cpgnSB = new StringBuilder();
				PgnGame game = new PgnGame();

				foreach (string line in File.ReadLines(cleanPgnFilename))
				{
					++lines;
					approximateConsumedSize += 2 + (string.IsNullOrEmpty(line) ? 0 : line.Length);

					if (string.IsNullOrWhiteSpace(line))
						continue;

					try
					{
						game.Consume(line);
						if (!game.IsComplete)
							continue;

						totalGames++;

						if (game.Result == GameResult.WhiteWin || game.Result == GameResult.BlackWin || game.Result == GameResult.Draw)
						{
							string rejectReason;
							int? ply = game.GetCompactPgn(cpgnSB, out rejectReason);

							if (!string.IsNullOrEmpty(rejectReason) || !ply.HasValue)
							{
								++rejectedGames;
								//Debug.WriteLine(rejectReason);
							}
							else
							{
								//if(game.MovePairs.Count > 1 && game.MovePairs[0].WhiteMove.Move == "a2a3" && game.MovePairs[0].WhiteMove.Evaluation.HasValue)
								//	Debug.WriteLine(cpgnSB.ToString());

								cpgnStream.WriteLine(cpgnSB.ToString());
							}
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message);

						totalErrors++;
					}

					//Must start fresh
					game = new PgnGame();

					if (progressCallback != null && ((lines % 1000) == 0 || (totalGames % 100) == 0))
					{
						if (!doCallback(sw.ElapsedMilliseconds))
							return false;
					}
				}
			}

			doCallback(null);

			return true;
		}

		#endregion
	}
}