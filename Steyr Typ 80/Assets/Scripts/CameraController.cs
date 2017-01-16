using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject tractor;

    private Vector3 offset;
    
	void Start () {
        offset = transform.position - tractor.transform.position;
	}
	
	void LateUpdate () {
        transform.position = tractor.transform.position + offset;
	}
}
