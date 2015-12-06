using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
struct IndexedGameObject
{
    public int index;
    public GameObject gameObject;
}
public class InfiniteRunGenerator : MonoBehaviour
{
    public float parallaxBackgroundSpeed;

    public GameObject backgroundPrefab;
    public GameObject deathBoxPrefab;
    public GameObject nonHookLeafPrefab;
    public GameObject hookLeafPrefab;
    public GameObject mushroomPrefab;
    public GameObject stemPrefab;
    public GameObject treeTrunkPrefab;
    public GameObject stemSidewaysPrefab;
    public GameObject treeTrunkSidewaysPrefab;

    private List<GameObject> backgrounds;
    private List<LevelPart> levelParts;
    private List<IndexedGameObject> generatedSections;

    private Vector2 deathBoxPos;
    private Vector2 backgroundPos;
    private GameObject envFolder;
    private GameObject bgFolder;
    private bool doneRespawning;
    private int oldSection = 0;
    private int section = 0;
    private int oldParallaxSection = 0;
    private int parallaxSection = 0;
    private float bgWidth;
    private float oldPositionX;
    private float positionX;
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
        player = GetComponent<Rigidbody2D>();
        movement = GetComponent<LateralMovement>();
        doneRespawning = true;
        backgrounds = new List<GameObject>();
        levelParts = new List<LevelPart>();
        generatedSections = new List<IndexedGameObject>();
        bgFolder = GameObject.Find("Background");
        bgWidth = backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 2;
        backgroundPos = new Vector2(-bgWidth, 0);
        deathBoxPos = backgroundPos + (Vector2.down * 5);
        CreateLevelParts();
        InitalizeEnvironment();
        oldPositionX = 0;
        positionX = 0;
    }
    void OnDestroy()
    {
        KillEnemies.OnRespawn -= DoneRespawning;
    }
    void InitalizeEnvironment()
    {
        for (int i = 0; i <3; i++)
        {
            GameObject background = (GameObject)Instantiate(backgroundPrefab, backgroundPos + (Vector2.right * bgWidth * i), Quaternion.identity);
            backgrounds.Add(background);
            backgrounds[i].transform.parent = bgFolder.transform;
        }
    }

    void CreateLevelParts()
    {
        string file = Read("LevelParts");
        string[] fileLines = file.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        List<string> lines = new List<string>();
        lines.AddRange(fileLines);
        for (int i = 0; i < lines.Count; i++)
        {
            string s = lines[i];
            if (s.Split(' ')[0] == "LevelPart")
            {
                string name = s.Split('=')[1].Trim();
                ParseLevelPart(lines, i, name);
                i++;
            }
        }
        GameObject initialPart1 = LevelPart.Instantiate(levelParts[1], Vector2.left * levelPartWidth, Quaternion.identity, -1);
        GameObject initialPart2 = LevelPart.Instantiate(levelParts[0], Vector2.zero, Quaternion.identity, 0);
        GameObject initialPart3 = LevelPart.Instantiate(levelParts[1], Vector2.right * levelPartWidth, Quaternion.identity, 1);
        IndexedGameObject i1;
        IndexedGameObject i2;
        IndexedGameObject i3;
        i1.index = -1;
        i2.index = 0;
        i3.index = 1;
        i1.gameObject = initialPart1;
        i2.gameObject = initialPart2;
        i3.gameObject = initialPart3;
        generatedSections.Add(i1);
        generatedSections.Add(i2);
        generatedSections.Add(i3);
    }
    void Update()
    {
        oldPositionX = positionX;
        UpdateLevelParts();
        ParallaxBackground();
        CheckForDeath();
        positionX = transform.position.x;
        double score = Mathf.Abs(positionX - oldPositionX) * (player.velocity.x / movement.speed) * 10;
        Score(score);
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
            foreach (IndexedGameObject i in generatedSections)
            {
                canGenerate = canGenerate && section != i.index;
                canGenerateAhead = canGenerateAhead && section + direction != i.index;
            }
            if (canGenerate)
            {
                GenerateSection(direction, section);
            }
            if (canGenerateAhead)
            {
                GenerateSection(direction, section + direction);
            }
            DetermineVisibleSections();
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
        for (int i = 0; i < 3; i++)
        {
            Vector2 offset = Vector2.right * bgWidth * parallaxSection;
            backgrounds[i].transform.position += (Vector3)offset;
        }
    }

    void GenerateSection(int direction, int sec)
    {
        int i = Mathf.RoundToInt(Random.value * (LevelPart.Count - 1));
        IndexedGameObject ir1;
        GameObject randomPart1 = LevelPart.Instantiate(levelParts[i], Vector2.right * levelPartWidth * sec, Quaternion.identity, sec);
        ir1.index = sec;
        ir1.gameObject = randomPart1;
        generatedSections.Add(ir1);
    }

    void DetermineVisibleSections()
    {
        foreach(IndexedGameObject ig in generatedSections)
        {
            ig.gameObject.SetActive(Mathf.Abs(ig.index - section) <= 1);
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
    }
    public static string Read(string filename)
    {
        TextAsset theTextFile = Resources.Load<TextAsset>(filename);
        if (theTextFile != null)
            return theTextFile.text;
        return string.Empty;
    }

    void ParseLevelPart(List<string> lines, int levelPartIndex, string name)
    {
        int i = levelPartIndex + 1;
        float x = 0;
        float y = 0;
        int difficulty = 0;
        List<GameObject> objects = new List<GameObject>();
        while (i < lines.Count && lines[i].Split(' ')[0] != "LevelPart")
        {
            string s = lines[i];
            if (s.Contains("x = "))
            {
                string[] line = s.Split(' ');
                try
                {
                    x = float.Parse((line[line.Length - 1]));
                }
                catch (System.FormatException e)
                {
                    Debug.LogError("Attempted to parse a number on line " + (i + 1) + " and failed.");
                    Debug.LogException(e);
                }
            }
            else if (s.Contains("\ty = "))
            {
                string[] line = s.Split(' ');
                try
                {
                    y = float.Parse((line[line.Length - 1]));
                }
                catch (System.FormatException e)
                {
                    Debug.LogError("Attempted to parse a number on line " + (i + 1) + " and failed.");
                    Debug.LogException(e);
                }
            }
            else if (s.Contains("difficulty = "))
            {
                string[] line = s.Split(' ');
                difficulty = int.Parse((line[line.Length - 1]));
            }
            else if (s.Contains("Object"))
            {
                i = ParseObject(lines, i, ref objects);
            }
            i++;
        }
        levelParts.Add(new LevelPart(new Vector2(x, y), objects, difficulty, name));
    }

    int ParseObject(List<string> lines, int ObjectIndex, ref List<GameObject> objects)
    {
        int i = ObjectIndex + 1;
        float x = 0;
        float y = 0;
        string type = "";

        while (i < lines.Count && lines[i].Trim().Substring(0) != "}")
        {
            string s = lines[i];
            if (s.Contains("x = "))
            {
                string[] line = s.Split(' '); try
                {
                    x = float.Parse((line[line.Length - 1]));
                }
                catch (System.FormatException e)
                {
                    Debug.LogError("Attempted to parse a number on line " + (i + 1) + " and failed.");
                    Debug.LogException(e);
                }
            }
            else if (s.Contains("\ty = "))
            {
                string[] line = s.Split(' ');
                try
                {
                    y = float.Parse((line[line.Length - 1]));
                }
                catch (System.FormatException e)
                {
                    Debug.LogError("Attempted to parse a number on line " + (i + 1) + " and failed.");
                    Debug.LogException(e);
                }
            }
            else if (s.Contains("type = "))
            {
                string[] line = s.Split(' ');
                type = line[line.Length - 1];
            }
            i++;
        }
        objects.Add(InstantiateObject(x, y, type));
        return i;
    }

    GameObject InstantiateObject(float x, float y, string type)
    {
        GameObject g;
        if (type == "nonHookLeaf")
        {
            g = (GameObject)Instantiate(nonHookLeafPrefab, new Vector2(x, y), Quaternion.identity);
        }
        else if (type == "hookLeaf")
        {
            g = (GameObject)Instantiate(hookLeafPrefab, new Vector2(x, y), Quaternion.identity);
        }
        else if (type == "mushroom")
        {
            g = (GameObject)Instantiate(mushroomPrefab, new Vector2(x, y), Quaternion.identity);
        }
        else if (type == "stem")
        {
            g = (GameObject)Instantiate(stemPrefab, new Vector2(x, y), Quaternion.identity);
        }
        else if (type == "treeTrunk")
        {
            g = (GameObject)Instantiate(treeTrunkPrefab, new Vector2(x, y), Quaternion.identity);
        }
        else if (type == "stemSideways")
        {
            g = (GameObject)Instantiate(stemSidewaysPrefab, new Vector2(x, y), Quaternion.identity);
        }
        else if (type == "treeTrunkSideways")
        {
            g = (GameObject)Instantiate(treeTrunkSidewaysPrefab, new Vector2(x, y), Quaternion.identity);
        }
        else
        {
            g = new GameObject("Unknown Type (" + type + ")");
        }
        return g;
    }

    string Join(List<string> info)
    {
        string r = "";
        foreach (string s in info)
        {
            r += s;
        }
        return r;
    }
}
