using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TractorController : MonoBehaviour {

    public List<AxleInfo> axleInfos;    //info about each individual axle
    public float maxPower = 500f;       //max amount of power put out by each wheel
    public float maxSteerAngle = 45f;   //max rotation angle for wheels
    
    //each wheel needs its own mesh
    //public Transform[] wheelMesh = new Transform[4];
    

	//use FixedUpdate when dealing with RigidBody
	void FixedUpdate () {
        float motor = Input.GetAxis("Vertical") * maxPower;
        float steering = Input.GetAxis("Horizontal") * maxSteerAngle;
        
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
	}
    
    public void ApplyLocalPositionToVisuals(WheelCollider collider) {

        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;

    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
