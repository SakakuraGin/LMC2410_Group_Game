using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private float cameraX;
    private float backgroundX;
    [SerializeField] private float distance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraX = Camera.main.transform.position.x;
        backgroundX = transform.position.x;
        //Debug.Log("camX " + cameraX);
        //Debug.Log("bgX " + backgroundX);
        if (cameraX - backgroundX >= distance)
        {
            transform.position = new Vector3(cameraX + distance, transform.position.y, transform.position.z);
        } else if (backgroundX - cameraX >= distance)
        {
            transform.position = new Vector3(cameraX - distance, transform.position.y, transform.position.z);
        }
    }
}
