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

    private void SnapPiece(Vector3 otherPiecePosition)
    {
        mIsMoving = false;
        //TODO: Check if the spot is taken, if it is, than we check opposite direction
        var snapPosition = otherPiecePosition + (transform.position - otherPiecePosition).normalized;
        var lp = BoardUtils.GetLineAndPosition(snapPosition);
        transform.position = new Vector3(lp.x + (BoardUtils.IsShortLine(lp.y) ? 0.5f : 0), -lp.y);
        mBoardView.gameView.gameEngine.UpdatePiecePosition(piece, lp.y, lp.x);
        mBoardView.gameView.LockPiece(this);

        Debug.LogError($"OtherPiecePosition: {otherPiecePosition}\n" +
            $"Direction: {(transform.position - otherPiecePosition).normalized}" +
            $"\nSnapPosition: {snapPosition}" +
            $"\nFinalPosition: {transform.position}" +
            $"\nBoardPosition: {lp}");
        mBoardView.gameView.Dump();
    }

    private void OnBreak()
    {
        Destroy(gameObject);
    }

    private void OnFall()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        mBoardView.RemovePiece(this);
    }

    private void Update()
    {
        if (!mIsMoving)
            return;

        transform.position += mMovingDirection * SPEED;
        var lp = BoardUtils.GetLineAndPosition(transform.position);
        mBoardView.gameView.gameEngine.UpdatePiecePosition(piece, lp.y, lp.x);

        Predict();
    }

    private void Predict()
    {
        for (int i = 0; i < 5; i++)
        {
            var futurePosition = transform.position + (mMovingDirection * (0.8f + (i/5f)));
            var boardPosition = BoardUtils.GetLineAndPosition(futurePosition);

            var p = mBoardView.gameView.GetPieceOnBoard(boardPosition.y, boardPosition.x);
            if (p != null)
            {
                SnapPiece(p.transform.position);
                break;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Roof")
            SnapPiece(collision.bounds.ClosestPoint(transform.position));
        else
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
        if (mSpriteRenderer == null)
            Debug.Log("NULL RENDERER", gameObject);
        else
            mSpriteRenderer.color = c;
    }
}
