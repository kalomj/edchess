using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PieceState : IPieceState
{
    public SpaceState space;

    public Piece.PieceType pieceType;

    public Player player;

    public bool Alive = true;

    Piece.PieceType IPieceState.GetPieceType()
    {
        return pieceType;
    }

    Player.PlayerNumber IPieceState.GetPlayer()
    {
        return player.playerNumber;
    }

    ISpaceState IPieceState.GetSpaceState()
    {
        return space;
    }

    PieceState IPieceState.CreatePieceState()
    {
        PieceState ps = new PieceState();

        ps.Alive = this.Alive;
        ps.pieceType = this.pieceType;
        ps.player = this.player;

        return ps;
    }

    Color IPieceState.GetPieceTint()
    {
        return player.PieceTint;
    }
}
