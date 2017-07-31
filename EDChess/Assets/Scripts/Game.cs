using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public Player player1;
    public Player player2;
    public GameBoard gameBoard;
    public Camera mainCamera;
    public UIController uiController;
    public int AIThoughtDepth = 1;
    public long MaxStatesToSearch = 10000;

    public float MoveTime = 1f;

    System.Random sysRandom = new System.Random();

    public AIMinMaxResult lastResult;

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
        AIMinMaxResult aiResult;
        yield return new WaitForSeconds(1f);
        while (true)
        {

            //start thinking asyncronously
            gameBoard.AIMinMaxSearchAsyncBegin(AIThoughtDepth+3, Player.PlayerNumber.Player1);
            //a player can think about his move for as long as it takes his opponent's move to animate
            while (AIMinMax.jobStatus == AIMinMaxJobStatus.Started || AIMinMax.jobStatus == AIMinMaxJobStatus.StopRequested)
            {
                yield return new WaitForSeconds(MoveTime);
                if(AIMinMax.jobStatus == AIMinMaxJobStatus.Started && AIMinMax.StatesSearched > MaxStatesToSearch)
                {
                    gameBoard.AiMinMaxSearchAsyncStopRequest();
                }
            }
            //finish thinking and get the move
            aiResult = gameBoard.AIMinMaxSearchAsyncEnd();
            lastResult = aiResult;
            //this is the best place to stop if the game should be paused for testing
            while (Paused)
            {
                yield return new WaitForSeconds(MoveTime);
            }

            gameBoard.Move(aiResult.Move);
            
            //render the gameboard to the 2d representation
            uiController.RenderBoard(gameBoard);

            if (CheckGameOver())
            {
                yield break;
            }

            //Do it again for player 2, without the comments. Not a subroutine because you can't refactor a yield to a subroutine

            gameBoard.AIMinMaxSearchAsyncBegin(AIThoughtDepth, Player.PlayerNumber.Player2);
            while (AIMinMax.jobStatus == AIMinMaxJobStatus.Started)
            {
                yield return new WaitForSeconds(MoveTime);
            }
            aiResult = gameBoard.AIMinMaxSearchAsyncEnd();
            lastResult = aiResult;
            while (Paused)
            {
                yield return new WaitForSeconds(MoveTime);
            }
            gameBoard.Move(aiResult.Move);
            uiController.RenderBoard(gameBoard);

            if (CheckGameOver())
            {
                yield break;
            }
        }
    }

    public bool CheckGameOver()
    {
        int p1Count = 0;
        int p2Count = 0;

        foreach (Piece piece in gameBoard.AlivePieces)
        {
            if (piece.player.playerNumber == Player.PlayerNumber.Player1 && piece.pieceType == Piece.PieceType.king)
            {
                p1Count++;
            }
            else if (piece.player.playerNumber == Player.PlayerNumber.Player2 && piece.pieceType == Piece.PieceType.king)
            {
                p2Count++;
            }
        }

        if(p1Count == 0 || p2Count == 0)
        {
            return true;
        }
        return false;
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
