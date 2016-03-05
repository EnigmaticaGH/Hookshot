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
