using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

static class MoveGenerator
{

    public static List<Move> GetMoves(IGameBoardState gameBoard, IPieceState piece)
    {
        List<Move> moves;

        switch(piece.GetPieceType())
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

    public static List<Move> GetCaptureMoves(IGameBoardState gameBoard, IPieceState piece)
    {
        List<Move> moves;

        switch (piece.GetPieceType())
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

        return moves.Where(s => s.moveType == Move.MoveType.cap).Select(s => s).ToList();
    }

    private static List<Move> GetPawnMoves(IGameBoardState gameBoard, IPieceState piece)
    {
        List<Move> moves = new List<Move>();
        int moveDirection = GetPawnMoveDirection(piece);
        int pawnStartRow = GetPawnStartRow(gameBoard, piece);
        int possibleDistance;


        //check one space ahead, two spaces ahead if on starting row
        if(piece.GetSpaceState().GetRow() == pawnStartRow)
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
                ISpaceState cSpace = gameBoard.GetSpaceState(piece.GetSpaceState().GetLevel() + vOffset, piece.GetSpaceState().GetRow() + moveDirection * i, piece.GetSpaceState().GetCol());

                if (cSpace != null && cSpace.IsOccupied())
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
                ISpaceState cSpace = gameBoard.GetSpaceState(piece.GetSpaceState().GetLevel(), piece.GetSpaceState().GetRow() + moveDirection, piece.GetSpaceState().GetCol() + cOffset);

                if (cSpace != null && cSpace.IsOccupied() && cSpace.Occupier().GetPlayer() != piece.GetPlayer())
                {
                    moves.Add(new Move(piece, cSpace, Move.MoveType.cap));
                }
            }
        }

        //TODO: check for en passant

        return moves;
    }

    private static int GetPawnStartRow(IGameBoardState gameBoard, IPieceState piece)
    {
        int pawnStartRow;
        if (piece.GetPlayer() == Player.PlayerNumber.Player1)
        {
            pawnStartRow = 1;
        }
        else
        {
            pawnStartRow = gameBoard.GetNumRows() - 2;
        }

        return pawnStartRow;
    }

    private static int GetPawnMoveDirection(IPieceState piece)
    {
        int moveDirection;
        if (piece.GetPlayer() == Player.PlayerNumber.Player1)
        {
            moveDirection = 1;
        }
        else if (piece.GetPlayer() == Player.PlayerNumber.Player2)
        {
            moveDirection = -1;
        }
        else
        {
            throw new Exception("Player not set on piece.");
        }

        return moveDirection;
    }

    private static List<Move> GetKingMoves(IGameBoardState gameBoard, IPieceState piece)
    {
        List<Move> moves = new List<Move>();

        //king can move one space in any direction, so generate cartesian product of all possible directions
        int[] rowDirections = { -1, 0, 1 };
        int[] colDirections = { -1, 0, 1 };
        int[] lvlDirections = { -1, 0, 1 };

        foreach(int rowDirection in rowDirections)
        {
            foreach(int colDirection in colDirections)
            {
                foreach(int lvlDirection in lvlDirections)
                {
                    moves.AddRange(GetMovesInDirection(gameBoard, piece, lvlDirection, rowDirection, colDirection, 1).ToArray());
                }
            }
        }

        //TODO: castle move

        return moves;
    }

    private static List<Move> GetRookMoves(IGameBoardState gameBoard, IPieceState piece)
    {
        List<Move> moves = new List<Move>();

        //rook can move straight forward, back, left, right, up, or down 
        int[,] triplets = { { 0, 1, 0 }, { 0, -1, 0 }, { 0, 0, 1 }, { 0, 0, -1 }, { 1, 0, 0 }, { -1, 0, 0 } };

        for(int i = 0; i < triplets.GetLength(0); i++)
        {
            moves.AddRange(GetMovesInDirection(gameBoard, piece, triplets[i,0], triplets[i,1], triplets[i,2]).ToArray());
        }

        return moves;
    }

    private static List<Move> GetKnightMoves(IGameBoardState gameBoard, IPieceState piece)
    {
        List<Move> moves = new List<Move>();

        //knight moves 2 forward 1 left, 2 forward 1 right, 2 right 1 foward, 2 right 1 back, 2 left 1 forward, 2 left 1 back, 2 back 1 left, 2 back 1 right
        //these moves can happen on the same level, 2 levels up, or 2 levels down only.
        int[,] doublets = { { 2, 1 }, { 2, -1 }, { 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 }, { -2, 1 }, { -2, -1 } };
        int[] levelDirections = { -2, 0, 2 };

        foreach (int levelDirection in levelDirections)
        {
            for (int i = 0; i < doublets.GetLength(0); i++)
            {
                moves.AddRange(GetMovesInDirection(gameBoard, piece, levelDirection, doublets[i, 0], doublets[i, 1],  1).ToArray());
            }
        }

        return moves;
    }

    private static List<Move> GetQueenMoves(IGameBoardState gameBoard, IPieceState piece)
    {
        List<Move> moves = new List<Move>();

        //queen can move one space in any direction, so generate cartesian product of all possible directions
        int[] rowDirections = { -1, 0, 1 };
        int[] colDirections = { -1, 0, 1 };
        int[] lvlDirections = { -1, 0, 1 };

        foreach (int rowDirection in rowDirections)
        {
            foreach (int colDirection in colDirections)
            {
                foreach (int lvlDirection in lvlDirections)
                {
                    moves.AddRange(GetMovesInDirection(gameBoard, piece, lvlDirection, rowDirection, colDirection).ToArray());
                }
            }
        }

        return moves;
    }

    private static List<Move> GetBishopMoves(IGameBoardState gameBoard, IPieceState piece)
    {
        List<Move> moves = new List<Move>();

        //bishop can move diagonally foward-left, foward-right, back-left, back-right, forward-left-up, forward-left-down, forward-right-up, forward-right-down, back-left-up, back-left-down, back-right-up, back-right-down
        int[,] triplets = { { 0, 1, 1 }, { 0, 1, -1 }, { 0, -1, 1 }, { 0, -1, -1 }, { 1, 1, 1 }, { -1, 1, 1 }, { 1, 1, -1 }, { -1, 1, -1 }, { 1, -1, 1 }, { -1, -1, 1 }, { 1, -1, -1 }, { -1, -1, -1 } };

        for (int i = 0; i < triplets.GetLength(0); i++)
        {
            moves.AddRange(GetMovesInDirection(gameBoard, piece, triplets[i, 0], triplets[i, 1], triplets[i, 2]).ToArray());
        }

        return moves;
    }

    private static List<Move> GetMovesInDirection(IGameBoardState gameBoard, IPieceState piece, int levelDirection, int rowDirection, int colDirection, int max=99)
    {
        List<Move> moves = new List<Move>();

        int levelChange = levelDirection;
        int rowChange = rowDirection;
        int colChange = colDirection;

        for(int i = 0; i < max; i++)
        {
            ISpaceState cSpace = gameBoard.GetSpaceState(piece.GetSpaceState().GetLevel()+ levelChange, piece.GetSpaceState().GetRow() + rowChange, piece.GetSpaceState().GetCol() + colChange);

            if(cSpace == null)
            {
                break;
            }

            if(cSpace.IsOccupied() && cSpace.Occupier().GetPlayer() != piece.GetPlayer())
            {
                //last move in the sequence is a cap move
                moves.Add(new Move(piece, cSpace, Move.MoveType.cap));
                break;
            }

            if(cSpace.IsOccupied() && cSpace.Occupier().GetPlayer() == piece.GetPlayer())
            {
                break;
            }

            if(!cSpace.IsOccupied())
            {
                moves.Add(new Move(piece, cSpace, Move.MoveType.nocap));
                //amplify move for next iteration
                levelChange += levelDirection;
                rowChange += rowDirection;
                colChange += colDirection;
            }
        }

        return moves;
    }
}
