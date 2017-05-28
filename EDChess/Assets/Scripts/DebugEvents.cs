using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugEvents : MonoBehaviour {

    public Button testButton;
    public Text statusText;
    public Game game;

    int counter = 0;

    System.Diagnostics.Stopwatch sw;

    System.Random sysRandom = new System.Random();

    void Awake()
    {
        sw = new System.Diagnostics.Stopwatch();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void TestButton()
    {
        counter++;
        statusText.text = counter.ToString();

        sw.Reset();
        sw.Start();

        GameBoardState gbs = game.gameBoard.CreateGameBoardState();
        GameBoardState gbs2 = gbs.Clone();

        DoMove(gbs);
        DoMove(gbs2);
        
        sw.Stop();

        double tickTime = (double)sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;

        statusText.text = "Elapsed Ticks : " + sw.ElapsedTicks + " Frequency : " + System.Diagnostics.Stopwatch.Frequency + " Tick Time : " + tickTime + " Elapsed ms : " + sw.ElapsedMilliseconds;
    }

    public Move GetRandomMove(GameBoardState gameBoard, Player player)
    {
        PieceState piece;
        List<Move> moves = new List<Move>();

        int i = 0;
        do
        {
            piece = gameBoard.AlivePieces[sysRandom.Next(0, gameBoard.AlivePieces.Count)];

            moves = MoveGenerator.GetMoves(gameBoard, piece);
            i++;
        }
        while ((piece.player != player || moves.Count == 0) && i < 10000 && gameBoard.AlivePieces.Count > 0);

        if (i == 10000)
        {
            throw new System.Exception("no move found " + i + " " + gameBoard.AlivePieces.Count);
        }

        return moves[sysRandom.Next(0, moves.Count)];
    }

    private void DoMove(GameBoardState gameBoard)
    {
        Move move;

        move = GetRandomMove(gameBoard, game.player1);
        gameBoard.Move(move);

        move = GetRandomMove(gameBoard, game.player2);
        gameBoard.Move(move);
    }
}
