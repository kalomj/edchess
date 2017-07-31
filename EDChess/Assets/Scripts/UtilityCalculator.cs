using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UtilityCalculator {

    private static Dictionary<Piece.PieceType, int> ValueMap;

    private static int PAWNS_PER_LEVEL = 8;
    private static int KNIGHTS_PER_LEVEL = 2;
    private static int BISHOPS_PER_LEVEL = 2;
    private static int ROOKS_PER_LEVEL = 2;
    private static int QUEENS_PER_LEVEL = 1;
    private static int KINGS_PER_LEVEL = 1;

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
	
    public static double CalculateUtility(this IGameBoardState gbs)
    {
        int p1score = gbs.GetAlivePieces().Where(alive => alive.GetPlayer() == Player.PlayerNumber.Player1).Select(alive => ValueMap[alive.GetPieceType()]).Sum();
        int p2score = gbs.GetAlivePieces().Where(alive => alive.GetPlayer() == Player.PlayerNumber.Player2).Select(alive => ValueMap[alive.GetPieceType()]).Sum();
        return (double)(p1score - p2score) / (double)MaxRawScore(gbs.GetNumLevels());
    }

    private static int MaxRawScore(int numlevels)
    {
        return (PAWNS_PER_LEVEL * ValueMap[Piece.PieceType.pawn] +
            KNIGHTS_PER_LEVEL * ValueMap[Piece.PieceType.knight] +
            BISHOPS_PER_LEVEL * ValueMap[Piece.PieceType.bishop] +
            ROOKS_PER_LEVEL * ValueMap[Piece.PieceType.rook]     +
            QUEENS_PER_LEVEL * ValueMap[Piece.PieceType.queen]   +
            KINGS_PER_LEVEL * ValueMap[Piece.PieceType.king])
                                                                 * numlevels;
    }

}
