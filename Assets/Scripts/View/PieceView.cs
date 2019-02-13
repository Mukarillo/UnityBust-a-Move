using UnityEngine;
using BAMEngine;
using System;

public class PieceView : MonoBehaviour
{
    private const float SPEED = 0.15f;

    public Piece piece { get; private set; }

    private SpriteRenderer mSpriteRenderer;
    private bool mIsMoving = false;
    private Vector3 mMovingDirection;

    private void Awake()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initiate(Piece piece)
    {
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
        Debug.Log(transform.position);
        var line = Mathf.FloorToInt(transform.position.y);
        var shortLine = IsShortLine(line);
        var position = Mathf.Clamp(Mathf.Floor(transform.position.x), 0, shortLine ? 6 : 7) + (shortLine ? 0.5f : 0f);
        transform.position = new Vector3(position, line, 0);
        
        Debug.Log(line + " " + Mathf.Abs(line) + " ||| " + position + " " + Mathf.FloorToInt(position));
        GameView.Instance.PlacePieceOnBoard(this, Mathf.Abs(line), Mathf.FloorToInt(position));
    }

    private bool IsShortLine(int index)
    {
        return index % 2 != 0;
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
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!mIsMoving) return;

        if (collision.gameObject.GetComponent<PieceView>() != null)
        {
            //HITTING A PIECE, WE MUST STOP
            mIsMoving = false;
            SnapPiece();
        }
        else
        {
            //HITTING A WALL, WE MUST INVERT DIRECTION
            mMovingDirection.x *= -1;
        }
    }
}
