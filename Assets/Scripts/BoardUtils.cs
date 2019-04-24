using BAMEngine;
using UnityEngine;

public class BoardUtils
{
    public static Vector3 GetClampedPosition(Vector3 position, Board board)
    {
        var y = Mathf.Clamp(Mathf.RoundToInt(position.y), - (Board.MAX_LINES - 1), 0);
        var shortLine = board.lines[Mathf.Abs(y)].IsShortLine;
        var pos = Mathf.FloorToInt(position.x + (shortLine ? 0 : 0.5f));
        var x = Mathf.Clamp(pos, 0, shortLine ? 6 : 7);

        return new Vector3(x, y, 0);
    }

    public static Vector2Int GetLineAndPosition(Vector3 worldPosition, Board board)
    {
        var pos = GetClampedPosition(worldPosition, board);
        return new Vector2Int(Mathf.Abs((int)pos.x), Mathf.Clamp(Mathf.Abs(Mathf.FloorToInt(pos.y)), 0, Board.MAX_LINES - 1));
    }
}
