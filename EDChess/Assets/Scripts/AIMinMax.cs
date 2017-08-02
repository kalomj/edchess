using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

public enum AIMinMaxJobStatus
{
    None,
    Started,
    StopRequested,
    Finished
}

public class AIMinMaxResult
{
    public Move Move;
    public double AlphaBeta;
    public long TotalStatesSearched;

    public AIMinMaxResult(Move Move, double AlphaBeta, long TotalStatesSearched)
    {
        this.Move = Move;
        this.AlphaBeta = AlphaBeta;
        this.TotalStatesSearched = TotalStatesSearched;
    }
}

/// <summary>
/// This static class implements extension methods on any object that implements the IGameBoardState interface.
/// 
/// Both syncronous and asyncrous AI jobs are implemented.
/// </summary>
public static class AIMinMax
{
    public static int LevelsSearched = 0;
    public static long StatesSearched = 0;
    public static long MovesSearched = 0;

    public static AIMinMaxJobStatus jobStatus = AIMinMaxJobStatus.None;
    public static Thread jobThread;
    private static AIMinMaxResult jobResult;
    public static object jobLock = new object();

    private static System.Random rng = new System.Random();

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    /// <summary>
    /// Recursive alpha beta pruning minimax search strategy.
    /// </summary>
    /// <param name="gbs">The Game Board State.</param>
    /// <param name="searchLevels">The number of levels to search (depth)</param>
    /// <param name="currentPlayer">The current player's turn.</param>
    /// <param name="BestMove">The best move found during the search</param>
    /// <param name="alpha">Starting alpha value.</param>
    /// <param name="beta">Starting beta value.</param>
    /// <returns></returns>
    public static AIMinMaxResult AIMinMaxSearch(this IGameBoardState gbs, int searchLevels, Player.PlayerNumber currentPlayer, bool QuiescenceSearch = false, double alpha = -1.0, double beta = 1.0)
    {
        Move BestMove = null;

        double checkWinner = gbs.CheckWinner();

        //cutoff for search (recursive base cases)
        if(checkWinner == -1.0)
        {
            return new AIMinMaxResult(BestMove, -1.0, 1);
        }

        if (checkWinner == 1.0)
        {
            return new AIMinMaxResult(BestMove, 1.0, 1);
        }

        if (searchLevels == 0)
        {
            return new AIMinMaxResult(BestMove,gbs.CalculateUtility(),1);
        }

        AIMinMaxResult result = null;
        long statesSearched = 0;
        //iterate by looking at all possible moves for each piece
        List<IPieceState> pieces = gbs.GetAlivePieces().Where(s => s.GetPlayer() == currentPlayer).Select(s => s).ToList();
        foreach (IPieceState piece in pieces.Shuffle())
        {
            List<Move> moves;
            if (QuiescenceSearch)
            {
                moves = MoveGenerator.GetCaptureMoves(gbs, piece);
            }
            else
            {
                moves = MoveGenerator.GetMoves(gbs, piece);
            }

            MovesSearched += moves.Count;

            //perform each move on a cloned board and search clone recursively, swapping players each turn
            foreach (Move move in moves.Shuffle())
            {
                IGameBoardState clone = gbs.Clone();
                clone.Move(move.piece, move.space);

                if (currentPlayer == Player.PlayerNumber.Player1)
                {
                    result = clone.AIMinMaxSearch(searchLevels - 1, Player.PlayerNumber.Player2, true, alpha, beta);
                    statesSearched += result.TotalStatesSearched;
                    if(statesSearched > StatesSearched)
                    {
                        StatesSearched = statesSearched;
                    }
                    if (result.AlphaBeta > alpha)
                    {
                        alpha = result.AlphaBeta;
                        BestMove = move;
                    }

                    //beta cut off
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                else /* (currentPlayer == Player.PlayerNumber.Player2) */
                {
                    result = clone.AIMinMaxSearch(searchLevels - 1, Player.PlayerNumber.Player1, true, alpha, beta);
                    statesSearched += result.TotalStatesSearched;
                    if (statesSearched > StatesSearched)
                    {
                        StatesSearched = statesSearched;
                    }
                    if (result.AlphaBeta < beta)
                    {
                        beta = result.AlphaBeta;
                        BestMove = move;
                    }

                    //alpha cut off
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                if (jobStatus == AIMinMaxJobStatus.StopRequested && LevelsSearched > 0)
                {
                    searchLevels = 1;
                }
            }

            if(jobStatus == AIMinMaxJobStatus.StopRequested && LevelsSearched > 0)
            {
                searchLevels = 1;
            }
        }

        //no moves found, treat as a base case
        if(BestMove == null)
        {
            return new AIMinMaxResult(BestMove, gbs.CalculateUtility(), 1);
        }
        
        //Create a result and return it
        return new AIMinMaxResult(BestMove, result.AlphaBeta, statesSearched);
    }

    /// <summary>
    /// Returns 1 if player 1 is a winner, -1 if player 2 is a winner, and 0 if it is not a win state
    /// </summary>
    /// <param name="gbs"></param>
    /// <returns></returns>
    public static double CheckWinner(this IGameBoardState gbs)
    {
        int p1Count = 0;
        int p2Count = 0;

        foreach (IPieceState piece in gbs.GetAlivePieces())
        {
            if (piece.GetPlayer() == Player.PlayerNumber.Player1 && piece.GetPieceType() == Piece.PieceType.king)
            {
                p1Count++;
            }
            else if (piece.GetPlayer() == Player.PlayerNumber.Player2 && piece.GetPieceType() == Piece.PieceType.king)
            {
                p2Count++;
            }
        }

        if(p1Count == 0 && p2Count == 0)
        {
            throw new System.Exception("Game should end as soon as one player reaches 0 kings.");
        }
        else if(p1Count == 0)
        {
            return -1.0;
        }
        else if(p2Count == 0)
        {
            return 1.0;
        }
        else
        {
            return 0.0;
        }
    }

    public static void AIMinMaxSearchAsyncBegin(this IGameBoardState gbs, int searchLevels, Player.PlayerNumber currentPlayer)
    {
        if(jobStatus != AIMinMaxJobStatus.None)
        {
            throw new System.Exception("Only 1 async job should run at a time. This must be enforced in the game logic.");
        }

        jobStatus = AIMinMaxJobStatus.Started;
        LevelsSearched = 0;
        StatesSearched = 0;
        MovesSearched = 0;
        Debug.Log("AI Thread Started");

        //clone gameboardstate to ensure it is not a MonoBehavior - we need to pass it to a new thread
        IGameBoardState gbs_clone = gbs.Clone();

        jobThread = new Thread(() =>
        {
            ///must run at least one level to complete
            jobResult = gbs_clone.AIMinMaxSearch(1, currentPlayer);
            LevelsSearched++;
            //code is in an iterative deepening pattern, but I is initialized to the deepest level for testing
            //usually i should start at 2 and iterate forward
            for (int i = searchLevels; i <= searchLevels; i++)
            {
                AIMinMaxResult res = gbs_clone.AIMinMaxSearch(i, currentPlayer);
                
                if (jobStatus == AIMinMaxJobStatus.StopRequested)
                {
                    break;
                }

                LevelsSearched++;
                jobResult = res;
            }

            lock(jobLock)
            {
                jobStatus = AIMinMaxJobStatus.Finished;
                Debug.Log("AI Thread Finished");
            }
        });
        jobThread.IsBackground = true;
        jobThread.Start();
    }


    /// <summary>
    /// Asyncronous end blocks the calling thread until it's finished.
    /// </summary>
    /// <returns></returns>
    public static AIMinMaxResult AIMinMaxSearchAsyncEnd(this IGameBoardState gbs)
    {
        Debug.Log("Tried ending AI thread in state " + jobStatus.ToString());

        if(jobStatus != AIMinMaxJobStatus.Started && jobStatus != AIMinMaxJobStatus.Finished)
        {
            throw new System.Exception("Impossible to end an AI job that has not started.");
        }

        lock(jobLock)
        {
            if (jobStatus != AIMinMaxJobStatus.Finished)
            {
                jobStatus = AIMinMaxJobStatus.StopRequested;
            }
        }

        jobThread.Join();

        jobStatus = AIMinMaxJobStatus.None;

        return jobResult;
    }

    public static void JoinThreads()
    {
        if(jobThread != null)
        {
            lock (jobLock)
            {
                if (jobStatus != AIMinMaxJobStatus.Finished)
                {
                    jobStatus = AIMinMaxJobStatus.StopRequested;
                }
            }

            jobThread.Join();
            jobStatus = AIMinMaxJobStatus.None;
        }
    }

    public static void AiMinMaxSearchAsyncStopRequest(this IGameBoardState gbs)
    {
        if (jobStatus != AIMinMaxJobStatus.Started && jobStatus != AIMinMaxJobStatus.Finished)
        {
            throw new System.Exception("Impossible to end an AI job that has not started.");
        }

        lock(jobLock)
        {
            if (jobStatus == AIMinMaxJobStatus.Started)
            {
                jobStatus = AIMinMaxJobStatus.StopRequested;
            }
        }
    }
}
