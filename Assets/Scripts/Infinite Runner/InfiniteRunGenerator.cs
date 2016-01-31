using UnityEngine;
using System.Collections.Generic;

public class InfiniteRunGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct StartingPieces
    {
        public int Location;
        public string Name;
    }
    public StartingPieces[] startingPieces;

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
        for(int i = 0; i < startingPieces.Length; i++)
        {
            int location = startingPieces[i].Location;

            GameObject section = (GameObject)Instantiate(
            levelPartPicker.FindByName(startingPieces[i].Name),
            Vector2.right * location * levelPartWidth, Quaternion.identity);
            
            indexedGameObjects.Add(location, section);
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
    }

    void DetermineVisibleSections()
    {
        foreach(KeyValuePair<int, GameObject> pair in indexedGameObjects)
        {
            pair.Value.SetActive(Mathf.Abs(pair.Key - section) <= 1);
        }
    }
}
