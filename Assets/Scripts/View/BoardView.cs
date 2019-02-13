using UnityEngine;
using BAMEngine;
using System.Collections.Generic;

public class BoardView : MonoBehaviour
{
    public Board board { get; private set; }

    private List<PieceView> mViewPieces = new List<PieceView>();

    private void Start()
    {
        board = GameView.Instance.gameEngine.board;
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
                var piece = GameObject.Instantiate(GameView.Instance.piecePrefab, new Vector3(j + (line.IsShortLine ? 0.5f : 0f), -i, 0f), Quaternion.identity).AddComponent<PieceView>();
                piece.Initiate(line[j]);
                mViewPieces.Add(piece);
            }
        }
    }

    public void PlacePiece(PieceView piece, int line, int position)
    {
        mViewPieces.Add(piece);
    }
}
