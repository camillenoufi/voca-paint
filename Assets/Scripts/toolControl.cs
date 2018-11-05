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
		if(gameObject.name == "scissors") 
		{
            paintGM.toolType = "scissors";
            //Debug.Log("scissor selected");
        }
        if (gameObject.name == "pencil")
        {
            paintGM.toolType = "pencil";
            //Debug.Log("pencil selected");
        }
        if (gameObject.name == "sizeUp")
        {
            paintGM.currentScale += 1.0f;
            //Debug.Log("pencil selected");
        }
        if (gameObject.name == "sizeDown")
        {
            paintGM.currentScale -= 1.0f;
            //Debug.Log("pencil selected");
        }
        if (gameObject.name == "adc")
        {
            paintGM.toolType = "adc";
            //Debug.Log("scissor selected");
        }
        if (gameObject.name == "tempoUp")
        {
            PlayLineController.currentTempo += 5.0f;
            //Debug.Log("pencil selected");
        }
        if (gameObject.name == "tempoDown")
        {
            PlayLineController.currentTempo -= 5.0f;
            //Debug.Log("pencil selected");
        }
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
}
