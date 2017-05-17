using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    List<Level> Levels;

    public int numberLevels = 8;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numberLevels; i++)
        {
            Levels.Add(new Level());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
