using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolControl : MonoBehaviour {
	
	// Use this for initialization
	void Start () 
	{
        SetThisHaloRender(false);
	}

    void Update()
    {
        
        if (paintGM.toolType == "brush") 
        {
            SetOtherHaloRender("eraser", false);
            SetOtherHaloRender("adc", false);
        }
        if (paintGM.toolType == "adc")
        {
            SetOtherHaloRender("eraser", false);
            SetOtherHaloRender("brush", false);
        }
        if (paintGM.toolType == "eraser")
        {
            SetOtherHaloRender("brush", false);
            SetOtherHaloRender("adc", false);
        }
        
    }

	void OnMouseDown() 
	{
        SetToolType(gameObject.name);
        SetStrokeScale(gameObject.name);
        SetTempo(gameObject.name);
        
        SetThisHaloRender(true);
        

	}

    void OnMouseUp()
    {
        if(paintGM.toolType != "eraser" 
            && paintGM.toolType != "brush" 
            && paintGM.toolType != "adc")
                SetThisHaloRender(false);
    }

    void SetThisHaloRender(bool state)
    {
        Component halo = gameObject.GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, state, null);
    }
    void SetOtherHaloRender(string name, bool state)
    {
        Component halo = GameObject.Find(name).GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, state, null);
    }

    void SetToolType(string toolName)
    {
        if (toolName == "eraser")
        {
            paintGM.toolType = "eraser";
            //Debug.Log("eraser selected");
        }
        if (toolName == "brush")
        {
            paintGM.toolType = "brush";
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

            if (PlayLineController.currentTempo < 155.0f) //max tempo 160bpm
            {
                PlayLineController.currentTempo += 5.0f;
            }
            Debug.Log("tempoUp selected");
        }
        if (toolName == "tempoDown")
        {
            if (PlayLineController.currentTempo > 5.0f) //min tempo 5bpm
            {
                PlayLineController.currentTempo -= 5.0f;
            }
            
            Debug.Log("tempoDown selected");
        }
    }
    
    //if tags or gameobject names are changed and don't match, this will not work!!
}
