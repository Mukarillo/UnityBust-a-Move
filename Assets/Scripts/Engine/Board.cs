using System;
using System.Collections.Generic;
using UnityEngine;

namespace BAMEngine
{
    public class Board
    {
        private const int MAX_PIECES_PER_LINE = 8;
        private const int MAX_LINES = 12;
        private const int INITIAL_LINE_AMOUNT = 5;
        private const int MIN_AMOUNT_TO_BREAK = 3;
        public List<PiecesLine> lines = new List<PiecesLine>();

        public override string ToString()
        {
            var boardText = "\n";
            foreach (var line in lines)
            {
                if (line.IsShortLine)
                    boardText += "  ";
                foreach (var piece in line)
                    boardText += piece != null ? piece.ToString() : "[N]";

                boardText += "\n";
            }
            return boardText;
        }

        public Board()
        {
            CreateBoard();
        }

        private void CreateBoard()
        {
            for (var i = 0; i < MAX_LINES; i++)
            {
                var line = new PiecesLine(i);
                var lineAmount = MAX_PIECES_PER_LINE - (line.IsShortLine ? 1 : 0);
                for (var j = 0; j < lineAmount; j++)
                {
                    var pieceToAdd = i < INITIAL_LINE_AMOUNT ? NormalPiece.GetRandom() : null;
                    pieceToAdd?.UpdatePosition(line, j);
                    line.Add(pieceToAdd);
                }

                lines.Add(line);
            }

            UpdateConnections();

            Dump();
        }

        private void UpdateConnections()
        {
            foreach (var line in lines)
            {
                foreach (var piece in line)
                {
                    if(piece != null)
                        RefreshPieceConnections(piece);
                }
            }
        }

        private void RefreshPieceConnections(Piece piece)
        {
            var connections = new List<Piece>();
            var holdConnections = new List<Piece>();
            Piece outPiece = null;
            var lineIndex = piece.Line.Index;
            //UP RIGHT/LEFT
            if (TryGetPiece(lineIndex + 1, piece.Index + (piece.Line.IsShortLine ? 1 : -1), out outPiece))
            {
                connections.Add(outPiece);
                holdConnections.Add(outPiece);
            }
            //UP
            if (TryGetPiece(lineIndex + 1, piece.Index, out outPiece))
            {
                connections.Add(outPiece);
                holdConnections.Add(outPiece);
            }
            //RIGHT
            if (TryGetPiece(lineIndex, piece.Index + 1, out outPiece))
            {
                connections.Add(outPiece);
                holdConnections.Add(outPiece);
            }
            //LEFT
            if (TryGetPiece(lineIndex, piece.Index - 1, out outPiece))
            {
                connections.Add(outPiece);
                holdConnections.Add(outPiece);
            }
            //DOWN
            if (TryGetPiece(lineIndex - 1, piece.Index, out outPiece))
                connections.Add(outPiece);
            //DOWN RIGHT/LEFT
            if (TryGetPiece(lineIndex - 1, piece.Index + (piece.Line.IsShortLine ? 1 : -1), out outPiece))
                connections.Add(outPiece);

            piece.UpdateConnections(connections, holdConnections);
        }

        internal void PlacePiece(Piece piece, int lineIndex, int positionIndex)
        {
            Debug.LogWarning($"Pacing piece: {lineIndex} {positionIndex}");
            lines[lineIndex][positionIndex] = piece;
            piece.UpdatePosition(lines[lineIndex], positionIndex);

            RefreshPieceConnections(piece);
            foreach (var pieceConnection in piece.Connections)
                RefreshPieceConnections(pieceConnection);

            if (piece is SpecialPiece)
                ((SpecialPiece)piece).Execute();
            else if(piece is NormalPiece)
                TryBreaking((NormalPiece)piece);
        }

        private void TryBreaking(NormalPiece piece)
        {
            var piecesToBreak = GetNeighborPiecesSameType(piece, new List<NormalPiece>());

            if (piecesToBreak.Count >= MIN_AMOUNT_TO_BREAK)
            {
                foreach (var p in piecesToBreak)
                    RemoveNormalPiece(p, p.Break);
                CheckFallingPieces();
            }

            Dump();
        }

        private void CheckFallingPieces()
        {
            for (var i = 0; i < lines.Count; i++)
            {
                if(!lines[i].CanFall)
                    continue;

                for (var j = 0; j < lines[i].Count; j++)
                {
                    if(lines[i][j] == null) continue;
                    if (lines[i][j].HoldConnections.Count == 0)
                        RecursiveFall((NormalPiece) lines[i][j]);
                }
            }
        }

        private void RecursiveFall(NormalPiece piece)
        {
            RemoveNormalPiece(piece, piece.Fall);
            foreach (var pieceConnection in piece.Connections)
            {
                if(!pieceConnection.isFalling)
                    RecursiveFall((NormalPiece)pieceConnection);
            }
        }

        private void RemoveNormalPiece(NormalPiece piece, Action act)
        {
            act?.Invoke();
            piece.Line[piece.Index] = null;
            foreach (var pConnection in piece.Connections)
                RefreshPieceConnections(pConnection);
        }

        private List<NormalPiece> GetNeighborPiecesSameType(NormalPiece piece, List<NormalPiece> list)
        {
            foreach (var pieceConnection in piece.Connections.ConvertAll(x => (NormalPiece)x))
            {
                if (pieceConnection.Compare(piece) && !list.Contains(pieceConnection))
                {
                    list.Add(pieceConnection);
                    GetNeighborPiecesSameType(pieceConnection, list);
                }
            }

            return list;
        }

        private bool TryGetPiece(int lineIndex, int index, out Piece piece)
        {
            var lineExists = (lineIndex >= 0 && lineIndex < lines.Count);
            var pieceExists = false;
            piece = null;
            if (lineExists)
            {
                var line = lines[lineIndex];
                pieceExists = index >= 0 && index < line.Count && line[index] != null;
                if (pieceExists)
                    piece = line[index];
            }

            return pieceExists;
        }

        private void Dump()
        {
            Debug.Log(ToString());
        }
    }
}