using UnityEngine;
using BAMEngine;

public class PiecesController : MonoBehaviour
{
    public static PiecesController Instance;
    public Sprite[] ballsSprite;

    private void Awake()
    {
        Instance = this;
    }

    public Sprite GetPieceByType(NormalPiece.PieceType pieceType)
    {
        return ballsSprite[(int)pieceType];
    }
}
