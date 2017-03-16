using UnityEngine;
using System.Collections;

public class TractorController2 : MonoBehaviour {
    
    //The max distance a wheel can turn
    public float maxSteerAngle = 45f;
    
    //Brakeforce to be applied
    public float BrakeForce;
    public float steer = 0;
    public WheelCollider[] wheelCollider = new WheelCollider[4];

    //Each wheel needs its own mesh
    public Transform[] wheelMesh = new Transform[4];

    //To change the center of gravity
    private Rigidbody tractor;
    private Vector3 tractorCoM;

    //Origin of the pedal
    private Quaternion origin;

    //The maximum amount of power put out by each wheel
    private float maxTorque = 500f;

    //Control parts
    private GameObject steeringWheel;
    private GameObject pedal;
    private GameObject gearShift;
    private GameObject gasLever;
    private GameObject blObj;
    private GameObject tlObj;

    public void Start()
    {
        //To balance the vehicle during turns
        tractor = GetComponent<Rigidbody>();
        tractorCoM = tractor.centerOfMass;
        tractor.centerOfMass = new Vector3(tractorCoM.x, tractorCoM.y-3, tractorCoM.z);

        steeringWheel = GameObject.Find("SteeringWheel");
        pedal = GameObject.Find("Pedal");
        gearShift = GameObject.Find("GearShift");
        gasLever = GameObject.Find("GasLever");
        blObj = GameObject.Find("BLObj");
        tlObj = GameObject.Find("TLObj");

        origin = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, 1);
    }

    public void Update()
    {
        //Sets the wheel meshs to match the rotation of the physics WheelCollider
        UpdateWheelPosition();

        //Updates the stering wheel rotation
        UpdateSteeringWheel();

        //Updates the brake control and the pedal
        UpdateBrake();
        
        //Updates gear shift
        UpdateGearShift();

        //Updates object rotations on the panel
        UpdatePanelObjects();

    }

    public void UpdatePanelObjects()
    {
        if (Input.GetKey(KeyCode.G))
        {
            gasLever.transform.localRotation = Quaternion.Slerp(gasLever.transform.localRotation, Quaternion.Euler(10, -90, 0), Time.deltaTime * 5);
        }

        if (Input.GetKey(KeyCode.B))
        {
            blObj.transform.localRotation = Quaternion.Slerp(blObj.transform.localRotation, Quaternion.Euler(45, 0, 0), Time.deltaTime * 5);
        }

        if (Input.GetKey(KeyCode.T))
        {
            tlObj.transform.localRotation = Quaternion.Slerp(tlObj.transform.localRotation, Quaternion.Euler(0, 90, -15), Time.deltaTime * 5);
        }
        if (Input.GetKey(KeyCode.Y))
        {
            tlObj.transform.localRotation = Quaternion.Slerp(tlObj.transform.localRotation, Quaternion.Euler(0, -90, 15), Time.deltaTime * 5);
        }
    }

    public void UpdateGearShift()
    {
        //reset
        if (Input.GetKey(KeyCode.Keypad5))
        {
            gearShift.transform.localRotation = Quaternion.Slerp(origin, gearShift.transform.localRotation, Time.deltaTime * 2);
        }

        //left
        if (Input.GetKey(KeyCode.Keypad4))
        {
            gearShift.transform.localRotation = Quaternion.Slerp(gearShift.transform.localRotation, Quaternion.Euler(0, 0, 10), Time.deltaTime * 5);
            maxTorque = 350;
        }

        //right
        if (Input.GetKey(KeyCode.Keypad6))
        {
            gearShift.transform.localRotation = Quaternion.Slerp(gearShift.transform.localRotation, Quaternion.Euler(0, 0, -5), Time.deltaTime * 5);
            maxTorque = 600;
        }

        //forward
        if (Input.GetKey(KeyCode.Keypad8))
        {
            gearShift.transform.localRotation = Quaternion.Slerp(gearShift.transform.localRotation, Quaternion.Euler(0, 10, 0), Time.deltaTime * 5);
            maxTorque = 750;
        }

        //backward
        if (Input.GetKey(KeyCode.Keypad2))
        {
            gearShift.transform.localRotation = Quaternion.Slerp(gearShift.transform.localRotation, Quaternion.Euler(0, -10, 0), Time.deltaTime * 5);
            maxTorque = 250;
        }
    }

    public void UpdateBrake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            wheelCollider[2].brakeTorque = BrakeForce;
            wheelCollider[3].brakeTorque = BrakeForce;
            pedal.transform.localRotation = Quaternion.Slerp(pedal.transform.localRotation, Quaternion.Euler(30, 0, 0), Time.deltaTime * 2);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            wheelCollider[2].brakeTorque = 0;
            wheelCollider[3].brakeTorque = 0;
            pedal.transform.localRotation = Quaternion.Slerp(origin, pedal.transform.localRotation, Time.deltaTime * 2);
        }
    }

    public void UpdateSteeringWheel()
    {
        steeringWheel.transform.localRotation = Quaternion.AngleAxis(-30, new Vector3(30, 0, 0));
        steeringWheel.transform.Rotate(new Vector3(0, steer, 0));

        //steeringWheel.transform.eulerAngles = new Vector3(steeringWheel.transform.eulerAngles.x, steer, steeringWheel.transform.eulerAngles.z);

        //float minRotation = -30;
        //float maxRotation = 30;
        //Vector3 currentRotation = steeringWheel.transform.localRotation.eulerAngles;
        //currentRotation.y = Mathf.Clamp(steer, minRotation, maxRotation);
        //steeringWheel.transform.localRotation = Quaternion.Euler(currentRotation);
        //steeringWheel.transform.localRotation = Quaternion.AngleAxis(-30, new Vector3(0,1,0));
    }

    //Sets each wheel to move with the physics WheelColliders
    public void UpdateWheelPosition()
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

        /*
        if (torque > 200)
        {
            if (steer < 0)  //turning left
            {
                wheelCollider[3].mass = 500;
            } else {        //turning right
                wheelCollider[2].mass = 500;
            }
        } else {
            wheelCollider[2].mass = 70;
            wheelCollider[3].mass = 70;
        }
        */
    }
}
