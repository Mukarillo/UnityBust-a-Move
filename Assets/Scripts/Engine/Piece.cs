using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace BAMEngine
{
    public abstract class Piece {
        public PiecesLine Line { get; protected set; }
        public int Index { get; protected set; }

        public List<Piece> Connections { get; protected set; }
        public List<Piece> HoldConnections { get; protected set; }

        public Action OnBreakCallback;
        public Action OnFallCallback;

        public bool isFalling { get; protected set; }

        internal virtual void Break()
        {
            OnBreakCallback?.Invoke();
        }

        internal virtual void Fall()
        {
            isFalling = true;
            OnFallCallback?.Invoke();
        }

        public void UpdatePosition(PiecesLine line, int index)
        {
            Line = line;
            Index = index;
        }

        public void UpdateConnections(List<Piece> connections, List<Piece> holdConnections)
        {
            Connections = connections;
            HoldConnections = holdConnections;
        }
    }
}