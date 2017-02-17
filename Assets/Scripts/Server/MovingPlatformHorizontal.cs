using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformHorizontal : MonoBehaviour
{

    public int maxSpeed;

    public float maxDistanceVariance;

    private Vector3 startPosition;

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        MoveVertical();
    }

    void MoveVertical()
    {
        transform.position = new Vector3(transform.position.x, startPosition.y, (transform.position.z + Mathf.Sin(Time.time * maxSpeed) * maxDistanceVariance));

        if (transform.position.z > 1.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.z < -1.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
