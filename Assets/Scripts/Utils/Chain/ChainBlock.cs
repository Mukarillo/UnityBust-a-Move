using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainBlock : MonoBehaviour
{
    public bool CanAddMore => mPieces.Count < mPiecesPositions.Count;
    public bool HasPiece => mPieces.Count > 0;

    private List<Transform> mPiecesPositions = new List<Transform>();
    private Queue<PieceView> mPieces = new Queue<PieceView>();

    private void Awake()
    {
        mPiecesPositions = GetComponentsInChildren<Transform>().Where(x => x.transform != transform).ToList();
    }

    public void AddPiece(PieceView piece)
    {
        mPieces.Enqueue(piece);
        piece.transform.SetParent(mPiecesPositions[mPieces.Count - 1]);
        piece.transform.localPosition = Vector3.zero;
    }

    public PieceView GetPiece()
    {
        return mPieces.Dequeue();
    }
}
