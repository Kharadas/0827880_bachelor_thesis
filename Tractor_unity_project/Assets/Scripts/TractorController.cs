using UnityEngine;
using System.Collections;

public class TractorController : MonoBehaviour {
    
    //The max distance a wheel can turn
    public float maxSteerAngle = 45f;
    
    //Brakeforce to be applied
    public float BrakeForce;
    public float steer = 0;
    public WheelCollider[] wheelCollider = new WheelCollider[4];

    //Audio sources for tractor sounds
    public AudioSource tractorSound;
    public AudioClip tractorStart;
    public AudioClip tractorIdle;
    public AudioClip tractorMoving;

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
    private GameObject attachment;

    //Cameras
    private Camera firstPersonCamera;
    private Camera overheadCamera;

    //Conditions to move the tractor
    private bool tractorStarted;
    private bool tractorOnMove;
    private bool tractorReady1;
    private bool tractorReady2;
    private bool tractorReady3;

    public void Start()
    {
        //To balance the vehicle during turns
        tractor = GetComponent<Rigidbody>();
        tractorCoM = tractor.centerOfMass;
        tractor.centerOfMass = new Vector3(tractorCoM.x, tractorCoM.y-4, tractorCoM.z);

        steeringWheel = GameObject.Find("SteeringWheel");
        pedal = GameObject.Find("Pedal");
        gearShift = GameObject.Find("GearShift");
        gasLever = GameObject.Find("GasLever");
        blObj = GameObject.Find("BLObj");
        tlObj = GameObject.Find("TLObj");
        attachment = GameObject.Find("Attachment");

        firstPersonCamera = GameObject.Find("FirstPersonCamera").GetComponent<Camera>();
        overheadCamera = GameObject.Find("OverheadCamera").GetComponent<Camera>();
        overheadCamera.GetComponent<AudioListener>().enabled = false;

        origin = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, 1);

        tractorStarted = false;
        tractorOnMove = false;
        tractorReady1 = false;
        tractorReady2 = false;
        tractorReady3 = false;
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

        //Updates attachment's position
        UpdateAttachment();

        //Updates camera view
        UpdateCameraView();

        
        if (!tractorSound.isPlaying && tractorStarted)
        {
            tractorSound.clip = tractorIdle;
            tractorSound.Play();
        }
        
        if (tractorStarted && tractorOnMove)
        {
            tractorSound.clip = tractorMoving;
            //tractorSound.Play();
        }
    }

    public void UpdateCameraView()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            firstPersonCamera.enabled = !firstPersonCamera.enabled;
            overheadCamera.enabled = !firstPersonCamera.enabled;
        }

        if (firstPersonCamera.enabled)
        {
            overheadCamera.GetComponent<AudioListener>().enabled = false;
            firstPersonCamera.GetComponent<AudioListener>().enabled = true;
        }
        if (overheadCamera.enabled)
        {
            firstPersonCamera.GetComponent<AudioListener>().enabled = false;
            overheadCamera.GetComponent<AudioListener>().enabled = true;
        }
    }

    public void UpdateAttachment()
    {
        if (Input.GetKey(KeyCode.L))
        {
            attachment.transform.localRotation = Quaternion.Slerp(attachment.transform.localRotation, Quaternion.Euler(45, 0, 0), Time.deltaTime * 5);
        }
        
        if (Input.GetKey(KeyCode.K))
        {
            attachment.transform.localRotation = Quaternion.Slerp(attachment.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 5);
        }
        
    }

    public void UpdatePanelObjects()
    {
        if (Input.GetKey(KeyCode.G))
        {
            gasLever.transform.localRotation = Quaternion.Slerp(gasLever.transform.localRotation, Quaternion.Euler(10, -90, 0), Time.deltaTime * 5);
            tractorReady1 = true;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            
            tractorSound.clip = tractorStart;
            if (tractorStarted == false)
                tractorSound.Play();
            tractorStarted = true;
        }

        if (Input.GetKey(KeyCode.B))
        {
            blObj.transform.localRotation = Quaternion.Slerp(blObj.transform.localRotation, Quaternion.Euler(45, 0, 0), Time.deltaTime * 5);
            tractorReady2 = true;
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

        if (Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.Keypad6) || Input.GetKey(KeyCode.Keypad8))
        {
            tractorReady3 = true;
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

    public bool TractorReady()
    {
        return tractorReady1 && tractorReady2 && tractorReady3;
    }

    // Update every fixed framerate frame, use this when dealing with Rigidbody
    public void FixedUpdate()
    {
        //Turn the wheels to a set max, with an input.
        steer = Input.GetAxis("Horizontal") * maxSteerAngle;

        if (TractorReady() == true)
        {
            tractorOnMove = true;
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
}
