using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


[System.Serializable]
public struct NamedMaterial
{
    public string name;
    public Material material;

    public static Dictionary<string,Material> CreateDictionary(NamedMaterial[] nmArray)
    {
        Dictionary<string, Material> ret = new Dictionary<string, Material>();

        foreach(NamedMaterial nm in nmArray)
        {
            ret.Add(nm.name, nm.material);
        }

        return ret;
    }
}

