using UnityEngine;
using BAMEngine;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;

public class BoardView : MonoBehaviour
{
    public Board board { get; private set; }
    public List<PieceView> viewPieces { get; private set; } = new List<PieceView>();
    public GameView gameView { get; private set; }

    public void Initiate(GameView gameView)
    {
        this.gameView = gameView;
        board = gameView.gameEngine.board;
        DrawBoard();
    }

    private void DrawBoard()
    {
        for (int i = 0; i < board.lines.Count; i++)
        {
            var line = board.lines[i];
            for (int j = 0; j < line.Count; j++)
            {
                if (line[j] == null) continue;
                var piece = gameView.piecesPool.Collect();
                piece.transform.localPosition = new Vector3(j + (line.IsShortLine ? 0.5f : 0f), -i, 0f);
                piece.Initiate(this, line[j]);
                viewPieces.Add(piece);
            }
        }
    }

    public void StepDown(float timeToDown)
    {
        foreach (var p in viewPieces)
        {
            p.transform.DOLocalMoveY(p.transform.localPosition.y - 1f, timeToDown).SetEase(Ease.OutBounce);
        }
    }

    public PieceView GetPiece(Piece piece)
    {
        return GetPiece(piece.Line.Index, piece.Index);
    }

    public PieceView GetPiece(int line, int position)
    {
        return viewPieces.FirstOrDefault(x => x.piece.Line.Index == line && x.piece.Index == position);
    }

    public void LockPiece(PieceView piece)
    {
        viewPieces.Add(piece);
    }

    public void RemovePiece(PieceView piece)
    {
        viewPieces.Remove(piece);
    }
}
