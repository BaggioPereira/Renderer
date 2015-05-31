using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tracer : MonoBehaviour 
{
    //texture and resolution
    public int resolution;
    Texture2D outTex;

    //how far the ray goes and amount of iterations
    public float rayDistance;
    public int recursion;

    ColorTrace cTrace;

    //boolean for walls
    bool wallShine = false;

    void Start()
    {
        //get 
        cTrace = GetComponent<ColorTrace>();

        //delete texture if already made
        if (outTex)
            Destroy(outTex);
        
        //set resolution to 1 by default
        resolution = 1;

        //create new texture
        outTex = new Texture2D(Screen.width*resolution,Screen.height*resolution);

        //run ray tracer
        RayTrace();
    }

    void OnGUI()
    {
        //draw the texture to screen after the ray trace is done
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), outTex);

        //x and y position for material buttons
        int xMat = Mathf.RoundToInt(Screen.width *0.05f);
        int yMat = Mathf.RoundToInt(Screen.height*0.05f);

        //submenu x and y location for materials
        int subX = Mathf.RoundToInt(Screen.width * 0.05f) + 64;
        int matInital = subX;
        int subY = Mathf.RoundToInt(Screen.height * 0.05f);

        //x and y location for position buttons
        int xPos = Mathf.RoundToInt(Screen.width * 0.05f);
        int yPos = Mathf.RoundToInt(Screen.height * 0.3f);

        //submenu x and y location for position
        int subX2 = Mathf.RoundToInt(Screen.width * 0.05f) +64;
        int subY2 = Mathf.RoundToInt(Screen.height * 0.3f);

        //x and y location for material shininess buttons
        int xShine = Mathf.RoundToInt(Screen.width * 0.05f);
        int yShine = Mathf.RoundToInt(Screen.height * 0.65f);

        //submenu x and y location for position
        int subX3 = Mathf.RoundToInt(Screen.width * 0.05f) +64;
        int subY3 = Mathf.RoundToInt(Screen.height * 0.65f);

        //buttons to change materials of objects
        GUI.Label(new Rect(xMat, yMat - 20, 64, 20), "Materials");
        for (int i = 0; i < cTrace.meshes.Length; i++)
        {
            //if button is pressed
            if (GUI.Button(new Rect(xMat, yMat, 64, 20), cTrace.meshes[i].name))
            {
                //set boolean
                cTrace.matButton[i] = !cTrace.matButton[i];
            }
            yMat += 20;
        }

        //Submenu for materials
        for (int i = 0; i < cTrace.matButton.Length; i++)
        {
            //if button is active
            if (cTrace.matButton[i])
            {
                //create buttons for each material available
                for (int j = 0; j < cTrace.materials.Length; j++)
                {
                    //if a material button is pressed
                    if (GUI.Button(new Rect(subX, subY, 90, 20), cTrace.materials[j].name))
                    {
                        //set the new material, close all material buttons and run the ray tracer
                        cTrace.meshes[i].GetComponent<Renderer>().material = (Material)cTrace.materials[j];
                        cTrace.matButton[i] = false;
                        RayTrace();
                    }
                    subX += 90;
                }
            }
            subX = matInital;
            subY += 20;
        }

        //buttons for positions
        GUI.Label(new Rect(xPos, yPos - 20, 64, 20), "Position");
        GUI.Label(new Rect(subX2, subY2 - 20, 64, 20), "X Position");
        GUI.Label(new Rect(subX2 + 64, subY2 - 20, 64, 20), "Y Position");
        GUI.Label(new Rect(subX2 + 128, subY2 - 20, 64, 20), "Z Position");

        for (int i = 0; i < cTrace.meshes.Length; i++)
        {
            //if button is pressed
            if(GUI.Button(new Rect(xPos, yPos,64,20), cTrace.meshes[i].name))
            {
                //set boolean
                cTrace.posButton[i] = !cTrace.posButton[i];
            }
            yPos += 40;
        }

        //submenu for position
        for (int i = 0; i < cTrace.posButton.Length; i++)
        {
            //if button is active
            if (cTrace.posButton[i])
            {
                //create x,y,z sliders for the mesh
                cTrace.meshes[i].transform.position = new Vector3(GUI.HorizontalSlider(new Rect(subX2, subY2, 64, 20), cTrace.meshes[i].transform.position.x, -4.0f, 4.0f), 
                    GUI.HorizontalSlider(new Rect(subX2 + 64, subY2, 64, 20), cTrace.meshes[i].transform.position.y, -4.0f, 4.0f), 
                    GUI.HorizontalSlider(new Rect(subX2 + 128, subY2, 64, 20), cTrace.meshes[i].transform.position.z, -4.0f, 4.0f));
                GUI.Label(new Rect(subX2, subY2 + 20, 25, 20), cTrace.meshes[i].transform.position.x.ToString());
                GUI.Label(new Rect(subX2 + 64, subY2 + 20, 25, 20), cTrace.meshes[i].transform.position.y.ToString());
                GUI.Label(new Rect(subX2 + 128, subY2 + 20, 25, 20), cTrace.meshes[i].transform.position.z.ToString());
            }
            subY2 += 40;
        }

        //buttons to alter the shine/specularity of the object
        GUI.Label(new Rect(xShine, yShine-20, 128, 20), "Shininess");
        for (int i = 0; i < cTrace.meshes.Length; i++)
        {
            //if button is pressed
            if (GUI.Button(new Rect(xShine, yShine, 64, 20), cTrace.meshes[i].name))
            {
                //set boolean
                cTrace.shineButton[i] = !cTrace.shineButton[i];
            }
            yShine += 40;
        }

        //sliders to change the shine/specularity of the obejct
        for (int i = 0; i < cTrace.shineButton.Length; i++)
        {
            //if button is active
            if (cTrace.shineButton[i])
            {
                //if object is metallic
                //create sliders for metallic and shinniness
                if (cTrace.meshes[i].GetComponent<Renderer>().material.HasProperty("_Metallic"))
                {
                    GUI.Label(new Rect(subX3, subY3 - 20, 50, 20), "Metallic, ");
                    GUI.Label(new Rect(subX3 + 55, subY3 - 20, 25, 20), cTrace.meshes[i].GetComponent<Renderer>().material.GetFloat("_Metallic").ToString());
                    cTrace.meshes[i].GetComponent<Renderer>().material.SetFloat("_Metallic", GUI.HorizontalSlider(new Rect(subX3, subY3, 64, 20), cTrace.meshes[i].GetComponent<Renderer>().material.GetFloat("_Metallic"), 0.0f, 1.0f));
                    GUI.Label(new Rect(subX3 + 90, subY3 - 20, 75, 20), "Smoothness, ");
                    GUI.Label(new Rect(subX3 + 170, subY3 - 20, 25, 20), cTrace.meshes[i].GetComponent<Renderer>().material.GetFloat("_Glossiness").ToString());
                    cTrace.meshes[i].GetComponent<Renderer>().material.SetFloat("_Glossiness", GUI.HorizontalSlider(new Rect(subX3 + 80, subY3, 64, 20), cTrace.meshes[i].GetComponent<Renderer>().material.GetFloat("_Glossiness"), 0.0f, 1.0f));
                }

                //if object is specular
                //create sliders for specularity
                else if (cTrace.meshes[i].GetComponent<Renderer>().material.HasProperty("_SpecColor"))
                {
                    GUI.Label(new Rect(subX3, subY3 - 20, 140, 20), "Specular Smoothness, ");
                    GUI.Label(new Rect(subX3 + 135, subY3 - 20, 25, 20), cTrace.meshes[i].GetComponent<Renderer>().material.GetFloat("_Glossiness").ToString());
                    cTrace.meshes[i].GetComponent<Renderer>().material.SetFloat("_Glossiness", GUI.HorizontalSlider(new Rect(subX3, subY3, 130, 20), cTrace.meshes[i].GetComponent<Renderer>().material.GetFloat("_Glossiness"), 0.0f, 1.0f));
                }
            }
            subY3+=40;
        }

        //slider for recursion
        GUI.Label(new Rect(xShine, yShine - 20, 75, 20), "Recursion " + recursion.ToString());
        recursion = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(subX3 + 20, subY3 - 15, 65, 20), recursion, 1, 5));

        //button for new render
        if (GUI.Button(new Rect(xShine, yShine, 64, 20), "Render"))
        {
            //set all buttons to false and re trace scene
            for (int i = 0; i < cTrace.meshes.Length; i++)
            {
                cTrace.posButton[i] = false;
                cTrace.matButton[i] = false;
                cTrace.shineButton[i] = false;
            }
            RayTrace();
        }

        //adds shinniness to walls     
        if(GUI.Button(new Rect(xShine +64,yShine,120,20),"Add Wall Shine"))
        {
            wallShine = !wallShine;
            for (int i = 0; i < cTrace.walls.Length; i++)
            {
                //if wallshine is active, add metallic
                if(wallShine)
                {
                    cTrace.walls[i].GetComponent<Renderer>().material.SetFloat("_Metallic", 1);
                }
                
                //else remove metallic
                else
                {
                    cTrace.walls[i].GetComponent<Renderer>().material.SetFloat("_Metallic", 0);
                }
            }
            RayTrace();
        }
    }

    //ray trace and set the pixel
    void RayTrace()
    {
        for(int y = 0; y<Screen.height; y++)
        {
            for(int x = 0; x<Screen.width; x++)
            {
                //set colour or x,y pixel in the texture
                outTex.SetPixel(x, y, cTrace.TracePixel(new Vector2(x, y)));
            }
        }

        //apply the texture tot he screen
        outTex.Apply();
    }
}