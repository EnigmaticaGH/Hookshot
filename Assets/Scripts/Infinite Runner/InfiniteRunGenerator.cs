﻿using UnityEngine;
using System.Collections.Generic;

public class InfiniteRunGenerator : MonoBehaviour
{
    public string[] startingPieces;

    private LevelPartPicker levelPartPicker;

    private float levelWidth;

    private Dictionary<int, LevelPartPicker.LevelPart> indexedGameObjects;


    private int section = 0;

    private CameraFollow2D cam;

    /* ********************************************************************* */
    //                                Start Up                    
    /* ********************************************************************* */
    void Awake()
    {
        InitializeVariables();
        InitializeLevel();
    }

    void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<CameraFollow2D>();
    }

    void InitializeVariables()
    {
        levelPartPicker = new LevelPartPicker();
        indexedGameObjects = new Dictionary<int, LevelPartPicker.LevelPart>();
    }

    void InitializeLevel()
    {
        levelWidth = 0;
        for(int i = 0; i < startingPieces.Length; i++)
        {
            LevelPartPicker.LevelPart? levelPartMaybe = levelPartPicker.FindByName(startingPieces[i]);
            if (!levelPartMaybe.HasValue) continue;
            LevelPartPicker.LevelPart levelPart = levelPartMaybe.Value;

            Debug.Log("Spawning initial object at " + levelWidth);

            GameObject section = (GameObject)Instantiate(levelPart.GameObject,
            Vector2.right * levelWidth, Quaternion.identity);

            levelPart.GameObject = section;
            indexedGameObjects.Add(i, levelPart);
            levelWidth += levelPart.Width;
        }
    }

    LevelPartPicker.LevelPart GetRandomLevelPart(Vector2 position)
    {
        LevelPartPicker.LevelPart levelPart = levelPartPicker.random;
        levelPart.GameObject = (GameObject)Instantiate(levelPart.GameObject,
            position, Quaternion.identity);
        return levelPart;
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
        if (!indexedGameObjects.ContainsKey(section))
            GenerateSection(section);
        int newSection = (int)(pos / indexedGameObjects[section].Width);
        section = newSection;
        if (!indexedGameObjects.ContainsKey(section + 1))
            GenerateSection(section + 1);
        DetermineVisibleSections();
    }

    void GenerateSection(int index)
    {
        indexedGameObjects.Add(index, GetRandomLevelPart(Vector2.right * levelWidth));
        //Debug.Log("Spawning object at " + levelWidth);
        levelWidth += indexedGameObjects[index].Width;
    }

    void DetermineVisibleSections()
    {
        foreach(KeyValuePair<int, LevelPartPicker.LevelPart> pair in indexedGameObjects)
        {
            float position = pair.Value.GameObject.transform.position.x;
            float rightBoundDistanceFromPosition = pair.Value.Right;
            float leftBoundDistanceFromPosition = pair.Value.Left;
            float leftMostPosition = position + leftBoundDistanceFromPosition;
            float rightMostPosition = position + rightBoundDistanceFromPosition;
            float cameraLeftBound = transform.position.x - cam.GetScreenWidth();
            float cameraRightBound = transform.position.x + cam.GetScreenWidth();
            pair.Value.GameObject.SetActive(leftMostPosition - cameraRightBound < pair.Value.Width && rightMostPosition - cameraLeftBound > -pair.Value.Width);
        }
    }
}

﻿/*using UnityEngine;
 System.Collections.Generic;

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

    private GameObject player;
    private int section = 0;

    /* ********************************************************************* 
    //                                Start Up                    
    /* ********************************************************************* 
    void Awake()
    {
        InitializeVariables();
        InitializeLevel();
    }

    void InitializeVariables()
    {
        levelPartPicker = new LevelPartPicker();
        indexedGameObjects = new Dictionary<int, GameObject>();
        player = GameObject.FindGameObjectWithTag("Player");
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

    /* ********************************************************************* 
    //                             Process
    /* ********************************************************************* 
    void Update()
    {
        UpdateLevelParts();
    }

    void UpdateLevelParts()
    {
        float pos = player.transform.position.x;
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
>>>>>>> master*/
