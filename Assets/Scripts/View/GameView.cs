using UnityEngine;
using BAMEngine;
using pooling;

public class GameView : MonoBehaviour
{
    public bool DEBUG { get; private set; } = true;

    public GameObject piecePrefab;
    public GameObject aimArrow;
    public GameObject chainRef;
    public GameEngine gameEngine { get; private set; }
    public Pooling<PieceView> piecesPool { get; private set; } = new Pooling<PieceView>();

    private BoardView mBoardView;
    private PieceView mCurrentPiece;
    private AimController mAimController;
    private Chain mChain;
    private readonly Vector3 mBallSpawnPoint = new Vector3(3.5f, -13f, 0f);

    private void Awake()
    {
        gameEngine = new GameEngine(CreateNextPiece);
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
        CreateNextPiece();
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

    private void CreateNextPiece()
    {
        mCurrentPiece = mChain.GetPiece();
    }

    public PieceView GetFuturePiece()
    {
        var p = piecesPool.Collect();
        p.Initiate(mBoardView, gameEngine.GetNextPiece());

        return p;
    }

    private void OnShoot(Vector2 direction)
    {
        mCurrentPiece.Shoot(direction);
    }

    public void Dump()
    {
        gameEngine.board.Dump();
    }
}