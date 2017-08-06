using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraRig : MonoBehaviour {

    public List<GameObject> Views;
    public Camera main;

    public bool running;

    // Use this for initialization
    void Start () {
        StartCoroutine(AroundTheWorld());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Go()
    {
        running = true;
        StartCoroutine(AroundTheWorld());
    }

    public void Stop()
    {
        running = false;
    }

    public IEnumerator AroundTheWorld()
    {
        while(running)
        {
            foreach(GameObject view in Views)
            {
                if(running)
                {
                    main.transform.DOMove(view.transform.position, 5f);
                    main.transform.DORotate(view.transform.eulerAngles, 5f);

                    yield return new WaitForSeconds(5f);
                }
            }
        }
    }
}
