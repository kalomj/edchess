using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Move
{
    public Piece piece;
    public Space space;

    public enum MoveType { cap, nocap };
    public MoveType moveType;

    public Move(Piece piece, Space space, MoveType moveType)
    {
        this.piece = piece;
        this.space = space;
        this.moveType = moveType;
    }
}

