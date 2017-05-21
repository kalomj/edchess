using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public Space SpaceTemplate;
    public int rows = 8;
    public int cols = 8;

    public List<Space> Spaces;

    public Level parentLvl;
    public float spacing = 1.25f;

    private void Awake()
    {
        Spaces = new List<Space>();

        if (SpaceTemplate == null)
        {
            SpaceTemplate = GetComponentInParent<GameBoard>().SpaceTemplate;
        }


        Vector3 spaceVector = SpaceTemplate.transform.position;
        Quaternion spaceRot = SpaceTemplate.transform.rotation;
        Vector3 spaceScale = new Vector3(spacing,spacing,spacing);

        spaceVector.y = spacing * parentLvl.level;

        for (int i = 0; i < rows; i++)
        {
            spaceVector.z = 0;

            for (int j = 0; j < cols; j++)
            {
                Space space = Instantiate(SpaceTemplate, spaceVector, spaceRot);
                space.transform.localScale = spaceScale;
                Spaces.Add(space);
                space.row = i;
                space.col = j;
                space.level = parentLvl.level;

                space.gameObject.GetComponent<MeshRenderer>().enabled = false;

                spaceVector.z += spacing;
            }

            spaceVector.x += spacing;

        }

    }

    // Use this for initialization
    void Start () {

        
    }

    public Space GetSpace(int row, int col)
    {
        return Spaces[row * rows + col];
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
