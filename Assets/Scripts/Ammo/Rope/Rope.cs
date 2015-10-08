using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class Rope 
{
    private RopeProperties properties;

    private RopeLink[] rope;
    private int jointCount;

    private ChainLinkJoint connectedJoint;

    public Rope(RopeProperties properties)
    {
        this.properties = properties;
        rope = new RopeLink[properties.maxLinkCount];
        jointCount = 0;

        GenerateInitialLinks();
    }

    private void GenerateInitialLinks()
    {
        Vector2 connectedPoint = properties.connectedPosition + properties.connectedObject.transform.rotation * properties.connectionOffset;
        float length = Vector2.Distance(properties.tetherPosition, connectedPoint);

        connectedJoint = new ChainLinkJoint(properties.connectedObject, properties.tetherObject, length);
        ChangeLength(length);
    }

    public void Destroy()
    {
        for (int i = 0; i < jointCount; i++) 
        {
            GameObject.Destroy(rope[i].gameObject);
        }
    }

    public float Length()
    {
        float distance = connectedJoint.distance;
        for (int i = 0; i < jointCount; i++)
            distance += rope[i].length;
        return distance;
    }

    public void ChangeLength(float distance)
    {
        float length = Length();
        float newLength = Mathf.Clamp(length + distance, properties.minLength, properties.maxLength);
        if (length == newLength)
            return;

        int newJointCount = (int)(newLength / properties.linkLength);

        if (distance < 0.0f)
            Shorten(newJointCount);
        else
            Lengthen(newJointCount);

        jointCount = newJointCount;

        connectedJoint.connectedTo = jointCount == 0 ? properties.tetherObject : rope[jointCount - 1].gameObject;
        connectedJoint.distance = newLength - newJointCount * properties.linkLength;
    }

    private void Shorten(int newJointCount)
    {
        for (int i = jointCount; i > newJointCount; i--)
            GameObject.Destroy(rope[i-1].gameObject);
    }

    private void Lengthen(int newJointCount)
    {
        Vector2 direction = GetNewJointDirection();
        for (int i = jointCount; i < newJointCount; i++)
        {
            GameObject previousBody = i == 0 ? properties.tetherObject : rope[i - 1].gameObject; 
            Vector2 position = (Vector2)(previousBody.transform.position) + direction * properties.linkLength;
            rope[i] = new RopeLink(properties.linkPrefab, position, previousBody);
            rope[i].transform.parent = properties.parentObject.transform;
        }
    }

    public Vector2 GetNewJointDirection()
    {
        return connectedJoint.DirectionOfJoint();
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < jointCount; i++)
        {
            rope[i].FixedUpdate();
        }
        connectedJoint.FixedUpdate();
    }

    public void Render(LineRenderer renderer)
    {
        renderer.SetVertexCount(jointCount + 2);
        renderer.SetPosition(0, properties.tetherObject.transform.position);
        for (int i = 0; i < jointCount; i++)
            renderer.SetPosition(i + 1, rope[i].transform.position);
        renderer.SetPosition(jointCount + 1, properties.connectedPosition);
    }
}