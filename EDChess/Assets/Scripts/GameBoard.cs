using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    public List<Level> Levels;
    public List<Piece> Pieces;

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
        Pieces = new List<Piece>();

        for (int i = 0; i < numberLevels; i++)
        {
            if (LevelTemplate != null)
            {
                LevelTemplate.level = i;
                Level lvl = Instantiate(LevelTemplate);
                Levels.Add(lvl);
            }

            for (int j = 0; j < GridTemplate.cols; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Piece piece = Instantiate(PieceTemplate);
                    Pieces.Add(piece);
                }
            }
        }

        SpaceTemplate.GetComponent<MeshRenderer>().enabled = false;
        PieceTemplate.GetComponent<MeshRenderer>().enabled = false;
    }

    // Use this for initialization
    void Start () {
        IEnumerator<Space> spaceEnumerator = GetPieceStartingSpaceEnumerator();
        foreach(Piece piece in Pieces)
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
                }
                else
                {
                    //p2 pieces
                    piece.SetTint(game.player2.PieceTint);
                }
            }
        }

        StartCoroutine("MovePieceEvent");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator MovePieceEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // wait half a second
            DoRandomMove();
        }
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

    public Space GetSpace(int level, int row, int col)
    {
        return Levels[level].GridInstance.GetSpace(row, col);
    }

    /// <summary>
    /// select a random piece, then select spaces at random until an unoccied space is found. swap the piece to the space.
    /// </summary>
    public void DoRandomMove()
    {
        Piece piece = Pieces[sysRandom.Next(0, Pieces.Count)];
        Space space;

        do
        {
            space = GetRandomSpace();
        }
        while (space.occupied);

        Move(piece, space);
    }

    private static void Move(Piece piece, Space space)
    {
        if(space.occupied)
        {
            throw new System.Exception("Cannot move into an occupied space.");
        }

        piece.transform.position = space.transform.position;
        piece.space.occupied = false;
        piece.space.occupier = null;
        piece.space.AnimateShell(.5f);
        piece.space = space;
        space.occupied = true;
        space.occupier = piece;
        space.AnimateShell(.5f);
    }

    public Space GetRandomSpace()
    {
        return GetSpace(sysRandom.Next(0, numberLevels), sysRandom.Next(0, GridTemplate.rows), sysRandom.Next(0, GridTemplate.cols));   
    }
}
