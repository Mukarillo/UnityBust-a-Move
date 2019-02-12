using System.Collections.Generic;

namespace BAMEngine
{
    public class PiecesLine : List<Piece>
    {
        public int Index { get; private set; }
        public bool IsShortLine => Index % 2 != 0;
        public bool CanFall => Index != 0;

        public PiecesLine(int index)
        {
            Index = index;
        }
    }
}