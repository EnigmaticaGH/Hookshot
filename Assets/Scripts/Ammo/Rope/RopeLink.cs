using UnityEngine;
using System.Collections.Generic;

class RopeLink
{
    public GameObject gameObject;
    public Transform transform;
    public ChainLinkJoint joint;

    public Rigidbody2D body {
        get {
            return gameObject.GetComponent<Rigidbody2D>();
        }
    }

    public float length {
        get {
            return Vector2.Distance(transform.position, joint.connectedTo.transform.position);
        }
    }

    public RopeLink(Object prefab, Vector2 position, GameObject linkedObject)
    {
        gameObject = (GameObject)GameObject.Instantiate(prefab);
        transform = gameObject.transform;
        transform.position = position;

        float linkDistance = Vector2.Distance(position, linkedObject.transform.position);
        joint = new ChainLinkJoint(gameObject, linkedObject, linkDistance);

        SetupCollider();
    }

    void SetupCollider()
    {
        BoxCollider2D bCollider = gameObject.GetComponent<BoxCollider2D>();
        Vector2 size = new Vector2(0.06f, length);
        bCollider.size = size;
        transform.rotation = Quaternion.FromToRotation(Vector2.up, joint.connectedTo.transform.position - transform.position);
        Physics2D.IgnoreCollision(bCollider, joint.connectedTo.GetComponent<BoxCollider2D>());
        Physics2D.IgnoreCollision(bCollider, joint.connectedTo.GetComponent<CircleCollider2D>());
    }

    void RotateCollider()
    {
        Vector3 direction = joint.connectedTo.transform.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector2.up, direction);
    }

    public void FixedUpdate()
    {
        joint.FixedUpdate();
        RotateCollider();
    }
}
