using System.Collections.Generic;
using System.Linq;

namespace BAMEngine
{
    public class PiecesLine : List<Piece>
    {
        public int Index { get; private set; }
        public bool IsShortLine => Index % 2 != 0;
        public bool CanFall => Index != 0;
        public bool HasPiece => Count > 0 && this.Any(x => x != null);

        public PiecesLine(int index)
        {
            Index = index;
        }

        public void StepDown()
        {
            Index++;
        }
    }
}