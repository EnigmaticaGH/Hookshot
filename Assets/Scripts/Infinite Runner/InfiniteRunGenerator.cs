using UnityEngine;
using System.Collections.Generic;

public class InfiniteRunGenerator : MonoBehaviour
{
    private LevelPartPicker levelPartPicker;

    public float levelPartWidth;

    private Dictionary<int, GameObject> indexedGameObjects;

    private int section = 0;

    /* ********************************************************************* */
    //                                Start Up                    
    /* ********************************************************************* */
    void Awake()
    {
        InitializeVariables();
        InitializeLevel();
    }

    void InitializeVariables()
    {
        levelPartPicker = new LevelPartPicker();
        indexedGameObjects = new Dictionary<int, GameObject>();
    }

    void InitializeLevel()
    {
        GameObject startSection = (GameObject)Instantiate(
            levelPartPicker.FindByName("Start"),
            Vector2.zero, Quaternion.identity);

        indexedGameObjects.Add(-1, GetRandomLevelPart(Vector2.left * levelPartWidth));
        indexedGameObjects.Add(0, startSection);
        indexedGameObjects.Add(1, GetRandomLevelPart(Vector2.right * levelPartWidth));

        foreach (KeyValuePair<int, GameObject> pair in indexedGameObjects)
        {
            foreach (Transform part in pair.Value.transform)
            {
                if (part.GetComponent<SpringJoint2D>() != null)
                {
                    part.GetComponent<SpringJoint2D>().connectedAnchor 
                        = (Vector2)(part.transform.position) + (Vector2.up * 0.4f);
                }
            }
        }
    }

    GameObject GetRandomLevelPart(Vector2 position)
    {
        return (GameObject)Instantiate(levelPartPicker.random, 
            position, Quaternion.identity);
    }

    /* ********************************************************************* */
    //                             Process
    /* ********************************************************************* */
    void Update()
    {
        UpdateLevelParts();
    }

    void UpdateLevelParts()
    {
        float pos = transform.position.x;
        int newSection = (int)(pos / levelPartWidth);
        int direction = newSection - section;
        section = newSection;
        if (!indexedGameObjects.ContainsKey(section + direction))
            GenerateSection(section + direction);
        DetermineVisibleSections();
    }

    void GenerateSection(int index)
    {
        indexedGameObjects.Add(index, GetRandomLevelPart(Vector2.right * levelPartWidth * index));

        foreach (Transform part in indexedGameObjects[index].transform)
        {
            if (part.GetComponent<SpringJoint2D>() != null)
            {
                part.GetComponent<SpringJoint2D>().connectedAnchor
                    = (Vector2)(part.transform.position) + (Vector2.up * 0.4f);
            }
        }
    }

    void DetermineVisibleSections()
    {
        foreach(KeyValuePair<int, GameObject> pair in indexedGameObjects)
        {
            pair.Value.SetActive(Mathf.Abs(pair.Key - section) <= 1);
        }
    }
}
