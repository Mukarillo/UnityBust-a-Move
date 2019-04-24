using UnityEngine;
using BAMEngine;
using pooling;
using DG.Tweening;
using System;
using System.Collections;

public class GameView : MonoBehaviour, IGameView
{
    private const float TIME_TO_ROOF_DOWN = 0.3f;

    public bool DEBUG { get; private set; } = true;

    public bool ShouldStepDown => true;

    public GameObject piecePrefab;
    public GameObject aimArrow;
    public GameObject chainRef;
    public GameObject roofRef;
    public GameEngine gameEngine { get; private set; }
    public Pooling<PieceView> piecesPool { get; private set; } = new Pooling<PieceView>();

    private BoardView mBoardView;
    private PieceView mCurrentPiece;
    private AimController mAimController;
    private Chain mChain;
    private readonly Vector3 mBallSpawnPoint = new Vector3(3.5f, -13f, 0f);

    private void Awake()
    {
        gameEngine = new GameEngine(this);
        GameObject board = new GameObject("board");
        board.transform.position = new Vector3(0f, 0.9f, 0f);
        mBoardView = board.AddComponent<BoardView>();
        mChain = chainRef.AddComponent<Chain>();
        piecesPool.Initialize(50, piecePrefab, mBoardView.transform);
    }

    private void Start()
    {
        mBoardView.Initiate(this);
        mChain.Initiate(this, mBoardView);
        OnCreateNextPiece();
        mAimController = aimArrow.AddComponent<AimController>();
        mAimController.Initiate(mBallSpawnPoint, OnShoot);
    }

    public void LockPiece(PieceView piece)
    {
        mBoardView.LockPiece(piece);
        gameEngine.LockPiece(piece.piece);
    }

    public void RemovePiece(PieceView piece)
    {
        mBoardView.RemovePiece(piece);
    }

    public PieceView GetPieceOnBoard(int line, int position)
    {
        return mBoardView.GetPiece(line, position);
    }

    public void OnCreateNextPiece()
    {
        mCurrentPiece = mChain.GetPiece();
    }

    public bool CanStepDown()
    {
        var canStepDown = !mCurrentPiece?.IsMoving ?? true;
        if (!canStepDown)
            StartCoroutine(WaitAndStepDown());
        return canStepDown;
    }

    private IEnumerator WaitAndStepDown()
    {
        yield return new WaitWhile(() => mCurrentPiece?.IsMoving ?? true);

        gameEngine.StepDown();
    }

    public void OnStepDown()
    {
        roofRef.transform.DOLocalMoveY(roofRef.transform.localPosition.y - 0.3f, TIME_TO_ROOF_DOWN).SetEase(Ease.OutBounce);
        mBoardView.StepDown(TIME_TO_ROOF_DOWN);

        gameEngine.board.Dump();
    }

    public void OnGameOver()
    {
        //TODO
    }

    public PieceView GetFuturePiece()
    {
        var p = piecesPool.Collect();
        p.Initiate(mBoardView, gameEngine.GetNextPiece());
        return p;
    }

    private void OnShoot(Vector2 direction)
    {
        mCurrentPiece.piece.UpdatePosition(mBoardView.board.lines[Board.MAX_LINES - 1], (Board.MAX_PIECES_PER_LINE / 2) - 1);
        mCurrentPiece.Shoot(direction);
    }

    public void Dump()
    {
        gameEngine.board.Dump();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            gameEngine.StepDown();
        }
    }
}