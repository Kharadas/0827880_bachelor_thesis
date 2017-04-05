using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    
    public Camera destCamera;
    public Terrain terrain;
    public GameObject camPos;
    public float speed = 1.0f;

    private Vector3 cameraOrigin;
    private Vector3 destination;
    private float distToGo;
    private float startTime;
    private bool colliding;

    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        colliding = false;
    }

    // Update is called once per frame
    void Update () {
        cameraOrigin = camPos.transform.position;
        destination = destCamera.transform.position;
        distToGo = Vector3.Distance(cameraOrigin, destination);
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == terrain.name)
        {
            colliding = true;
            StartCoroutine("MoveCam");
            RepositionCamera();
        }
    }

    IEnumerator MoveCam()
    {
        yield return new WaitForSeconds(3.0f);
        RepositionCamera();
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == terrain.name)
        {
            colliding = false;
            StartCoroutine("MoveCam");
        }
    }

    public void RepositionCamera()
    {
        float distCovered = (Time.time - startTime) * speed;
        float ratio = distCovered / distToGo;
        if (colliding)
        {
            transform.position = Vector3.Lerp(transform.position, destination, ratio);
        } else {
            transform.position = Vector3.Lerp(transform.position, cameraOrigin, ratio);
        }
    }
}
