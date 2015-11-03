using UnityEngine;
using System.Collections;

public class CameraFollow2D : MonoBehaviour
{
    public float dampTime;
    private Vector3 velocity = Vector3.zero;
    private Transform target;
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    public float rotationSpeed;
    public bool followY;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
        Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        Vector3 destination = Vector3.zero;
        if(followY)
        {
            destination = transform.position + delta;
        }
        else
        {
            destination = new Vector3(transform.position.x + delta.x, transform.position.y, transform.position.z + delta.z);
        }
            
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

    }
}