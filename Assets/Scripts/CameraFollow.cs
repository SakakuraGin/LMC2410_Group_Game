using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    private bool swap = false;

    private Transform target;
    [SerializeField] private Transform player;
    [SerializeField] private Transform teleport;

    private void Awake() {
        target = player;
    }
    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            swapTarget();
        }
    }

    public void swapTarget() {
        if (!swap) {
            target = teleport;
            swap = true;
        } else {
            target = player;
            swap = false;
        }
    }
}
