using UnityEngine;
using BAMEngine;
using pooling;
using DG.Tweening;
using System.Collections;

public class GameView : MonoBehaviour, IGameView
{
    private const float TIME_TO_ROOF_DOWN = 0.3f;

    public bool DEBUG { get; private set; } = true;
    public readonly Vector3 BallSpawnPoint = new Vector3(3.5f, -11f, 0f);

    public bool ShouldStepDown => false;

    public GameObject piecePrefab;
    public GameObject aimArrow;
    public GameObject chainRef;
    public GameObject roofRef;
    public GameEngine gameEngine { get; private set; }
    public BoardView boardView { get; private set; }
    public Pooling<PieceView> piecesPool { get; private set; } = new Pooling<PieceView>();

    private PieceView mCurrentPiece;
    private AimController mAimController;
    private Chain mChain;

    private void Awake()
    {
        gameEngine = new GameEngine(this);
        GameObject board = new GameObject("board");
        board.transform.position = new Vector3(0f, 0.9f, 0f);
        boardView = board.AddComponent<BoardView>();
        mChain = chainRef.AddComponent<Chain>();
        piecesPool.Initialize(50, piecePrefab, boardView.transform);
    }

    private void Start()
    {
        boardView.Initiate(this);
        mChain.Initiate(this, boardView);
        OnCreateNextPiece();
        mAimController = aimArrow.AddComponent<AimController>();
        mAimController.Initiate(BallSpawnPoint, OnShoot, this);
    }

    public void LockPiece(PieceView piece)
    {
        boardView.LockPiece(piece);
        gameEngine.LockPiece(piece.piece);
        mAimController.ShowGuideLine();
        mAimController.UpdateGuideLine(0.3f);
    }

    public void RemovePiece(PieceView piece)
    {
        boardView.RemovePiece(piece);
    }

    public PieceView GetPieceOnBoard(int line, int position)
    {
        return boardView.GetPiece(line, position);
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
        boardView.StepDown(TIME_TO_ROOF_DOWN);

        gameEngine.board.Dump();
    }

    public void OnGameOver()
    {
        //TODO
    }

    public PieceView GetFuturePiece()
    {
        var p = piecesPool.Collect();
        p.Initiate(boardView, gameEngine.GetNextPiece());
        return p;
    }

    private void OnShoot(Vector2 direction)
    {
        mAimController.HideGuideLine();
        mCurrentPiece.piece.UpdatePosition(boardView.board.lines[Board.MAX_LINES - 1], (Board.MAX_PIECES_PER_LINE / 2) - 1);
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