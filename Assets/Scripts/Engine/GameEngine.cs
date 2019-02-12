using System;
namespace BAMEngine
{
    public class GameEngine
    {
        public Board board { get; private set; }

        public GameEngine()
        {
            board = new Board();
        }

        public Piece GetNextPiece()
        {
            return NormalPiece.GetRandom();
        }
    }
}
