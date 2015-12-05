using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class LevelPart
{
    private GameObject levelPartFolder;
    private List<GameObject> parts;
    private int difficulty;
    private int section;
    private static int numberOfParts = 0;
    public LevelPart(Vector3 position, List<GameObject> levelParts, int diff)
    {
        levelPartFolder = new GameObject("Level Part Reference " + Count);
        levelPartFolder.transform.position = position;
        parts = levelParts;
        difficulty = diff;
        foreach (GameObject part in parts)
        {
            part.transform.position += levelPartFolder.transform.position;
            part.transform.parent = levelPartFolder.transform;
            if (part.GetComponent<SpringJoint2D>() != null)
            {
                part.GetComponent<SpringJoint2D>().connectedAnchor = (Vector2)(part.transform.position) + (Vector2.up * 0.4f);
            }
        }
        levelPartFolder.SetActive(false);
        numberOfParts++;
    }
    public static GameObject Instantiate(LevelPart level, Vector2 position, Quaternion rotation, int section)
    {
        GameObject copy;
        copy = (GameObject)Object.Instantiate(level.levelPartFolder, position, rotation);
        copy.name = "Level Part " + section;
        
        copy.SetActive(true);
        foreach (Transform part in copy.transform)
        {
            if (part.GetComponent<SpringJoint2D>() != null)
            {
                part.GetComponent<SpringJoint2D>().connectedAnchor = (Vector2)(part.transform.position) + (Vector2.up * 0.4f);
            }
        }
        return copy;
    }
    public List<GameObject> Parts
    {
        get
        {
            return parts;
        }
        set
        {
            parts = Parts;
        }
    }
    public int Difficulty
    {
        get
        {
            return difficulty;
        }
        private set
        {
            difficulty = Difficulty;
        }
    }
    public Vector2 Position
    {
        get
        {
            return levelPartFolder.transform.position;
        }
        set
        {
            levelPartFolder.transform.position = Position;
        }
    }
    public string Name
    {
        get
        {
            return levelPartFolder.name;
        }
    }
    public static int Count
    {
        get
        {
            return numberOfParts;
        }
    }
}