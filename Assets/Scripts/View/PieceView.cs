using UnityEngine;
using BAMEngine;
using pooling;
using DG.Tweening;

public class PieceView : PoolingObject
{
    private const float SPEED = 0.4f;

    private const float TIME_TO_OUT = 1f;
    private const float MIN_OUT_SCALE = 8f;
    private const float MAX_OUT_SCALE = 15f;
    private const float COLLIDER_RADIUS_SHOOTING = 0.01f;
    private const float COLLIDER_RADIUS_IDLE = 0.17f;

    public int connections;

    public Piece piece { get; private set; }
    public bool IsMoving { get; protected set; }

    private Vector3 mMovingDirection;
    private BoardView mBoardView;
    private SpriteRenderer mSpriteRenderer;
    private CircleCollider2D mCollider;
    private TrailRenderer mTrailRenderer;

    private void Awake()
    {
        mTrailRenderer = GetComponent<TrailRenderer>();
        mTrailRenderer.emitting = false;
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mCollider = GetComponent<CircleCollider2D>();
        gameObject.AddComponent<SequenceAnimation2D>().Initiate(AssetController.ME.EyeAnimation, Random.Range(0, AssetController.ME.EyeAnimation.Length), Random.Range(8, 16));
        GameObject glow = new GameObject("Glow");
        glow.transform.SetParent(transform);
        glow.AddComponent<PieceGlow>().Initiate(Random.Range(3f, 30f));
        mCollider.radius = COLLIDER_RADIUS_IDLE;
    }

    public virtual void Initiate(BoardView boardView, Piece piece)
    {
        mTrailRenderer.emitting = false;

        mSpriteRenderer.sortingOrder = 20;
        transform.localScale = Vector3.one * 4f;

        mBoardView = boardView;

        this.piece = piece;

        piece.OnFallCallback += OnFall;
        piece.OnBreakCallback += OnBreak;

        if (piece is NormalPiece)
            PiecesController.Instance.ApplyColorByType(GetComponent<Renderer>(), mTrailRenderer, ((NormalPiece)piece).pieceType);
    }

    public void Shoot(Vector3 direction)
    {
        mTrailRenderer.emitting = true;
        mCollider.radius = COLLIDER_RADIUS_SHOOTING;
        IsMoving = true;
        mMovingDirection = direction;
    }

    protected virtual void SnapPiece(Vector3 otherPiecePosition)
    {
        IsMoving = false;
        Vector2Int linePos;
        Vector3 snapPosition;
        Vector3 finalPosition = transform.localPosition;
        PieceView otherPiece;
        int tries = 20;
        do
        {
            tries--;
            snapPosition = otherPiecePosition + (transform.localPosition - otherPiecePosition).normalized;
            linePos = BoardUtils.GetLineAndPosition(snapPosition, mBoardView.board);
            finalPosition = new Vector3(linePos.x + (mBoardView.board.lines[linePos.y].IsShortLine ? 0.5f : 0), -linePos.y);
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
        mTrailRenderer.emitting = false;

        mCollider.radius = COLLIDER_RADIUS_IDLE;

        mBoardView.gameView.Dump();
    }

    private void OnBreak()
    {
        AnimateOut();
    }

    private void OnFall()
    {
        AnimateOut();
    }

    private void AnimateOut()
    {
        mTrailRenderer.emitting = true;
        mSpriteRenderer.sortingOrder = 30;
        IsMoving = false;

        var initialPos = transform.localPosition;
        var xAmount = Random.Range(-6f, 6f);
        var finalPos = new Vector3(initialPos.x + xAmount, -25f, 0f);
        var midPos = new Vector3(initialPos.x + xAmount / 2f, initialPos.y + 3f, 0f);

        DOTween.Sequence()
        .Join(transform.DOScale(Random.Range(MIN_OUT_SCALE, MAX_OUT_SCALE), TIME_TO_OUT))
        .Join(transform.DOLocalMoveX(midPos.x, TIME_TO_OUT / 3f).SetEase(Ease.InQuad))
        .Join(transform.DOLocalMoveY(midPos.y, TIME_TO_OUT / 3f).SetEase(Ease.OutQuad))
        .Insert(TIME_TO_OUT / 3f, transform.DOLocalMoveX(finalPos.x, (TIME_TO_OUT / 3f) * 2f).SetEase(Ease.OutQuad))
        .Insert(TIME_TO_OUT / 3f, transform.DOLocalMoveY(finalPos.y, (TIME_TO_OUT / 3f) * 2f).SetEase(Ease.InQuad))
        .OnComplete(OnRelease);
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
        if(piece?.HoldConnections != null)
            connections = piece.HoldConnections.Count;
        if (!IsMoving || !isUsing)
            return;

        transform.localPosition += mMovingDirection * SPEED;
        if (piece != null)
        {
            var lp = BoardUtils.GetLineAndPosition(transform.localPosition, mBoardView.board);
            mBoardView.gameView.gameEngine.UpdatePiecePosition(piece, lp.y, lp.x);
        }

        Predict();
    }

    private void Predict()
    {
        for (int i = 0; i < 5; i++)
        {
            var futurePosition = transform.localPosition + (mMovingDirection * (0.8f + (i / 5f)));
            var boardPosition = BoardUtils.GetLineAndPosition(futurePosition, mBoardView.board);

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
        if (!IsMoving || !isUsing)
            return;

        if (collision.tag == "Roof")
            SnapPiece(collision.bounds.ClosestPoint(transform.localPosition));
        else if (collision.tag == "Wall")
            mMovingDirection.x *= -1;
    }
}