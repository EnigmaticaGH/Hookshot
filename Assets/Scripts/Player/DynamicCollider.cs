using UnityEngine;
using System.Collections;

public class DynamicCollider : MonoBehaviour
{
    private BoxCollider2D[] Sensors;
    private Sprite sprite;
    private LateralMovement player;
    private Vector2[] defaultCollider;
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

        player = GetComponentInParent<LateralMovement>();
        defaultCollider = GetComponent<PolygonCollider2D>().points;

        StartCoroutine(UpdateCollider());
    }

    IEnumerator UpdateCollider()
    {
        while (true)
        {
            sprite = GetComponent<SpriteRenderer>().sprite;
            Vector3 center = sprite.bounds.center;
            Vector3 extents = sprite.bounds.extents;

            Sensors[1].offset = new Vector2(0, extents.y - 0.15f);
            Sensors[2].offset = new Vector2(extents.x - 0.2f, 0);
            Sensors[3].offset = new Vector2(-extents.x + 0.2f, 0);
            if(!player.isGrounded())
            {
                Sensors[0].offset = new Vector2(0, -extents.y + 0.1f);
                Destroy(GetComponent<PolygonCollider2D>());
                gameObject.AddComponent<PolygonCollider2D>();
            }
            else if(!player.isHooked())
            {
                GetComponent<PolygonCollider2D>().points = defaultCollider;
            }
            yield return new WaitForSeconds(COLLIDER_UPDATE_DELTA);
        }
    }
}
