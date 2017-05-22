using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public Player player1;
    public Player player2;
    public GameBoard gameBoard;
    public Camera mainCamera;

    public float MoveTime = .1f;

    System.Random sysRandom = new System.Random();

    // Use this for initialization
    void Start () {
        StartCoroutine(MovePieceEvent());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator MovePieceEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(MoveTime);
            DoRandomMove(player1);
            yield return new WaitForSeconds(MoveTime);
            DoRandomMove(player2);

        }
    }

    /// <summary>
    /// select a random piece until a legal move is found. perform the move.
    /// </summary>
    public void DoRandomMove(Player player)
    {
        Piece piece;
        List<Move> moves = new List<Move>();

        int i = 0;
        do
        {
            piece = gameBoard.AlivePieces[sysRandom.Next(0, gameBoard.AlivePieces.Count)];

            moves = MoveGenerator.GetMoves(gameBoard, piece);
            i++;
        }
        while ((piece.player != player || moves.Count == 0) && i < 10000 && gameBoard.AlivePieces.Count > 0);

        if(i == 10000)
        {
            throw new System.Exception("no move found " + i + " " + gameBoard.AlivePieces.Count);
        }

        gameBoard.Move(moves[sysRandom.Next(0,moves.Count)]);
    }

    
}
