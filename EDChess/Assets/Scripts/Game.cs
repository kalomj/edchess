using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Game : MonoBehaviour {

    public Player player1;
    public Player player2;
    public Player currentPlayer;
    private Player[] players;
    public GameBoard gameBoard;
    public Camera mainCamera;
    public UIController uiController;
    public int AIThoughtDepth = 1;
    public long MaxStatesToSearch = 10000;

    public float MoveTime = 1f;

    System.Random sysRandom = new System.Random();

    public AIMinMaxResult lastResult;

    public bool Paused = false;

    public bool RestartScene = false;

    // Use this for initialization
    void Start () {
        StartCoroutine(MovePieceEvent());
        players = new Player[] { player1, player2 };

    }
	
	// Update is called once per frame
	void Update () {
        TryRestart();
    }

    private IEnumerator MovePieceEvent()
    {
        AIMinMaxResult aiResult = null;
        Move nextMove = null;
        yield return new WaitForSeconds(1f);
        while (true)
        {
            foreach (Player p in players)
            {
                //render the gameboard to the 2d representation
                uiController.RenderBoard(gameBoard);

                currentPlayer = p;
                currentPlayer.playerState = Player.PlayerState.Thinking;

                StartCoroutine(uiController.FadeText(uiController.PlayerTurnText, "Go " + currentPlayer.playerNumber.ToString() + "!", currentPlayer.PieceTint));

                if (currentPlayer.playerType == Player.PlayerType.Computer)
                {
                    //start thinking asyncronously
                    gameBoard.AIMinMaxSearchAsyncBegin(currentPlayer.AIThoughtDepth, currentPlayer.playerNumber);

                    while (AIMinMax.jobStatus == AIMinMaxJobStatus.Started || AIMinMax.jobStatus == AIMinMaxJobStatus.StopRequested)
                    {
                        yield return new WaitForSeconds(MoveTime);
                        if (AIMinMax.jobStatus == AIMinMaxJobStatus.Started && AIMinMax.StatesSearched > MaxStatesToSearch)
                        {
                            gameBoard.AiMinMaxSearchAsyncStopRequest();

                        }
                    }
                    //finish thinking and get the move
                    aiResult = gameBoard.AIMinMaxSearchAsyncEnd();
                    lastResult = aiResult;
                    nextMove = aiResult.Move;
                    currentPlayer.selectedMove = aiResult.Move;
                    currentPlayer.playerState = Player.PlayerState.Moving;
                }
                else if(currentPlayer.playerType == Player.PlayerType.Human)
                {
                    while(currentPlayer.playerState == Player.PlayerState.Thinking)
                    {
                        yield return new WaitForSeconds(MoveTime);

                        if(currentPlayer.playerType == Player.PlayerType.Computer)
                        {
                            //start thinking asyncronously
                            gameBoard.AIMinMaxSearchAsyncBegin(currentPlayer.AIThoughtDepth, currentPlayer.playerNumber);

                            while (AIMinMax.jobStatus == AIMinMaxJobStatus.Started || AIMinMax.jobStatus == AIMinMaxJobStatus.StopRequested)
                            {
                                yield return new WaitForSeconds(MoveTime);
                                if (AIMinMax.jobStatus == AIMinMaxJobStatus.Started && AIMinMax.StatesSearched > MaxStatesToSearch)
                                {
                                    gameBoard.AiMinMaxSearchAsyncStopRequest();

                                }
                            }
                            //finish thinking and get the move
                            aiResult = gameBoard.AIMinMaxSearchAsyncEnd();
                            lastResult = aiResult;
                            nextMove = aiResult.Move;
                            currentPlayer.selectedMove = aiResult.Move;
                            currentPlayer.playerState = Player.PlayerState.Moving;
                        }
                    }

                    nextMove = currentPlayer.selectedMove;
                }

                //this is the best place to stop if the game should be paused
                while (Paused)
                {
                    yield return new WaitForSeconds(MoveTime);
                }

                gameBoard.Move(nextMove);
                currentPlayer.playerState = Player.PlayerState.Waiting;

                if (CheckGameOver())
                {
                    yield break;
                }
            }
        }
    }

    private void TryRestart()
    {
        if (RestartScene)
        {
            AIMinMax.JoinThreads();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    bool flying = false;

    public IEnumerator CenterCameraOnPiece(Piece piece)
    {
        if(!flying)
        {
            flying = true;
            Vector3 pieceVector = piece.transform.position;
            pieceVector.z = pieceVector.z + 10;
            pieceVector.x = pieceVector.x + 5;
            mainCamera.transform.DOMove(pieceVector, 1f);
            yield return new WaitForSeconds(1f);
            mainCamera.transform.DOLookAt(piece.transform.position, 1f);
            yield return new WaitForSeconds(1f);
            flying = false;
        }
        
    }
}
