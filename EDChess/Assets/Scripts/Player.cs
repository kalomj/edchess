using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Color PieceTint;

    public enum PlayerNumber { None, Player1, Player2 };
    public enum PlayerType { None, Human, Computer, Remote }
    public PlayerNumber playerNumber;
    public PlayerType playerType;
    public int AIThoughtDepth = 1;
    public enum PlayerState { None, Thinking, Moving, Waiting }
    public PlayerState playerState;
    public Move selectedMove;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
