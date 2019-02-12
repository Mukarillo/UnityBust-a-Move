using System;
using System.Collections.Generic;

namespace BAMEngine
{
    public abstract class Piece {
        public PiecesLine Line { get; protected set; }
        public int Index { get; protected set; }

        public List<Piece> Connections { get; protected set; }

        public Action OnBreakCallback;
        public Action OnFallCallback;

        internal virtual void Break()
        {
            OnBreakCallback?.Invoke();
        }

        internal virtual void Fall()
        {
            OnFallCallback?.Invoke();
        }

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