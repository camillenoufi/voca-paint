using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTrail : MonoBehaviour {

    public KeyCode mouseLeft;
	public GameObject trailPrefab;
	GameObject thisTrail;
	Vector3 startPos;


	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
	
		// if mouse is clicked down, render a NEW trail
		if (Input.GetMouseButtonDown(0))
        {
            //first click
			if (Input.GetKeyDown(mouseLeft))
            {
                startPos = this.transform.position;
            }
            thisTrail = (GameObject) Instantiate(trailPrefab,
													this.transform.position = objPosition,
													Quaternion.identity);
			
        }
		// if mouse is released and distance is really small, don't render a trail
		else if(Input.GetMouseButtonUp(0)) 
		{
			if (Vector2.Distance(thisTrail.transform.position,startPos) < 0.1) 
			{
				Destroy(thisTrail);
			}
		}
    }
}
