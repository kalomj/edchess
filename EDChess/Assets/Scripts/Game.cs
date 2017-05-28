﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public Player player1;
    public Player player2;
    public GameBoard gameBoard;
    public Camera mainCamera;
    public UIController uiController;

    public float MoveTime = 1f;

    System.Random sysRandom = new System.Random();

    public bool Paused = false;

    // Use this for initialization
    void Start () {
        StartCoroutine(MovePieceEvent());
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P))
        {
            Paused = !Paused;
        }
	}

    private IEnumerator MovePieceEvent()
    {
        Move move;
        yield return new WaitForSeconds(1f);
        while (true)
        {
            move = GetRandomMove(player1);
            yield return new WaitForSeconds(MoveTime);
            while(Paused)
            {
                yield return new WaitForSeconds(MoveTime);
            }
            gameBoard.Move(move);
            uiController.RenderBoard(gameBoard);

            move = GetRandomMove(player2);
            yield return new WaitForSeconds(MoveTime);
            while (Paused)
            {
                yield return new WaitForSeconds(MoveTime);
            }
            gameBoard.Move(move);
            uiController.RenderBoard(gameBoard);
        }
    }

    public IEnumerator HighlightMoves(List<Move> moves)
    {
        for(int i = 0; i < moves.Count; i++)
        {
           ((Space)moves[i].space).AnimateShell(MoveTime - i* MoveTime / moves.Count, Color.yellow);
            yield return new WaitForSeconds(MoveTime/moves.Count);
        }
    }

    public IEnumerator HighlightMoves(List<Move> moves, float time)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            ((Space)moves[i].space).AnimateShell(time - i * time / moves.Count, Color.yellow);
            yield return new WaitForSeconds(time / moves.Count);
        }
    }

    /// <summary>
    /// select a random piece until a legal list of moves is found. select a random move from the list.
    /// </summary>
    public Move GetRandomMove(Player player)
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

        piece.space.AnimateShell(MoveTime, Color.red);
        StartCoroutine(HighlightMoves(moves));
        return moves[sysRandom.Next(0,moves.Count)];
    }

    
}
