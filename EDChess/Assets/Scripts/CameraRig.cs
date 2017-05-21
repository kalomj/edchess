using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraRig : MonoBehaviour {

    public List<GameObject> Views;
    public Camera main;

    private void Awake()
    {

    }

    // Use this for initialization
    void Start () {
        StartCoroutine(AroundTheWorld());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator AroundTheWorld()
    {
        while(true)
        {
            foreach(GameObject view in Views)
            {
                main.transform.DOMove(view.transform.position, 5f);
                main.transform.DORotateQuaternion(view.transform.rotation, 5f);
                yield return new WaitForSeconds(5f);
            }
        }
    }
}
