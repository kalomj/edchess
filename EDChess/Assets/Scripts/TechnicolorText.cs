using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechnicolorText : MonoBehaviour {

    public List<Text> AllText;
    private System.Random rng;
    private float LastSavedTime;

	// Use this for initialization
	void Start () {
        AllText = gameObject.GetComponentsInChildren<Text>().ToList();
        rng = new System.Random();
        LastSavedTime = Time.time;
    }
	
	// Update is called once per frame
	void OnGUI () {
        if(Time.time - LastSavedTime > 1.0)
        {
            foreach (Text t in AllText)
            {
                t.CrossFadeColor(new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble()), 1.0f, false, false);
                //t.color = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
            }
            LastSavedTime = Time.time;
        }
		
	}
}

public static class TechnicolorTextExtensions
{
    public static List<Text> ToList(this Text[] TextArray)
    {
        return new List<Text>(TextArray);
    }
}


