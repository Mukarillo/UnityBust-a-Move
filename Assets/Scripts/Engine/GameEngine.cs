using System;
namespace BAMEngine
{
    public class GameEngine
    {
        public bool DEBUG { get; private set; } = true;

        public Board board { get; private set; }

        private readonly Action onCreateNextPiece;

        public GameEngine(Action onCreateNextPiece)
        {
            board = new Board(this);
            this.onCreateNextPiece = onCreateNextPiece;
        }

        public void UpdatePiecePosition(Piece piece, int line, int position)
        {
            board.UpdatePiecePosition(piece, line, position);
        }

        public void LockPiece(Piece piece)
        {
            board.LockPiece(piece);
            onCreateNextPiece?.Invoke();
        }

        public Piece GetNextPiece()
        {
            var piece = NormalPiece.GetRandom();
            piece.UpdatePosition(board.lines[Board.MAX_LINES - 1], (Board.MAX_PIECES_PER_LINE / 2) - 1);
            return piece;
        }
    }
}
