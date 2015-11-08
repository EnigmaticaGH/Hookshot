using UnityEngine;
using System.Collections;

public class DynamicCollider : MonoBehaviour
{
    private BoxCollider2D[] Sensors;
    private Sprite sprite;
    private const float COLLIDER_UPDATE_DELTA = 0.1f;
    void Start()
    {
        Sensors = new BoxCollider2D[4]
        {
            GameObject.Find("GroundSensor").GetComponent<BoxCollider2D>(),
            GameObject.Find("CeilingSensor").GetComponent<BoxCollider2D>(),
            GameObject.Find("WallSensorR").GetComponent<BoxCollider2D>(),
            GameObject.Find("WallSensorL").GetComponent<BoxCollider2D>()
        };

        StartCoroutine(UpdateCollider());
    }

    IEnumerator UpdateCollider()
    {
        while (true)
        {
            sprite = GetComponent<SpriteRenderer>().sprite;
            Vector3 center = sprite.bounds.center;
            Vector3 extents = sprite.bounds.extents;
            Sensors[0].offset = new Vector2(0, 0.5f + (-extents.y));
            Sensors[1].offset = new Vector2(0, 0.3f + extents.y);
            Sensors[2].offset = new Vector2(extents.x -0.1f, 0.5f);
            Sensors[3].offset = new Vector2(-extents.x + 0.1f, 0.5f);
            Destroy(GetComponent<PolygonCollider2D>());
            gameObject.AddComponent<PolygonCollider2D>();
            yield return new WaitForSeconds(COLLIDER_UPDATE_DELTA);
        }
    }
}
