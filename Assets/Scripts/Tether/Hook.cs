using UnityEngine;
using System.Collections;
using System.IO;

public class Hook : MonoBehaviour
{
    [HideInInspector]
    public HookshotControl hookGun;
    public float hookSpeed;

    private Rigidbody2D hookBody;
    private GameObject hookedObject;
    private Vector3 offsetToHookPoint;

    void Start()
    {
        hookBody = GetComponent<Rigidbody2D>();
        hookBody.AddForce(transform.rotation * Vector2.right * hookSpeed, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        if (hookGun.IsHooked())
        {
            transform.position = hookedObject.transform.position + hookedObject.transform.rotation * offsetToHookPoint;
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        StreamWriter f;
        using (f = new StreamWriter(Application.dataPath + "\\file4.txt"))
        {
            f.WriteLine(c.name + ", " + c.transform.position);
        }
        if (c.CompareTag("Hookable"))
        {
            hookedObject = c.gameObject;
            offsetToHookPoint = transform.position - hookedObject.transform.position;
            offsetToHookPoint = Quaternion.Inverse(hookedObject.transform.rotation) * offsetToHookPoint;
            Rigidbody2D hookBody = GetComponent<Rigidbody2D>();
            hookBody.isKinematic = true;
            hookBody.velocity = Vector3.zero;
            hookGun.HookOn();
            GetComponent<CircleCollider2D>().enabled = false;
        }
        else if(!TagsToIgnore(c))
        {
            hookGun.CancelHook();
        }

        if (c.CompareTag("Edible"))
        {
            Destroy(c.gameObject);
            hookGun.CancelHook();
        }
    }

    bool TagsToIgnore(Collider2D c)
    {
        return c.CompareTag("Player Sprite") ||
            c.CompareTag("Feet") ||
            c.CompareTag("Back") ||
            c.CompareTag("Mouth") ||
            c.CompareTag("Ass") ||
            c.CompareTag("Respawn");
    }
}