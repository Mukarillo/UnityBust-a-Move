using UnityEngine;
using BAMEngine;
using System;

public class GameView : MonoBehaviour
{
    public bool DEBUG { get; private set; } = true;

    public GameObject piecePrefab;
    public GameObject aimArrow;
    public GameEngine gameEngine { get;private set; }

    private BoardView mBoardView;
    private PieceView mCurrentPiece;
    private AimController mAimController;
    private readonly Vector3 mBallSpawnPoint = new Vector3(3.5f, -13f, 0f);

    private void Awake()
    {
        gameEngine = new GameEngine(CreateNextPiece);
        mBoardView = gameObject.AddComponent<BoardView>();
    }

    private void Start()
    {
        mBoardView.Initiate(this);
        CreateNextPiece();
        mAimController = aimArrow.AddComponent<AimController>();
        mAimController.Initiate(mBallSpawnPoint, OnShoot);
    }

    public void LockPiece(PieceView piece)
    {
        gameEngine.LockPiece(piece.piece);
        mBoardView.LockPiece(piece);
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
        mCurrentPiece = GameObject.Instantiate(piecePrefab, mBallSpawnPoint, Quaternion.identity).AddComponent<PieceView>();
        mCurrentPiece.Initiate(mBoardView, gameEngine.GetNextPiece());
    }

    public void HighlightNeighbors(PieceView piece)
    { 
        foreach(var p in piece.piece.Connections)
        {
            var vp = mBoardView.GetPiece(p);
            vp.Highlight(Color.black);
        }

        var c = Color.black;
        c.a = 0.2f;
        foreach (var p in piece.piece.HoldConnections)
        {
            var vp = mBoardView.GetPiece(p);
            vp.Highlight(c);
        }
    }

    public void ResetHighlight()
    {
        mBoardView.viewPieces.ForEach(x => x.Highlight(Color.white));
    }

    private void OnShoot(Vector2 direction)
    {
        mCurrentPiece.Shoot(direction);
    }

    PieceView oldPiece;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            var dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mCurrentPiece.transform.position).normalized;
            dir.z = 0f;
        }

        DEBUG = Input.GetKey(KeyCode.B);

        if (DEBUG)
        {
            oldPiece?.Highlight(Color.white);
            var lp = BoardUtils.GetLineAndPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            oldPiece = mBoardView.GetPiece(lp.y, lp.x);
            oldPiece?.Highlight(Color.clear);
        }
    }

    public void Dump()
    {
        gameEngine.board.Dump();
    }
}
