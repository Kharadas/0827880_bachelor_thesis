using UnityEngine;
using System.Collections;

public class TractorController : MonoBehaviour {
    
    //The max distance a wheel can turn
    private float maxSteerAngle = 45f;
    
    //Brakeforce to be applied
    public float BrakeForce;
    private float steer = 0;
    public WheelCollider[] wheelCollider = new WheelCollider[4];

    //Audio sources for tractor sounds
    public AudioSource tractorSound;
    public AudioSource clickSound;
    public AudioSource gearSwitchSound;
    public AudioClip tractorStart;
    public AudioClip tractorIdle;
    public AudioClip tractorMoving;
    public AudioClip tractorStop;
    public AudioClip click;

    //Each wheel needs its own mesh
    public Transform[] wheelMesh = new Transform[4];

    //To change the center of gravity
    private Rigidbody tractor;
    private Vector3 tractorCoM;

    //Onject's origin
    private Quaternion origin;

    //The maximum amount of power put out by each wheel
    private float maxTorque = 500f;

    //Control parts
    public GameObject steeringWheel;
    public GameObject pedal;
    public GameObject gearShift;
    public GameObject gasLever;
    public GameObject blObj;
    public GameObject tlObj;
    public GameObject attachment;

    //Cameras
    public Camera firstPersonCamera;
    public Camera overheadCamera;
    public Camera frontViewCamera;

    //Conditions to move the tractor
    private bool tractorStarted;
    private bool tractorStopped;
    private bool tractorOnMove;
    private bool tractorReady1;
    private bool tractorReady2;
    private bool tractorReady3;

    //Tractor movement check - LateUpdate
    private Vector3 oldPos;
    private Vector3 newPos;
    private bool isMoving;

    private int countB;

    public void Start()
    {
        //To balance the vehicle during turns
        tractor = GetComponent<Rigidbody>();
        tractorCoM = tractor.centerOfMass;
        tractor.centerOfMass = new Vector3(tractorCoM.x, tractorCoM.y-4, tractorCoM.z);

        frontViewCamera.GetComponent<AudioListener>().enabled = false;
        overheadCamera.GetComponent<AudioListener>().enabled = false;

        origin = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, 1);

        tractorStarted = false;
        tractorStopped = false;
        tractorOnMove = false;
        tractorReady1 = false;
        tractorReady2 = false;
        tractorReady3 = false;

        isMoving = false;
        
        countB = 0;
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

        //Updates engine's sound
        UpdateSounds();
        
    }

    void LateUpdate()
    {
        newPos.z = tractor.position.z;

        if (Mathf.Abs(oldPos.z-newPos.z) > 0.01)
        {
            isMoving = true;
        }

        oldPos.z = newPos.z;
    }

    public void UpdateSounds()
    {
            if (!tractorSound.isPlaying && tractorStarted && !tractorStopped)
            {
                tractorSound.clip = tractorIdle;
                if (tractorOnMove)
                {
                    tractorSound.clip = tractorMoving;
                    tractorSound.loop = true;
                }
                tractorSound.Play();
            }
    }

    public void UpdateCameraView()
    {
        if (Input.GetKey(KeyCode.Alpha1)) {
            firstPersonCamera.enabled = true;
            overheadCamera.enabled = false;
            frontViewCamera.enabled = false;
        } else if (Input.GetKey(KeyCode.Alpha2)) {
            firstPersonCamera.enabled = false;
            overheadCamera.enabled = true;
            frontViewCamera.enabled = false;
        } else if (Input.GetKey(KeyCode.Alpha3)) {
            firstPersonCamera.enabled = false;
            overheadCamera.enabled = false;
            frontViewCamera.enabled = true;
        }

        if (firstPersonCamera.enabled)
        {
            overheadCamera.GetComponent<AudioListener>().enabled = false;
            firstPersonCamera.GetComponent<AudioListener>().enabled = true;
            frontViewCamera.GetComponent<AudioListener>().enabled = false;
        }

        if (overheadCamera.enabled)
        {
            firstPersonCamera.GetComponent<AudioListener>().enabled = false;
            overheadCamera.GetComponent<AudioListener>().enabled = true;
            frontViewCamera.GetComponent<AudioListener>().enabled = false;
        }

        if (frontViewCamera.enabled)
        {
            firstPersonCamera.GetComponent<AudioListener>().enabled = false;
            overheadCamera.GetComponent<AudioListener>().enabled = false;
            frontViewCamera.GetComponent<AudioListener>().enabled = true;
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
        if (Input.GetKey(KeyCode.H))
        {
            gasLever.transform.localRotation = Quaternion.Slerp(gasLever.transform.localRotation, Quaternion.Euler(10, 0, 0), Time.deltaTime * 5);
            tractorReady1 = false;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            tractorSound.loop = false;
            tractorStarted = false;
            tractorSound.clip = tractorStop;
            if (tractorStopped == false)
                tractorSound.Play();
            tractorStopped = true;
            blObj.transform.localRotation = Quaternion.Slerp(origin, blObj.transform.localRotation, Time.deltaTime * 1);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            tractorStopped = false;
            tractorSound.clip = tractorStart;
            if (tractorStarted == false)
                tractorSound.Play();
            tractorStarted = true;
        }

        if (Input.GetKey(KeyCode.B))
        {
            if (countB < 1)
                blObj.transform.localRotation = Quaternion.Slerp(blObj.transform.localRotation, Quaternion.Euler(45, 0, 0), Time.deltaTime * 5);            
            tractorReady2 = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (countB < 1)
                clickSound.Play();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            countB++;
        }

        if (Input.GetKey(KeyCode.T))
        {
            tlObj.transform.localRotation = Quaternion.Slerp(tlObj.transform.localRotation, Quaternion.Euler(0, 90, -15), Time.deltaTime * 5);
        }
        if (Input.GetKey(KeyCode.Y))
        {
            tlObj.transform.localRotation = Quaternion.Slerp(tlObj.transform.localRotation, Quaternion.Euler(0, -90, 15), Time.deltaTime * 5);
        }

        if (tractorStopped)
        {
            countB = 0;
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

        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            gearSwitchSound.Play();
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

    // Check if tractor is ready to move
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
            if (isMoving == true)
            {
                tractorOnMove = true;
            }
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
