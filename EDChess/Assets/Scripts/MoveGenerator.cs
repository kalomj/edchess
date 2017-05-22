using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

static class MoveGenerator
{

    public static List<Move> GetMoves(GameBoard gameBoard, Piece piece)
    {
        List<Move> moves;

        switch(piece.pieceType)
        {
            case Piece.PieceType.pawn:
                moves = GetPawnMoves(gameBoard, piece);
                break;

            case Piece.PieceType.bishop:
                moves = GetBishopMoves(gameBoard, piece);
                break;

            case Piece.PieceType.king:
                moves = GetKingMoves(gameBoard, piece);
                break;

            case Piece.PieceType.knight:
                moves = GetKnightMoves(gameBoard, piece);
                break;

            case Piece.PieceType.queen:
                moves = GetQueenMoves(gameBoard, piece);
                break;

            case Piece.PieceType.rook:
                moves = GetRookMoves(gameBoard, piece);
                break;

            default:
                moves = new List<Move>();
                break;
        }

        return moves;
    }

    private static List<Move> GetPawnMoves(GameBoard gameBoard, Piece piece)
    {
        List<Move> moves = new List<Move>();
        int moveDirection = GetPawnMoveDirection(piece);
        int pawnStartRow = GetPawnStartRow(gameBoard, piece);
        int possibleDistance;


        //check one space ahead, two spaces ahead if on starting row
        if(piece.space.row == pawnStartRow)
        {
            possibleDistance = 2;
        }
        else
        {
            possibleDistance = 1;
        }

        int[] vOffsets = { -1, 0, 1 };
        foreach (int vOffset in vOffsets)
        {
            for (int i = 1; i <= possibleDistance; i++)
            {
                Space cSpace = gameBoard.GetSpace(piece.space.level + vOffset, piece.space.row + moveDirection * i, piece.space.col);

                if (cSpace != null && cSpace.occupied)
                {
                    break;
                }
                else if (cSpace != null)
                {
                    moves.Add(new Move(piece, cSpace, Move.MoveType.nocap));
                }
            }
        }

        //check diagonals for possible capture
        int[] cOffsets = { -1, 1 };

        foreach (int vOffset in vOffsets)
        {
            foreach (int cOffset in cOffsets)
            {
                Space cSpace = gameBoard.GetSpace(piece.space.level, piece.space.row + moveDirection, piece.space.col + cOffset);

                if (cSpace != null && cSpace.occupied && cSpace.occupier.player.playerNumber != piece.player.playerNumber)
                {
                    moves.Add(new Move(piece, cSpace, Move.MoveType.cap));
                }
            }
        }

        //TODO: check for en passant

        return moves;
    }

    private static int GetPawnStartRow(GameBoard gameBoard, Piece piece)
    {
        int pawnStartRow;
        if (piece.player.playerNumber == Player.PlayerNumber.Player1)
        {
            pawnStartRow = 1;
        }
        else
        {
            pawnStartRow = gameBoard.GridTemplate.rows - 2;
        }

        return pawnStartRow;
    }

    private static int GetPawnMoveDirection(Piece piece)
    {
        int moveDirection;
        if (piece.player.playerNumber == Player.PlayerNumber.Player1)
        {
            moveDirection = 1;
        }
        else if (piece.player.playerNumber == Player.PlayerNumber.Player2)
        {
            moveDirection = -1;
        }
        else
        {
            throw new Exception("Player not set on piece.");
        }

        return moveDirection;
    }

    private static List<Move> GetKingMoves(GameBoard gameBoard, Piece piece)
    {
        List<Move> moves = new List<Move>();

        return moves;
    }

    private static List<Move> GetRookMoves(GameBoard gameBoard, Piece piece)
    {
        List<Move> moves = new List<Move>();

        return moves;
    }

    private static List<Move> GetKnightMoves(GameBoard gameBoard, Piece piece)
    {
        List<Move> moves = new List<Move>();

        return moves;
    }

    private static List<Move> GetQueenMoves(GameBoard gameBoard, Piece piece)
    {
        List<Move> moves = new List<Move>();

        return moves;
    }

    private static List<Move> GetBishopMoves(GameBoard gameBoard, Piece piece)
    {
        List<Move> moves = new List<Move>();

        return moves;
    }

    private static List<Move> GetMovesInDirection(GameBoard gameBoard, Piece piece, int levelDirection, int rowDirection, int colDirection)
    {
        List<Move> moves = new List<Move>();

        if(levelDirection < -1 || levelDirection > 1)
        {
            throw new Exception("not a valid direction");
        }

        if (rowDirection < -1 || rowDirection > 1)
        {
            throw new Exception("not a valid direction");
        }

        if (colDirection < -1 || colDirection > 1)
        {
            throw new Exception("not a valid direction");
        }

        while(true)
        {
            Space cSpace = gameBoard.GetSpace(piece.space.level + levelDirection, piece.space.row + rowDirection, piece.space.col + colDirection);

            if(cSpace == null)
            {
                break;
            }

            if(cSpace.occupied && cSpace.occupier.player.playerNumber != piece.player.playerNumber)
            {
                moves.Add(new Move(piece, cSpace, Move.MoveType.cap));
                break;
            }

            if(cSpace.occupied && cSpace.occupier.player.playerNumber == piece.player.playerNumber)
            {
                break;
            }

            if(!cSpace.occupied)
            {
                moves.Add(new Move(piece, cSpace, Move.MoveType.nocap));
            }
        }
    }
}
