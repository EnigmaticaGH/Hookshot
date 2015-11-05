using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Hookable"))
        {
            hookedObject = collision.collider.gameObject;
            offsetToHookPoint = transform.position - hookedObject.transform.position;
            offsetToHookPoint = Quaternion.Inverse(hookedObject.transform.rotation) * offsetToHookPoint;
            Rigidbody2D hookBody = GetComponent<Rigidbody2D>();
            hookBody.isKinematic = true;
            hookBody.velocity = Vector3.zero;
            hookGun.HookOn();
            GetComponent<PolygonCollider2D>().enabled = false;
        }
        else 
        {
            hookGun.CancelHook();
        }

        if (collision.collider.CompareTag("Edible"))
        {
            Destroy(collision.collider.gameObject);
            hookGun.CancelHook();
        }
    }
}
