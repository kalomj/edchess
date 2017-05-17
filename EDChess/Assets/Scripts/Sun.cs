using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

    public float x_rate = 0.1f;
    public float y_rate = 0.1f;
    public float z_rate = 0.1f;

    Quaternion change;

	// Use this for initialization
	void Start () {
        change = Quaternion.Euler(x_rate, y_rate, z_rate);
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.rotation = this.transform.rotation * change;
	}
}
