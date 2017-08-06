using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameBoard : MonoBehaviour, IGameBoardState {

    public List<Level> Levels;
    public List<Piece> AlivePieces;
    public List<Piece> DeadPieces;

    public Level LevelTemplate;
    public Grid GridTemplate;
    public Space SpaceTemplate;
    public Piece PieceTemplate;

    public Game game;

    public int numberLevels = 8;

    System.Random sysRandom = new System.Random();

    private void Awake()
    {
        Levels = new List<Level>();
        AlivePieces = new List<Piece>();
        DeadPieces = new List<Piece>();

        for (int i = 0; i < numberLevels; i++)
        {
            if (LevelTemplate != null)
            {
                LevelTemplate.level = i;
                Level lvl = Instantiate(LevelTemplate);
                Levels.Add(lvl);
            }

            IEnumerator<Piece.PieceType> ptEnumerator = GetPieceStartingTypeEnumerator(GridTemplate.cols);

            for (int k = 0; k < 4; k++)
            {
                for (int j = 0; j < GridTemplate.cols; j++)
                {
                    if (ptEnumerator.MoveNext())
                    {
                        PieceTemplate.pieceType = ptEnumerator.Current;
                    }

                    Piece piece = Instantiate(PieceTemplate);
                    AlivePieces.Add(piece); 
                }
            }
        }

        SpaceTemplate.GetComponent<MeshRenderer>().enabled = false;
        PieceTemplate.GetComponent<MeshRenderer>().enabled = false;
    }

    void Start () {
        IEnumerator<Space> spaceEnumerator = GetPieceStartingSpaceEnumerator();
        foreach(Piece piece in AlivePieces)
        {
            if (spaceEnumerator.MoveNext())
            {
                piece.transform.position = spaceEnumerator.Current.transform.position;
                spaceEnumerator.Current.occupied = true;
                spaceEnumerator.Current.occupier = piece;
                piece.space = spaceEnumerator.Current;

                if (piece.space.row < 2)
                {
                    //p1 pieces
                    piece.SetTint(game.player1.PieceTint);
                    piece.SetPlayer(game.player1);
                }
                else
                {
                    //p2 pieces
                    piece.SetTint(game.player2.PieceTint);
                    piece.SetPlayer(game.player2);
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator<Space> GetPieceStartingSpaceEnumerator()
    {
        int[] pieceRows = { 0, 1, GridTemplate.rows - 2, GridTemplate.rows - 1 };

        foreach (Level lvl in Levels)
        {
            foreach (int i in pieceRows)
            {
                for (int j = 0; j < GridTemplate.cols; j++)
                {
                    yield return lvl.GridInstance.GetSpace(i, j);
                }
            }
        }
    }

    public IEnumerator<Piece.PieceType> GetPieceStartingTypeEnumerator(int cols)
    {
        
        Piece.PieceType[] special = { Piece.PieceType.rook, Piece.PieceType.knight, Piece.PieceType.bishop,
                                      Piece.PieceType.queen, Piece.PieceType.king, Piece.PieceType.bishop,
                                      Piece.PieceType.knight, Piece.PieceType.rook };

        //return cols special pieces, then cols pawns, then cols pawns, then cols special pieces
        for (int i = 0; i < cols; i++)
        {
            yield return special[i % special.Length];
        }

        for (int i = 0; i < cols*2; i++)
        {
            yield return Piece.PieceType.pawn;
        }

        for (int i = 0; i < cols; i++)
        {
            yield return special[i % special.Length];
        }
    }

    /// <summary>
    /// Gets a space in the gameboard, returns null if the space is not legally indexed.
    /// </summary>
    /// <param name="level">The level of the chess board in 3D</param>
    /// <param name="row">The row of the chess board</param>
    /// <param name="col">The column of the chess board</param>
    /// <returns></returns>
    public Space GetSpace(int level, int row, int col)
    {
        if(level >= numberLevels || level < 0)
        {
            return null;
        }

        return Levels[level].GridInstance.GetSpace(row, col);
    }

    
    public void Move(Piece piece, Space space)
    {
        if(space.occupied)
        {
            DeadPieces.Add(space.occupier);
            AlivePieces.Remove(space.occupier);
            space.occupier.enabled = false;
            space.occupier.Alive = false;
            
        }

        piece.transform.position = space.transform.position;
        piece.space.occupied = false;
        piece.space.occupier = null;
        piece.space.AnimateShell(.5f, Color.grey);
        piece.space = space;
        space.occupied = true;
        space.occupier = piece;
        space.AnimateShell(.5f, Color.blue);
    }

    public bool Move(Move move)
    {
        //Move objects can be passed between games so we
        //have to look up the piece and space owned by this game
        //and check to ensure the move is applicable to this game

        IPieceState ips = move.piece;
        ISpaceState iss_dest = move.space;
        ISpaceState iss_source = ips.GetSpaceState();

        ISpaceState this_iss_dest = this.GetSpace(iss_dest.GetLevel(), iss_dest.GetRow(), iss_dest.GetCol());
        ISpaceState this_iss_source = this.GetSpace(iss_source.GetLevel(), iss_source.GetRow(), iss_source.GetCol());

        if(!this_iss_source.IsOccupied() || this_iss_source.Occupier().GetPieceType() != ips.GetPieceType() || this_iss_source.Occupier().GetPlayer() != ips.GetPlayer())
        {
            Debug.Log("Illegal move attempted. Skipping turn." + move.ToString());

            if(!this_iss_source.IsOccupied())
            {
                Debug.Log("Source is not occupied in real game board.");
            }
            else if(this_iss_source.Occupier().GetPieceType() != ips.GetPieceType())
            {
                Debug.Log("A different piece type is at the source location in real game board.");
            }
            else if(this_iss_source.Occupier().GetPlayer() != ips.GetPlayer())
            {
                Debug.Log("A different player owns the piece in the real game board.");
            }

            return false;
            //throw new Exception("Invalid move for this real game board.");
        }
        
        Move((Piece)this_iss_source.Occupier(), (Space)this_iss_dest);
        return true;
    }

    public Space GetRandomSpace()
    {
        return GetSpace(sysRandom.Next(0, numberLevels), sysRandom.Next(0, GridTemplate.rows), sysRandom.Next(0, GridTemplate.cols));   
    }

    ISpaceState IGameBoardState.GetSpaceState(int level, int row, int col)
    {
        return GetSpace(level, row, col);
    }

    int IGameBoardState.GetNumLevels()
    {
        return numberLevels;
    }

    int IGameBoardState.GetNumRows()
    {
        return GridTemplate.rows;
    }

    int IGameBoardState.GetNumCols()
    {
        return GridTemplate.cols;
    }

    public GameBoardState CreateGameBoardState()
    {
        IGameBoardState igbs = this;

        GameBoardState newGbs = new GameBoardState(igbs.GetNumLevels(), igbs.GetNumRows(), igbs.GetNumCols());
        
        for(int lvl = 0; lvl < (igbs.GetNumLevels()); lvl++)
        {
            for(int row = 0; row < (igbs.GetNumRows()); row++)
            {
                for(int col = 0; col < (igbs.GetNumCols()); col++)
                {
                    ISpaceState iss = igbs.GetSpaceState(lvl, row, col);
                    newGbs.spaces[lvl][row][col].occupied = iss.IsOccupied();
                    IPieceState ips = iss.Occupier();
                    if(ips != null)
                    {
                        PieceState ps = ips.CreatePieceState();
                        newGbs.spaces[lvl][row][col].occupier = ps;
                        newGbs.AlivePieces.Add(ps);
                        ps.space = newGbs.spaces[lvl][row][col];
                    }
                }
            }
        }

        foreach(IPieceState piece in DeadPieces)
        {
            newGbs.DeadPieces.Add(piece.CreatePieceState());
        }

        return newGbs;
    }

    void IGameBoardState.Move(IPieceState piece, ISpaceState space)
    {
        Move m = new Move(piece, space, global::Move.MoveType.none);
        this.Move(m);
    }

    List<IPieceState> IGameBoardState.GetAlivePieces()
    {
        return AlivePieces.Select(alive => (IPieceState)alive).ToList();
    }

    IGameBoardState IGameBoardState.Clone()
    {
        return this.CreateGameBoardState();
    }

    public Piece GetPiece(IPieceState pieceState)
    {
        ISpaceState iss_source = pieceState.GetSpaceState();

        Space space = this.GetSpace(iss_source.GetLevel(), iss_source.GetRow(), iss_source.GetCol());

        return space.occupier;
    }
}