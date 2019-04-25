using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Chain : MonoBehaviour
{
    private const float TIME_TO_MOVE = 0.5f;
    private GameView mGameView;
    private BoardView mBoardView;

    private List<ChainBlock> mChainBlock = new List<ChainBlock>();
    private int mCurrentBlock = 2;

    private int mMoviments = 0;
    public bool IsMoving { get; private set; }

    private int NextBlockIndex => mCurrentBlock + 1 >= mChainBlock.Count ? 0 : mCurrentBlock + 1;
    private int PreviousBlockIndex => mCurrentBlock - 1 < 0 ? (mChainBlock.Count - 1) : mCurrentBlock - 1;

    private void Awake()
    {
        foreach (Transform t in transform)
            mChainBlock.Add(t.gameObject.AddComponent<ChainBlock>());
    }

    public void Initiate(GameView gameView, BoardView boardView)
    {
        mGameView = gameView;
        mBoardView = boardView;

        for (int i = mCurrentBlock; i >= 0; i--)
            FillChainBlock(i);
    }

    public PieceView GetPiece()
    {
        if (IsMoving)
            return null;

        var piece = mChainBlock[mCurrentBlock].GetPiece();
        piece.transform.SetParent(mBoardView.transform);
        piece.transform.DOMove(mGameView.BallSpawnPoint, TIME_TO_MOVE).SetEase(Ease.OutExpo);

        MoveChain();

        return piece;
    }

    private void MoveChain()
    {
        mMoviments++;
        IsMoving = true;

        foreach (var c in mChainBlock)
        {
            var endValue = c.transform.localPosition + new Vector3(0.57f, 0f, 0f);
            c.transform.DOLocalMove(endValue, TIME_TO_MOVE).SetEase(Ease.Linear);
        }

        transform.DOShakePosition(TIME_TO_MOVE, Random.Range(0.05f, 0.15f), fadeOut: false).OnComplete(CheckLastChain);
    }

    private void CheckLastChain()
    {
        if (mMoviments % 2 == 0 && mMoviments != 0)
        {
            mMoviments = 0;
            var p = mChainBlock[NextBlockIndex].transform.localPosition;
            mChainBlock[NextBlockIndex].transform.localPosition = new Vector3(-2.638f, p.y, p.z);
            FillChainBlock(NextBlockIndex);
        }

        if (!mChainBlock[mCurrentBlock].HasPiece)
            mCurrentBlock = PreviousBlockIndex;

        IsMoving = false;
    }

    private void FillChainBlock(int index)
    {
        while (mChainBlock[index].CanAddMore)
            mChainBlock[index].AddPiece(mGameView.GetFuturePiece());
    }
}