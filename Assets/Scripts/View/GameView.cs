using UnityEngine;
using BAMEngine;

public class GameView : MonoBehaviour
{
    public static GameView Instance;

    public GameObject piecePrefab;
    public GameEngine gameEngine { get;private set; }

    private BoardView mBoardView;
    private PieceView mCurrentPiece;

    private void Awake()
    {
        Instance = this;

        gameEngine = new GameEngine();
        mBoardView = gameObject.AddComponent<BoardView>();
    }

    private void Start()
    {
        CreateNextPiece();
    }

    public void PlacePieceOnBoard(PieceView piece, int lineIndex, int positionIndex)
    {
        gameEngine.board.PlacePiece(piece.piece, lineIndex, positionIndex);
    }

    private void CreateNextPiece()
    {
        mCurrentPiece = GameObject.Instantiate(piecePrefab, new Vector3(3.5f, -13f, 0f), Quaternion.identity).AddComponent<PieceView>();
        mCurrentPiece.Initiate(gameEngine.GetNextPiece());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mCurrentPiece.Shoot(new Vector3(0.5f, 1));
        }
    }
}
