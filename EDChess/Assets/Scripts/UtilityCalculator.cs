using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityCalculator {

    private static Dictionary<Piece.PieceType, int> ValueMap;

    static UtilityCalculator()
    {
        ValueMap = new Dictionary<Piece.PieceType, int>();
        ValueMap.Add(Piece.PieceType.pawn, 1);
        ValueMap.Add(Piece.PieceType.knight, 3);
        ValueMap.Add(Piece.PieceType.bishop, 3);
        ValueMap.Add(Piece.PieceType.rook, 5);
        ValueMap.Add(Piece.PieceType.queen, 9);
        ValueMap.Add(Piece.PieceType.king, 200);
    }
	
}
