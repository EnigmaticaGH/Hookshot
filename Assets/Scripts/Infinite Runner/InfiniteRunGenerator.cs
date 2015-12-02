using UnityEngine;
using System.Collections;

public class InfiniteRunGenerator : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject deathBoxPrefab;
    public GameObject nonHookLeafPrefab;
    private GameObject[] backgrounds;
    private GameObject[] deathBoxes;
    private GameObject[] nonHookLeaves;
    private Vector2 deathBoxPos;
    private Vector2 backgroundPos;
    private Vector2 nonHookLeafPos;
    private GameObject envFolder;
    private GameObject bgFolder;
    int oldSection = 0;
    int section = 0;
    float bgWidth;

    // Use this for initialization
    void Start()
    {
        backgrounds = new GameObject[3];
        deathBoxes = new GameObject[3];
        nonHookLeaves = new GameObject[12];

        envFolder = GameObject.Find("Environment");
        bgFolder = GameObject.Find("Background");

        bgWidth = backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 2;

        deathBoxPos = new Vector2(-bgWidth, -5);
        backgroundPos = new Vector2(-bgWidth, 0);
        nonHookLeafPos = new Vector2(-bgWidth - (bgWidth / 2), -0.4f);
        for (int i = 0; i < 3; i++)
        {
            backgrounds[i] = (GameObject)Instantiate(backgroundPrefab, backgroundPos + (Vector2.right * bgWidth * i), Quaternion.identity);
            deathBoxes[i] = (GameObject)Instantiate(deathBoxPrefab, deathBoxPos + (Vector2.right * bgWidth * i), Quaternion.identity);
            backgrounds[i].transform.parent = bgFolder.transform;
            deathBoxes[i].transform.parent = envFolder.transform;
        }
        for (int i = 0; i < 12; i++)
        {
            nonHookLeaves[i] = (GameObject)Instantiate(nonHookLeafPrefab, nonHookLeafPos + (Vector2.right * (bgWidth / 4) * i), Quaternion.identity);
            nonHookLeaves[i].GetComponent<SpringJoint2D>().connectedAnchor = (Vector2)nonHookLeaves[i].transform.position + (Vector2.up * 0.4f);
            nonHookLeaves[i].transform.parent = envFolder.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        oldSection = section;
        float pos = Mathf.Floor(transform.position.x);
        section = (int)(pos / bgWidth);
        if (section > oldSection || section < oldSection)
        {
            MoveSection(section);
        }
    }

    void MoveSection(int section)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 offset = Vector2.right * bgWidth * (i + section);
            backgrounds[i].transform.position = backgroundPos + offset;
            deathBoxes[i].transform.position = deathBoxPos + offset;
        }
        for (int i = 0; i < 12; i++)
        {
            Vector2 offset = Vector2.right * ((bgWidth * section) + (i * (bgWidth / 4)));
            nonHookLeaves[i].transform.position = nonHookLeafPos + offset;
            nonHookLeaves[i].GetComponent<SpringJoint2D>().connectedAnchor = (Vector2)nonHookLeaves[i].transform.position + (Vector2.up * 0.4f);
        }
    }
}
