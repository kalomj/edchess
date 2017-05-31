using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public GameObject DebugPanel;
    public GameObject GameStats;
    public GameObject Board2D;

    bool toggleDebugPanel = false;
    bool toggleGameStats = false;
    bool toggleBoard2D = false;

    public Sprite BlankSprite;
    public Sprite PawnSprite;
    public Sprite RookSprite;
    public Sprite BishopSprite;
    public Sprite QueenSprite;
    public Sprite KingSprite;
    public Sprite KnightSprite;

    public Dictionary<Piece.PieceType, Sprite> PieceSprites;
    public List<List<List<Image>>> squareLookup;
    public List<Move> moveTargets;

    public Game game;

    private void Awake()
    {
        PieceSprites = new Dictionary<Piece.PieceType, Sprite>();

        squareLookup = new List<List<List<Image>>>();

        GameObject[] boards = GameObject.FindGameObjectsWithTag("Board");

        //initialize space structure with nulls
        squareLookup = new List<List<List<Image>>>();

        for(int i = 0; i < game.gameBoard.numberLevels; i++)
        {
            squareLookup.Add(new List<List<Image>>());

            for(int j = 0; j < game.gameBoard.GridTemplate.rows; j++)
            {
                squareLookup[i].Add(new List<Image>());

                for(int k = 0; k < game.gameBoard.GridTemplate.cols; k++)
                {
                    squareLookup[i][j].Add(null);
                }
            }
        }

        //iterate through potentially unordered game objects and add to the space structure
        foreach(GameObject board in boards)
        {
            int boardNumber = Int32.Parse(board.name.Substring(board.name.Length-1));

            Image[] images = board.GetComponentsInChildren<Image>();

            foreach(Image img in images)
            {
                if(img.name.Substring(0,7) == "Square_")
                {
                    int squareNumber = Int32.Parse(img.name.Substring(img.name.Length - 1));

                    int rowNumber = Int32.Parse(img.rectTransform.parent.name.Substring(img.rectTransform.parent.name.Length - 1));

                    squareLookup[boardNumber][rowNumber][squareNumber] = img;

                    SquareButton sb = img.gameObject.AddComponent<SquareButton>();
                    sb.level = boardNumber;
                    sb.row = rowNumber;
                    sb.col = squareNumber;

                    Button b = img.GetComponent<Button>();
                    Navigation n = b.navigation;
                    n.mode = Navigation.Mode.None;
                    b.navigation = n;
                    b.onClick.AddListener(() => SquareClickEvent(boardNumber, rowNumber, squareNumber));
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        DebugPanel.SetActive(false);

        Image[] images = Board2D.GetComponentsInChildren<Image>();
        foreach(Image i in images)
        {
            i.sprite = BlankSprite;
        }

        PieceSprites.Add(Piece.PieceType.bishop, BishopSprite);
        PieceSprites.Add(Piece.PieceType.king, KingSprite);
        PieceSprites.Add(Piece.PieceType.knight, KnightSprite);
        PieceSprites.Add(Piece.PieceType.pawn, PawnSprite);
        PieceSprites.Add(Piece.PieceType.queen, QueenSprite);
        PieceSprites.Add(Piece.PieceType.rook, RookSprite);

        Board2D.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.D))
        {
            toggleDebugPanel = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            toggleGameStats = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            toggleBoard2D = true;
        }
    }

    void OnGUI()
    {
        if(toggleDebugPanel)
        {
            DebugPanel.SetActive(!DebugPanel.activeSelf);
            toggleDebugPanel = false;
        }

        if(toggleGameStats)
        {
            GameStats.SetActive(!GameStats.activeSelf);
            toggleGameStats = false;
        }

        if(toggleBoard2D)
        {
            Board2D.SetActive(!Board2D.activeSelf);
            toggleBoard2D = false;
        }
    }

    public void RenderBoard(IGameBoardState gameBoard)
    {
        int levels = gameBoard.GetNumLevels();
        int rows = gameBoard.GetNumRows();
        int cols = gameBoard.GetNumCols();

        for(int i = 0; i < levels; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                for(int k = 0; k < cols; k++)
                {
                    Image img = squareLookup[i][j][k];

                    if(img != null)
                    {
                        if (gameBoard.GetSpaceState(i, j, k).IsOccupied())
                        {
                            Piece.PieceType pieceType = gameBoard.GetSpaceState(i, j, k).Occupier().GetPieceType();
                            img.sprite = PieceSprites[pieceType];

                            img.color = gameBoard.GetSpaceState(i, j, k).Occupier().GetPieceTint();
                            img.Desaturate();
                        }
                        else
                        {
                            img.sprite = BlankSprite;
                            img.color = Color.white;
                        }
                    }
                    else
                    {
                        Debug.Log("Error: image not found at index " + i + " " + j + " " + k);
                    }
                }
            }
        }
    }

    public void SquareClickEvent(int level, int row, int col)
    {
        ISpaceState s = game.gameBoard.GetSpace(level, row, col);
        bool moved = false;
        if (moveTargets != null)
        {
            foreach(Move m in moveTargets)
            {
                if(m.space == s)
                {
                    game.gameBoard.Move(m);
                    RenderBoard(game.gameBoard);
                    moved = true;
                    break;
                }
            }
            moveTargets = null;
        }

        ResetButtonHighlights();

        if(s.IsOccupied() && !moved)
        {
            moveTargets = MoveGenerator.GetMoves(game.gameBoard, s.Occupier());
            foreach(Move m in moveTargets)
            {
                squareLookup[m.space.GetLevel()][m.space.GetRow()][m.space.GetCol()].color = Color.cyan;
            }

            StartCoroutine(game.HighlightMoves(moveTargets, 5f));
        }
    }

    public void ResetButtonHighlights()
    {
        for(int i = 0; i < game.gameBoard.numberLevels; i++)
        {
            for(int j = 0; j < game.gameBoard.GridTemplate.rows; j++)
            {
                for(int k = 0; k < game.gameBoard.GridTemplate.cols; k++)
                {
                    Image img = squareLookup[i][j][k];

                    if (game.gameBoard.GetSpace(i, j, k).occupied)
                    {
                        img.color = game.gameBoard.GetSpace(i, j, k).occupier.player.PieceTint;
                        img.Desaturate();
                    }
                    else
                    {
                        img.color = Color.white;
                    }
                }
            }
        }
    }
}
