using UnityEngine;
using System.Collections;

public class CameraFollow2D : MonoBehaviour
{
    public int numberOfBgLayers;
    public float dampTime;
    public Transform[] Layers;
    public float[] speedOfLayers;
    private Vector3 velocity = Vector3.zero;
    private Transform target;
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    public float rotationSpeed;
    public bool followY;
    public bool followX;
    public bool LimitCameraPos;
    public Vector2 minPos;
    public Vector2 maxPos;
    private float cameraScreenWidth;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        cameraScreenWidth = GetComponent<Camera>().orthographicSize;
    }

    void Update()
    {
        Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
        Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)) + Vector3.right * cameraScreenWidth;

        float yCoordinate = 0;
        float xCoordinate = 0;
        Vector3 destination = Vector3.zero;
        if (followY)
        {
            yCoordinate = (transform.position.y + delta.y);
        }
        else
        {
            yCoordinate = transform.position.y;
        }

        if (followX)
        {
            xCoordinate = transform.position.x + delta.x;
        }
        else
        {
            xCoordinate = transform.position.x;
        }

        if (LimitCameraPos)
        {
            if (followY)
            {
                if (yCoordinate > maxPos.y)
                    yCoordinate = maxPos.y;
                else if (yCoordinate < minPos.y)
                    yCoordinate = minPos.y;
            }

            if (followX)
            {
                if (xCoordinate > maxPos.x)
                    xCoordinate = maxPos.x;
                else if (xCoordinate < minPos.x)
                    xCoordinate = minPos.x;
            }
        }

        destination = new Vector3(xCoordinate, yCoordinate, transform.position.z + delta.z);

        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

    }
}