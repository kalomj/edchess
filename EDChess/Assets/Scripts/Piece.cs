using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    public Space space;

    private MeshRenderer meshRenderer;

    public Dictionary<string, Material> skinMap;

    public NamedMaterial[] skins;

    public enum PieceType { pawn, rook, queen, king, bishop, knight};
    public PieceType pieceType;

    public Player player;

    public bool alive = true;

    public bool Alive
    {
        get
        {
            return alive;
        }
        set
        {
            alive = value;
            meshRenderer.enabled = value;
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        skinMap = NamedMaterial.CreateDictionary(skins);
        meshRenderer.material = skinMap[pieceType.ToString()];
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTint(Color tint)
    {
        meshRenderer.materials[0].SetColor("_EmissionColor", tint);
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void SetPieceType(PieceType pieceType)
    {
        this.pieceType = pieceType;
    }
}
