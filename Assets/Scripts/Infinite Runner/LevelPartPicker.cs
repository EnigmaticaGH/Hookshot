using UnityEngine;
using System;
using System.Collections.Generic;

class LevelPartPicker
{
    private List<GameObject> prefabs;

    private System.Random randomGen;

    public LevelPartPicker()
    {
        randomGen = new System.Random();
        LoadAssets();
    }

    void LoadAssets()
    {
        prefabs = new List<GameObject>();
        foreach(GameObject prefab in Resources.LoadAll("Level Parts"))
        {
            prefabs.Add(prefab);
        } 
    }

    public GameObject random {
        get {
            return prefabs[randomGen.Next(prefabs.Count)];
        }
    }

    public GameObject FindByName(string name)
    {
        foreach(GameObject prefab in prefabs) {
            if(prefab.name == name)
                return prefab;
        }
        return null;
    }
}
