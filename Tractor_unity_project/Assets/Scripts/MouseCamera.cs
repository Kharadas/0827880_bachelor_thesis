using UnityEngine;
using System.Collections;

public class MouseCamera : MonoBehaviour {

    public float speedH = 1.5f;
    public float speedV = 1.5f;

    private float xMouse = 0.0f;
    private float yMouse = 0.0f;

    void Update()
    {
        xMouse += speedH * Input.GetAxis("Mouse X");
        yMouse -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(yMouse, xMouse, 0.0f);
    }

}

