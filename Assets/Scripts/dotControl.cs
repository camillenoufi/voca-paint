using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dotControl : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GetComponent<SpriteRenderer>().color = paintGM.currentColor;
        GetComponent<SpriteRenderer>().tag = paintGM.currentTag;
        GetComponent<Transform>().localScale = new Vector2(paintGM.currentScale,paintGM.currentScale);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseOver()
	{
		if(paintGM.toolType == "eraser")
			Destroy(gameObject);
	}

    void OnTriggerEnter(Collider other) //turn on halo
    {
		if (other.gameObject.CompareTag("Player"))
        {
			if (gameObject.CompareTag("voiceDot"))
			{
				Debug.Log("voice Dot collide");
			}
			if (gameObject.CompareTag("colorDot"))
			{
				Debug.Log("color Dot collide");
			}
        }
    }
}
