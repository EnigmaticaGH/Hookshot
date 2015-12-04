using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

class LevelPart
{
    private GameObject levelPartFolder;
    private List<GameObject> parts;
    private int difficulty;
    private static int numberOfParts = 0;
    public LevelPart(Vector3 position, List<GameObject> levelParts, int diff)
    {
        levelPartFolder = new GameObject("Level Part Reference");
        levelPartFolder.transform.position = position;
        parts = levelParts;
        difficulty = diff;
        foreach(GameObject part in parts)
        {
            part.transform.position += levelPartFolder.transform.position;
            part.transform.parent = levelPartFolder.transform;
            if(part.GetComponent<SpringJoint2D>() != null)
            {
                part.GetComponent<SpringJoint2D>().connectedAnchor = (Vector2)(part.transform.position) + (Vector2.up * 0.4f);
            }
        }
        levelPartFolder.SetActive(false);
    }
    public static void Instantiate(LevelPart level, Vector2 position, Quaternion rotation)
    {
        GameObject copy;
        copy = (GameObject)Object.Instantiate(level.levelPartFolder, position, rotation);
        copy.name = "Level Part " + level.Count;
        numberOfParts++;
        copy.SetActive(true);
        foreach(Transform part in copy.transform)
        {
            //part.position = (Vector2)part.position + position;
            if (part.GetComponent<SpringJoint2D>() != null)
            {
                part.GetComponent<SpringJoint2D>().connectedAnchor = (Vector2)(part.transform.position) + (Vector2.up * 0.4f);
            }
        }
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
    private int Count
    {
        get
        {
            return numberOfParts;
        }
    }
}

public class InfiniteRunGenerator : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject deathBoxPrefab;
    public GameObject nonHookLeafPrefab;
    public GameObject hookLeafPrefab;
    public GameObject mushroomPrefab;
    public GameObject stemPrefab;
    public GameObject treeTrunkPrefab;
    
    private List<GameObject> backgrounds;
    private List<GameObject> deathBoxes;
    private List<LevelPart> levelParts;

    private Vector2 deathBoxPos;
    private Vector2 backgroundPos;
    private GameObject envFolder;
    private GameObject bgFolder;
    int oldSection = 0;
    int section = 0;
    float bgWidth;
    float bgHeight;

    // Use this for initialization
    void Start()
    {
        backgrounds = new List<GameObject>();
        deathBoxes = new List<GameObject>();
        levelParts = new List<LevelPart>();

        envFolder = GameObject.Find("Environment");
        bgFolder = GameObject.Find("Background");

        bgWidth = backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 2;
        bgHeight = backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 2;

        backgroundPos = new Vector2(-bgWidth, 0);
        deathBoxPos = backgroundPos + (Vector2.down * 5);
        CreateLevelParts();
        InitalizeEnvironment();
    }

    void InitalizeEnvironment()
    {
        for (int i = 0; i <3; i++)
        {
            GameObject background = (GameObject)Instantiate(backgroundPrefab, backgroundPos + (Vector2.right * bgWidth * i), Quaternion.identity);
            GameObject deathBox = (GameObject)Instantiate(deathBoxPrefab, deathBoxPos + (Vector2.right * bgWidth * i), Quaternion.identity);
            backgrounds.Add(background);
            deathBoxes.Add(deathBox);
            backgrounds[i].transform.parent = bgFolder.transform;
            deathBoxes[i].transform.parent = envFolder.transform;
        }
    }

    void CreateLevelParts()
    {
        //string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string path = Application.dataPath;
        string folder = "\\Level Data\\";
        string[] files = Directory.GetFiles(path + folder, "*.dat");
        foreach(string file in files)
        {
            ReadFile(file);
        }
        LevelPart.Instantiate(levelParts[0], Vector2.zero, Quaternion.identity);
        LevelPart.Instantiate(levelParts[1], Vector2.right * bgWidth, Quaternion.identity);
        LevelPart.Instantiate(levelParts[1], Vector2.left * bgWidth, Quaternion.identity);
        LevelPart.Instantiate(levelParts[0], Vector2.right * bgWidth * 2, Quaternion.identity);
        LevelPart.Instantiate(levelParts[0], Vector2.left * bgWidth * 2, Quaternion.identity);
    }

    private void ReadFile(string file)
    {
        List<string> lines = new List<string>();
        int numLeftBrackets = 0;
        int numRightBrackets = 0;
        try
        {
            string line;
            using (StreamReader levelPartsReader = new StreamReader(file))
            {
                while((line = levelPartsReader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
        }
        catch(IOException e)
        {
            Debug.LogError(e);
        }
        foreach (char c in Join(lines))
        {
            if (c == '{') numLeftBrackets++;
            if (c == '}') numRightBrackets++;
        }
        if (numLeftBrackets != numRightBrackets)
            Debug.LogError("Error parsing " + file + ", brackets don't match up");
        else
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string s = lines[i];
                if (s == "LevelPart")
                {
                    ParseLevelPart(lines, i);
                    i++;
                }
            }
        }
    }

    void ParseLevelPart(List<string> lines, int levelPartIndex)
    {
        int i = levelPartIndex + 1;
        float x = 0;
        float y = 0;
        int difficulty = 0;
        List<GameObject> objects = new List<GameObject>();
        while (i < lines.Count && lines[i] != "LevelPart")
        {
            string s = lines[i];
            if (s.Contains("x = "))
            {
                string[] line = s.Split(' ');
                if (line[line.Length - 1] == "BGWIDTH")
                {
                    x = bgWidth;
                }
                else
                {
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
            }
            else if (s.Contains("\ty"))
            {
                string[] line = s.Split(' ');
                if (line[line.Length - 1] == "BGHEIGHT")
                {
                    y = bgHeight;
                }
                else
                {
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
        levelParts.Add(new LevelPart(new Vector2(x, y), objects, difficulty));
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
                string[] line = s.Split(' ');
                if (line[line.Length - 1] == "BGWIDTH")
                {
                    x = bgWidth;
                }
                else
                {
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
            }
            else if (s.Contains("\ty = "))
            {
                string[] line = s.Split(' ');
                if (line[line.Length - 1] == "BGHEIGHT")
                {
                    y = bgHeight;
                }
                else
                {
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
        else
        {
            g = new GameObject("Unknown Type (" + type + ")");
        }
        return g;
}

    string Join(List<string> info)
    {
        string r = "";
        foreach(string s in info)
        {
            r += s;
        }
        return r;
    }

    // Update is called once per frame
    void Update()
    {
        oldSection = section;
        float pos = Mathf.Floor(transform.position.x);
        section = (int)(pos / bgWidth);
        if (section > oldSection || section < oldSection)
        {
            MoveSection();
        }
    }

    void MoveSection()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 offset = Vector2.right * bgWidth * (i + section);
            backgrounds[i].transform.position = backgroundPos + offset;
            deathBoxes[i].transform.position = deathBoxPos + offset;
        }
    }
}
