using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameBoardState : IGameBoardState
{
    public List<List<List<SpaceState>>> spaces;
    public List<PieceState> AlivePieces;
    public List<PieceState> DeadPieces;

    private int levels;
    private int rows;
    private int cols;

    public GameBoardState(int levels, int rows, int cols)
    {
        this.levels = levels;
        this.rows = rows;
        this.cols = cols;

        AlivePieces = new List<PieceState>();
        DeadPieces = new List<PieceState>();

        spaces = new List<List<List<SpaceState>>>();

        for(int i = 0; i < levels; i++)
        {
            spaces.Add(new List<List<SpaceState>>());

            for(int j = 0; j < rows; j++)
            {
                spaces[i].Add(new List<SpaceState>());

                for(int k = 0; k < cols; k++)
                {
                    spaces[i][j].Add(new SpaceState());

                    spaces[i][j][k].level = i;
                    spaces[i][j][k].row = j;
                    spaces[i][j][k].col = k;
                }
            }
        }
    }

    int IGameBoardState.GetNumCols()
    {
        return cols;
    }

    int IGameBoardState.GetNumLevels()
    {
        return levels;
    }

    int IGameBoardState.GetNumRows()
    {
        return rows;
    }

    ISpaceState IGameBoardState.GetSpaceState(int level, int row, int col)
    {
        //Debug.Log(level + " " + row + " " + col);
        if(levels > level && rows > row && cols > col && level >= 0 && row >= 0 && col >= 0)
        {
            return spaces[level][row][col];
        }
        else
        {
            return null;
        }
    }

    public GameBoardState Clone()
    {
        GameBoardState newGbs = new GameBoardState(this.levels, this.rows, this.cols);

        IGameBoardState igbs = this;

        for (int lvl = 0; lvl < (igbs.GetNumLevels()); lvl++)
        {
            for (int row = 0; row < (igbs.GetNumRows()); row++)
            {
                for (int col = 0; col < (igbs.GetNumCols()); col++)
                {
                    ISpaceState iss = igbs.GetSpaceState(lvl, row, col);
                    newGbs.spaces[lvl][row][col].occupied = iss.IsOccupied();
                    IPieceState ips = iss.Occupier();

                    if (ips != null)
                    {
                        PieceState ps = ips.CreatePieceState();
                        newGbs.spaces[lvl][row][col].occupier = ps;
                        newGbs.AlivePieces.Add(ps);
                        ps.space = newGbs.spaces[lvl][row][col];
                    }
                }
            }
        }

        foreach (IPieceState piece in DeadPieces)
        {
            newGbs.DeadPieces.Add(piece.CreatePieceState());
        }

        return newGbs;
    }

    public void Move(PieceState piece, SpaceState space)
    {
        if (space.occupied)
        {
            DeadPieces.Add(space.occupier);
            AlivePieces.Remove(space.occupier);
            space.occupier.Alive = false;
        }

        piece.space.occupied = false;
        piece.space.occupier = null;
        piece.space = space;
        space.occupied = true;
        space.occupier = piece;
    }

    public void Move(Move move)
    {
        Move((PieceState)move.piece, (SpaceState)move.space);
    }

    void IGameBoardState.Move(IPieceState piece, ISpaceState space)
    {
        this.Move((PieceState)piece, (SpaceState)space);
    }
}

