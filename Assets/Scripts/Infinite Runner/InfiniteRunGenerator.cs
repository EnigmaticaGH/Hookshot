using UnityEngine;
using System.Collections.Generic;

public class InfiniteRunGenerator : MonoBehaviour
{
    public string[] startingPieces;

    public float parallaxBackgroundSpeed;

    public GameObject backgroundPrefab;
    private GameObject backgroundFolder;
    private List<GameObject> backgrounds;

    private LevelPartPicker levelPartPicker;

    private float bgWidth;
    private float levelWidth;

    private Dictionary<int, LevelPartPicker.LevelPart> indexedGameObjects;

    private Vector2 backgroundPos;

    private int section = 0;
    private int parallaxSection = 0;

    private CameraFollow2D cam;

    /* ********************************************************************* */
    //                                Start Up                    
    /* ********************************************************************* */
    void Awake()
    {
        InitializeVariables();
        InitializeEnvironment();
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

        backgroundFolder = new GameObject("Background");
        backgroundFolder.transform.position = Vector2.zero;
        backgrounds = new List<GameObject>();

        //Make the backgrounds overlap just a little bit, to prevent the white back-background from showing
        bgWidth = (backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 2) - 0.01f;
        backgroundPos = Vector2.zero;
    }

    void InitializeEnvironment()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject background = (GameObject)Instantiate(backgroundPrefab, 
                backgroundPos + (Vector2.right * bgWidth * i), 
                Quaternion.identity);
            backgrounds.Add(background);
            backgrounds[i].transform.parent = backgroundFolder.transform;
        }
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
        ParallaxBackground();
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

    void ParallaxBackground()
    {
        foreach(GameObject bg in backgrounds)
        {
            bg.GetComponent<Rigidbody2D>().velocity = Vector2.right * (GetComponent<Rigidbody2D>().velocity.x / GetComponent<LateralMovement>().speed) * parallaxBackgroundSpeed;
        }
        float positionXFromCenter = transform.position.x - backgrounds[1].transform.position.x;
        parallaxSection = (int)(positionXFromCenter / bgWidth);
        MoveSection();
    }

    void MoveSection()
    {
        for (int i = 0; i < backgrounds.Count; i++)
        {
            Vector2 offset = Vector2.right * bgWidth * parallaxSection;
            backgrounds[i].transform.position += (Vector3)offset;
        }
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
