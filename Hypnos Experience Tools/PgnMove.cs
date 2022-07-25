using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Hypnos.Experience.Tools
{
	[DebuggerDisplay("Move = {Move}, Eval = {Evaluation}, Depth = {Depth}, Comment = {Comments}")]
	public class PgnMove
	{
		#region Static/Constant variables

		static readonly char[] UciTrimMoveChars = new char[] { '+', '#', '\r', '\n' };

		const double VALUE_KNOWN_WIN = 100.0f;
		const double VALUE_MATE = 320.0f;

		static string _evaluationPattern1 = @"(?<Sign>((^[+-])|([\s+-]))?)(?<Letter>[M#]?)(?<EvalPart1>\d{1,})(?<Seperator1>[\.,/])(?<EvalPart2>\d{1,})(?<Seperator2>[,/]?)(?<EvalPart3>\d{0,})";
		static Regex _evaluationRegex1 = new Regex(_evaluationPattern1, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		static string _evaluationPattern2 = @".*?ev=(?<Eval>.*?),.*?d=(?<Depth>.*?),";
		static Regex _evaluationRegex2 = new Regex(_evaluationPattern2, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		#endregion


		#region Public properties

		public string Move;
		public int Ply;
		public string Comments;
		public double? Evaluation;
		public int? Depth;
		public bool IsMateInX;
		public bool IsBookMove;

		public int? EvaluationStockfish
		{
			get
			{
				if (!Evaluation.HasValue)
					return null;

				if (IsMateInX || Evaluation.Value > VALUE_KNOWN_WIN)
					return (int)(Evaluation.Value * 100.0f);
				else
					return (int)(Evaluation.Value * 208.0f);
			}
		}

		public int? MateIn
		{
			get
			{
				if (!IsMateInX)
					return null;

				Debug.Assert(Evaluation.HasValue);

				return (int)(VALUE_MATE - Math.Abs(Evaluation.Value));
			}
		}

		public string UciMove
		{
			get
			{
				return Move.TrimEnd(UciTrimMoveChars);
			}
		}

		#endregion


		#region Private methods

		private void ParseEvaluation(string comments)
		{
			Evaluation = null;
			Depth = null;

			if (string.IsNullOrEmpty(comments))
				return;

			try
			{
				string commentsCopy = comments;

				//Fix special cases of extra space here and there!
				if (commentsCopy.EndsWith(" s")) commentsCopy = commentsCopy.Remove(commentsCopy.Length - 1, 1);
				if (commentsCopy.Contains(" /")) commentsCopy = commentsCopy.Replace(" /", "/");
				if (commentsCopy.Contains("/ ")) commentsCopy = commentsCopy.Replace("/ ", "/");
				if (commentsCopy.Contains("+ ")) commentsCopy = commentsCopy.Replace("+ ", "+");
				if (commentsCopy.Contains("- ")) commentsCopy = commentsCopy.Replace("- ", "-");
				if (commentsCopy.Contains(". ")) commentsCopy = commentsCopy.Replace(". ", ".");
				if (commentsCopy.Contains(", ")) commentsCopy = commentsCopy.Replace(", ", ",");
				if (commentsCopy.Contains(",-")) commentsCopy = commentsCopy.Replace(",-", ","); //Avoid stupid engines reporting negative depth

				bool matchFound = false;
				string depth = string.Empty;
				string eval = string.Empty;
				bool? isCentiPawn = null;

				#region Hardcoded logic 1 (Infinity chess)

				bool knownBookMove =
                    commentsCopy.Contains("[%eval 0,5]")    || commentsCopy.Contains("[%eval +0,5]")    || commentsCopy.Contains("[%eval -0,5]")    ||
					commentsCopy.Contains("[%eval 5.00,1]") || commentsCopy.Contains("[%eval -5.00,1]") || commentsCopy.Contains("[%eval -5.00,1]") ||
					commentsCopy.Contains("[%eval 5,00/1]") || commentsCopy.Contains("[%eval -5,00/1]") || commentsCopy.Contains("[%eval -5,00/1]") ||
					commentsCopy.Contains("[%eval 0.05/1]") || commentsCopy.Contains("[%eval -0.05/1]") || commentsCopy.Contains("[%eval +0.05/1]") ||
					commentsCopy.Contains("[%eval 0,05/1]") || commentsCopy.Contains("[%eval -0,05/1]") || commentsCopy.Contains("[%eval +0,05/1]") ||
					commentsCopy.Contains("[%eval 0,0]")    || commentsCopy.Contains("[%eval -0,0]")    || commentsCopy.Contains("[%eval +0,0]")    ||
					commentsCopy.Contains("[%eval 1,0]")    || commentsCopy.Contains("[%eval -1,0]")    || commentsCopy.Contains("[%eval +1,0]")    ||
					commentsCopy.Contains("[%B]");

				if (knownBookMove)
				{
					matchFound = true;
					Evaluation = null;
					Depth = 0;
					IsMateInX = false;
					IsBookMove = true;
					return;
				}

				#endregion


				#region Pattern 1

				//Apply regex filter
				Match m = _evaluationRegex1.Match(commentsCopy);
				if (m.Success)
				{
					matchFound = true;

					//EvalPart1 cannot be empty
					if (string.IsNullOrEmpty(m.Groups["EvalPart1"].Value))
						throw new Exception(string.Format("Could not parse evaluation part 1 from: {0}", comments));

					//Seperator1 cannot be empty
					if (string.IsNullOrEmpty(m.Groups["Seperator1"].Value))
						throw new Exception(string.Format("Could not parse seperator1 from: {0}", comments));

					//EvalPart2 cannot be empty
					if (string.IsNullOrEmpty(m.Groups["EvalPart2"].Value))
						throw new Exception(string.Format("Could not parse evaluation part 2 from: {0}", comments));

					//MateInX
					IsMateInX = m.Groups["Letter"].Value == "M" || m.Groups["Letter"].Value == "#";

					//Evaluation
					if (m.Groups["Sign"].Value == "+" || m.Groups["Sign"].Value == "-")
						eval = m.Groups["Sign"].Value;

					eval += m.Groups["EvalPart1"].Value;

					if (
						m.Groups["Seperator1"].Value == "." ||
						(m.Groups["Seperator1"].Value == "," && m.Groups["Seperator2"].Value == "/"))
					{
						eval += "." + m.Groups["EvalPart2"].Value;
						isCentiPawn = false;
					}

                    if (m.Groups["Seperator1"].Value == "," && string.IsNullOrEmpty(m.Groups["Seperator2"].Value))
                    {
                        depth = m.Groups["EvalPart2"].Value;
                        isCentiPawn = true;
                    }

                    if (m.Groups["Seperator2"].Value == "/")
						depth = m.Groups["EvalPart3"].Value;
					else if (m.Groups["Seperator1"].Value == "/")
						depth = m.Groups["EvalPart2"].Value;

					//IsCentiPawn
					if(!isCentiPawn.HasValue)
						isCentiPawn =
							!IsMateInX &&
							!eval.Contains(".") &&
							m.Groups["Seperator1"].Value == "," &&
							string.IsNullOrEmpty(m.Groups["Seperator2"].Value) &&
							string.IsNullOrEmpty(m.Groups["EvalPart3"].Value);
				}

				#endregion


				#region Pattern 2

				if(!matchFound)
				{
					m = _evaluationRegex2.Match(commentsCopy);
					if (m.Success)
					{
						matchFound = true;

						//Eval cannot be empty
						if (string.IsNullOrEmpty(m.Groups["Eval"].Value))
							throw new Exception(string.Format("Could not parse evaluation from: {0}", comments));

						//Depth cannot be empty
						if (string.IsNullOrEmpty(m.Groups["Depth"].Value))
							throw new Exception(string.Format("Could not parse depth from: {0}", comments));

						isCentiPawn = false; //Not always correct!
						IsMateInX = false;
						eval = m.Groups["Eval"].Value;
						depth = m.Groups["Depth"].Value;
					}
				}

				#endregion

				if (!matchFound)
				{
					//throw new Exception(string.Format("No matching pattern for comments: {0}", comments));
					Evaluation = null;
					Depth = null;
					IsMateInX = false;
					IsBookMove = false;
					return;
				}

				#region Depth

				if (string.IsNullOrEmpty(depth))
				{
					Depth = 0;
				}
				else
				{
					int d;
					if (!int.TryParse(depth, out d))
						throw new Exception(string.Format("Could not parse depth from: {0}", comments));

					Depth = d;
				}

				#endregion

				#region Evaluation

				double ev;
				bool validEvaluation = true;
				if (!double.TryParse(eval, out ev))
					throw new Exception(string.Format("Could not parse evaluation from: {0}", comments));

				if (IsMateInX)
				{
					if (Math.Abs(ev) > 245)
					{
						validEvaluation = false;
					}
					else
					{
						ev = (VALUE_MATE - Math.Abs(ev)) * (ev < 0.0f ? -1.0f : +1.0f);
					}
				}
				else
				{
					if (isCentiPawn.Value)
					{
						if (Math.Abs(ev) > VALUE_KNOWN_WIN * 100.0)
							ev /= 208.0f;
						else
							ev /= 100.0f;
					}

					if (Math.Abs(ev) > VALUE_KNOWN_WIN)
						ev = VALUE_MATE * (ev < 0.0f ? -1.0f : +1.0f);
				}

				if (validEvaluation)
				{
					Evaluation = ev;
				}
				else
				{
					//Clear everything
					Evaluation = null;
					Depth = null;
					IsMateInX = false;
					IsBookMove = false;
				}

				#endregion

				#region IsBookMove

				IsBookMove = (Depth.HasValue && (Depth.Value == 0 || Depth.Value == 1)) && (!Evaluation.HasValue || Math.Abs(Evaluation.Value) <= 0.05);
				if (IsBookMove)
				{
					Evaluation = null;
					Depth = null;
				}

				#endregion
			}
			catch (Exception ex)
			{
				ex = ex.InnerException; //Avoid warning
				Evaluation = null;
				Depth = null;
				IsMateInX = false;
				IsBookMove = false;
			}
		}

		#endregion


		#region Constructor

		public PgnMove(int ply, string move, string comments)
		{
			Ply = ply;
			Move = move;
			Comments = comments;

			IsMateInX = false;

			Evaluation = null;

			IsBookMove = false;

			ParseEvaluation(Comments);
		}

		#endregion
	}
}
