using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIStatsEvent : MonoBehaviour {

    public Game game;
    public Text stat1;
    public Text stat2;
    public Text stat3;
    public Text stat4;
    public Text stat5;
    public Text stat6;

    public long value1 = 0;
    public long value2 = 0;
    public double value3 = 0;
    public long value4;
    public int value5;
    public int value6;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /*if(game.lastResult != null)
        {
            value3 = game.lastResult.AlphaBeta;
        }*/

        value1 = AIMinMax.StatesSearched;
        value2 = AIMinMax.LevelsSearched;

        value3 = game.gameBoard.CalculateUtility();

        value4 = AIMinMax.MovesSearched;

        value5 = game.gameBoard.CalculateRawScore(Player.PlayerNumber.Player1);
        value6 = game.gameBoard.CalculateRawScore(Player.PlayerNumber.Player2);

        stat1.text = value1.ToString();
        stat2.text = value2.ToString();
        stat3.text = value3.ToString();
        stat4.text = value4.ToString();
        stat5.text = value5.ToString();
        stat6.text = value6.ToString();
	}
}
