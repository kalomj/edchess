using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Move
{
    public IPieceState piece;
    public ISpaceState space;

    public enum MoveType { cap, nocap };
    public MoveType moveType;

    public Move(IPieceState piece, ISpaceState space, MoveType moveType)
    {
        this.piece = piece;
        this.space = space;
        this.moveType = moveType;
    }
}

