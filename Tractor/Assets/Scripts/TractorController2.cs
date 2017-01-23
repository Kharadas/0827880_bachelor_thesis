using UnityEngine;
using System.Collections;

public class TractorController2 : MonoBehaviour {

    //The maximum amount of power put out by each wheel
    public float maxTorque = 500f;

    //The max distance a wheel can turn
    public float maxSteerAngle = 45f;

    //Brakeforce to be applied
    public float BrakeForce;
    private float steer = 0;
    public GameObject steeringWheel;
    public WheelCollider[] wheelCollider = new WheelCollider[4];

    //Each wheel needs its own mesh
    public Transform[] wheelMesh = new Transform[4];

    public void Start()
    {
        steeringWheel = GameObject.Find("SteeringWheel");
    }

    public void Update()
    {
        //Sets the wheel meshs to match the rotation of the physics WheelCollider
        UpdateMeshPosition();

        //Updates the stering wheel rotation
        UpdateSteeringWheel();

        if (Input.GetKey(KeyCode.Space))
        {
            wheelCollider[2].brakeTorque = BrakeForce;
            wheelCollider[3].brakeTorque = BrakeForce;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            wheelCollider[2].brakeTorque = 0;
            wheelCollider[3].brakeTorque = 0;
        }
    }

    public void UpdateSteeringWheel()
    {
        steeringWheel.transform.rotation = Quaternion.AngleAxis(-30f, new Vector3(30f, 0, 0));
        steeringWheel.transform.Rotate(0, steer, 0);
    }

    //Sets each wheel to move with the physics WheelColliders
    public void UpdateMeshPosition()
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 pos;

            //Gets the current position of the physics WheelColliders
            wheelCollider[i].GetWorldPose(out pos, out quat);

            ///Sets the mesh to match the position and rotation of the physics WheelColliders
            wheelMesh[i].position = pos;
            wheelMesh[i].rotation = quat;
        }
    }

    // Update every fixed framerate frame, use this when dealing with Rigidbody
    public void FixedUpdate()
    {
        //Turn the wheels to a set max, with an input.
        steer = Input.GetAxis("Horizontal") * maxSteerAngle;
        //Move forward or backwards based on the maxTorque, with an input.
        float torque = Input.GetAxis("Vertical") * maxTorque;

        //Sets which wheels turn, this is the two front wheels.
        wheelCollider[0].steerAngle = steer;
        wheelCollider[1].steerAngle = steer;

        //Sets which wheels move forward or backwards.
        for (int i = 0; i < 4; i++)
        {
            wheelCollider[i].motorTorque = torque;
        }
    }
}
