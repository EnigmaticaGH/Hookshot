using UnityEngine;
using System.Collections;

public class LateralMovement : MonoBehaviour
{
    public float speed;
    public float speedInWater;
    private float regularSpeed;
    public float force;
    public float moveForce;
    public HookshotControl hookshotControl;
    public SpriteRenderer characterSprite;
    private Rigidbody2D player;
    private Vector2 contactNormal;
    public WallSensor wallSensorRight;
    public WallSensor wallSensorLeft;

    void Start()
    {
        regularSpeed = speed;
        player = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        Move(horizontal);
        Orient(horizontal);
    }

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.CompareTag("Water"))
            speed = speedInWater;
    }

    //void OnTriggerEnter2D(Collider2D c)
    //{
    //    if (c.CompareTag("Water"))
    //        player.velocity /= 2;
    //}

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Water"))
            speed = regularSpeed;
    }

    void Move(float horizontal)
    {
        int traverse = 1;

        if (!checkWalls(horizontal))
            traverse = 0;

        if (!hookshotControl.IsHooked())
        {
            Vector2 lateralForce = new Vector2(horizontal * moveForce * traverse, 0);

            if (Mathf.Abs(player.velocity.x) < speed)
                player.AddForce(lateralForce);

            if (player.velocity.x > 0 && horizontal < 0
             || player.velocity.x < 0 && horizontal > 0)
                player.velocity = new Vector2(0, player.velocity.y);
        }
        else
        {
            Vector2 pivotPoint = hookshotControl.HookPoint();
            if (horizontal > 0 && pivotPoint.x >= transform.position.x || horizontal < 0 && pivotPoint.x <= transform.position.x)
            {
                Vector2 lateralForce = Vector3.Cross((Vector3)pivotPoint - transform.position, Vector3.forward).normalized;                
                lateralForce *= horizontal * force * traverse/ (player.velocity.magnitude + 1f);
                player.AddForce(lateralForce);
            }
        }
    }

    void Orient(float horizontal)
    {
        if (!hookshotControl.IsHooked() && horizontal != 0)
        {
            Quaternion rot = horizontal == 1 ? Quaternion.Euler(0, 0, -5.73f) : Quaternion.Euler(0, 180, -5.73f);
            characterSprite.transform.rotation = rot;
        }
    }

    bool checkWalls(float d)
    {
        if (d > 0)
        {
            return wallSensorRight.emptySpace;
        }
        else if (d < 0)
        {
            return wallSensorLeft.emptySpace;
        }
        else
            return true;
    }
}
