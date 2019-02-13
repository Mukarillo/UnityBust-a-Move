using UnityEngine;
using BAMEngine;

public class PiecesController : MonoBehaviour
{
    private bool DEBUG = true;

    public static PiecesController Instance;
    public Sprite[] ballsSprite;
    public Sprite whiteSprite;

    private void Awake()
    {
        Instance = this;
    }

    public Sprite GetPieceByType(NormalPiece.PieceType pieceType)
    {
        return DEBUG ? whiteSprite : ballsSprite[(int)pieceType];
    }
}
