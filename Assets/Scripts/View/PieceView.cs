using UnityEngine;
using BAMEngine;
using pooling;

public class PieceView : PoolingObject
{
    private const float SPEED = 0.4f;

    public Piece piece { get; private set; }

    private bool mIsMoving = false;
    private Vector3 mMovingDirection;
    private BoardView mBoardView;

    private void Awake()
    {
        gameObject.AddComponent<SequenceAnimation2D>().Initiate(AssetController.ME.EyeAnimation, Random.Range(0, AssetController.ME.EyeAnimation.Length), Random.Range(8, 16));
        GameObject glow = new GameObject("Glow");
        glow.transform.SetParent(transform);
        glow.AddComponent<PieceGlow>().Initiate(Random.Range(3f, 30f));
    }

    public void Initiate(BoardView boardView, Piece piece)
    {
        mBoardView = boardView;
        this.piece = piece;

        piece.OnFallCallback += OnFall;
        piece.OnBreakCallback += OnBreak;

        if(piece is NormalPiece)
            PiecesController.Instance.ApplyColorByType(GetComponent<Renderer>(), ((NormalPiece)piece).pieceType);
    }

    public void Shoot(Vector3 direction)
    {
        mIsMoving = true;
        mMovingDirection = direction;
    }

    private void SnapPiece(Vector3 otherPiecePosition)
    {
        mIsMoving = false;
        Vector2Int linePos;
        Vector3 snapPosition;
        Vector3 finalPosition = transform.localPosition;
        PieceView otherPiece;
        int tries = 20;
        do
        {
            tries--;
            snapPosition = otherPiecePosition + (transform.localPosition - otherPiecePosition).normalized;
            linePos = BoardUtils.GetLineAndPosition(snapPosition);
            finalPosition = new Vector3(linePos.x + (BoardUtils.IsShortLine(linePos.y) ? 0.5f : 0), -linePos.y);
            otherPiece = mBoardView.GetPiece(linePos.y, linePos.x);
            if (otherPiece != null)
                otherPiecePosition = otherPiece.transform.localPosition;
        }
        while (otherPiece != null && tries >= 0);

        if(tries <= 0)
        {
            //TODO: GAME OVER!
            Debug.Log("Game is Over!");
        }

        transform.localPosition = finalPosition;
        mBoardView.gameView.gameEngine.UpdatePiecePosition(piece, linePos.y, linePos.x);
        mBoardView.gameView.LockPiece(this);

        //mBoardView.gameView.Dump();
    }

    private void OnBreak()
    {
        OnRelease();
    }

    private void OnFall()
    {
        OnRelease();
    }

    public override void OnRelease()
    {
        if (piece != null)
        { 
            piece.OnFallCallback -= OnFall;
            piece.OnBreakCallback -= OnBreak;
            piece = null;
        }

        mBoardView?.RemovePiece(this);
        base.OnRelease();
    }

    private void Update()
    {
        if (!mIsMoving || !isUsing)
            return;

        transform.localPosition += mMovingDirection * SPEED;
        var lp = BoardUtils.GetLineAndPosition(transform.localPosition);
        mBoardView.gameView.gameEngine.UpdatePiecePosition(piece, lp.y, lp.x);

        Predict();
    }

    private void Predict()
    {
        for (int i = 0; i < 5; i++)
        {
            var futurePosition = transform.localPosition + (mMovingDirection * (0.8f + (i/5f)));
            var boardPosition = BoardUtils.GetLineAndPosition(futurePosition);

            var p = mBoardView.gameView.GetPieceOnBoard(boardPosition.y, boardPosition.x);
            if (p != null)
            {
                SnapPiece(p.transform.localPosition);
                break;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Roof")
            SnapPiece(collision.bounds.ClosestPoint(transform.localPosition));
        else
            mMovingDirection.x *= -1;
    }
}