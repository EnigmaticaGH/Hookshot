using UnityEngine;
using System;
using System.Collections.Generic;

class LevelPartPicker
{
    public struct LevelPart
    {
        public GameObject GameObject;
        public float Width;
        public float Right;
        public float Left;
    }

    private List<LevelPart> prefabs;

    private System.Random randomGen;

    public LevelPartPicker()
    {
        randomGen = new System.Random();
        LoadAssets();
    }

    void LoadAssets()
    {
        prefabs = new List<LevelPart>();
        foreach(GameObject prefab in Resources.LoadAll("Level Parts"))
        {
            float right, left;
            LevelPart part;
            part.GameObject = prefab;
            part.Width = GetLevelPartWidth(prefab, out left, out right);
            part.Right = right;
            part.Left = left;
            Debug.Log("Adding prefab: " + prefab.name + ", Width: " + part.Width + ", Distance Right: " + part.Right + ", Distance Left: " + part.Left);
            prefabs.Add(part);
        }
    }

    public LevelPart random {
        get {
            return prefabs[randomGen.Next(prefabs.Count)];
        }
    }

    public LevelPart? FindByName(string name)
    {
        foreach(LevelPart prefab in prefabs) {
            if(prefab.GameObject.name == name)
                return prefab;
        }
        return null;
    }

    public float GetLevelPartWidth(GameObject g, out float min, out float max)
    {
        //Get the width of a level part, including the width of the sprites at the edges
        GameObject minObj = g.transform.GetChild(0).gameObject, maxObj = g.transform.GetChild(0).gameObject;
        Sprite sprite;
        min = 0;
        max = 0;
        float spriteWidth = 0;
        foreach (Transform child in g.transform)
        {
            if (minObj.transform.position.x > child.position.x)
                minObj = child.gameObject;
            if (maxObj.transform.position.x < child.position.x)
                maxObj = child.gameObject;
        }

        if (minObj.GetComponent<SpriteRenderer>().sprite != null)
        {
            sprite = minObj.GetComponent<SpriteRenderer>().sprite;
            spriteWidth = sprite.bounds.center.x + (sprite.bounds.extents.x * minObj.transform.localScale.x);
        }
        min = minObj.transform.position.x - spriteWidth - g.transform.position.x;

        spriteWidth = 0;

        if (maxObj.GetComponent<SpriteRenderer>().sprite != null)
        {
            sprite = maxObj.GetComponent<SpriteRenderer>().sprite;
            spriteWidth = sprite.bounds.center.x + (sprite.bounds.extents.x * maxObj.transform.localScale.x);
        }
        max = maxObj.transform.position.x + spriteWidth - g.transform.position.x;

        return max - min;
    }
}
