using System.Collections.Generic;
using System.Linq;

namespace BAMEngine
{
    public class PiecesLine : List<Piece>
    {
        public static PiecesLine EmptyLine(int index, int amount)
        {
            var pl = new PiecesLine(index, true, false);
            for (int i = 0; i < amount; i++)
                pl.Add(null);
            return pl;
        }

        public int Index { get; private set; }
        public bool IsShortLine { get; private set; }
        public bool IsRoof { get; private set; }
        public bool HasPiece => Count > 0 && this.Any(x => x != null);

        public PiecesLine(int index, bool isShortLine, bool isRoof)
        {
            Index = index;
            IsShortLine = isShortLine;
            IsRoof = isRoof;
        }

        public void StepDown()
        {
            Index++;
        }
    }
}