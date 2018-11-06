using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorPicker : MonoBehaviour {


	// Use this for initialization
	void Start () {
        SetHaloRender(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() 
	{
		paintGM.currentColor = GetComponent<SpriteRenderer>().color;
		//Debug.Log(paintGM.currentColor);
		//paintGM.currentOrder += 1;

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
