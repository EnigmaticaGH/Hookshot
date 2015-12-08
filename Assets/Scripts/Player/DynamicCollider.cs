using UnityEngine;
using System.Collections;

public class DynamicCollider : MonoBehaviour
{
    private BoxCollider2D[] Sensors;
    private Sprite sprite;
    private LateralMovement player;
    private GameObject GroundColliders;
    private GameObject JumpColliders;
    private GameObject HookColliders;
    private bool running = true;
    private const float SENSOR_UPDATE_DELTA = 0.1f;

    void Start()
    {
        KillEnemies.OnRespawn += ResetCollider;
        LateralMovement.GroundState += Ground;
        LateralMovement.JumpState += Jump;
        LateralMovement.HookState += Hook;
        player = GetComponentInParent<LateralMovement>();
        GroundColliders = GameObject.Find("GroundColliders");
        JumpColliders = GameObject.Find("JumpColliders");
        HookColliders = GameObject.Find("HookColliders");
        Sensors = new BoxCollider2D[4]
        {
            GameObject.Find("GroundSensor").GetComponent<BoxCollider2D>(),
            GameObject.Find("CeilingSensor").GetComponent<BoxCollider2D>(),
            GameObject.Find("WallSensorR").GetComponent<BoxCollider2D>(),
            GameObject.Find("WallSensorL").GetComponent<BoxCollider2D>()
        };
        Ground();
        StartCoroutine(UpdateSensors());
    }

    void OnDestroy()
    {
        LateralMovement.GroundState -= Ground;
        LateralMovement.JumpState -= Jump;
        LateralMovement.HookState -= Hook;
        KillEnemies.OnRespawn -= ResetCollider;
    }

    void Ground()
    {
        GroundColliders.SetActive(true);
        JumpColliders.SetActive(false);
        HookColliders.SetActive(false);
    }

    void Jump()
    {
        GroundColliders.SetActive(false);
        JumpColliders.SetActive(true);
        HookColliders.SetActive(false);
    }

    void Hook()
    {
        GroundColliders.SetActive(false);
        JumpColliders.SetActive(false);
        HookColliders.SetActive(true);
    }

    void OnEnable()
    {
        if (!running) StartCoroutine(UpdateSensors());
    }

    void OnDisable()
    {
        running = false;
        StopAllCoroutines();
    }

    void ResetCollider()
    {
        Ground();
    }

    IEnumerator UpdateSensors()
    {
        while (true)
        {
            sprite = GetComponent<SpriteRenderer>().sprite;
            Vector3 center = sprite.bounds.center;
            Vector3 extents = sprite.bounds.extents;
            Sensors[1].offset = center + new Vector3(0, extents.y - 0.15f);
            Sensors[2].offset = center + new Vector3(extents.x - 0.15f, 0);
            Sensors[3].offset = center + new Vector3(-extents.x + 0.15f, 0);

            if(!player.isGrounded())
            {
                Sensors[0].offset = new Vector2(0, -extents.y + 0.15f);
            }
            yield return new WaitForSeconds(SENSOR_UPDATE_DELTA);
        }
    }
}
