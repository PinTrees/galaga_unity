using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float ClampX = 5f;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(horizontalInput, 0f, 0f);

        Vector3 newPosition = transform.position + movement * moveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, -ClampX, ClampX);

        transform.position = newPosition;
    }
}
