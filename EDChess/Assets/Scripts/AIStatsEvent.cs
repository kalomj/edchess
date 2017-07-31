using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIStatsEvent : MonoBehaviour {

    public Game game;
    public Text stat1;
    public Text stat2;
    public Text stat3;

    public long value1 = 0;
    public long value2 = 0;
    public double value3 = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(game.lastResult != null)
        {
            value1 = AIMinMax.StatesSearched;
            value2 = AIMinMax.LevelsSearched;
            value3 = game.lastResult.AlphaBeta;
        }
        stat1.text = value1.ToString();
        stat2.text = value2.ToString();
        stat3.text = value3.ToString();
	}
}
