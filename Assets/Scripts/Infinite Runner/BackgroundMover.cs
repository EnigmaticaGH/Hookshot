using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundMover : MonoBehaviour {
    public float parallaxBackgroundSpeed;

    public GameObject backgroundPrefab;
    private GameObject backgroundFolder;
    private List<GameObject> backgrounds;

    private float bgWidth;

    private Vector2 backgroundPos;
    private int parallaxSection = 0;

    private Rigidbody2D playerBody;
    private float playerSpeed;

    void Awake()
    {
        // Pick out some properties of the player so we can move the backgrounds 
        // at appropriate speeds
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerBody = player.GetComponent<Rigidbody2D>();
        playerSpeed = player.GetComponent<LateralMovement>().speed;

        // Assemble background pool
        backgroundFolder = new GameObject("Background");
        backgroundFolder.transform.position = Vector2.zero;
        backgrounds = new List<GameObject>();

        // Make the backgrounds overlap just a little bit, to prevent the white back-background from showing
        bgWidth = (backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * 2) - 0.01f;
        backgroundPos = Vector2.zero;

        // Initialize environment
        for (int i = 0; i < 4; i++)
        {
            GameObject background = (GameObject)Instantiate(backgroundPrefab, 
                backgroundPos + (Vector2.right * bgWidth * i), 
                Quaternion.identity);
            backgrounds.Add(background);
            backgrounds[i].transform.parent = backgroundFolder.transform;
        }
    }
	
	void Update () 
    {
        Vector2 halfPlayerVelocity = Vector2.right * (playerBody.velocity.x / playerSpeed) * parallaxBackgroundSpeed;
        foreach(GameObject bg in backgrounds)
        {
            bg.GetComponent<Rigidbody2D>().velocity = halfPlayerVelocity;
        }

        float positionXFromCenter = playerBody.transform.position.x - backgrounds[1].transform.position.x;
        parallaxSection = (int)(positionXFromCenter / bgWidth);
        MoveSection();
	}

    void MoveSection()
    {
        for (int i = 0; i < backgrounds.Count; i++)
        {
            Vector3 offset = Vector2.right * bgWidth * parallaxSection;
            backgrounds[i].transform.position += offset;
        }
    }
}
