﻿using UnityEngine;
using System.Collections.Generic;

public class InfiniteRunGenerator : MonoBehaviour
{
    public string[] startingPieces;

    private LevelPartPicker levelPartPicker;

    private float levelWidth;

    private Dictionary<int, LevelPartPicker.LevelPart> indexedGameObjects;

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

            Debug.Log("Spawning initial object at " + (levelWidth - levelPart.Left) + ": " + levelPart.GameObject.name);

            GameObject section = (GameObject)Instantiate(levelPart.GameObject,
            Vector2.right * (levelWidth - levelPart.Left), Quaternion.identity);

            levelPart.GameObject = section;
            indexedGameObjects.Add(i, levelPart);
            levelWidth += indexedGameObjects[i].Right + levelPart.GameObject.GetComponent<PrefabProperties>().distanceBetweenOtherParts;
        }
        StartCoroutine(GenerateSections(startingPieces.Length));
    }

    LevelPartPicker.LevelPart GetRandomLevelPart(Vector2 position)
    {
        LevelPartPicker.LevelPart levelPart = levelPartPicker.random;
        levelPart.GameObject = (GameObject)Instantiate(levelPart.GameObject,
            position, Quaternion.identity);
        return levelPart;
    }

    System.Collections.IEnumerator GenerateSections(int startingIndex)
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            GenerateSection(startingIndex++);
        }
    }

    /* ********************************************************************* */
    //                             Process
    /* ********************************************************************* */
    void Update()
    {
        DetermineVisibleSections();
    }

    void GenerateSection(int index)
    {
        indexedGameObjects.Add(index, GetRandomLevelPart(Vector2.right * levelWidth));
        indexedGameObjects[index].GameObject.transform.position -= Vector3.right * indexedGameObjects[index].Left;
        Debug.Log("Spawning object at " + (levelWidth - indexedGameObjects[index].Left) + ": " + indexedGameObjects[index].GameObject.name);
        levelWidth += indexedGameObjects[index].Right + indexedGameObjects[index].GameObject.GetComponent<PrefabProperties>().distanceBetweenOtherParts;
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
            float cameraLeftBound = cam.transform.position.x - cam.GetScreenWidth();
            float cameraRightBound = cam.transform.position.x + cam.GetScreenWidth();
            pair.Value.GameObject.SetActive(leftMostPosition - cameraRightBound < pair.Value.Width && rightMostPosition - cameraLeftBound > -pair.Value.Width);
        }
    }
}
