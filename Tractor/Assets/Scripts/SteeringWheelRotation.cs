using UnityEngine;
using System.Collections;

public class SteeringWheelRotation : MonoBehaviour {

    [Range(-45f, 45f)]
    public float sWRotation = 0f;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        transform.rotation = Quaternion.AngleAxis(-30f, new Vector3(30f,0,0));
        transform.Rotate(0, -sWRotation, 0);
    }
}
