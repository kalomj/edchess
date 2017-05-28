using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpaceState : ISpaceState
{
    public bool occupied = false;
    public PieceState occupier;
    public int level;
    public int row;
    public int col;

    int ISpaceState.GetCol()
    {
        return col;
    }

    int ISpaceState.GetLevel()
    {
        return level;
    }

    int ISpaceState.GetRow()
    {
        return row;
    }

    bool ISpaceState.IsOccupied()
    {
        return occupied;
    }

    IPieceState ISpaceState.Occupier()
    {
        return occupier;
    }

    SpaceState ISpaceState.CreateSpaceState()
    {
        SpaceState ss = new SpaceState();

        IPieceState ps = ((ISpaceState)this).Occupier();

        if (ps != null)
        {
            ss.occupier = ps.CreatePieceState(); 
        }
        ss.occupied = ((ISpaceState)this).IsOccupied();
        ss.level = this.level;
        ss.row = this.row;
        ss.col = this.col;

        return ss;
    }
}

