using UnityEngine;
using System.Collections;

public class TractorLights : MonoBehaviour {

    public Renderer headLightLEFT;
    public Renderer headLightRIGHT;
    public Material headLightsON;
    public Material headLightsOFF;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown("O"))
        {
            headLightLEFT.material = headLightsON;
            headLightRIGHT.material = headLightsON;

        }

        if (Input.GetKeyDown("F"))
        {
            headLightLEFT.material = headLightsOFF;
            headLightRIGHT.material = headLightsOFF;
        }
    }
}
