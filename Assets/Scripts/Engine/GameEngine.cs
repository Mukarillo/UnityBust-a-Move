using System;
namespace BAMEngine
{
    public class GameEngine
    {
        public Board board { get; private set; }

        private readonly Action onCreateNextPiece;

        public GameEngine(Action onCreateNextPiece)
        {
            board = new Board();
            this.onCreateNextPiece = onCreateNextPiece;
        }

        public void PlacePiece(Piece piece, int line, int position)
        {
            board.PlacePiece(piece, line, position);
            onCreateNextPiece?.Invoke();
        }

        public Piece GetNextPiece()
        {
            return NormalPiece.GetRandom();
        }
    }
}
