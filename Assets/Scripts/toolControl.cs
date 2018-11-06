using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolControl : MonoBehaviour {
	
	// Use this for initialization
	void Start () 
	{
        SetHaloRender(false);
	}

	void OnMouseDown() 
	{
        SetToolType(gameObject.name);
        SetStrokeScale(gameObject.name);
        SetTempo(gameObject.name);
        SetColorTag(gameObject.name);
        SetHaloRender(true);
        

	}

    void OnMouseUp()
    {
        SetHaloRender(false);
    }

    void SetHaloRender(bool state)
    {
        Component halo = gameObject.GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, state, null);
    }

    void SetToolType(string toolName)
    {
        if (toolName == "eraser")
        {
            paintGM.toolType = "eraser";
            //Debug.Log("eraser selected");
        }
        if (toolName == "adc")
        {
            paintGM.toolType = "adc";
            //Debug.Log("adc selected");
        }
    }
    void SetStrokeScale(string toolName)
    {
        if (toolName == "sizeUp")
        {
            paintGM.currentScale += 1.0f;
            //Debug.Log("pencil selected");
        }
        if (toolName == "sizeDown")
        {
            paintGM.currentScale -= 1.0f;
            //Debug.Log("pencil selected");
        }
    }
    void SetTempo(string toolName)
    {
        if (toolName == "tempoUp")
        {
            PlayLineController.currentTempo += 5.0f;
            Debug.Log("tempoUp selected");
        }
        if (toolName == "tempoDown")
        {
            PlayLineController.currentTempo -= 5.0f;
            Debug.Log("tempoDown selected");
        }
    }
    
    //if tags or gameobject names are changed and don't match, this will not work!!
    void SetColorTag(string toolName)
    {
        paintGM.currentTag = toolName;
        Debug.Log(paintGM.currentTag);
    }
}
