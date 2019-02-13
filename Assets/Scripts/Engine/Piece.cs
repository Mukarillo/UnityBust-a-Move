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

        public bool isActive => !isFalling && !isBreaking;
        public bool isFalling { get; protected set; }
        public bool isBreaking { get; protected set; }
        public bool isLocked { get; protected set; }

        public void Lock()
        {
            isLocked = true;
        }

        internal virtual void Break()
        {
            isBreaking = true;
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