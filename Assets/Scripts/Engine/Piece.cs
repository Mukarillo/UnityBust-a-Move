using System.Collections.Generic;

namespace BAMEngine
{
    public abstract class Piece {
        public PiecesLine Line { get; protected set; }
        public int Index { get; protected set; }

        public List<Piece> Connections { get; protected set; }

        public abstract void Break();
        public abstract void Fall();

        public void UpdatePosition(PiecesLine line, int index)
        {
            Line = line;
            Index = index;
        }

        public void UpdateConnections(List<Piece> connections)
        {
            Connections = connections;
        }
    }
}