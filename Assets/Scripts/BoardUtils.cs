using BAMEngine;
using UnityEngine;

public class BoardUtils
{
    public static Vector3 GetClampedPosition(Vector3 position)
    {
        var y = Mathf.Clamp(Mathf.FloorToInt(position.y), -(Board.MAX_LINES - 1), 0);
        Debug.Log(y);
        var shortLine = IsShortLine(y);
        var x = Mathf.Clamp(Mathf.Floor(position.x), 0, shortLine ? 6 : 7) + (shortLine ? 0.5f : 0f);

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
