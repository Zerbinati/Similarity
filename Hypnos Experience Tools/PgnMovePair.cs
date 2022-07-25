namespace Hypnos.Experience.Tools
{
	public class PgnMovePair
	{
		#region Public properties

		public int MoveNumber { get; set; }
		public PgnMove WhiteMove { get; set; }
		public PgnMove BlackMove { get; set; }

		#endregion


		#region Constructor

		public PgnMovePair(int moveNumber, PgnMove whiteMove, PgnMove blackMove)
		{
			MoveNumber = moveNumber;
			WhiteMove = whiteMove;
			BlackMove = blackMove;
		}

		#endregion
	}
}
