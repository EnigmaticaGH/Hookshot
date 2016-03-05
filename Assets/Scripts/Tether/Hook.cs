using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{
    [HideInInspector]
    public HookshotControl hookGun;
    public float hookSpeed;

    private Rigidbody2D hookBody;
    private GameObject hookedObject;
    private Vector3 offsetToHookPoint;
    private ParticleEffectManager partManag;

    void Start()
    {
        hookBody = GetComponent<Rigidbody2D>();
        hookBody.AddForce(transform.rotation * Vector2.right * hookSpeed, ForceMode2D.Impulse);
        partManag = GameObject.FindGameObjectWithTag("Particles Manager").GetComponent<ParticleEffectManager>();
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
        if (c.CompareTag("Hookable"))
        {
            partManag.SendMessage("generateSalivaSplash", transform.position + offsetToHookPoint);
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
            c.CompareTag("Sensor") ||
            c.CompareTag("Respawn");
    }
}