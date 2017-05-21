using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public Grid GridTemplate;
    public Grid GridInstance;

    public int level = 0;

    private void Awake()
    {
        if (GridTemplate == null)
        {
            GridTemplate = GetComponentInParent<GameBoard>().GridTemplate;  
        }

        GridInstance = Instantiate(GridTemplate);
        GridInstance.parentLvl = this;
    }

    // Use this for initialization
    void Start () {

        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
