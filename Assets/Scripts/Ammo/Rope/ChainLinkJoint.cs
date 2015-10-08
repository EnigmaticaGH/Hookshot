using UnityEngine;
using System;

class ChainLinkJoint
{
    private GameObject self;
    public GameObject connectedTo;
    public float distance;

    public ChainLinkJoint(GameObject centralObject, GameObject connectedTo, float distance)
    {
        self = centralObject;
        this.connectedTo = connectedTo;
        this.distance = distance;
    }

    public void FixedUpdate()
    {
        Vector2 position = self.transform.position;
        Vector2 connectPoint = connectedTo.transform.position;
        float currentDistance = Vector2.Distance(position, connectPoint);
        if (currentDistance > distance)
        {
            Vector2 targetPosition = connectPoint + (-DirectionOfJoint()) * distance;
            self.transform.position = targetPosition;
        }
    }

    public Vector2 DirectionOfJoint()
    {
        return (connectedTo.transform.position - self.transform.position).normalized;
    }
}
