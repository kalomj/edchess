using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsEvents : MonoBehaviour {

    public Game game;
    public Text p1Score;
    public Text p2Score;

    public int p1RawScore;
    public int p2RawScore;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        int p1Count = 0;
        int p2Count = 0;

        foreach (Piece piece in game.gameBoard.AlivePieces)
        {
            if (piece.player.playerNumber == Player.PlayerNumber.Player1 && piece.pieceType == Piece.PieceType.king)
            {
                p1Count++;
            }
            else if(piece.player.playerNumber == Player.PlayerNumber.Player2 && piece.pieceType == Piece.PieceType.king)
            {
                p2Count++;
            }
        }

        p1RawScore = p1Count;
        p2RawScore = p2Count;

        p1Score.text = p1RawScore.ToString();
        p2Score.text = p2RawScore.ToString();
    }
}
