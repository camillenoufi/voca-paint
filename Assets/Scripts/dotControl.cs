using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dotControl : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GetComponent<SpriteRenderer>().color = paintGM.currentColor;
        GetComponent<Transform>().localScale = new Vector2(paintGM.currentScale,paintGM.currentScale);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseOver()
	{
		if(paintGM.toolType == "scissors")
			Destroy(gameObject);
	}

    void OnTriggerEnter(Collider other) //turn on halo
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //based on dot color
        }
    }
}
