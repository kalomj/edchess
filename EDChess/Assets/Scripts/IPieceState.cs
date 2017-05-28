using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IPieceState
{
    Player.PlayerNumber GetPlayer();
    ISpaceState GetSpaceState();
    Piece.PieceType GetPieceType();
    PieceState CreatePieceState();
    Color GetPieceTint();
}

