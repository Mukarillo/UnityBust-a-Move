using BAMEngine;
using UnityEngine;

public class BoardUtils
{
    public static Vector3 GetClampedPosition(Vector3 position)
    {
        var y = Mathf.Clamp(Mathf.RoundToInt(position.y), -(Board.MAX_LINES - 1), 0);
        var shortLine = IsShortLine(y);
        var pos = Mathf.FloorToInt(position.x + (shortLine ? 0 : 0.5f));
        var x = Mathf.Clamp(pos, 0, shortLine ? 6 : 7);

        return new Vector3(x, y, 0);
    }

    public static Vector2Int GetLineAndPosition(Vector3 worldPosition)
    {
        var pos = GetClampedPosition(worldPosition);
        return new Vector2Int(Mathf.Abs((int)pos.x), Mathf.Clamp(Mathf.Abs(Mathf.FloorToInt(pos.y)), 0, Board.MAX_LINES - 1));
    }

    public static bool IsShortLine(int index)
    {
        return index % 2 != 0;
    }
}
