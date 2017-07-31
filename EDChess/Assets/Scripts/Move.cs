using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Move
{
    public IPieceState piece;
    public ISpaceState space;

    public enum MoveType { none, cap, nocap };
    public MoveType moveType;

    public Move(IPieceState piece, ISpaceState space, MoveType moveType)
    {
        this.piece = piece;
        this.space = space;
        this.moveType = moveType;
    }

    public override string ToString()
    {
        return piece.GetPieceType() + "@(" + piece.GetSpaceState().GetLevel() + "," + piece.GetSpaceState().GetRow() + "," + piece.GetSpaceState().GetCol() + ") to (" +
            space.GetLevel() + "," + space.GetRow() + "," + space.GetCol() + ")";
    }
}

