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

    public void ApplyColorByType(Renderer renderer, TrailRenderer trail, NormalPiece.PieceType pieceType)
    {
        var c = colors[(int)pieceType];
        renderer.GetPropertyBlock(mPropBlock);
        mPropBlock.SetColor("_Color", c);
        renderer.SetPropertyBlock(mPropBlock);

        trail.startColor = c;
        c.a = 0f;
        trail.endColor = c;
    }
}
