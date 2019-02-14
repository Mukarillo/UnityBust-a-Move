using UnityEngine;
using BAMEngine;

public class GameView : MonoBehaviour
{
    public bool DEBUG { get; private set; } = false;

    public GameObject piecePrefab;
    public GameEngine gameEngine { get;private set; }

    private BoardView mBoardView;
    private PieceView mCurrentPiece;

    private void Awake()
    {
        gameEngine = new GameEngine(CreateNextPiece);
        mBoardView = gameObject.AddComponent<BoardView>();
    }

    private void Start()
    {
        mBoardView.Initiate(this);
        CreateNextPiece();
    }

    public void PlacePiece(PieceView piece, int lineIndex, int positionIndex)
    {
        gameEngine.LockPiece(piece.piece);
        mBoardView.PlacePiece(piece);
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
        mCurrentPiece = GameObject.Instantiate(piecePrefab, new Vector3(3.5f, -13f, 0f), Quaternion.identity).AddComponent<PieceView>();
        mCurrentPiece.Initiate(mBoardView, gameEngine.GetNextPiece());
    }

    public void HighlightNeighbors(PieceView piece)
    { 
        foreach(var p in piece.piece.Connections)
        {
            var vp = mBoardView.GetPiece(p);
            vp.Highlight(Color.red);
        }

        foreach (var p in piece.piece.HoldConnections)
        {
            var vp = mBoardView.GetPiece(p);
            vp.Highlight(Color.blue);
        }
    }

    public void ResetHighlight()
    {
        mBoardView.viewPieces.ForEach(x => x.Highlight(Color.white));
    }

    PieceView oldPiece;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            var dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mCurrentPiece.transform.position).normalized;
            dir.z = 0f;
            mCurrentPiece.Shoot(dir);
        }

        if (DEBUG)
        {
            oldPiece?.Highlight(Color.white);
            var lp = BoardUtils.GetLineAndPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            oldPiece = mBoardView.GetPiece(lp.y, lp.x);
            oldPiece.Highlight(Color.green);
        }
    }

    public void Dump()
    {
        gameEngine.board.Dump();
    }
}
