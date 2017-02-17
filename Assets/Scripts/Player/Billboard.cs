using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{

    void Update()
    {
        var mainCamera = GameObject.FindWithTag("Main Camera").GetComponent<Camera>();

        transform.LookAt(mainCamera.transform);
    }
}