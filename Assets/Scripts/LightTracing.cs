using UnityEngine;
using System.Collections;

public class LightTracing : MonoBehaviour {

    //array for lights
    Light[] lights;

	// Use this for initialization
	void Start () 
    {
        //get lights
        lights = FindObjectsOfType<Light>();	
	}

    //get the points of all the lights, and get he color of the lights
    public Color TraceLight(Vector3 pos, Vector3 normal)
    {
        //set the light colour to the default ambient light
        Color returnCol = RenderSettings.ambientLight;

        //for each light, run a light trace
        foreach (Light light in lights)
        {
            returnCol += LightTrace(light, pos, normal);
        }

        //returnt eh light colour
        return returnCol;
    }

    //trace the lighting for one light
    Color LightTrace(Light light, Vector3 pos, Vector3 normal)
    {
        //checks if the distance is within the light range
        if (Vector3.Distance(pos, light.transform.position) <= light.range)
        {
            //runs a trace for the light and applies linear falloff
            Vector3 direction = (light.transform.position - pos).normalized;
            float temp = (light.range - Vector3.Distance(pos, light.transform.position)) / light.range * light.intensity;
            return transparencyTrace(new Color(temp, temp, temp) * (1 - Quaternion.Angle(Quaternion.identity, Quaternion.FromToRotation(normal, direction)) / 90), 
                pos, direction, Vector3.Distance(light.transform.position, pos));
        }

        //otherwise set to black
        return Color.black;
    }

    //for transparent shadows
    Color transparencyTrace(Color col, Vector3 pos, Vector3 dir, float dist)
    {
        Color tempCol = col;
        RaycastHit[] hits;


        //does a raycast hit for all
        hits = Physics.RaycastAll(pos, dir, dist);

        //for each hit it gets the alpha and returns its colour
        foreach (RaycastHit hit in hits)
        {
            Material mat = hit.collider.GetComponent<Renderer>().material;
            tempCol *= 1 - mat.color.a;
        }
        return tempCol;
    }
}
