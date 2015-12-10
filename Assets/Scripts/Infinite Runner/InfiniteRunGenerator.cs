using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class InfiniteRunGenerator : MonoBehaviour
{
    public float parallaxBackgroundSpeed;

    public GameObject backgroundPrefab;

    private Dictionary<string, GameObject> prefabs;
    private Dictionary<int, string> indexes;
    private Dictionary<int, GameObject> indexedGameObjects;

    private GameObject backgroundFolder;
    private List<GameObject> backgrounds;

    private Vector2 deathBoxPos;
    private Vector2 backgroundPos;
    private GameObject bgFolder;

    private bool doneRespawning;

    private int oldSection = 0;
    private int section = 0;
    private int oldParallaxSection = 0;
    private int parallaxSection = 0;

    private float maxPositionX = 0;
    private float oldPositionX = 0;
    private float positionX = 0;

    private float bgWidth;
    public float levelPartWidth;

    private Rigidbody2D player;
    private LateralMovement movement;

    public delegate void RespawnAction();
    public static event RespawnAction Respawn;
    public delegate void UpdateScore(double score);
    public static event UpdateScore Score;
    void Start()
    {
        KillEnemies.OnRespawn += DoneRespawning;
        InitalizeVariables();
        FindObjects();
        //LoadAssets();
        InitalizeEnvironment();
        
    }
    void InitalizeVariables()
    {
        prefabs = new Dictionary<string, GameObject>();
        indexes = new Dictionary<int, string>();
        indexedGameObjects = new Dictionary<int, GameObject>();

        backgroundFolder = new GameObject("Background");
        backgroundFolder.transform.position = Vector2.zero;
        backgrounds = new List<GameObject>();

        //Make the backgrounds overlap just a little bit, to prevent the white back-background from showing
        bgWidth = (backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 2) - 0.01f;
        backgroundPos = Vector2.zero;
        deathBoxPos = backgroundPos + (Vector2.down * 5);

        doneRespawning = true;
        oldPositionX = 0;
        positionX = 0;
    }
    void FindObjects()
    {
        player = GetComponent<Rigidbody2D>();
        movement = GetComponent<LateralMovement>();
        bgFolder = GameObject.Find("Background");
    }
    void OnDestroy()
    {
        KillEnemies.OnRespawn -= DoneRespawning;
    }
    void InitalizeEnvironment()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject background = (GameObject)Instantiate(backgroundPrefab, backgroundPos + (Vector2.right * bgWidth * i), Quaternion.identity);
            backgrounds.Add(background);
            backgrounds[i].transform.parent = bgFolder.transform;
        }
    }
    void InitalizeLevel()
    {
        indexedGameObjects.Add(-1, (GameObject)Instantiate(prefabs[indexes[(int)(Random.value * prefabs.Count)]],
            Vector2.left * levelPartWidth, Quaternion.identity));

        indexedGameObjects.Add(0, (GameObject)Instantiate(prefabs["Start"],
            Vector2.zero, Quaternion.identity));

        indexedGameObjects.Add(1, (GameObject)Instantiate(prefabs[indexes[(int)(Random.value * prefabs.Count)]], 
            Vector2.right * levelPartWidth, Quaternion.identity));

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
    void Update()
    {
        UpdateLevelParts();
        ParallaxBackground();
        CheckForDeath();
        if (transform.position.x > maxPositionX)
            maxPositionX = transform.position.x;
        oldPositionX = positionX;
        positionX = transform.position.x;
        double scoreDelta = positionX - oldPositionX;
        Score(scoreDelta);
    }
    void UpdateLevelParts()
    {
        oldSection = section;
        bool canGenerate = true;
        bool canGenerateAhead = true;
        float pos = transform.position.x;
        section = (int)(pos / levelPartWidth);
        if (section != oldSection)
        {
            int direction = section - oldSection;
            canGenerate = !indexedGameObjects.ContainsKey(section);
            canGenerateAhead = !indexedGameObjects.ContainsKey(section + direction);
            if (canGenerate)
            {
                GenerateSection(section);
            }
            if (canGenerateAhead)
            {
                GenerateSection(section + direction);
            }
            DetermineVisibleSections();
        }
    }

    void GenerateSection(int index)
    {
        int i = (int)(Random.value * prefabs.Count);
        indexedGameObjects.Add(index, (GameObject)Instantiate(prefabs[indexes[i]],
            Vector2.right * levelPartWidth * index, Quaternion.identity));

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
        oldParallaxSection = parallaxSection;
        float positionXFromCenter = transform.position.x - backgrounds[1].transform.position.x;
        parallaxSection = (int)(positionXFromCenter / bgWidth);
        if(parallaxSection != oldParallaxSection)
        {
            MoveSection();
        }
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
        positionX = 0;
        oldPositionX = 0;
    }

    void LoadAssets()
    {
        foreach(GameObject g in Resources.LoadAll("Level Parts"))
        {
            indexes.Add(prefabs.Count, g.name);
            prefabs.Add(g.name, g);
        }
        if (prefabs.Count > 0)
            InitalizeLevel();
    }
}
