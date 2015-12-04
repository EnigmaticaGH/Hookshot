using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

class LevelPart
{
    private GameObject levelPartFolder;
    private List<GameObject> parts;
    private static int numberOfParts = 0;

    public LevelPart(Vector3 position, List<GameObject> levelParts)
    {
        levelPartFolder = new GameObject("Level Part #" + Count);
        levelPartFolder.transform.position = position;
        parts = levelParts;
        foreach(GameObject part in parts)
        {
            part.transform.position += levelPartFolder.transform.position;
            part.transform.parent = levelPartFolder.transform;
        }
        numberOfParts++;
    }
    public LevelPart(Vector3 position, GameObject levelPart)
    {
        levelPartFolder = new GameObject("Level Part #" + Count);
        levelPartFolder.transform.position = position;
        parts = new List<GameObject>();
        parts.Add(levelPart);
        parts[0].transform.position += levelPartFolder.transform.position;
        parts[0].transform.parent = levelPartFolder.transform;
        numberOfParts++;
    }
    public LevelPart(Vector3 position)
    {
        parts = new List<GameObject>();
        levelPartFolder = new GameObject("Level Part #" + Count);
        levelPartFolder.transform.position = position;
        numberOfParts++;
    }

    public LevelPart()
    {
        parts = new List<GameObject>();
        levelPartFolder = new GameObject("Level Part #" + Count);
        levelPartFolder.transform.position = Vector2.zero;
        numberOfParts++;
    }

    public void AddPart(GameObject part)
    {
        parts.Add(part);
        part.transform.position += levelPartFolder.transform.position;
        part.transform.parent = levelPartFolder.transform;
    }

    public List<GameObject> Parts
    {
        get
        {
            return parts;
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
        InitalizeEnvironment();
        CreateLevelParts();
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
        int lx;
        int ly;
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string folder = "\\Hookshot\\";
        string[] files = Directory.GetFiles(path + folder);
        foreach(string file in files)
        {
            ReadFile(file);
        }
    }

    private void ReadFile(string file)
    {
        List<string> lines = new List<string>();
        string dat;
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
        dat = Join(lines);
        foreach (char c in dat)
        {
            if (c == '{') numLeftBrackets++;
            if (c == '}') numRightBrackets++;
        }
        if (numLeftBrackets != numRightBrackets)
            Debug.LogError("Error parsing " + file + ", brackets don't match up");

        for(int i = 0; i < lines.Count; i++)
        {
            string s = lines[i];
            if (s == "LevelPart")
            {
                Debug.Log("Found LevelPart");
                ParseLevelPart(lines, i);
                i++;
            }
        }
    }

    void ParseLevelPart(List<string> lines, int levelPartIndex)
    {
        int failSafe = 0;
        int i = levelPartIndex + 1;
        float x = 0;
        float y = 0;
        int difficulty = 0;
        while(i < lines.Count && lines[i] != "LevelPart" && failSafe < 10000)
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
                Debug.Log("Found Object");
                i = ParseObject(lines, i);
            }
            i++;
            failSafe++;
        }
        Debug.Log("Lpt x: " + x);
        Debug.Log("Lpt y: " + y);
        Debug.Log("Lpt difficulty: " + difficulty);
        return;
    }

    int ParseObject(List<string> lines, int ObjectIndex)
    {
        int failSafe = 0;
        int i = ObjectIndex + 1;
        float x = 0;
        float y = 0;
        string type = "";
        while (i < lines.Count && lines[i].Trim().Substring(0) != "}" && failSafe < 10000)
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
                    }
                }
            }
            else if (s.Contains("type = "))
            {
                string[] line = s.Split(' ');
                type = line[line.Length - 1];
            }
            else if (s[s.Length - 1] == '}')
            {
                return i;
            }
            i++;
            failSafe++;
        }
        Debug.Log("Obj x: " + x);
        Debug.Log("Obj y: " + y);
        Debug.Log("Obj type: " + type);
        return i;
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
