using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Space : MonoBehaviour
{

    public bool occupied = false;
    public Piece occupier;
    public int level;
    public int row;
    public int col;

    private MeshRenderer meshRender;

    private void Awake()
    {
        meshRender = GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AnimateShell(float t)
    {
        meshRender.enabled = true;
        meshRender.material.color = new Color(meshRender.materials[0].color.r, meshRender.materials[0].color.g, meshRender.materials[0].color.b, 1.0f);
        meshRender.material.DOFade(0f, t);
        StartCoroutine(Disable(t));
    }

    public void AnimateShell(float t, Color color)
    {
        meshRender.enabled = true;
        meshRender.material.color = new Color(meshRender.materials[0].color.r, meshRender.materials[0].color.g, meshRender.materials[0].color.b, 1.0f);
        meshRender.materials[0].SetColor("_EmissionColor",color*.25f);
        meshRender.material.DOFade(0f, t);
        StartCoroutine(Disable(t));
    }

    IEnumerator Disable(float fadeTime)
    {
        yield return new WaitForSeconds(fadeTime);
        meshRender.enabled = false;
    }

    
}
