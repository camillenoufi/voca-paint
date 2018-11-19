using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolControl : MonoBehaviour {
	
	public static bool playButtonFlag = false;
    public static bool stopButtonFlag = false;
    
    // Use this for initialization
	void Start () 
	{
        SetThisHaloRender(false);
	}

    void Update()
    {
        
        switch (paintGM.toolType)
        {
            case "brush":
            {
                SetOtherHaloRender("eraser", false);
                SetOtherHaloRender("adc", false);
                break;
            }
        
            case "adc":
            {
                SetOtherHaloRender("eraser", false);
                SetOtherHaloRender("brush", false);
                break;
            }
            case "eraser":
            {
                SetOtherHaloRender("brush", false);
                SetOtherHaloRender("adc", false);
                break;
            }
        }
        
    }

	void OnMouseDown() 
	{
        SetToolType(gameObject.name);
        SetStrokeScale(gameObject.name);
        SetTempo(gameObject.name);
        SetPlayback(gameObject.name);
        SetThisHaloRender(true);
        

	}

    void OnMouseUp()
    {
        if( paintGM.toolType != "eraser" 
            && paintGM.toolType != "brush" 
            && paintGM.toolType != "adc" )
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
        }
        else if (toolName == "brush")
        {
            paintGM.toolType = "brush";
        }
        else if (toolName == "adc")
        {
            paintGM.toolType = "adc";
        }
    }
    void SetStrokeScale(string toolName)
    {
        if (toolName == "sizeUp")
        {
            paintGM.currentScale *= 1.1f;
        }
        if (toolName == "sizeDown")
        {
            paintGM.currentScale *= 0.9f;
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
        }
        if (toolName == "tempoDown")
        {
            if (PlayLineController.currentTempo > 5.0f) //min tempo 5bpm
            {
                PlayLineController.currentTempo -= 5.0f;
            }
            
        }
    }

    void SetPlayback(string toolName) 
    {
        if (toolName == "playButton")
            PlayLineController.playRightFlag = true;
            SetOtherHaloRender("stopButton", false);
        if (toolName == "stopButton")
        {
            PlayLineController.playRightFlag = false;
            PlayLineController.playLeftFlag = false;
            SetOtherHaloRender("playButton", false);
        }
    }
}
