using UnityEngine;
using System.Collections.Generic;

public class InfiniteRunGenerator : MonoBehaviour
{
    public float parallaxBackgroundSpeed;

    public GameObject backgroundPrefab;
    private GameObject backgroundFolder;
    private List<GameObject> backgrounds;

    private LevelPartPicker levelPartPicker;

    private float bgWidth;
    public float levelPartWidth;

    private Dictionary<int, GameObject> indexedGameObjects;

    private Vector2 deathBoxPos;
    private Vector2 backgroundPos;

    private bool doneRespawning;

    private int section = 0;
    private int parallaxSection = 0;

    public delegate void RespawnAction();
    public static event RespawnAction Respawn;

    /* ********************************************************************* */
    //                                Start Up                    
    /* ********************************************************************* */
    void Awake()
    {
        KillEnemies.OnRespawn += DoneRespawning;
        InitializeVariables();
        InitializeEnvironment();
        InitializeLevel();
    }

    void InitializeVariables()
    {
        levelPartPicker = new LevelPartPicker();
        indexedGameObjects = new Dictionary<int, GameObject>();

        backgroundFolder = new GameObject("Background");
        backgroundFolder.transform.position = Vector2.zero;
        backgrounds = new List<GameObject>();

        //Make the backgrounds overlap just a little bit, to prevent the white back-background from showing
        bgWidth = (backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 2) - 0.01f;
        backgroundPos = Vector2.zero;
        deathBoxPos = backgroundPos + (Vector2.down * 5);

        doneRespawning = true;
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
        ParallaxBackground();
        CheckForDeath();
    }

    void UpdateLevelParts()
    {
        float pos = transform.position.x;
        int newSection = (int)(pos / levelPartWidth);
        if (newSection != section)
        {
            int direction = newSection - section;
            bool canGenerate = !indexedGameObjects.ContainsKey(newSection);
            bool canGenerateAhead = !indexedGameObjects.ContainsKey(newSection + direction);
            if (canGenerate)
            {
                Debug.Log("Generating");
                GenerateSection(newSection);
            }
            if (canGenerateAhead)
            {
                GenerateSection(newSection + direction);
            }
            DetermineVisibleSections();
        }
        section = newSection;
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

    void ParallaxBackground()
    {
        foreach(GameObject bg in backgrounds)
        {
            bg.GetComponent<Rigidbody2D>().velocity = Vector2.right * (GetComponent<Rigidbody2D>().velocity.x / GetComponent<LateralMovement>().speed) * parallaxBackgroundSpeed;
        }
        float positionXFromCenter = transform.position.x - backgrounds[1].transform.position.x;
        int newParallaxSection = (int)(positionXFromCenter / bgWidth);
        if(newParallaxSection != parallaxSection)
        {
            MoveSection();
        }
        parallaxSection = newParallaxSection;
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
        foreach(KeyValuePair<int, GameObject> pair in indexedGameObjects)
        {
            pair.Value.SetActive(Mathf.Abs(pair.Key - section) <= 1);
        }
    }

    void CheckForDeath()
    {
        if (transform.position.y < deathBoxPos.y && doneRespawning)
        {
            if (Respawn != null)
            {
                Respawn();
                doneRespawning = false;
            }
        }
    }

    void DoneRespawning()
    {
        doneRespawning = true;
        ScoreTracker.ResetScore();
    }

    /* ********************************************************************* */
    //                              Clean Up                      
    /* ********************************************************************* */
    void OnDestroy()
    {
        KillEnemies.OnRespawn -= DoneRespawning;
    }
}
