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

        public static NormalPiece GetRandom() => new NormalPiece(GetRandomType());
        public static PieceType GetRandomType() => (PieceType) Random.Range(0, (int) PieceType.MAX);
    }
}