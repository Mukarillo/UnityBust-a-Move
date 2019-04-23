using UnityEngine;
using BAMEngine;

public class PiecesController : MonoBehaviour
{
    public static PiecesController Instance;
    public Color[] colors;
    private MaterialPropertyBlock mPropBlock;
 
    void Awake()
    {
        mPropBlock = new MaterialPropertyBlock();
        Instance = this;
    }

    public void ApplyColorByType(Renderer renderer, NormalPiece.PieceType pieceType)
    {
        renderer.GetPropertyBlock(mPropBlock);
        mPropBlock.SetColor("_Color", colors[(int)pieceType]);
        renderer.SetPropertyBlock(mPropBlock);
    }
}
