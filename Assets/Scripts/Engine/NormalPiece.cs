using UnityEngine;

namespace BAMEngine
{
    public class NormalPiece : Piece
    {
        public override string ToString()
        {
            return $"[{((int) pieceType).ToString()}]";
        }

        public enum PieceType
        {
            GREEN,
            ORANGE,
            YELLOW,
            BLUE,
            RED,
            PURPLE,
            MAX
        }

        public PieceType pieceType { get; private set; }

        public NormalPiece(PieceType pieceType)
        {
            this.pieceType = pieceType;
        }

        public bool Compare(NormalPiece piece)
        {
            return piece.pieceType == pieceType;
        }

        public static PieceType GetRandom()
        {
            return (PieceType) Random.Range(0, (int) PieceType.MAX);
        }

        public override void Break()
        {
        }
        
        public override void Fall()
        {
        }
    }
}