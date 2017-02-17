using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformVertical : MonoBehaviour {

    public int maxSpeed;

    public float maxHeightVariance;

    private Vector3 startPosition;

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
    }


    // Update is called once per frame
    void Update () {
        MoveVertical();
    }

    void MoveVertical()
    {
        transform.position = new Vector3(transform.position.x, (startPosition.y + Mathf.Sin(Time.time * maxSpeed) * maxHeightVariance), transform.position.z);

        if (transform.position.y > 100.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.y < -100.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
