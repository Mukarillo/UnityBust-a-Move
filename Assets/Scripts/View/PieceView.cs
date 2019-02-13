using UnityEngine;
using BAMEngine;

public class PieceView : MonoBehaviour
{
    private const float SPEED = 0.4f;

    public Piece piece { get; private set; }

    private SpriteRenderer mSpriteRenderer;
    private bool mIsMoving = false;
    private Vector3 mMovingDirection;
    private BoardView mBoardView;

    private void Awake()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initiate(BoardView boardView, Piece piece)
    {
        mBoardView = boardView;
        this.piece = piece;

        piece.OnFallCallback = OnFall;
        piece.OnBreakCallback = OnBreak;

        if(piece is NormalPiece)
        {
            mSpriteRenderer.sprite = PiecesController.Instance.GetPieceByType(((NormalPiece)piece).pieceType);
        }
    }

    public void Shoot(Vector3 direction)
    {
        mIsMoving = true;
        mMovingDirection = direction;
    }

    private void SnapPiece()
    {
        mIsMoving = false;

        transform.position = BoardUtils.GetClampedPosition(transform.position);
        var lineAndPos = BoardUtils.GetLineAndPosition(transform.position);
        mBoardView.gameView.PlacePieceOnBoard(this, lineAndPos.y, lineAndPos.x);
    }

    private void OnBreak()
    {
        Destroy(gameObject);
    }

    private void OnFall()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!mIsMoving)
            return;

        transform.position = transform.position + (mMovingDirection * SPEED);
        var lp = BoardUtils.GetLineAndPosition(transform.position);
        mBoardView.gameView.gameEngine.UpdatePiecePosition(piece, lp.y, lp.x);

        Predict();
    }

    private void Predict()
    {
        var futurePosition = transform.position + (mMovingDirection * 1.25f);
        var boardPosition = BoardUtils.GetLineAndPosition(futurePosition);

        mBoardView.gameView.Dump();

        var p = mBoardView.gameView.GetPieceOnBoard(boardPosition.y, boardPosition.x);
        if (p != null)
            SnapPiece();
    }

    private void OnDrawGizmos()
    {
        if(mIsMoving)
        {
            var futurePosition = transform.position + (mMovingDirection * 1.25f);
            Gizmos.DrawSphere(futurePosition, 0.1f);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        mMovingDirection.x *= -1;
    }

    void OnMouseOver()
    {
        if(mBoardView.gameView.DEBUG)
            mBoardView.gameView.HighlightNeighbors(this);
    }

    void OnMouseExit()
    {
        if (mBoardView.gameView.DEBUG)
            mBoardView.gameView.ResetHighlight();
    }

    public void Highlight(Color c)
    {
        mSpriteRenderer.color = c;
    }
}
