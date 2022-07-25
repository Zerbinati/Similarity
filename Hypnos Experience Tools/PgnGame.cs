using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Hypnos.Experience.Tools
{
	public class PgnGame
	{
		#region Static stuff

		static readonly string StartPosFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

		#endregion


		#region Static/Constant variables

		static string _movesRegexPattern = @"(?<MoveNumber>\d+)(?<MoveColorIndicator>\.|\.{3}) (?<Move>.+?) { (?<Comments>.*?) }";
		static Regex _movesRegex = new Regex(_movesRegexPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _dateTagPattern = @"\[Date ""(?<Date>.+?)""\]";
		static Regex _dateTagRegex = new Regex(_dateTagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _whiteEloTagPattern = @"\[WhiteElo ""(?<Elo>.+?)""\]";
		static string _blackEloTagPattern = @"\[BlackElo ""(?<Elo>.+?)""\]";
		static Regex _whiteEloTagRegex = new Regex(_whiteEloTagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex _blackEloTagRegex = new Regex(_blackEloTagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _whiteEngineTagPattern = @"\[WhiteEngine ""(?<Name>.+?)""\]";
		static string _blackEngineTagPattern = @"\[BlackEngine ""(?<Name>.+?)""\]";
		static string _whiteTagPattern = @"\[White ""(?<Name>.+?)""\]";
		static string _blackTagPattern = @"\[Black ""(?<Name>.+?)""\]";
		static Regex _whiteEngineTagRegex = new Regex(_whiteEngineTagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex _blackEngineTagRegex = new Regex(_blackEngineTagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex _whiteTagRegex = new Regex(_whiteTagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex _blackTagRegex = new Regex(_blackTagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        static string _ecoCodePattern = @"\[ECO ""(?<Eco>.+?)""\]";
        static Regex _ecoCodeRegex = new Regex(_ecoCodePattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _fenPattern = @"\[FEN ""(?<FEN>.+?)""\]";
		static Regex _fenRegex = new Regex(_fenPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _gameResultPattern = @"\[Result ""(?<Result>.+?)""\]";
		static Regex _gameResultRegex = new Regex(_gameResultPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _siteTagPattern = @"\[Site ""(?<Site>.+?)""\]";
		static Regex _siteTagRegex = new Regex(_siteTagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _pgnCleanup1SearchPattern = @"\s\$\d*?\s";
		static string _pgnCleanup1ReplacePattern = @" ";
		static Regex _pgnCleanup1Regex = new Regex(_pgnCleanup1SearchPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _pgnCleanup2SearchPattern = @"\(.*?\)";
		static string _pgnCleanup2ReplacePattern = string.Empty;
		static Regex _pgnCleanup2Regex = new Regex(_pgnCleanup2SearchPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
		List<Tuple<Regex, string>> _pgnCleanupRegexList = new List<Tuple<Regex, string>>()
		{
			new Tuple<Regex, string>(_pgnCleanup1Regex, _pgnCleanup1ReplacePattern),
			new Tuple<Regex, string>(_pgnCleanup2Regex, _pgnCleanup2ReplacePattern),
		};

		#endregion


		#region Private variables

		private List<string> _tags = new List<string>();
		private List<string> _comments = new List<string>();
		private string _originalMovesLine = null;
		private string _cleanMovesLine = null;

		List<PgnMovePair> _movePairs = null;
		private GameResult? _result = null;
		bool? _isCheckmate = null;
		private bool _isComplete = false;

		private DateTime? _date = null;
		private int? _whiteElo = null;
		private int? _blackElo = null;
		private string _whiteEngine = null;
		private string _blackEngine = null;
        private string _ecoCode = null;
		private string _startingFEN = null;
		private string _resultTagValue = null;
		private string _siteTagValue = null;

		#endregion


		#region Private static methods

		private static DateTime ParseDateTag(List<string> metaData, Regex dateTagRegexExpression)
		{
			DateTime date = DateTime.MinValue;
			foreach (string tag in metaData)
			{
				if (string.IsNullOrEmpty(tag))
					continue;

				Match m = dateTagRegexExpression.Match(tag);
				if (!m.Success)
					continue;

				if (!string.IsNullOrEmpty(m.Groups["Date"].Value))
				{
					//Step 1: Parse as DateTime
					try
					{
						date = DateTime.Parse(m.Groups["Date"].Value);
					}
					catch
					{
						//Assume it is an invalid date!
					}

					//Parse as integer
					if (date == DateTime.MinValue)
					{
						try
						{
							int tempTear = int.Parse(m.Groups["Date"].Value);
							date = new DateTime(tempTear, 1, 1);
						}
						catch
						{
							//Assume it is an invalid year!
						}
					}
				}

			}

			return date;
		}

		private static int ParseEloTag(List<string> metaData, Regex eloTagRegexExpression)
		{
			int elo = int.MinValue;
			foreach (string tag in metaData)
			{
				if (string.IsNullOrEmpty(tag))
					continue;

				Match m = eloTagRegexExpression.Match(tag);
				if (!m.Success)
					continue;

				if (!string.IsNullOrEmpty(m.Groups["Elo"].Value))
				{
					try
					{
						elo = int.Parse(m.Groups["Elo"].Value);
					}
					catch
					{
						//Assume it is an invalid elo!
					}
				}
			}

			return elo;
		}

		private static string ParseEngine(List<string> metaData, Regex engineTagRegex, Regex playerColorTagRegex)
		{
			string engineName = null;

			foreach (Regex regex in new Regex[2] { engineTagRegex, playerColorTagRegex })
			{
				foreach (string tag in metaData)
				{
					if (string.IsNullOrEmpty(tag))
						continue;

					Match m = regex.Match(tag);
					if (!m.Success)
						continue;

					if (!string.IsNullOrEmpty(m.Groups["Name"].Value))
					{
						engineName = m.Groups["Name"].Value;
						break;
					}
				}

				if (engineName != null)
					break;
			}

			if (!string.IsNullOrEmpty(engineName) && engineName.Contains(","))
			{
				string[] tokens = engineName.Split(new char[] { ',' }, 2, StringSplitOptions.None);
				engineName = tokens[1];
			}

			if (engineName != null)
				engineName = engineName.Trim();
			else
				engineName = string.Empty;

			return engineName;
		}

        private static string ParseEcoCode(List<string> metaData, Regex ecoCodeTagRegex)
        {
            string resultTag = null;
            foreach (string tag in metaData)
            {
                if (string.IsNullOrEmpty(tag))
                    continue;

                Match m = ecoCodeTagRegex.Match(tag);
                if (!m.Success)
                    continue;

                if (!string.IsNullOrEmpty(m.Groups["Eco"].Value))
                {
                    resultTag = m.Groups["Eco"].Value;
                    break;
                }
            }

            if (resultTag != null)
                resultTag = resultTag.Trim();
            else
                resultTag = string.Empty;

            return resultTag;
        }

		private static string ExtractStartingFEN(List<string> metaData, Regex fenTagRegex)
		{
			string resultTag = null;
			foreach (string tag in metaData)
			{
				if (string.IsNullOrEmpty(tag))
					continue;

				Match m = fenTagRegex.Match(tag);
				if (!m.Success)
					continue;

				if (!string.IsNullOrEmpty(m.Groups["FEN"].Value))
				{
					resultTag = m.Groups["FEN"].Value;
					break;
				}
			}

			if (resultTag != null)
				resultTag = resultTag.Trim();
			else
				resultTag = string.Empty;

			return resultTag;
		}

		private static string ParseResult(List<string> metaData, Regex gameResultRegex)
		{
			string resultTag = null;
			foreach (string tag in metaData)
			{
				if (string.IsNullOrEmpty(tag))
					continue;

				Match m = gameResultRegex.Match(tag);
				if (!m.Success)
					continue;

				if (!string.IsNullOrEmpty(m.Groups["Result"].Value))
				{
					resultTag = m.Groups["Result"].Value;
					break;
				}
			}

			if (resultTag != null)
				resultTag = resultTag.Trim();
			else
				resultTag = string.Empty;

			return resultTag;
		}

		private static string ParseSiteTag(List<string> metaData, Regex siteTagRegex)
		{
			string resultTag = null;
			foreach (string tag in metaData)
			{
				if (string.IsNullOrEmpty(tag))
					continue;

				Match m = siteTagRegex.Match(tag);
				if (!m.Success)
					continue;

				if (!string.IsNullOrEmpty(m.Groups["Site"].Value))
				{
					resultTag = m.Groups["Site"].Value;
					break;
				}
			}

			if (resultTag != null)
				resultTag = resultTag.Trim();
			else
				resultTag = string.Empty;

			return resultTag;
		}		

		private static string FindMove(string text, out string move, out string comments)
		{
			int moveStartIndex = 0;
			int moveEndIndex = -1;
			int commentsStartIndex = -1;
			int commentsEndIndex = -1;

			//Find move end index
			int index = moveStartIndex + 1;
			while (index <= text.Length - 1)
			{
				char ch = text[index];
				if (index == text.Length - 1)
				{
					moveEndIndex = index;
					break;
				}
				else if (ch == ' ')
				{
					moveEndIndex = index - 1;
					break;
				}
				else if (ch == '{')
				{
					moveEndIndex = index - 1;
					commentsStartIndex = index;
					break;
				}

				index++;
			}

			//Find comments
			if (commentsStartIndex != -1)
			{
				index = commentsStartIndex + 1;
				while (index <= text.Length - 1)
				{
					char ch = text[index];
					if (ch == '}')
					{
						commentsEndIndex = index;
						break;
					}

					index++;
				}

				if (commentsEndIndex == -1)
					throw new Exception(string.Format("Could not parse move: {0}", text));
			}

			move = text.Substring(moveStartIndex, moveEndIndex - moveStartIndex + 1);
			comments = commentsStartIndex != -1 && commentsEndIndex != -1 ? text.Substring(commentsStartIndex + 1, commentsEndIndex - commentsStartIndex - 1) : null;
			int lastIndex = (commentsEndIndex == -1 ? moveEndIndex + 1 : commentsEndIndex);

			return lastIndex >= text.Length - 1 ? string.Empty : text.Substring(lastIndex + 1, text.Length - 1 - lastIndex);
		}

		private static void ParseMovesPair(int moveNumber, string movePairText, out PgnMove whiteMove, out PgnMove blackMove)
		{
			string whiteMoveStartMark = string.Format("{0}. ", moveNumber);
			string blackMoveStartMark = string.Format("{0}... ", moveNumber);
			if (!movePairText.StartsWith(whiteMoveStartMark))
				throw new Exception(string.Format("Missing move start mark [{0}] from move text {1}", whiteMoveStartMark, movePairText));

			//Prepare for parsing
			string movePairTextCopy = movePairText;
			movePairTextCopy = movePairTextCopy.Replace(whiteMoveStartMark, string.Empty);
			movePairTextCopy = movePairTextCopy.Replace(blackMoveStartMark, string.Empty);
			while (movePairTextCopy.Contains("  ")) movePairTextCopy = movePairTextCopy.Replace("  ", " ");
			while (movePairTextCopy.Contains("{ ")) movePairTextCopy = movePairTextCopy.Replace("{ ", "{");
			while (movePairTextCopy.Contains(" {")) movePairTextCopy = movePairTextCopy.Replace(" {", "{");
			while (movePairTextCopy.Contains(" }")) movePairTextCopy = movePairTextCopy.Replace(" }", "}");
			while (movePairTextCopy.Contains("} ")) movePairTextCopy = movePairTextCopy.Replace("} ", "}");
			movePairTextCopy = movePairTextCopy.Replace("}{", "*** ");

			string wMove;
			string wComments;
			string bMove;
			string bComments;

			string remaining = FindMove(movePairTextCopy, out wMove, out wComments);
			remaining = FindMove(remaining, out bMove, out bComments);

			if (!string.IsNullOrEmpty(remaining))
				throw new Exception(string.Format("Could not parse move pair: {0}", movePairText));

			whiteMove = new PgnMove(moveNumber * 2 - 1, wMove, wComments);

			if (!string.IsNullOrEmpty(bMove))
				blackMove = new PgnMove(moveNumber * 2, bMove, bComments);
			else
				blackMove = null;
		}

		private static GameResult? DetectResult(string resultString)
		{
			if (string.IsNullOrWhiteSpace(resultString))
				return null;

			if (resultString == "1-0") return GameResult.WhiteWin;
			if (resultString == "0-1") return GameResult.BlackWin;
			if (resultString == "1/2-1/2") return GameResult.Draw;
			if (resultString.Contains("1-0") && !resultString.Contains("0-1") && !resultString.Contains("1/2-1/2")) return GameResult.WhiteWin;
			if (!resultString.Contains("1-0") && resultString.Contains("0-1") && !resultString.Contains("1/2-1/2")) return GameResult.BlackWin;
			if (!resultString.Contains("1-0") && !resultString.Contains("0-1") && resultString.Contains("1/2-1/2")) return GameResult.Draw;

			return null;
		}

		#endregion


		#region Private methods

		private void ParseMoves()
		{
			if (!_isComplete)
				throw new Exception("Should not call ParseMoves() if the game is not complete!");

			_movePairs = new List<PgnMovePair>();

			//Further cleaning of moves line
			foreach (Tuple<Regex, string> pgnCleanupRegexPair in _pgnCleanupRegexList)
				_cleanMovesLine = pgnCleanupRegexPair.Item1.Replace(_cleanMovesLine, pgnCleanupRegexPair.Item2);

			//Parse moves
			int currentMoveNumber = 0;
			int lastIndex = 0;
			while (lastIndex < _cleanMovesLine.Length)
			{
				#region Find next move text

				currentMoveNumber++;
				string moveStartMark = string.Format(" {0}. ", currentMoveNumber + 1);
				int nextMoveStart = _cleanMovesLine.IndexOf(moveStartMark, lastIndex);
				while (true)
				{
					if (nextMoveStart == -1)
					{
						nextMoveStart = _cleanMovesLine.Length;
						break;
					}

					int nextCommentStart = _cleanMovesLine.IndexOf("{", lastIndex);
					if (nextCommentStart == -1 || nextMoveStart < nextCommentStart)
						break;

					int nextCommentEnd = _cleanMovesLine.IndexOf("}", nextCommentStart + 1);
					if (nextCommentEnd < nextMoveStart)
						break;

					nextMoveStart = _cleanMovesLine.IndexOf(moveStartMark, nextCommentEnd + 1);
				}

				string movePairText = _cleanMovesLine.Substring(lastIndex, nextMoveStart - lastIndex);

				#endregion

				PgnMove whiteMove;
				PgnMove blackMove;
				ParseMovesPair(currentMoveNumber, movePairText, out whiteMove, out blackMove);

				PgnMovePair movePair = new PgnMovePair(currentMoveNumber, whiteMove, blackMove);

				_movePairs.Add(movePair);

				//Proceed to next move
				//Update last index plus one because 'move mark' has a leading space
				lastIndex = nextMoveStart + 1;
			}

			//IsCheckmate
			PgnMovePair lastMovePair = _movePairs[_movePairs.Count - 1];
			_isCheckmate = (lastMovePair.BlackMove == null ? lastMovePair.WhiteMove : lastMovePair.BlackMove).Move.EndsWith("#");
		}

		#endregion


		#region Public properties

		public List<PgnMovePair> MovePairs
		{
			get
			{
				return _movePairs;
			}
		}

		public bool IsComplete
		{
			get
			{
				return _isComplete;
			}
		}

		//Accessing this property if the game is not "Complete" may throw an exception
		public GameResult Result
		{
			get
			{
				return _result.Value;
			}
		}

		//Accessing this property if the game is not "Complete" may throw an exception
		public bool IsCheckmate
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve IsCheckmate of an incomplete game!");

				return _isCheckmate.Value;
			}
		}

		public DateTime Date
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve date of an incomplete game!");

				if (!_date.HasValue)
					_date = ParseDateTag(_tags, _dateTagRegex);					

				return _date.Value;
			}
		}

		public int WhiteElo
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve WhiteElo of an incomplete game!");

				if (!_whiteElo.HasValue)
					_whiteElo = ParseEloTag(_tags, _whiteEloTagRegex);

				return _whiteElo.Value;
			}
		}

		public int BlackElo
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve BlackElo of an incomplete game!");

				if (!_blackElo.HasValue)
					_blackElo = ParseEloTag(_tags, _blackEloTagRegex);

				return _blackElo.Value;
			}
		}

		public string WhiteEngine
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve WhiteEngine of an incomplete game!");

				if (_whiteEngine == null)
					_whiteEngine = ParseEngine(_tags, _whiteEngineTagRegex, _whiteTagRegex);

				return _whiteEngine;
			}
		}

		public string BlackEngine
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve BlackEngine of an incomplete game!");

				if (_blackEngine == null)
					_blackEngine = ParseEngine(_tags, _blackEngineTagRegex, _blackTagRegex);

				return _blackEngine;
			}
		}

        public string EcoCode
        {
            get
            {
                if (!_isComplete)
                    throw new Exception("Cannot retireve EcoCode of an incomplete game!");

                if (_ecoCode == null)
                    _ecoCode = ParseEcoCode(_tags, _ecoCodeRegex);

                return _ecoCode;
            }
        }

		public string StartingFEN
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve Starting FEN string of an incomplete game!");

				if (_startingFEN == null)
					_startingFEN = ExtractStartingFEN(_tags, _fenRegex);

				return _startingFEN;
			}
		}

		public string ResultTag
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve ResultTag of an incomplete game!");

				if (_resultTagValue == null)
					_resultTagValue = ParseResult(_tags, _gameResultRegex);

				return _resultTagValue;
			}
		}

		public string SiteTag
		{
			get
			{
				if (!_isComplete)
					throw new Exception("Cannot retireve SiteTag of an incomplete game!");

				if (_siteTagValue == null)
					_siteTagValue = ParseSiteTag(_tags, _siteTagRegex);

				return _siteTagValue;
			}
		}

		public int Ply
		{
			get
			{
				PgnMovePair lastMovePair = _movePairs[_movePairs.Count - 1];
				return (lastMovePair.BlackMove == null ? lastMovePair.WhiteMove : lastMovePair.BlackMove).Ply;
			}
		}

		#endregion


		#region Public methods

		public void Consume(string line)
		{
			if (_isComplete)
				throw new Exception("Cannot consume more data when PGN game is already complete!");

			bool validLine = !string.IsNullOrEmpty(line);
			bool isTag = validLine && line.StartsWith("[");
			bool isComment = validLine && !isTag && line.StartsWith("{");
			bool isMoves = validLine && !isTag && !isComment && line.StartsWith("1. ");

			if (isTag)
			{
				_tags.Add(line);
			}
			else if (isComment)
			{
				_comments.Add(line);
			}
			else if (isMoves)
			{
				_originalMovesLine = line;
				_isComplete = true;
			}
			else
			{
				throw new Exception(string.Format("Unrecognized input line: {0}", line));
			}

			if (!_isComplete)
				return;

			//Find game result
			int lastSpace = line.LastIndexOf(' ');
			if (lastSpace == -1 || lastSpace == 0 || lastSpace == line.Length)
				throw new Exception(string.Format("Could not find game result in moves line: {0}", line));

			//Detect game result (reject if conflicting game-result with result tag)
			_resultTagValue = ParseResult(_tags, _gameResultRegex);
			string movesGameResult = line.Substring(lastSpace + 1).Trim();

			GameResult? result1 = DetectResult(_resultTagValue);
			GameResult? result2 = DetectResult(movesGameResult);

			if (!result1.HasValue || !result2.HasValue || result1.Value != result2.Value)
			{
				throw new Exception(
					string.Format(
						"Inconsistant result. Tag: {0}, Moves : {1}",
						string.IsNullOrEmpty(_resultTagValue) ? "N/A" : _resultTagValue,
						string.IsNullOrEmpty(movesGameResult) ? "N/A" : movesGameResult));
			}

			_result = result1.Value;

			//Remove game result from end of line
			_cleanMovesLine = line.Substring(0, lastSpace);
			ParseMoves();
		}

		public int? GetCompactPgn(StringBuilder cpgn, out string rejectReason)
		{
			if (_result != GameResult.WhiteWin && _result != GameResult.BlackWin && _result != GameResult.Draw)
			{
				rejectReason = string.Format("Result {0} is not supported/unknown", _result);
				return null;
			}

			if (_movePairs.Count < 20)
			{
				rejectReason = string.Format("Game contains only {0} moves", _movePairs.Count);
				return null;
			}

			//FishNet games have evaluations from player POV
			bool? KnownBlackPov = null;
			if (!string.IsNullOrEmpty(SiteTag))
			{
				if (SiteTag.Contains("tests.stockfishchess.org"))
					KnownBlackPov = true;
				else if (SiteTag.Contains("Engine Room") || SiteTag.Contains("Machines A") || SiteTag.Contains("Machines B") || SiteTag.Contains("Riom Engine") || SiteTag.Contains("Maschinenraum"))
					KnownBlackPov = false;
			}

			if (!KnownBlackPov.HasValue && _originalMovesLine.Contains(" Arena Adjudication "))
			{
				KnownBlackPov = true;
			}

			if (!KnownBlackPov.HasValue && _tags.Exists(_x => _x.Contains("Infinity Chess") || _x.Contains("playchess")))
			{
				KnownBlackPov = false;
			}

			//Function for detecting suspecious evaluations
			Func<PgnMove, double> SuspeciousMoveScore = delegate (PgnMove mv)
			{
				double suspecious = 0;

				int mn = mv.Ply / 2;
				double ev = Math.Abs(mv.Evaluation.Value);

				if (mn <= 8 && ev >= 2)
					suspecious += ev / 2;
				else if (mn <= 16 && ev >= 4)
					suspecious += ev / 4;

				if (mn <= 8 && mv.Depth.Value >= 50)
					suspecious += (mv.Depth.Value - 40) / 10.0;

				return suspecious;
			};

			//Function for detecting suspecious evaluations
			Func<PgnMovePair, double> SuspeciousMovePairScore = delegate (PgnMovePair mp)
			{
				double suspecious = 0;

				if (mp.WhiteMove.Evaluation.HasValue)
					suspecious += SuspeciousMoveScore(mp.WhiteMove);

				if (mp.BlackMove != null && mp.BlackMove.Evaluation.HasValue)
					suspecious += SuspeciousMoveScore(mp.BlackMove);

				if(mp.WhiteMove.Evaluation.HasValue && mp.BlackMove != null && mp.BlackMove.Evaluation.HasValue)
				{
					double w = Math.Max(Math.Min(mp.WhiteMove.Evaluation.Value, 8), -8);
					double b = Math.Max(Math.Min(mp.BlackMove.Evaluation.Value, 8), -8);

					if(Math.Abs(w) > 0.5 && Math.Abs(b) > 0.5)
						suspecious += Math.Abs(w - b) / 4;
				}

				return suspecious;
			};

			#region Detect Detect Black POV and Verify game result

			const int White = 0;
			const int Black = 1;

			double[] DrawEval = new double[] { 0.5f, 0.35f };
			double[,] WinEval = new double[,] { { 2.5f, 1.5f }, { 2.0f, 2.0f } };

			int[] evalCount = new int[] { 0, 0 };
			int[,] winWeight = new int[,] { { 0, 0 }, { 0, 0 } };
			int[] drawWeight = new int[] { 0, 0 };
			int[] blackPov = new int[] { 0, 0 };
			double suspeciousEvals = 0.0;

			for (int i = 0; i < _movePairs.Count; ++i)
			{
				PgnMovePair movePair = _movePairs[i];
				if (movePair.WhiteMove.Evaluation.HasValue)
				{
					//Eval count
					++evalCount[White];

					//Weights
					if (movePair.WhiteMove.Evaluation.Value >= WinEval[White, White])
					{
						winWeight[White, White] += movePair.MoveNumber;
						drawWeight[White] = Math.Max(drawWeight[White] - movePair.MoveNumber / 2, 0);
					}
					else if (movePair.WhiteMove.Evaluation.Value <= -WinEval[White, Black])
					{
						winWeight[White, Black] += movePair.MoveNumber;
						drawWeight[White] = Math.Max(drawWeight[White] - movePair.MoveNumber / 2, 0);
					}
					else if (Math.Abs(movePair.WhiteMove.Evaluation.Value) <= DrawEval[White])
					{
						drawWeight[White] += movePair.MoveNumber;
						winWeight[White, White] = Math.Max(winWeight[White, White] - movePair.MoveNumber / 2, 0);
						winWeight[White, Black] = Math.Max(winWeight[White, Black] - movePair.MoveNumber / 2, 0);
					}
				}

				if (movePair.BlackMove != null && movePair.BlackMove.Evaluation.HasValue)
				{
					//Eval count
					++evalCount[Black];

					//Weights
					if (movePair.BlackMove.Evaluation.Value >= WinEval[Black, White])
					{
						winWeight[Black, White] += movePair.MoveNumber;
						drawWeight[Black] = Math.Max(drawWeight[Black] - movePair.MoveNumber / 2, 0);
					}
					else if (movePair.BlackMove.Evaluation.Value <= -WinEval[Black, Black])
					{
						winWeight[Black, Black] += movePair.MoveNumber;
						drawWeight[Black] = Math.Max(drawWeight[Black] - movePair.MoveNumber / 2, 0);
					}
					else if (Math.Abs(movePair.BlackMove.Evaluation.Value) <= DrawEval[Black])
					{
						drawWeight[Black] += movePair.MoveNumber;
						winWeight[Black, White] = Math.Max(winWeight[Black, White] - movePair.MoveNumber / 2, 0);
						winWeight[Black, Black] = Math.Max(winWeight[Black, Black] - movePair.MoveNumber / 2, 0);
					}
				}

				//Suspecious evals
				suspeciousEvals += SuspeciousMovePairScore(movePair);
				if (suspeciousEvals > 12.0)
				{
					rejectReason = string.Format("Game rejected due to excessive suspeciouis score of: {0}", suspeciousEvals);
					return null;
				}

				//Black POV
				if (!KnownBlackPov.HasValue)
				{
					if (movePair.WhiteMove.Evaluation.HasValue && movePair.BlackMove != null && movePair.BlackMove.Evaluation.HasValue)
					{
						const double minScoreForPovDetection = 0.1;
						const double goodScoreForPovDetection = 2 * minScoreForPovDetection;

						if (Math.Abs(movePair.WhiteMove.Evaluation.Value) > minScoreForPovDetection && Math.Abs(movePair.BlackMove.Evaluation.Value) > minScoreForPovDetection)
						{
							if ((movePair.WhiteMove.Evaluation.Value > 0) == (movePair.BlackMove.Evaluation.Value > 0))
								blackPov[White] += 1 + (Math.Abs(movePair.WhiteMove.Evaluation.Value) >= goodScoreForPovDetection && Math.Abs(movePair.BlackMove.Evaluation.Value) >= goodScoreForPovDetection ? 1 : 0);
							else if ((movePair.WhiteMove.Evaluation.Value < 0) == (movePair.BlackMove.Evaluation.Value < 0))
								blackPov[Black] += 1 + (Math.Abs(movePair.WhiteMove.Evaluation.Value) >= goodScoreForPovDetection && Math.Abs(movePair.BlackMove.Evaluation.Value) >= goodScoreForPovDetection ? 1 : 0);
						}
						else if (movePair.BlackMove.Evaluation.Value > minScoreForPovDetection && _result == GameResult.WhiteWin)
						{
							blackPov[White] += 1 + (Math.Abs(movePair.BlackMove.Evaluation.Value) >= goodScoreForPovDetection ? 1 : 0);
						}
						else if (movePair.BlackMove.Evaluation.Value < -minScoreForPovDetection && _result == GameResult.WhiteWin)
						{
							blackPov[Black] += 1 + (Math.Abs(movePair.BlackMove.Evaluation.Value) >= goodScoreForPovDetection ? 1 : 0);
						}
						else if (movePair.BlackMove.Evaluation.Value < -minScoreForPovDetection && _result == GameResult.BlackWin)
						{
							blackPov[White] += 1 + (Math.Abs(movePair.BlackMove.Evaluation.Value) >= goodScoreForPovDetection ? 1 : 0);
						}
						else if (movePair.BlackMove.Evaluation.Value > minScoreForPovDetection && _result == GameResult.BlackWin)
						{
							blackPov[Black] += 1 + (Math.Abs(movePair.BlackMove.Evaluation.Value) >= goodScoreForPovDetection ? 1 : 0);
						}
					}
				}
			}

			if (evalCount[White] + evalCount[Black] < 20 || evalCount[White] < 8 || evalCount[Black] < 8)
			{
				rejectReason = string.Format("Game has {0} moves and not enought white evals (count: {1}) and black evals (count: {2})", _movePairs.Count, evalCount[White], evalCount[Black]);
				return null;
			}

			bool? blackScoreFromBlackPov = KnownBlackPov;
			if (!blackScoreFromBlackPov.HasValue)
			{
				int povFactor = KnownBlackPov.HasValue ? 2 : 5;
				if (blackPov[White] > povFactor * blackPov[Black])
				{
					blackScoreFromBlackPov = false;
				}
				else if (blackPov[Black] > povFactor * blackPov[White])
				{
					blackScoreFromBlackPov = true;
				}
				else
				{
					rejectReason = string.Format(
						"Black POV detection failed with White-POV: {0}, Black-POV: {1} (KnownBlackPOV={2})",
						blackPov[White],
						blackPov[Black],
						KnownBlackPov.HasValue ? KnownBlackPov.Value ? "Black" : "White" : "N/A");
					return null;
				}
			}

			////////////////////////////////////////////////////////////////////////////////////////////////
			//From this point forward, it is guarenteed that blackScoreFromBlackPov.HasValue is true
			////////////////////////////////////////////////////////////////////////////////////////////////

			//Swap win/draw weights if necessary
			if (blackScoreFromBlackPov.Value)
			{
				int temp = winWeight[Black, White];
				winWeight[Black, White] = winWeight[Black, Black];
				winWeight[Black, Black] = temp;

				temp = drawWeight[Black];
				drawWeight[Black] = drawWeight[White];
				drawWeight[White] = temp;
			}

			//Verify game result
			if (_result == GameResult.WhiteWin && (winWeight[White, White] < 3 * winWeight[White, Black] || winWeight[Black, White] < 3 * winWeight[Black, Black]))
			{
				rejectReason = string.Format(
					"Game result verification failed for WhiteWin game. KnownBlackPOV={0}, DetectedBlackPOV={1}, WinWeight[White,White]={2}, WinWeight[White,Black]={3}, WinWeight[Black,White]={4}, WinWeight[Black,Black]={5}",
					KnownBlackPov.HasValue ? KnownBlackPov.Value ? "Black" : "White" : "N/A",
					blackScoreFromBlackPov.Value,
					winWeight[White, White], winWeight[White, Black],
					winWeight[Black, White], winWeight[Black, Black]);
				return null;
			}
			else if (_result == GameResult.BlackWin && (winWeight[White, Black] < 3 * winWeight[White, White] || winWeight[Black, Black] < 3 * winWeight[Black, White]))
			{
				rejectReason = string.Format(
					"Game result verification failed for BlackWin game. KnownBlackPOV={0}, DetectedBlackPOV={1}, WinWeight[White,White]={2}, WinWeight[White,Black]={3}, WinWeight[Black,White]={4}, WinWeight[Black,Black]={5}",
					KnownBlackPov.HasValue ? KnownBlackPov.Value ? "Black" : "White" : "N/A",
					blackScoreFromBlackPov.Value,
					winWeight[White, White], winWeight[White, Black],
					winWeight[Black, White], winWeight[Black, Black]);
				return null;
			}
			else if(_result == GameResult.Draw)
			{
				bool wDraw  = drawWeight[White] > (winWeight[White, White] + winWeight[White, Black]) / 2;
				bool bDraw  = drawWeight[Black] > (winWeight[Black, White] + winWeight[Black, Black]) / 2;
				bool wbDraw = drawWeight[White] > (winWeight[White, Black] + winWeight[Black, White]) / 2;
				bool bwDraw = drawWeight[Black] > (winWeight[White, Black] + winWeight[Black, White]) / 2;

				if (!wDraw || !bDraw || !wbDraw || !bwDraw)
				{
					rejectReason = string.Format(
						"Game result verification failed for Draw game. KnownBlackPOV={0}, DetectedBlackPOV={1}, DrawWeight=[{2}, {3}], WW=[{4},{5}], BW=[{6},{7}]",
						KnownBlackPov.HasValue ? KnownBlackPov.Value ? "Black" : "White" : "N/A",
						blackScoreFromBlackPov.Value,
						drawWeight[White], drawWeight[Black],
						winWeight[White, White], winWeight[White, Black],
						winWeight[Black, White], winWeight[Black, Black]);
					return null;
				}
			}


			#endregion

			if (blackScoreFromBlackPov.Value)
				blackScoreFromBlackPov = true;


			#region Create Compact PGN

			int movesWithEvals = 0;

			//Start fresh
			cpgn.Clear();

			//Start tag
			cpgn.Append('{');

			//Starting FEN
			cpgn.Append(string.IsNullOrEmpty(StartingFEN) ? StartPosFEN : StartingFEN);

			//Game result
			cpgn.AppendFormat(",{0}", _result.Value == GameResult.WhiteWin ? 'w' : _result.Value == GameResult.BlackWin ? 'b' : 'd');

			//Function to format compact pgn moves
			Func<PgnMove, bool, string> CompactPgnMoveStr = delegate (PgnMove mv, bool reverseSign)
			{
				if (mv.Evaluation.HasValue && mv.Depth.HasValue)
					return string.Format("{0}:{1}:{2}", mv.UciMove, mv.EvaluationStockfish.Value * (reverseSign ? -1 : 1), mv.Depth.Value);
				else if(mv.Evaluation.HasValue)
					return string.Format("{0}:{1}", mv.UciMove, mv.EvaluationStockfish.Value * (reverseSign ? -1 : 1));
				else
					return mv.UciMove;
			};

			//Moves and evals
			foreach (PgnMovePair movePair in _movePairs)
			{
				//White move
				cpgn.AppendFormat(",{0}", CompactPgnMoveStr(movePair.WhiteMove, false));
				if (movePair.WhiteMove.Evaluation.HasValue)
					++movesWithEvals;

				//Black move
				if (movePair.BlackMove != null)
				{
					cpgn.AppendFormat(",{0}", CompactPgnMoveStr(movePair.BlackMove, !blackScoreFromBlackPov.Value));

					if (movePair.BlackMove.Evaluation.HasValue)
						++movesWithEvals;
				}
			}

			#endregion


			//End tag
			if (movesWithEvals > 0)
				cpgn.Append('}');

			rejectReason = null;
			return movesWithEvals > 0 ? movesWithEvals : (int?)null;
		}

		#endregion
	}
}
