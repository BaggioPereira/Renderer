using UnityEngine;
using System.Collections;

public class ColorTrace : MonoBehaviour {

    LightTracing lTrace;

    [HideInInspector]
    //array for materials
    public Object[] materials;

    [HideInInspector]
    //array for walls and meshes
    public GameObject[] meshes, walls;

    [HideInInspector]
    //boolean array for buttons
    public bool[] matButton, posButton, shineButton;

	// Use this for initialization
	public void Start () 
    {
        //get the Light Tracing script
        lTrace = GetComponent<LightTracing>();

        //get materials
        materials = Resources.LoadAll("Materials");

        //get objects
        meshes = GameObject.FindGameObjectsWithTag("Meshes");
        walls = GameObject.FindGameObjectsWithTag("Walls");

        //set size of booleans to number of meshes
        matButton = new bool[meshes.Length];
        posButton = new bool[meshes.Length];
        shineButton = new bool[meshes.Length];
	}

    //create a ray and shoot it at the pixel
    public Color TracePixel(Vector2 pos)
    {
        //create a ray at x,y
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(pos.x / GetComponent<Tracer>().resolution, pos.y / GetComponent<Tracer>().resolution, 0));

        //call Trace to get a colour for the ray
        return Trace(ray.origin, ray.direction, 0);
    }

    //Get the color the ray has hit and assign it to the texture(x,y)
    Color Trace(Vector3 origin, Vector3 dir, int rec)
    {
        RaycastHit hit;

        //set default temp colour to black
        Color returnColor = Color.black;
        float temp;

        //run loop for number of iterations
        if (rec < GetComponent<Tracer>().recursion && Physics.Raycast(origin, dir, out hit, GetComponent<Tracer>().rayDistance))
        {
            //get the material
            Material mat = hit.collider.GetComponent<Renderer>().material;

            //set a temp color to the material colour
            returnColor = mat.color;

            //add metallic effect if material has any
            if (mat.HasProperty("_Metallic"))
            {
                //temp float is the alpha for the material * metallic * shinniness
                temp = returnColor.a * mat.GetFloat("_Glossiness") * mat.GetFloat("_Metallic");

                //if temp is > 0, run lighting calculations
                if (temp > 0)
                    returnColor += temp * Trace(hit.point + hit.normal * 0.0001f, Vector3.Reflect(dir, hit.normal), rec + 1);
            }

            //add specular effect if material has any
            if (mat.HasProperty("_SpecColor"))
            {
                // temp float is aplha * specularity value
                temp = returnColor.a * mat.GetFloat("_Glossiness");

                //do specular lighting calulation
                returnColor += temp * Trace(hit.point + hit.normal * 0.0001f, Vector3.Reflect(dir, hit.normal), rec + 1);
            }

            //run lighting calculation for the ray
            returnColor *= lTrace.TraceLight(hit.point + hit.normal * 0.0001f, hit.normal);
        }

        //set alpha value to 1
        returnColor.a = 1;

        //send temp colour back
        return returnColor;
    }
}
